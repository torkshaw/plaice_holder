using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string level1;

    public void StartGame()
    {
        SceneManager.LoadScene(level1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}// end of class
// winter james
