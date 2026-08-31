using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    private AudioCapture audioCapture;
    private APIClient apiClient;
    private SubtitleDisplay subtitleDisplay;

    void Start()
    {
        audioCapture = FindAnyObjectByType<AudioCapture>();
        apiClient = FindAnyObjectByType<APIClient>();
        subtitleDisplay = FindAnyObjectByType<SubtitleDisplay>();
    }

    public void GoToConsultation()
    {
        // Reset any leftover session state (mic recording, in-flight
        // request, displayed text) before unloading this scene, so the
        // next person starts clean.
        audioCapture?.ResetSession();
        apiClient?.ResetSession();
        subtitleDisplay?.ClearDisplay();

        SceneManager.LoadScene("ConsultationScene");
    }
}