using UnityEngine;
using TMPro;

public class ColliderOne : MonoBehaviour //script by JG
{
    public GameObject PopUp1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PopUp1.SetActive(true);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PopUp1.SetActive(false);
        }
    }
}
