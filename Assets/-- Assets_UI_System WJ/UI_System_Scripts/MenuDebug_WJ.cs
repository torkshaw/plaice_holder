using UnityEngine;
using UnityEngine.SceneManagement; // to allow reloading of scene
public class MenuDebug : MonoBehaviour
{
    [SerializeField] private string levelSelect1;
    [SerializeField] private string levelSelect2;
    [SerializeField] private string levelSelect3;
    [SerializeField] private string levelSelect4;
    [SerializeField] private string levelSelect5;

    private void Update()
    {
        LevelSelect();
    }

    public void ResetMenu() // Reset the current scene
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // capture the current scene and reload it       

    } // end ResetMenu

    public void LevelSelect() //checks for F1-F5 key press and loads the relevant level
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadSceneAsync(levelSelect1);
        }

        else if (Input.GetKeyDown(KeyCode.F2))
        {
            SceneManager.LoadSceneAsync(levelSelect2);
        }

        else if (Input.GetKeyDown(KeyCode.F3))
        {
            SceneManager.LoadSceneAsync(levelSelect3);
        }

        else if (Input.GetKeyDown(KeyCode.F4))
        {
            SceneManager.LoadSceneAsync(levelSelect4);
        }

        else if (Input.GetKeyDown(KeyCode.F5))
        {
            SceneManager.LoadSceneAsync(levelSelect5);
        }

    } // end levelselect

}// end of class
// winter james
