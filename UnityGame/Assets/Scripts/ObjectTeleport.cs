using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTeleport : MonoBehaviour
{
    public GameObject destination; //Initialize this in the editor.
    
    private TransitionScript tranScript;
    private Movement_2D movementScript;
    private ObjectTeleport destScript;
    private bool canTeleport;

    private float TP_COOLDOWN = 1f;

    // Start is called before the first frame update
    void Start()
    {
        //Initializing scripts and variables
        tranScript = GameObject.Find("Main Camera").GetComponent<TransitionScript>();
        movementScript = GameObject.Find("Player").GetComponent<Movement_2D>();
        destScript = destination.GetComponent<ObjectTeleport>();
        canTeleport = true;
    }

    void OnTriggerEnter2D(Collider2D other){
        //Makes sure the the object is the player, and check that teleport is not on cooldown
        if (other.gameObject.tag == "Player" && canTeleport){
            canTeleport = false;
            //Play the transition animation and move the player
            StartCoroutine(transitionAnimation(other.gameObject));
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if (other.gameObject.tag == "Player"){
            //Start the cooldown coroutine
            StartCoroutine(teleportCooldown());
        }
    }

    public void setCanTeleport(bool canTel){
        canTeleport = canTel;
        
    }

    //This manages the cooldown on the teleporter, so players aren't spamming it 
    //and/or being teleported in a loop (however that should be prevented by the program structure)
    IEnumerator teleportCooldown(){
        yield return new WaitForSeconds(TP_COOLDOWN);
        setCanTeleport(true);
    }


    //This handles all the animations for moving the player from one teleporter to another
    IEnumerator transitionAnimation(GameObject player){
        //Stop player movement
        Movement_2D.SetCanMove(false);

        //Prepare to move Player, this makes sure the player isn't instantly teleported back
        destScript.setCanTeleport(false);

        //Play Fade In Transition
        tranScript.FancyFadeOut();
        yield return new WaitForSeconds(2);

        //Move Player
        player.transform.position = destination.transform.position;

        //Play Fade Out Transition
        tranScript.FancyFadeIn();
        yield return new WaitForSeconds(2);

        //Start player movement
        Movement_2D.SetCanMove(true);
    }

}
