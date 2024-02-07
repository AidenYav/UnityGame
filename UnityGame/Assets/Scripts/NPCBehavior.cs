using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCBehavior : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public bool inRange;
    //public TextMeshProUGUI dialogueTextBox;

    private int dialogueCount;
    private List<string> textDialogue = new List<string>();

    //public PlayerInteraction playerInteractionScript;

    private GameManager gameManagerScript;

    // Start is called before the first frame update
    void Start()
    {   
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        //playerInteractionScript = GameObject.Find("Player").GetComponent<PlayerInteraction>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        dialogueCount = -1; //Default starting position

        //Temporary Text Dialogue for proof of concept of npc interaction
        //Will likely read off of a text document (This will mean no web compatability)
        //This current hard-coded dialogue will cause all NPCs to read this exact dialogue
        textDialogue.Add("Hello");
        textDialogue.Add("Hi");
        textDialogue.Add("Line 3");
        textDialogue.Add("End of dialogue.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Triggers when another Collder2D (always the player) enters the NPC's collider range, 
    //which has been manually set to a radius of 1 unit from the NPC. (Forms a 3x3 box)
    private void OnTriggerEnter2D(Collider2D other){
        spriteRenderer.color = Color.red;
        PlayerInteraction.SetCanInteract(true);
        inRange = true;
        gameManagerScript.Interactable(this.gameObject);
    }

    //OnTriggerStay2D isn't strictly necessary, and would trigger hundreds of times in a split second,
    // so maybe just stick with Enter and Exit?
    //private void OnTriggerStay2D(Collider2D other){}


    //Triggers when the Collder2D (Always the player) exits the NPC's collider range.
    //This should automatically shut down any NPC actions.
    private void OnTriggerExit2D(Collider2D other){
        spriteRenderer.color = Color.white;
        inRange = true;
        PlayerInteraction.SetCanInteract(false);
        dialogueCount = -1;
        gameManagerScript.NotInteractable();
    }


    //Returns the current textDialoue count, and increments the count by 1
    //If the next line does not exist, return "Error, no more lines"
    public string NextLine(){
        //Checks if there is a nextLine
        if (HasNext()){
            dialogueCount++;
            return textDialogue[dialogueCount];
        }
        gameManagerScript.DeactivateDialogue();
        return "Error, no more lines";
        
    }

    //Checks if there is a next line to read
    //Returns true if there is, otherwise false
    public bool HasNext(){
        return dialogueCount + 1 < textDialogue.Count;
    }

}
