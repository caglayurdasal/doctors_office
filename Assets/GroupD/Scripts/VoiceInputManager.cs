using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VoiceInputManager : MonoBehaviour
{
    [Header("Status UI")]
    public TMP_Text correctionHeading;
    public TMP_Text correctionText;

    [Header("Other Managers")]
    public PronunciationFeedbackManager pronunciationManager;

    [Header("Buttons")]
    public Button retryButton;

    [Header("Microphone Settings")]
    public float recordingTime = 4f;

    private string currentSentence;

    private AudioClip recordedAudio;
    private string microphoneDevice;


    // Called when Screen 05 opens
    public void BeginVoiceAttempt(string sentence)
    {
        currentSentence = sentence;

        StopAllCoroutines();

        StartCoroutine(VoiceRoutine());
    }


    // Called by Retry button
    public void RetryVoiceAttempt()
    {
        if (string.IsNullOrEmpty(currentSentence))
        {
            Debug.LogWarning(
                "No sentence is available for retry."
            );

            return;
        }

        BeginVoiceAttempt(currentSentence);
    }


    private IEnumerator VoiceRoutine()
    {
        // -----------------------------
        // 1. CHECK MICROPHONE
        // -----------------------------

        if (Microphone.devices.Length == 0)
        {
            ShowMicrophoneError(
                "No microphone was detected. Please connect or enable a microphone."
            );

            yield break;
        }


        microphoneDevice = Microphone.devices[0];

        Debug.Log(
            "Using microphone: " +
            microphoneDevice
        );


        // -----------------------------
        // 2. LISTENING STATE
        // -----------------------------

        correctionHeading.text =
            "● LISTENING...";

        correctionText.text =
            "Speak the German sentence now.";

        if (retryButton != null)
        {
            retryButton.interactable = false;
        }


        // Start recording
        recordedAudio = Microphone.Start(
            microphoneDevice,
            false,
            Mathf.CeilToInt(recordingTime),
            16000
        );


        if (recordedAudio == null)
        {
            ShowMicrophoneError(
                "Microphone recording could not be started."
            );

            yield break;
        }


        // Wait while the user speaks
        yield return new WaitForSeconds(
            recordingTime
        );


        // Stop recording
        if (Microphone.IsRecording(
            microphoneDevice))
        {
            Microphone.End(
                microphoneDevice
            );
        }


        // -----------------------------
        // 3. ANALYSING STATE
        // -----------------------------

        correctionHeading.text =
            "● ANALYSING...";

        correctionText.text =
            "Checking your pronunciation...";


        // Temporary waiting period.
        // Later your AI system will replace this.
        yield return new WaitForSeconds(2f);


        // -----------------------------
        // 4. DISPLAY FEEDBACK
        // -----------------------------

        if (pronunciationManager != null)
        {
            correctionHeading.text =
                "CORRECTION TIP";

            pronunciationManager.ShowFeedback(
                currentSentence
            );
        }
        else
        {
            Debug.LogError(
                "PronunciationFeedbackManager is not assigned."
            );

            correctionHeading.text =
                "ERROR";

            correctionText.text =
                "Pronunciation manager is not connected.";
        }


        if (retryButton != null)
        {
            retryButton.interactable = true;
        }
    }


    private void ShowMicrophoneError(
        string message)
    {
        correctionHeading.text =
            "⚠ MICROPHONE ERROR";

        correctionText.text =
            message;

        Debug.LogError(message);

        if (retryButton != null)
        {
            retryButton.interactable = true;
        }
    }
}