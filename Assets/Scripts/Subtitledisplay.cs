using UnityEngine;
using TMPro;

// Attach this to your Canvas GameObject in ConsultationScene
// It updates the text fields with data received from the AI team

public class SubtitleDisplay : MonoBehaviour
{
    [Header("UI Text Fields")]
    // Drag your TextMeshPro text objects here from the Inspector
    public TextMeshProUGUI subtitleText;
    public TextMeshProUGUI translationText;
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI instructionsText;
    public TextMeshProUGUI feedbackText;

    [Header("Labels")]
    // Prefixes shown before the actual line, e.g. "Doctor: Guten Morgen..."
    // Change these here (or per-instance in the Inspector) without touching code.
    public string subtitleLabel = "Doctor: ";
    public string translationLabel = "Doctor translation: ";

    [Header("Settings")]
    public float displayDuration = 5f;  // how long to show text before fading
    public bool autoClear = true;       // clear text after displayDuration seconds

    void Start()
    {
        // Set placeholder text on start
        SetPlaceholders();
    }

    private void SetPlaceholders()
    {
        if (subtitleText) subtitleText.text = subtitleLabel + "(waiting for response)";
        if (translationText) translationText.text = translationLabel + "(waiting for response)";
        if (hintText) hintText.text = "Waiting for input...";
        if (instructionsText) instructionsText.text = "Press the button to start speaking";
        if (feedbackText) feedbackText.text = "";
    }

    // Called by APIClient when response is received
    public void UpdateDisplay(string subtitle, string translation, string hint)
    {
        Debug.Log("Updating display - Subtitle: " + subtitle);

        if (subtitleText) subtitleText.text = subtitleLabel + subtitle;
        if (translationText) translationText.text = translationLabel + translation;
        if (hintText) hintText.text = hint;
        if (feedbackText) feedbackText.text = "Response received";

        // Auto clear after displayDuration seconds
        if (autoClear)
        {
            CancelInvoke(nameof(ClearDisplay));
            Invoke(nameof(ClearDisplay), displayDuration);
        }
    }

    // Called when there's a connection error
    public void ShowError(string errorMessage)
    {
        if (feedbackText) feedbackText.text = "Error: " + errorMessage;
        if (subtitleText) subtitleText.text = subtitleLabel + "(no response)";
        if (translationText) translationText.text = translationLabel + "(no response)";
        if (hintText) hintText.text = "Could not connect to server";
    }

    // Call this to show instructions to the user
    public void ShowInstructions(string instructions)
    {
        if (instructionsText) instructionsText.text = instructions;
    }

    // Clears all text fields back to their labeled placeholders
    public void ClearDisplay()
    {
        SetPlaceholders();
        if (feedbackText) feedbackText.text = "";
    }
}
