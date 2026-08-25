using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public void GoToConsultation()
    {
        SceneManager.LoadScene("ConsultationScene");
    }
}