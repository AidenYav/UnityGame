using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/* Highest level of puzzle scripts
 * This script manages the randomization
 * of puzzles and connects to the UI Manager
*/

public class PuzzleManager : MonoBehaviour
{

    public GameObject[] puzzles;

    public GameObject[] multiplayerPuzzles;

    //The current puzzle will be recorded using an index, to easily increment puzzles in an accending order.
    [SerializeField] private int currentPuzzle; 

    private UI_Manager uiScript;
    private bool inPuzzle;

    private bool multiplayerActive;

    // Start is called before the first frame update
    void Start()
    {
        uiScript = GameObject.Find("Canvas").GetComponent<UI_Manager>();
        RandomizePuzzle();
    }

    void Update()
    {
        //Resets the puzzle 
        if (Input.GetKeyDown(KeyCode.R) && inPuzzle){
            RestartPuzzle();
        } 
    }



    //Restarts the puzzle
    public void RestartPuzzle(){
        Puzzle puzScript = puzzles[currentPuzzle].GetComponent<Puzzle>();
        puzScript.StopTimer();
        puzScript.ResetObsticles();
        uiScript.MinigameStop();
        uiScript.PlayMinigameAgain();
    }

    //------------------------------------Puzzle Selection Methods---------------------------------------
    public GameObject IncrementPuzzle(){
        currentPuzzle++;//Incraments the puzzle index
        //This prevents an out-of-bounds error, by ensuring the index is always within range
        if(currentPuzzle >= puzzles.Length){
            currentPuzzle = 0;
        }
        return puzzles[currentPuzzle];
    }

    public GameObject SetPuzzle(int idx){
        currentPuzzle = idx;
        //This prevents an out-of-bounds error, by ensuring the index is always within range
        if(currentPuzzle >= puzzles.Length){
            currentPuzzle = 0;
        }
        return puzzles[currentPuzzle];
    }
    //Randomly selects a pre-built puzzle to use
    public GameObject RandomizePuzzle(){
        return puzzles[Random.Range(0,puzzles.Length)];
    }

    //This ensures that the previous puzzle is not selected again
    public GameObject RandomizePuzzle(int prev){
        //The available puzzle set must contain at least 2 puzzles
        //If that is true, this will continously 
        while (puzzles.Length > 1 && currentPuzzle == prev){
            currentPuzzle = Random.Range(0,puzzles.Length);
        }
        return puzzles[currentPuzzle];
    }

    //Returns the current puzzle object
    public GameObject GetCurrentPuzzle(){
        return puzzles[currentPuzzle];
    }

    public int GetCurrentPuzzleIndex(){
        return currentPuzzle;
    }

    //These are for UI purposes and to enable the player to restart the minigame
    public void SetInPuzzle(bool inPuzzle){
        this.inPuzzle = inPuzzle;
    }

    public bool GetInPuzzle(){
        return inPuzzle;
    }

    //This is when the player collides with enterance to the minigame.
    private void OnTriggerEnter2D(Collider2D other){
        if (other.gameObject.tag == "Player"){
            uiScript.PlayMinigameAgain();
        }
    }
}
