using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* Short script to manage the start and end 
* point components of a puzzle
*/
public class PuzzleStartEnd : MonoBehaviour
{

    public enum type{
        START,
        END
    }

    public type objectType;

    private Puzzle puzzleScript;
    private Movement_2D movementScript;

    // Start is called before the first frame update
    void Start()
    { 
        puzzleScript = this.transform.parent.GetComponent<Puzzle>();
        movementScript = GameObject.Find("Player").GetComponent<Movement_2D>();
    }


    //Once the player touches the end point
    private void OnTriggerEnter2D(Collider2D other){
        if (other.gameObject.tag == "Player"){
            //For the End Point
            //When the player reaches the end, show the results
            if(objectType == type.END){
                puzzleScript.ReachedEnd();
                movementScript.setCanMove(false);
            }
        }
    }

    //Once the player leaves the start point
    private void OnTriggerExit2D(Collider2D other){
        if (other.gameObject.tag == "Player"){
            //For the Start Point
            //When the player leaves, begin the puzzle timer
            if(objectType == type.START){
                puzzleScript.StartPuzzle();
            }
            //Otherwise, this object is END object
            else{
                puzzleScript.ResetObsticles();
                movementScript.setCanMove(true);
            }
        }
        
    }



}
