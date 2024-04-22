using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using Ink.UnityIntegration;
//Primary Script for Dialogue
//Credits to Shaped by Rain Studios (Trever Mock) for inspiration on this system
//https://www.youtube.com/watch?v=vY0Sk93YUhA&list=PL3viUl9h9k78KsDxXoAzgQ1yRjhm7p8kl&
public class DialogueManager : MonoBehaviour
{

    //------------------Variables for UI Objects--------------------
    //This will be used to update the text box
    //Serialize Field allows the variable to be interactable through the editor, while still being encapsulated by the class.
    [SerializeField] private GameObject dialogueTextBox; //Textbox panel
    [SerializeField] private GameObject interactButton; //Interaction button
    [SerializeField] private GameObject[] choices; //This stores the choice button object, Currently up to 3 choices can be written at the same time

    private TextMeshProUGUI[] choicesText; //This will work with the choices List, this is for convinence and readability

    private TextMeshProUGUI textBox; //Will be used as an alias since pathing to the textbox from the parent game object is unreadable
    
    
    //-----------------Variables for Interacting with NPCs ------------------
    //This will store the npc currently being interacted with
    
    [SerializeField] private TextAsset globalsInkFile;
    [SerializeField] private InkFile globalsFilePath;
    private Story currentStory;

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

    //--------------------Other Scripts-----------------------------

    private CurrencyManager currencyScript;
    private DialogueVariables dialogueVariables;
    private CloudSaveScript saveScript;

    // Start is called before the first frame update
    void Start()
    {
        textBox = dialogueTextBox.transform.Find("Dialogue").gameObject.GetComponent<TextMeshProUGUI>();
        currencyScript = GameObject.Find("GameManager").GetComponent<CurrencyManager>();
        saveScript = GameObject.Find("GameManager").GetComponent<CloudSaveScript>();
        //Initializes the variables used for the choice set-up
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach(GameObject choice in choices){
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            choice.SetActive(false);
            index++;
        }

        StartCoroutine(ResyncDialogueVariables());
    }

    
    public IEnumerator ResyncDialogueVariables(){
        yield return new WaitUntil(() => saveScript.GetDataLoaded());
        if (dialogueVariables == null){
            dialogueVariables = new DialogueVariables(globalsFilePath.filePath, saveScript);
        }
        dialogueVariables.LoadData();
    }

    // Update is called once per frame
    void Update()
    {
        //Continuously checks for User Inputs
        /* The Following is true if:
         *  canContinueDialogue = true                  --- This means the player can continue to the next portion of the dialogue
         *  isInteracting = true                        --- This means the player is currently interacting with the NPC
         *  E OR Mouse Left Click is pressed            --- This means the player wants to proceed with the dialogue by interacting with their mouse or keyboard*/
        if (canContinueDialogue && 
            isInteracting && 
            ( Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0) ) 
            ) {
            UpdateDialogueBox(); //While user is interacting, update text box
        }
        /*Intentionally placed as an else-if so this would trigger last
         *This avoids skipping the first line of the dialogue from double UpdateDialogueBox() calls
         *This setup also makes the Interact UI compatible as a button for clicking 
         * canInteract = true                           --- Player is currently NOT interacting with the NPC, however, they have the ability to do so*/
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
        if (currentStory.canContinue){ //Checks if the story has more text or not
            displayDialogueCoroutine = DisplayLine(currentStory.Continue());
            StartCoroutine(displayDialogueCoroutine);
        }
        else{
            DeactivateDialogue();
        }

    }

    //Initiates Dialogue with NPC
    public void ActivateDialogue(){
        Debug.Log(dialogueVariables);
        dialogueVariables.StartListening(currentStory);
        isInteracting = true;
        dialogueTextBox.SetActive(isInteracting);
        interactButton.SetActive(!isInteracting);
        UpdateDialogueBox();
        Movement_2D.SetCanMove(false);
        
    }

    //Deactivates the dialogue textbox
    public void DeactivateDialogue(){
        dialogueVariables.StopListening(currentStory);
        isInteracting = false;
        textBox.text = "";
        dialogueTextBox.SetActive(isInteracting);
        NotInteractable();
        Movement_2D.SetCanMove(true);
    }

    //Shows interact button and prepare dialogue interaction
    //Takes in the GameObject npc to souce dialogue information
    public void Interactable(TextAsset inkJson){
        currentStory = new Story(inkJson.text);
        canInteract = true;
        canContinueDialogue = canInteract;
        interactButton.transform.Find("NPC-Interact").GetComponent<TextMeshProUGUI>().text = "E - Interact";
        interactButton.SetActive(canInteract);
        
    }

    //Sets all dialogue-related text to false defaults
    public void NotInteractable(){
        currentStory = null;
        canInteract = false;
        isInteracting = canInteract;
        interactButton.SetActive(canInteract);
        dialogueTextBox.SetActive(canInteract);
    }

    private IEnumerator DisplayLine(string line){
        //Clear out any choice buttons that may still be active
        hideChoices(0);
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
        //Once the NPC finishes talking, offer choices
        DisplayChoices();
    }

    //Displays up to 3 available choices
    private void DisplayChoices(){
        List<Choice> currentChoices = currentStory.currentChoices;
        //If the player can make a choice, they must interact with the choice buttons
        if (currentChoices.Count != 0){
            canContinueDialogue = false;
        }

        //Makes sure there aren't too many choices that the UI cannot handle.
        if (currentChoices.Count > choices.Length){
            Debug.LogError("Too many choices were provided." + currentChoices.Count +" was provided, UI can only handle " + choices.Length);
        }
        //Just in case there are too many choices.
        else {
            int index = 0;
            //For each choice provided, enable and initialize the text choices
            foreach(Choice choice in currentChoices){
                choices[index].gameObject.SetActive(true);
                choicesText[index].text = choice.text;
                index++;
            }
            //Hides any extra choice buttons
            hideChoices(index);
        }
    }

    //Hides all choice buttons ranging from [idx,choices.Length)
    private void hideChoices(int idx){
        //This hides all the choice buttons following the player's decision
        for (int i=idx; i<choices.Length; i++){
            choices[i].gameObject.SetActive(false);
        }
    }

    //This is for the choice buttons to record what the player picks.
    //The choiceIndex is hard-coded into the button, and should correspond
    //to the corresponding text choice and name. Ex: Choice0 --> 0
    public void MakeChoice(int choiceIndex){
        currentStory.ChooseChoiceIndex(choiceIndex); //Updates the story to continue accordingly
        currentStory.Continue();//Skip the dialogue of the player's choice
        //Processes the player's decision using Tags
        DecisionTags(currentStory.currentTags);
        

        //This hides all the choice buttons following the player's decision
        hideChoices(0);

        skipLine = false; //This is to prevent the typing animtion to skip right after the player makes a choice
        UpdateDialogueBox(); //Following the player's response, immediately continue the dialogue
    }

    public void DecisionTags(List<string> currentTags){
        foreach(string tag in currentTags){
            //Split the tag
            string[] splitTag = tag.Split(':');
            //Ensures that the tag is approprietly parsed
            if (splitTag.Length != 2){
                Debug.Log("Error with tag, could not parse: " + tag);
            }
            else{
                string tagKey = splitTag[0].Trim();
                string tagValue = splitTag[1].Trim();
                Debug.Log(tagValue);
                //Currently only 1 tag, but this is set up for more tags in the future
                switch(tagKey){
                    case "Reputation":
                        int rep = int.Parse(tagValue);
                        currencyScript.ChangeReputation(rep);
                        break;
                    default:
                        Debug.Log("Error, tag [" + tagKey + "] could not be identified.");
                        break;
                }
            }
        }
    }


    //Getter method for canInteract.
    public bool getCanInteract(){
        return canInteract;
    }


}
