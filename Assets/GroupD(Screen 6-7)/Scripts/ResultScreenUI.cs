using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultScreenUI : MonoBehaviour
{
    [Header("1. Result Summary UI")]
    public TMP_Text sentencesPracticedText;
    public TMP_Text averageScoreText;
    public TMP_Text bestScoreText;
    public TMP_Text bestSentenceText;
    public TMP_Text needsWorkCountText;
    public TMP_Text needsWorkDetailText;

    [Header("2. Progress Bar Settings")]
    public TMP_Text progressLevelText;
    public TMP_Text progressPointsText;
    public Slider progressBarSlider;

    [Header("3. Screen 7 Action Buttons")]
    public Button practiceAgainButton;
    public Button detailedReportButton;
    public Button exitButton;

    [Header("4. Help Options")]
    public Button helpButton;
    public GameObject helpPanel;

    [Header("5. Screen Navigation")]
    public GameObject currentScreenCanvas;
    public GameObject nextScreenCanvas;

    void OnEnable()
    {
        BindButtons();
    }

    private void BindButtons()
    {
        helpButton = transform.Find("HelpButton")?.GetComponent<Button>();

        if (helpButton != null)
        {
            helpButton.onClick.RemoveAllListeners();
            helpButton.onClick.AddListener(ToggleHelpPanel);
        }

        if (practiceAgainButton != null)
        {
            practiceAgainButton.onClick.RemoveAllListeners();
            practiceAgainButton.onClick.AddListener(OnPracticeAgainClicked);
        }

        if (detailedReportButton != null)
        {
            detailedReportButton.onClick.RemoveAllListeners();
            detailedReportButton.onClick.AddListener(OnDetailedReportClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(OnExitClicked);
        }

        UpdateResultsUI(5, 90, 95, "Ich habe Kopfschmerzen", 1, "Fieber");
    }

    private GameObject FindMainPanel()
    {
        if (helpPanel != null) return helpPanel;

        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform t in allTransforms)
        {
            if (t.name == "MainPanel" && t.hideFlags == HideFlags.None)
            {
                helpPanel = t.gameObject;
                return helpPanel;
            }
        }
        return null;
    }

    public void ToggleHelpPanel()
    {
        GameObject targetPanel = FindMainPanel();

        if (targetPanel != null)
        {
            targetPanel.SetActive(true);
            targetPanel.transform.SetAsLastSibling();

            Debug.Log("Screen 7: Help Panel Forced ACTIVE!");
        }
        else
        {
            Debug.LogError("Screen 7 Error: MainPanel object was not found in the scene!");
        }
    }

    public void UpdateResultsUI(int sentencesPracticed, int averageScore, int bestScore, string bestSentence, int needsWorkCount, string needsWorkWord)
    {
        if (sentencesPracticedText != null) sentencesPracticedText.text = sentencesPracticed.ToString();
        if (averageScoreText != null) averageScoreText.text = averageScore.ToString() + "/100";
        if (bestScoreText != null) bestScoreText.text = bestScore.ToString();
        if (bestSentenceText != null) bestSentenceText.text = "Best: " + bestSentence;
        if (needsWorkCountText != null) needsWorkCountText.text = needsWorkCount.ToString();
        if (needsWorkDetailText != null) needsWorkDetailText.text = "Needs work: " + needsWorkWord + "...";

        UpdateProgressData(averageScore);
    }

    private void UpdateProgressData(int avgScore)
    {
        string level = "A2";
        int ptsGained = 0;

        if (avgScore >= 85) { level = "B1"; ptsGained = (avgScore - 80) / 2; }
        else if (avgScore >= 50) { level = "A2"; ptsGained = (avgScore - 50) / 5; }
        else { level = "A1"; ptsGained = (avgScore - 50) / 5; }

        if (progressLevelText != null) progressLevelText.text = level + " FORTSCHRITT...";
        if (progressPointsText != null) progressPointsText.text = (ptsGained >= 0 ? "+" : "") + ptsGained + " pts since last session";
        if (progressBarSlider != null) progressBarSlider.value = Mathf.Clamp01(avgScore / 100f);
    }

    public void OnPracticeAgainClicked()
    {
        GameObject targetPanel = FindMainPanel();
        if (targetPanel != null) targetPanel.SetActive(false);

        if (nextScreenCanvas != null) nextScreenCanvas.SetActive(true);
        if (currentScreenCanvas != null) currentScreenCanvas.SetActive(false);
    }

    public void OnDetailedReportClicked()
    {
        Debug.Log("Opening Detailed Report...");
    }

    public void OnExitClicked()
    {
        GameObject targetPanel = FindMainPanel();
        if (targetPanel != null) targetPanel.SetActive(false);

        if (currentScreenCanvas != null) currentScreenCanvas.SetActive(false);

        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}