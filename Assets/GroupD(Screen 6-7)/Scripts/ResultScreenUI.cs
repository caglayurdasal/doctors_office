using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultScreenUI : MonoBehaviour
{
    [Header("1. Result Summary UI")]
    public TMP_Text sentencesPracticedText;
    public TMP_Text averageScoreText;
    public TMP_Text bestSentenceText;
    public TMP_Text needsWorkText;

    [Header("2. Screen 7 Action Buttons")]
    public Button practiceAgainButton;
    public Button detailedReportButton;
    public Button exitButton;

    [Header("3. Help Options")]
    public Button helpButton;
    public GameObject helpPanel;

    [Header("4. Screen Navigation")]
    public GameObject screen1Canvas;
    public GameObject screen7Canvas;

    void Start()
    {
        AutoAssignReferences();

        if (practiceAgainButton != null) practiceAgainButton.onClick.AddListener(OnPracticeAgainClicked);
        if (detailedReportButton != null) detailedReportButton.onClick.AddListener(OnDetailedReportClicked);
        if (exitButton != null) exitButton.onClick.AddListener(OnExitClicked);
        if (helpButton != null) helpButton.onClick.AddListener(ToggleHelpPanel);
    }

    private void AutoAssignReferences()
    {
        if (practiceAgainButton == null) practiceAgainButton = GameObject.Find("Practice Again Box")?.GetComponent<Button>();
        if (detailedReportButton == null) detailedReportButton = GameObject.Find("Detailed Report")?.GetComponent<Button>();
        if (exitButton == null) exitButton = GameObject.Find("ExitButton")?.GetComponent<Button>();

        if (helpButton == null) helpButton = GameObject.Find("HelpButton")?.GetComponent<Button>();
        if (helpPanel == null) helpPanel = GameObject.Find("HelpPanel");

        if (screen7Canvas == null)
        {
            screen7Canvas = GameObject.Find("Screen7_ResultPanel");
            if (screen7Canvas == null) screen7Canvas = this.gameObject;
        }

        if (screen1Canvas == null)
        {
            
            Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
            foreach (Transform t in allTransforms)
            {
                if (t.name == "Screen1_WelcomePanel")
                {
                    screen1Canvas = t.gameObject;
                    break;
                }
            }
        }
    }

    public void ToggleHelpPanel()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(!helpPanel.activeSelf);
        }
    }

    public void OnPracticeAgainClicked()
    {
        Debug.Log("Practice Again clicked: Going back to Screen 1...");

        if (screen7Canvas != null)
            screen7Canvas.SetActive(false);

        if (screen1Canvas != null)
            screen1Canvas.SetActive(true);
    }

    public void OnDetailedReportClicked()
    {
        Debug.Log("Opening Detailed Report...");
    }

    public void OnExitClicked()
    {
        Debug.Log("Exiting Application...");

        
        if (screen7Canvas != null)
        {
            screen7Canvas.SetActive(false);
        }

        
        Application.Quit();

        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}