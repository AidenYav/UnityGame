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

    private bool player1, player2;

    // Start is called before the first frame update
    void Start()
    { 
        puzzleScript = this.transform.parent.GetComponent<Puzzle>();
        movementScript = GameObject.Find("Player").GetComponent<Movement_2D>();

        //Bools for two player puzzles
        player1 = false;
        player2 = true;
        if(puzzleScript.GetMultiplayerCompatible()){
            player2 = false;
        }
    }


    //Once the player touches the end point
    private void OnTriggerEnter2D(Collider2D other){
        if (other.gameObject.tag == "Player"){

            //For the End Point
            //When the player reaches the end, show the results
            if(other.gameObject.name == "Player"){
                player1 = true;
            }
            else if(other.gameObject.name == "Player2"){
                player2 = true;
            }
            //Puzzle only ends when all active players reach the end (Both player 1 and 2 are true)
            //If this is a single player puzzle, player2 will always be true
            if(objectType == type.END && (player1 && player2)){
                puzzleScript.ReachedEnd();
                Movement_2D.SetCanMove(false);
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
            //For multiplayer puzzles.
            //If a player leaves the start/exit piece, set the bool allowing the player to leave to false
            //Both bools must be true, which can only be activated by all active players entering the exit piece
            //By this triggerng on the start piece, it is a safeguard to reset puzzle bools properly
            if(other.gameObject.name == "Player"){
                player1 = false;
            }
            else if(other.gameObject.name == "Player2"){
                player2 = false;
            }
        }
        
    }



}
