using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundManager : MonoBehaviour
{
    public AudioSource audioSource; // Drag your AudioSource here in the Inspector
    public AudioClip buttonClickSound; // Drag your sound clip here in the Inspector

    void Start()
    {
        // Find all buttons in the scene
        Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            // Add a listener to play the sound when the button is clicked
            button.onClick.AddListener(() => PlayButtonSound());
        }
    }

    void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
}