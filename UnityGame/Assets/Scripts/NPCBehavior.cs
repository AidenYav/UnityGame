using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCBehavior : MonoBehaviour
{
    //--------------------Temporary Variables-----------------------------
    private SpriteRenderer spriteRenderer;

    //--------------------NPC Dialogue File-----------------------------
    public TextAsset inkFile;

    //--------------------Other Scripts-----------------------------
    private DialogueManager dialogueManagerScript;

    // Start is called before the first frame update
    void Start()
    {   
        dialogueManagerScript = GameObject.Find("GameManager").GetComponent<DialogueManager>();
        //playerInteractionScript = GameObject.Find("Player").GetComponent<PlayerInteraction>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //Triggers when another Collder2D (always the player) enters the NPC's collider range, 
    //which has been manually set to a radius of 1 unit from the NPC. (Forms a 3x3 box)
    private void OnTriggerEnter2D(Collider2D other){
        //Makes sure it is the player that entered the NPC's area
        if (other.gameObject.tag == "Player"){
            //spriteRenderer.color = Color.red;
            //inRange = true;
            dialogueManagerScript.Interactable(this.inkFile);
        }
    }

    //OnTriggerStay2D isn't strictly necessary, and would trigger hundreds of times in a split second,
    // so maybe just stick with Enter and Exit?
    //private void OnTriggerStay2D(Collider2D other){}


    //Triggers when the Collder2D (Always the player) exits the NPC's collider range.
    //This should automatically shut down any NPC actions.
    private void OnTriggerExit2D(Collider2D other){
        //Makes sure it is the player that exits the NPC's area
        if (other.gameObject.tag == "Player"){
            //spriteRenderer.color = Color.white;
            //inRange = true;
            dialogueManagerScript.NotInteractable();
        }
    }

}
