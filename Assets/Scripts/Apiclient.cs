using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;

// Attach this to the same GameObject as AudioCapture
// Sends raw PCM audio bytes to the local translate-audio server and receives JSON response

public class APIClient : MonoBehaviour
{
    [Header("API Settings")]
    // Local FastAPI server from the AI/Backend team's repo (uvicorn default port shown)
    public string endpointURL = "http://localhost:8000/translate-audio";

    // Set to true to use mock data instead of real API (for testing without network)
    public bool useMockData = true;

    [Header("Timeouts")]
    // The server loads a faster-whisper model + Argos translation package on first startup,
    // which can take a while. The very first real request needs a much longer timeout than
    // subsequent ones, or Unity will report a connection failure that isn't actually one.
    public int coldStartTimeoutSeconds = 90;
    public int normalTimeoutSeconds = 20;
    private bool firstRequestSent = false;

    private SubtitleDisplay subtitleDisplay;

    void Start()
    {
        subtitleDisplay = FindAnyObjectByType<SubtitleDisplay>();

        if (subtitleDisplay == null)
        {
            Debug.LogError("SubtitleDisplay script not found in scene!");
        }
    }

    public void SendAudio(byte[] pcmBytes)
    {
        if (useMockData)
        {
            Debug.Log("Using mock data (useMockData = true)");
            SimulateMockResponse();
            return;
        }

        StartCoroutine(PostAudio(pcmBytes));
    }

    private IEnumerator PostAudio(byte[] pcmBytes)
    {
        Debug.Log("Sending audio to: " + endpointURL);

        UnityWebRequest request = new UnityWebRequest(endpointURL, "POST");
        request.uploadHandler = new UploadHandlerRaw(pcmBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/octet-stream");

        request.timeout = firstRequestSent ? normalTimeoutSeconds : coldStartTimeoutSeconds;
        firstRequestSent = true;

        subtitleDisplay?.ShowInstructions("Waiting for response...");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Response received: " + jsonResponse);
            ParseAndDisplay(jsonResponse);
        }
        else
        {
            Debug.LogError("API Error: " + request.error);
            subtitleDisplay?.ShowError("Connection failed: " + request.error);
        }
    }

    private void ParseAndDisplay(string json)
    {
        // Matches the AI server's actual JSON keys exactly (JsonUtility maps by field name)
        try
        {
            APIResponse response = JsonUtility.FromJson<APIResponse>(json);

            // Mapping decision (Option A):
            // - doctor's German line is the main subtitle
            // - English translation of that line goes in the translation slot
            // - what the system heard the user say is shown as the "hint" (confirmation feedback)
            subtitleDisplay?.UpdateDisplay(
                response.doctor_reply_de,
                response.translated_text,
                response.user_transcript
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError("JSON parse error: " + e.Message);
            Debug.LogError("Raw response was: " + json);
        }
    }

    private void SimulateMockResponse()
    {
        // Mock response for testing UI without the real server running
        subtitleDisplay?.UpdateDisplay(
            subtitle: "Guten Morgen, wie kann ich Ihnen helfen?",
            translation: "Good morning, how can I help you today?",
            hint: "Heard: \"Ich habe Kopfschmerzen\""
        );
    }
}

// Field names match the server's JSON response exactly.
// Only doctor_reply_de, translated_text, and user_transcript are used in the current UI
// mapping (Option A) — user_transcript_en and doctor_reply_en are included in case a future
// debug view or alternate mapping needs them. translated_text is currently identical to
// doctor_reply_en (server keeps it for backward-compatible naming).
[System.Serializable]
public class APIResponse
{
    public string user_transcript;
    public string user_transcript_en;
    public string doctor_reply_de;
    public string doctor_reply_en;
    public string translated_text;
}
