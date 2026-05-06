using UnityEngine;
using UnityEngine.UI;

public class FetchButtons : MonoBehaviour
{
    
    void OnEnable ()
    {
        Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            button.onClick.RemoveListener(PlaySound);
            button.onClick.AddListener(PlaySound);
        }
    }

    void PlaySound()
    {
        UISFX.instance.PlayClick();
    }

    //void OnDisable()
    //{
    //    foreach (Button button in buttons)
    //    {
    //        button.onClick.RemoveListener(() =>
    //        {
    //            UISFX.instance.PlayClick();
    //        }
    //           );
    //    }
    //}
}
