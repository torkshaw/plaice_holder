using UnityEngine;
using UnityEngine.SceneManagement; // to allow reloading of scene
public class MenuDebug : MonoBehaviour
{
    public void ResetMenu() // Reset the current scene
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // capture the current scene and reload it       

    } // end ResetMenu

}// end of class
// winter james
