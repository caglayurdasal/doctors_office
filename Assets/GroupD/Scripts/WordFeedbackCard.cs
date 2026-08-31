using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordFeedbackCard : MonoBehaviour
{
    public TMP_Text wordText;
    public TMP_Text scoreText;
    public TMP_Text statusIcon;

    public Image cardBackground;
    public Image statusBox;

    public void Setup(string word, int score)
    {
        wordText.text = word;
        scoreText.text = score.ToString();

        if (score >= 85)
        {
            statusIcon.text = "✓";

            cardBackground.color =
                new Color32(42, 122, 48, 255);

            statusBox.color =
                new Color32(220, 245, 220, 255);
        }
        else if (score >= 60)
        {
            statusIcon.text = "!";

            cardBackground.color =
                new Color32(220, 110, 0, 255);

            statusBox.color =
                new Color32(255, 240, 190, 255);
        }
        else
        {
            statusIcon.text = "×";

            cardBackground.color =
                new Color32(190, 50, 50, 255);

            statusBox.color =
                new Color32(250, 220, 220, 255);
        }
    }
}
