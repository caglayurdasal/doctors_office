using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DoctorDialogueUI : MonoBehaviour
{
    [Header("1. Header & Status Texts")]
    public TMP_Text statusText;
    public TMP_Text statusSubtext;

    [Header("2. Doctor Dialogue Panel")]
    public TMP_Text doctorGermanText;
    public TMP_Text doctorEnglishText;

    [Header("3. User Response Panel")]
    public TMP_Text userTranscribedText;
    public TMP_Text scoreText;
    public TMP_Text scoreFeedbackText;

    [Header("4. Bottom Action Buttons")]
    public Button replayButton;
    public Button slowButton;
    public Button hintButton;
    public Button confirmButton;

    [Header("5. Help Options")]
    public Button helpButton;
    public GameObject helpPanel;

    [Header("6. Screen Navigation")]
    public GameObject currentScreenCanvas;
    public GameObject nextScreenCanvas;

    void Start()
    {
        AutoAssignReferences();

        if (replayButton != null) replayButton.onClick.AddListener(OnReplayClicked);
        if (slowButton != null) slowButton.onClick.AddListener(OnSlowClicked);
        if (hintButton != null) hintButton.onClick.AddListener(OnHintClicked);
        if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirmClicked);
        if (helpButton != null) helpButton.onClick.AddListener(ToggleHelpPanel);
    }

    private void AutoAssignReferences()
    {
        if (replayButton == null)
            replayButton = GameObject.Find("ReplayBox")?.GetComponent<Button>();

        if (slowButton == null)
            slowButton = GameObject.Find("Slow Box")?.GetComponent<Button>();

        if (hintButton == null)
            hintButton = GameObject.Find("Hint Box")?.GetComponent<Button>();

        if (confirmButton == null)
            confirmButton = GameObject.Find("Confirm Box")?.GetComponent<Button>();

        if (helpButton == null)
            helpButton = GameObject.Find("HelpButton")?.GetComponent<Button>();

        if (helpPanel == null)
            helpPanel = GameObject.Find("HelpPanel");

        if (currentScreenCanvas == null)
        {
            currentScreenCanvas = GameObject.Find("Screen6_DialoguePanel");
            if (currentScreenCanvas == null) currentScreenCanvas = this.gameObject;
        }

        if (nextScreenCanvas == null)
        {
            
            Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
            foreach (Transform t in allTransforms)
            {
                if (t.name == "Screen7_ResultPanel")
                {
                    nextScreenCanvas = t.gameObject;
                    break;
                }
            }
        }
    }

    public void UpdateDialogueUI(string docGerman, string docEnglish, string userText, string scoreVal, string feedbackVal)
    {
        if (doctorGermanText != null) doctorGermanText.text = docGerman;
        if (doctorEnglishText != null) doctorEnglishText.text = docEnglish;
        if (userTranscribedText != null) userTranscribedText.text = "\"" + userText + "\"";
        if (scoreText != null) scoreText.text = scoreVal + "%";
        if (scoreFeedbackText != null) scoreFeedbackText.text = feedbackVal;
    }

    public void ToggleHelpPanel()
    {
        if (helpPanel != null)
        {
            bool isActive = helpPanel.activeSelf;
            helpPanel.SetActive(!isActive);
        }
    }

    public void OnReplayClicked()
    {
        Debug.Log("Replay button pressed: Replaying doctor audio...");
    }

    public void OnSlowClicked()
    {
        Debug.Log("Slow button pressed: Playing audio slowly...");
    }

    public void OnHintClicked()
    {
        Debug.Log("Hint button pressed: Showing vocabulary hint...");
    }

    public void OnConfirmClicked()
    {
        Debug.Log("Confirm button pressed: Submitting response and moving to results...");

        if (helpPanel != null) helpPanel.SetActive(false);

        
        if (nextScreenCanvas != null)
        {
            nextScreenCanvas.SetActive(true);
        }
        else
        {
            Debug.LogError("Error: Next Screen Canvas (Screen7_ResultPanel) is NOT assigned!");
            return;
        }

        
        if (currentScreenCanvas != null)
        {
            currentScreenCanvas.SetActive(false);
        }
    }
}