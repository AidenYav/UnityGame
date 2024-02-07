using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//Primary Script that connects all other scripts together
public class GameManager : MonoBehaviour
{

    //This will be used to update the text box
    public GameObject dialogueTextBox;
    private TextMeshProUGUI textBox; //Will be used as an alias since pathing to the textbox from the parent game object is unreadable
    public GameObject interactButton;

    //This will store the npc currently being interacted with
    private GameObject npc;

    //This will control if the player can interact with an npc.
    private bool canInteract;

    //If the player is currently in a dialogue (We could use this to stop movement when talking if we wished)
    private bool isInteracting;

    // Start is called before the first frame update
    void Start()
    {
        textBox = dialogueTextBox.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        //Continuously checks for User Inputs
        //When E is pressed, it activates both IF statements.
        //Be aware of that when changing keybinds/logic for this
        if (canInteract && Input.GetKeyDown(KeyCode.E)){
            ActivateDialogue();
        }
        //While user is interacting, update text box
        if (isInteracting && Input.GetKeyDown(KeyCode.E)){
            UpdateDialogueBox();
        }
    }

    //Updates the text in the text box
    public void UpdateDialogueBox(){
        textBox.text = npc.GetComponent<NPCBehavior>().NextLine();
    }

    //Initiates Dialogue with NPC
    public void ActivateDialogue(){
        isInteracting = true;
        dialogueTextBox.SetActive(isInteracting);
        interactButton.SetActive(!isInteracting);
    }

    //Deactivates the dialogue textbox
    public void DeactivateDialogue(){
        isInteracting = false;
        dialogueTextBox.SetActive(isInteracting);
        NotInteractable();
    }

    //Shows interact button and prepare dialogue interaction
    //Takes in the GameObject npc to souce dialogue information
    public void Interactable(GameObject npc){
        this.npc = npc;
        canInteract = true;
        interactButton.SetActive(canInteract);
    }

    //Sets all dialogue-related text to false defaults
    public void NotInteractable(){
        this.npc = null;
        canInteract = false;
        isInteracting = canInteract;
        interactButton.SetActive(canInteract);
        dialogueTextBox.SetActive(canInteract);
    }

}
