using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string level1; // string to hold the name of the level 1 scene     


    public void StartGame()
    {
        SceneManager.LoadSceneAsync(level1); // loads the scene held as level1
    } //end startgame

    public void QuitGame()
    {
        Application.Quit();//quit to desktop
    }//end quitgame



}// end of class
// winter james
