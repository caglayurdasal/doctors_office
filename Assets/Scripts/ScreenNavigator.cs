using UnityEngine;

public class ScreenNavigator : MonoBehaviour
{
    public GameObject currentScreen;
    public GameObject nextScreen;

    public void GoToNextScreen()
    {
        currentScreen.SetActive(false);
        nextScreen.SetActive(true);
    }
}