using UnityEngine;
using System;

// Attach this script to any GameObject in your scene (e.g. a new empty GameObject called "AudioManager")
// It captures microphone input and converts it to 16kHz PCM bytes for sending to the AI team's server

public class AudioCapture : MonoBehaviour
{
    [Header("Settings")]
    public int sampleRate = 16000;       // AI team requires 16kHz
    public int recordSeconds = 5;        // how many seconds to record per clip
    public bool isRecording = false;

    private AudioClip micClip;
    private APIClient apiClient;

    void Start()
    {
        apiClient = GetComponent<APIClient>();

        if (apiClient == null)
        {
            Debug.LogError("APIClient script not found on this GameObject!");
        }

        // Check if a microphone is available
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone found!");
            return;
        }

        Debug.Log("Microphone ready: " + Microphone.devices[0]);
    }

    // Call this to start recording (wire to a button or gaze trigger)
    public void StartRecording()
    {
        if (isRecording) return;

        Debug.Log("Recording started...");
        isRecording = true;

        // Start recording from the default microphone
        micClip = Microphone.Start(null, false, recordSeconds, sampleRate);

        // Wait for recording to finish then send
        Invoke("StopAndSend", recordSeconds);
    }

    // Call this to stop early and send
    public void StopAndSend()
    {
        if (!isRecording) return;

        Microphone.End(null);
        isRecording = false;
        Debug.Log("Recording stopped. Converting to PCM...");

        // Convert AudioClip to raw 16kHz PCM bytes
        byte[] pcmBytes = ConvertToPCM(micClip);
        Debug.Log("PCM bytes ready: " + pcmBytes.Length + " bytes");

        // Send to AI team's server
        if (apiClient != null)
        {
            apiClient.SendAudio(pcmBytes);
        }
    }

    // Converts Unity AudioClip to raw 16-bit PCM byte array
    private byte[] ConvertToPCM(AudioClip clip)
    {
        float[] samples = new float[clip.samples];
        clip.GetData(samples, 0);

        // Convert float samples (-1 to 1) to 16-bit PCM (short)
        byte[] pcmBytes = new byte[samples.Length * 2];
        for (int i = 0; i < samples.Length; i++)
        {
            short value = (short)(samples[i] * short.MaxValue);
            pcmBytes[i * 2] = (byte)(value & 0xff);
            pcmBytes[i * 2 + 1] = (byte)((value >> 8) & 0xff);
        }

        return pcmBytes;
    }
}