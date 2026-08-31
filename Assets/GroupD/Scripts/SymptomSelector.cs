using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SymptomSelector : MonoBehaviour
{
    public TMP_Text generatedSentenceText;

    public List<string> selectedSymptoms =
        new List<string>();

    public string GeneratedSentence { get; private set; }

    public void ToggleSymptom(string symptom)
    {
        if (selectedSymptoms.Contains(symptom))
        {
            selectedSymptoms.Remove(symptom);
        }
        else
        {
            selectedSymptoms.Add(symptom);
        }

        UpdateSentence();
    }

    private void UpdateSentence()
    {
        if (selectedSymptoms.Count == 0)
        {
            GeneratedSentence =
                "Bitte wählen Sie Ihre Symptome.";

            generatedSentenceText.text =
                GeneratedSentence;

            return;
        }

        if (selectedSymptoms.Count == 1)
        {
            GeneratedSentence =
                "Ich habe " +
                selectedSymptoms[0] +
                ".";
        }

        else if (selectedSymptoms.Count == 2)
        {
            GeneratedSentence =
                "Ich habe " +
                selectedSymptoms[0] +
                " und " +
                selectedSymptoms[1] +
                ".";
        }

        else
        {
            string symptoms = "";

            for (int i = 0;
                 i < selectedSymptoms.Count;
                 i++)
            {
                if (i ==
                    selectedSymptoms.Count - 1)
                {
                    symptoms +=
                        "und " +
                        selectedSymptoms[i];
                }
                else
                {
                    symptoms +=
                        selectedSymptoms[i] +
                        ", ";
                }
            }

            GeneratedSentence =
                "Ich habe " +
                symptoms +
                ".";
        }

        generatedSentenceText.text =
            GeneratedSentence;
    }
}