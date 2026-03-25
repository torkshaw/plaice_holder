using UnityEngine;
using UnityEngine.UI; // to allow use of UI elements
using UnityEngine.SceneManagement; // to allow reloading of scene
public class MenuDebug : MonoBehaviour
{
    [SerializeField] private Button resetButton;
    public void ResetMenu() // Reset the current scene
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // capture the current scene and reload it       

    } // end ResetMenu
}
