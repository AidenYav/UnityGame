using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//Primary Script that connects all other scripts together
public class DialogueManager : MonoBehaviour
{

    //------------------Variables for UI Objects--------------------
    //This will be used to update the text box
    public GameObject dialogueTextBox;
    private TextMeshProUGUI textBox; //Will be used as an alias since pathing to the textbox from the parent game object is unreadable
    public GameObject interactButton;


    //-----------------Variables for Interacting with NPCs ------------------
    //This will store the npc currently being interacted with
    private GameObject npc;

    //This will control if the player can interact with an npc.
    private bool canInteract;

    //If the player is currently in a dialogue (We could use this to stop movement when talking if we wished)
    private bool isInteracting;


    //------------------Variables for Typing effect------------------
    private float typingSpeed = 0.04f; //Seconds per each character "typed" in dialogue

    //Used to store the coroutine being run for the typing animation
    private IEnumerator displayDialogueCoroutine;

    //Boolean to control the player from skipping dialogue
    private bool canContinueDialogue = true;
    
    private bool isAddingRichTextTag = false;

    private bool skipLine = false;
    // Start is called before the first frame update
    void Start()
    {
        textBox = dialogueTextBox.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        //Continuously checks for User Inputs
        if (canContinueDialogue && isInteracting && (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))) {
            UpdateDialogueBox(); //While user is interacting, update text box
        }
        /*Intentionally placed as an else-if so this would trigger last
         *This avoids skipping the first line of the dialogue from double UpdateDialogueBox() calls
         *This setup also makes the Interact UI compatible as a button for clicking */
        else if (canContinueDialogue && canInteract && Input.GetKeyDown(KeyCode.E)){
            ActivateDialogue();
        }
        /* Allows the user to skip the dialogue if the following conditions are met:
         *  canContinueDialogue = false                 --- This implies that the dialogue animation is currently running
         *  E or Left click is triggered                --- This means the player wants to skip the dialogue
         *  textBox's text length is greater than 10    --- The text animation has displayed at least 10 characters */
        else if (!canContinueDialogue 
                && (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                && textBox.text.Length > 10)
                {
            skipLine = true;
        }
        

    }

    //Updates the text in the text box
    public void UpdateDialogueBox(){
        
        //If there is a dialogue coroutine still running, stop/delete it
        if (displayDialogueCoroutine != null){
            StopCoroutine(displayDialogueCoroutine);
        }
        //Creates a new coroutine to begin the typing effect
        displayDialogueCoroutine = DisplayLine(npc.GetComponent<NPCBehavior>().NextLine());
        StartCoroutine(displayDialogueCoroutine);

    }

    //Initiates Dialogue with NPC
    public void ActivateDialogue(){
        isInteracting = true;
        dialogueTextBox.SetActive(isInteracting);
        interactButton.SetActive(!isInteracting);
        UpdateDialogueBox();
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
        interactButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "E - Interact";
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

    private IEnumerator DisplayLine(string line){
        //clear out any dialogue
        textBox.text = "";
        //Boolean to control the player from skipping dialogue
        canContinueDialogue = false;
        //Uses a for loop to type each individual character into the dialogue box
        //To create the "typing" effect
        foreach(char letter in line.ToCharArray()){

            //To skip parts of dialogue
            //skipLine can be toggled true by the Update function
            if (skipLine){
                textBox.text = line;
                skipLine = false;
                break;
            }
            //Implementation for RichText if we decide to use it
            //Looks for the opening tag of a RichTextTag
            if (letter == '<' || isAddingRichTextTag){
                isAddingRichTextTag = true;
                textBox.text += letter;
                //Looks for the closing tag of a RichTextTag
                if (letter == '>'){
                    isAddingRichTextTag = false;
                }
            }
            else{
                textBox.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        //Dialogue animation is complete
        canContinueDialogue = true;
    }


    public bool getCanInteract(){
        return canInteract;
    }


}
