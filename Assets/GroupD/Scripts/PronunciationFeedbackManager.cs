using TMPro;
using UnityEngine;

public class PronunciationFeedbackManager : MonoBehaviour
{
    [Header("References")]
    public TMP_Text targetSentenceText;

    public Transform wordCardsContainer;

    public GameObject wordCardPrefab;

    public TMP_Text overallScoreText;

    public TMP_Text correctionText;


    public void ShowFeedback(string sentence)
    {
        // Put selected sentence on Screen 05
        targetSentenceText.text = sentence;

        // Remove full stop so it does not become
        // part of final word
        string cleanSentence =
            sentence.Replace(".", "");

        // Break sentence into individual words
        string[] words =
            cleanSentence.Split(' ');

        // Delete previous feedback cards
        foreach (Transform child
                 in wordCardsContainer)
        {
            Destroy(child.gameObject);
        }

        int totalScore = 0;
        int lowestScore = 101;
        string weakestWord = "";


        // Create one card for every word
        foreach (string word in words)
        {
            int score =
                GetTemporaryScore(word);

            totalScore += score;

            if (score < lowestScore)
            {
                lowestScore = score;
                weakestWord = word;
            }

            GameObject newCard =
                Instantiate(
                    wordCardPrefab,
                    wordCardsContainer
                );

            WordFeedbackCard card =
                newCard.GetComponent<
                    WordFeedbackCard>();

            card.Setup(word, score);
        }


        // Calculate overall score
        int overall =
            totalScore / words.Length;

        overallScoreText.text =
            overall + "%";


        // Temporary correction feedback
        if (lowestScore < 60)
        {
            correctionText.text =
                "Try practising \"" +
                weakestWord +
                "\" more slowly.";
        }
        else if (lowestScore < 85)
        {
            correctionText.text =
                "Good attempt. Try repeating \"" +
                weakestWord +
                "\" more clearly.";
        }
        else
        {
            correctionText.text =
                "Excellent pronunciation!";
        }
    }


    // TEMPORARY simulated scores
    // AI group will replace this later
    private int GetTemporaryScore(string word)
    {
        switch (word)
        {
            case "Ich":
                return 98;

            case "habe":
                return 94;

            case "und":
                return 100;

            case "Fieber":
                return 72;

            case "Kopfschmerzen":
                return 48;

            case "Halsschmerzen":
                return 69;

            case "Husten":
                return 82;

            case "Bauchschmerzen":
                return 64;

            case "Schnitte":
                return 88;

            default:
                return 75;
        }
    }
}