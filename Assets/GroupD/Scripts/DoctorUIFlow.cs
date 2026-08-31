using UnityEngine;

public class DoctorUIFlow : MonoBehaviour
{
    [Header("Screens")]
    public GameObject screen04;
    public GameObject screen05;

    [Header("Managers")]
    public SymptomSelector symptomSelector;
    public VoiceInputManager voiceInputManager;


    private void Awake()
    {
        if (symptomSelector == null)
        {
            symptomSelector =
                GetComponent<SymptomSelector>();
        }

        if (voiceInputManager == null)
        {
            voiceInputManager =
                GetComponent<VoiceInputManager>();
        }
    }


    public void ShowScreen04()
    {
        if (screen04 == null ||
            screen05 == null)
        {
            Debug.LogError(
                "Screen references are missing."
            );

            return;
        }

        screen04.SetActive(true);
        screen05.SetActive(false);
    }


    public void ShowScreen05()
    {
        // Screen references
        if (screen04 == null)
        {
            Debug.LogError(
                "Screen04 is not assigned."
            );

            return;
        }

        if (screen05 == null)
        {
            Debug.LogError(
                "Screen05 is not assigned."
            );

            return;
        }


        // Symptom selector
        if (symptomSelector == null)
        {
            Debug.LogError(
                "SymptomSelector is not assigned."
            );

            return;
        }


        // Voice manager
        if (voiceInputManager == null)
        {
            Debug.LogError(
                "VoiceInputManager is not assigned."
            );

            return;
        }


        // Make sure user selected something
        if (symptomSelector.selectedSymptoms == null ||
            symptomSelector.selectedSymptoms.Count == 0)
        {
            Debug.LogWarning(
                "Select at least one symptom."
            );

            return;
        }


        if (string.IsNullOrEmpty(
            symptomSelector.GeneratedSentence))
        {
            Debug.LogError(
                "No generated sentence available."
            );

            return;
        }


        // Open Screen 05
        screen05.SetActive(true);

        screen04.SetActive(false);


        // Start voice-input process
        voiceInputManager.BeginVoiceAttempt(
            symptomSelector.GeneratedSentence
        );
    }
}