using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private DialogueUI dialogueUI; //JG dialogue system
    public DialogueUI DialogueUI => dialogueUI; //JG dialogue system
    public IInteractable Interactable { get; set; } //JG dialogue system

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (dialogueUI.IsOpen) return;

        if (Input.GetKeyDown(KeyCode.E)) //JG dialogue system
        {
            Interactable?.Interact(this);
        }// end of if, JG dialogue system
    }












}
