using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider; //reference to ui slider whichwill control volume
    [SerializeField] private TMP_Text toggleName; //the name on the soundtrack toggle button

    private void Start()
    {
        volumeSlider.value = AudioListener.volume; // set slider to current volume
        volumeSlider.onValueChanged.AddListener(SetVolume); //listens for changes in slider

        if (MusicManager.instance.ella_OST == true) //
        {                                           // this logic is to check the bool value on the MusicManager instance
            toggleName.text = "Ella";               //
        }                                           // it will then display the name corresponding to the soundtrack version on the UI button
        else                                        //
        {                                           //
            toggleName.text = "Winter";             //
        }                                           //
    }//end of start

    private void SetVolume(float value)//function for volume control
    {
        AudioListener.volume = value; //sets game volume to slider value
    }//end setvolume

    private void OnDestroy()
    {
        volumeSlider.onValueChanged.RemoveListener(SetVolume); // clean up to remove listener if object is removed
    }//end ondestroy

    public void ToggleButtonClicked()// toggle between soundtracks
    {
        MusicManager.instance.SoundtrackToggle(); //call soundtack toggle from MusicManager

        if (MusicManager.instance.ella_OST == true) //
        {                                           // this logic is to check the bool value on the MusicManager instance
            toggleName.text = "Ella";               //
        }                                           // it will then display the name corresponding to the soundtrack version on the UI button
        else                                        //
        {                                           //
            toggleName.text = "Winter";             //
        }                                           //

    }//end togglebuttonclicked

}//end of class
//winter james
