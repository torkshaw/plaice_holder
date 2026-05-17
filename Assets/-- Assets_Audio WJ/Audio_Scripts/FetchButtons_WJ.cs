using UnityEngine;
using UnityEngine.UI;

public class FetchButtons : MonoBehaviour
{
    
    void OnEnable () // called when the game object script is attached to is enabled
    {
        Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None); // finds all Buttons currently in the scene
                                                                                       // findObjectsSortMode.None means Unity does not sort the results

        foreach (Button button in buttons)
        {
            button.onClick.RemoveListener(PlaySound); // Removes PlaySound from the button's click event to prevent duplicate listeners from being added                                                      
            button.onClick.AddListener(PlaySound); // Adds the PlaySound method as a listener so whenever the button is clicked, PlaySound() will run
        }
    } //end OnEnable

    void PlaySound() // function to play ui button click sound
    {
        UISFX.instance.PlayClick(); // calls the PlayClick function from the UISFX script
    }//end PlaySound

}//end of class
