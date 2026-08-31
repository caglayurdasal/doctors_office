using UnityEngine;

public class HelpSupportUI : MonoBehaviour

{
    public GameObject helpScreen;
    public TMPro.TMP_Text doctorText;

    public TMPro.TMP_Text hintText;
    public TMPro.TMP_Text vocabularyText;

    private bool hintVisible = false;
    private bool vocabularyVisible = false;

    private void Start()
    {
        if (hintText != null)
            hintText.gameObject.SetActive(false);

        if (vocabularyText != null)
            vocabularyText.gameObject.SetActive(false);
    }

    private bool translated = false;
    public void RepeatAudio()
    {
        Debug.Log("Wiederholen clicked");
    }

    public void SlowAudio()
    {
        Debug.Log("Langsam clicked");
    }

    public void Translate()
    {
        translated = !translated;

        if (doctorText != null)
        {
            if (translated)
                doctorText.text = "Doctor: Are you in pain?";
            else
                doctorText.text = "Arzt: Haben Sie Schmerzen?";
        }
    }

    public void ShowHint()
    {
        hintVisible = !hintVisible;

        if (hintText != null)
        {
            hintText.gameObject.SetActive(hintVisible);
        }
    }

    public void ShowVocabulary()
    {
        vocabularyVisible = !vocabularyVisible;

        if (vocabularyText != null)
        {
            vocabularyText.gameObject.SetActive(vocabularyVisible);
        }
    }

    public void Emergency()
    {
        Debug.Log("Emergency clicked");
    }

    public void ExitScreen()
    {
        Debug.Log("Exit clicked");

        if (helpScreen != null)
        {
            Debug.Log("Closing HelpSupportCanvas");
            helpScreen.SetActive(false);
        }
        else
        {
            Debug.LogError("Help Screen has not been assigned!");
        }
    }
}