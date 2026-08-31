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

    void OnEnable()
    {
        AutoAssignReferences();

        if (replayButton != null) { replayButton.onClick.RemoveAllListeners(); replayButton.onClick.AddListener(OnReplayClicked); }
        if (slowButton != null) { slowButton.onClick.RemoveAllListeners(); slowButton.onClick.AddListener(OnSlowClicked); }
        if (hintButton != null) { hintButton.onClick.RemoveAllListeners(); hintButton.onClick.AddListener(OnHintClicked); }
        if (confirmButton != null) { confirmButton.onClick.RemoveAllListeners(); confirmButton.onClick.AddListener(OnConfirmClicked); }
        if (helpButton != null) { helpButton.onClick.RemoveAllListeners(); helpButton.onClick.AddListener(ToggleHelpPanel); }
    }

    private void AutoAssignReferences()
    {
        if (replayButton == null) replayButton = GameObject.Find("ReplayBox")?.GetComponent<Button>();
        if (slowButton == null) slowButton = GameObject.Find("Slow Box")?.GetComponent<Button>();
        if (hintButton == null) hintButton = GameObject.Find("Hint Box")?.GetComponent<Button>();
        if (confirmButton == null) confirmButton = GameObject.Find("Confirm Box")?.GetComponent<Button>();
        if (helpButton == null) helpButton = transform.Find("HelpButton")?.GetComponent<Button>();

        if (helpPanel == null)
        {
            Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
            foreach (Transform t in allTransforms)
            {
                if (t.name == "MainPanel")
                {
                    helpPanel = t.gameObject;
                    break;
                }
            }
        }

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

    public void ToggleHelpPanel()
    {
        if (helpPanel == null) AutoAssignReferences();

        if (helpPanel != null)
        {
            bool isActive = helpPanel.activeSelf;
            helpPanel.SetActive(!isActive);
            if (!isActive) helpPanel.transform.SetAsLastSibling();
        }
    }

    public void OnReplayClicked() { Debug.Log("Replay pressed"); }
    public void OnSlowClicked() { Debug.Log("Slow pressed"); }
    public void OnHintClicked() { Debug.Log("Hint pressed"); }

    public void OnConfirmClicked()
    {
        Debug.Log("Confirm button pressed: Moving to results...");

        
        if (helpPanel != null) helpPanel.SetActive(false);

        
        if (nextScreenCanvas != null)
        {
            nextScreenCanvas.SetActive(true);
        }

        if (currentScreenCanvas != null)
        {
            currentScreenCanvas.SetActive(false);
        }
    }
}