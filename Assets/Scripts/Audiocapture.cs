using UnityEngine;
using System.Collections;

// Attach this script to the AudioManager GameObject alongside APIClient and SubtitleDisplay.
// Captures microphone input and auto-stops on silence, then converts the recorded portion
// to 16kHz PCM bytes for sending to the local translate-audio server.

public class AudioCapture : MonoBehaviour
{
    [Header("Recording Settings")]
    public int sampleRate = 16000;            // server requires 16kHz
    public int maxRecordSeconds = 15;         // hard safety cap if silence is never detected
    public float minRecordSeconds = 0.5f;     // ignore silence before this (avoids instant stop)
    public bool isRecording = false;

    [Header("Silence Detection")]
    // Tune these against your actual mic/room. The server repo's tools/mic_volume_check.py
    // is useful for seeing real RMS values from your mic to pick a sensible threshold.
    // (Their Python prototype uses a different metric/threshold on raw int16 samples —
    // not directly portable to Unity's normalized float RMS, so recalibrate here.)
    public float silenceThreshold = 0.02f;
    public float silenceDurationToStop = 1.2f;

    // Requires a short run of "loud" samples before we consider the user to have actually
    // started speaking, so a brief click/breath at the start can't trigger a false early stop.
    public float minSpeechDuration = 0.75f;
    private float speechTimer = 0f;
    private bool hasSpoken = false;

    private AudioClip micClip;
    private APIClient apiClient;
    private string micDevice;
    private int lastSamplePos = 0;
    private float silenceTimer = 0f;
    private float elapsedTime = 0f;
    private Coroutine monitorCoroutine;

    void Start()
    {
        apiClient = GetComponent<APIClient>();

        if (apiClient == null)
        {
            Debug.LogError("APIClient script not found on this GameObject!");
        }

        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone found!");
            return;
        }

        micDevice = Microphone.devices[0];
        Debug.Log("Microphone ready: " + micDevice);
    }

    // Call this to start recording (wire to a button or gaze trigger)
    public void StartRecording()
    {
        if (isRecording) return;
        if (string.IsNullOrEmpty(micDevice))
        {
            Debug.LogError("No microphone device available.");
            return;
        }

        Debug.Log("Recording started (will auto-stop on silence)...");
        isRecording = true;
        silenceTimer = 0f;
        speechTimer = 0f;
        hasSpoken = false;
        elapsedTime = 0f;
        lastSamplePos = 0;

        // loop=false: recording auto-stops if maxRecordSeconds is ever reached without silence
        micClip = Microphone.Start(micDevice, false, maxRecordSeconds, sampleRate);

        monitorCoroutine = StartCoroutine(MonitorSilence());
    }

    private IEnumerator MonitorSilence()
    {
        // Wait for the mic to actually start producing samples
        while (Microphone.GetPosition(micDevice) <= 0)
            yield return null;

        while (isRecording)
        {
            yield return new WaitForSeconds(0.1f);
            elapsedTime += 0.1f;

            int currentPos = Microphone.GetPosition(micDevice);
            float rms = GetRMSVolume(currentPos);

            if (elapsedTime >= minRecordSeconds)
            {
                if (rms >= silenceThreshold)
                {
                    // Track sustained loud sound before trusting it as "real speech"
                    speechTimer += 0.1f;
                    if (speechTimer >= minSpeechDuration)
                        hasSpoken = true;

                    silenceTimer = 0f;
                }
                else
                {
                    speechTimer = 0f;

                    // Only allow silence to trigger a stop once the user has actually spoken
                    if (hasSpoken)
                    {
                        silenceTimer += 0.1f;
                        if (silenceTimer >= silenceDurationToStop)
                        {
                            StopAndSend();
                            yield break;
                        }
                    }
                }
            }

            if (elapsedTime >= maxRecordSeconds)
            {
                Debug.LogWarning("Max record time reached without detecting silence — stopping automatically.");
                StopAndSend();
                yield break;
            }
        }
    }

    // Computes RMS volume over the samples captured since the last check
    private float GetRMSVolume(int currentPos)
    {
        int sampleCount = currentPos - lastSamplePos;
        if (sampleCount <= 0) return 0f;

        float[] samples = new float[sampleCount];
        micClip.GetData(samples, lastSamplePos);

        float sum = 0f;
        for (int i = 0; i < samples.Length; i++)
            sum += samples[i] * samples[i];

        lastSamplePos = currentPos;
        return Mathf.Sqrt(sum / samples.Length);
    }

    // Call this to stop early and send (also called automatically on silence/max time)
    public void StopAndSend()
    {
        if (!isRecording) return;

        int finalPos = Microphone.GetPosition(micDevice);
        isRecording = false;

        if (monitorCoroutine != null)
            StopCoroutine(monitorCoroutine);

        Microphone.End(micDevice);
        Debug.Log("Recording stopped. Converting to PCM...");

        byte[] pcmBytes = ConvertToPCM(micClip, finalPos);
        Debug.Log("PCM bytes ready: " + pcmBytes.Length + " bytes");

        if (apiClient != null)
        {
            apiClient.SendAudio(pcmBytes);
        }
    }

    // Converts only the actually-recorded portion of the AudioClip to raw 16-bit PCM
    // (avoids sending trailing silence from the preallocated buffer)
    private byte[] ConvertToPCM(AudioClip clip, int sampleCount)
    {
        if (sampleCount <= 0) sampleCount = clip.samples;

        float[] samples = new float[sampleCount];
        clip.GetData(samples, 0);

        byte[] pcmBytes = new byte[samples.Length * 2];
        for (int i = 0; i < samples.Length; i++)
        {
            short value = (short)(Mathf.Clamp(samples[i], -1f, 1f) * short.MaxValue);
            pcmBytes[i * 2] = (byte)(value & 0xff);
            pcmBytes[i * 2 + 1] = (byte)((value >> 8) & 0xff);
        }

        return pcmBytes;
    }
}
