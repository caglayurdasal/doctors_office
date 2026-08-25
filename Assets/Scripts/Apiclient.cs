using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;

// Attach this to the same GameObject as AudioCapture
// Sends audio bytes to AI team's server and receives JSON response

public class APIClient : MonoBehaviour
{
    [Header("API Settings")]
    // !! UPDATE THIS after today's call with the AI team !!
    public string endpointURL = "http://REPLACE_WITH_AI_TEAM_IP:PORT/translate-audio";

    // Set to true to use mock data instead of real API (for testing without network)
    public bool useMockData = true;

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
            // Simulate API response for testing without network
            Debug.Log("Using mock data (useMockData = true)");
            SimulateMockResponse();
            return;
        }

        StartCoroutine(PostAudio(pcmBytes));
    }

    private IEnumerator PostAudio(byte[] pcmBytes)
    {
        Debug.Log("Sending audio to: " + endpointURL);

        // Create HTTP POST request with raw bytes
        UnityWebRequest request = new UnityWebRequest(endpointURL, "POST");
        request.uploadHandler = new UploadHandlerRaw(pcmBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/octet-stream");

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
            // Show error on subtitle display
            subtitleDisplay?.ShowError("Connection failed: " + request.error);
        }
    }

    private void ParseAndDisplay(string json)
    {
        // Parse the JSON response from AI team
        // !! UPDATE the field names below to match AI team's actual JSON keys !!
        // Example expected JSON: {"subtitle": "Hello", "translation": "Hallo", "hint": "Speak slowly"}
        try
        {
            APIResponse response = JsonUtility.FromJson<APIResponse>(json);
            subtitleDisplay?.UpdateDisplay(response.subtitle, response.translation, response.hint);
        }
        catch (System.Exception e)
        {
            Debug.LogError("JSON parse error: " + e.Message);
            Debug.LogError("Raw response was: " + json);
        }
    }

    private void SimulateMockResponse()
    {
        // Mock response for testing UI without real API
        // Change these strings to test different subtitle scenarios
        subtitleDisplay?.UpdateDisplay(
            subtitle: "Good morning, how can I help you today?",
            translation: "Guten Morgen, wie kann ich Ihnen helfen?",
            hint: "Speak clearly and slowly"
        );
    }
}

// !! UPDATE these field names to match AI team's actual JSON response !!
[System.Serializable]
public class APIResponse
{
    public string subtitle;      // e.g. "Hello doctor"
    public string translation;   // e.g. "Hallo Doktor"
    public string hint;          // e.g. "AI hint text"
}