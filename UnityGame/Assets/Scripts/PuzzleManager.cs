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

    private GameObject[] puzzles;
    private GameObject[] singlePlayerPuzzles;
    private GameObject[] multiplayerPuzzles;

    //The current puzzle will be recorded using an index, to easily increment puzzles in an accending order.
    [SerializeField] private int currentPuzzle; 

    private UI_Manager uiScript;

    private CameraMovement cameraMovementScript;

    private MultiplayerManager multiplayerScript;
    private bool inPuzzle;

    private bool multiplayerActive;
    public bool zoomActive;
    public bool zoomAllowed;

    // Start is called before the first frame update
    void Start()
    {
        uiScript = GameObject.Find("Canvas").GetComponent<UI_Manager>();
        multiplayerScript = GameObject.Find("GameManager").GetComponent<MultiplayerManager>();
        cameraMovementScript = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
        //Sets all puzzle objects here
        Transform single = this.transform.Find("SinglePlayerPuzzles");
        Transform multi = this.transform.Find("MultiplayerPuzzles");
        singlePlayerPuzzles = new GameObject[single.childCount];
        multiplayerPuzzles = new GameObject[multi.childCount];
        for(int i=0; i<singlePlayerPuzzles.Length; i++) singlePlayerPuzzles[i] = single.GetChild(i).gameObject;
        for(int i=0; i<multiplayerPuzzles.Length; i++) multiplayerPuzzles[i] = multi.GetChild(i).gameObject;
        SetPuzzleType();
        RandomizePuzzle();
    }

    void Update()
    {
        //Resets the puzzle 
        if (Input.GetKeyDown(KeyCode.R) && inPuzzle){
            RestartPuzzle();
        } 
        if (inPuzzle && multiplayerScript.GetIsMultiplayerActivated() && zoomAllowed && (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.RightShift))){
            toggleCameraZoom();
        }
    }

    public void SetPuzzleType(){
        //If multiplayer is activated, load multiplayer puzzles
        //Otherwise load single player puzzles
        puzzles = multiplayerScript.GetIsMultiplayerActivated() ? multiplayerPuzzles : singlePlayerPuzzles;
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
    
    public GameObject NextPuzzle(){
        return SetPuzzle(currentPuzzle + 1);
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

    public void toggleCameraZoom(){
        zoomActive = !zoomActive;
        GameObject puzzleObj = puzzles[currentPuzzle];
        Puzzle puzScript = puzzleObj.GetComponent<Puzzle>();
        //If zoom has been activated
        if (zoomActive) {
            cameraMovementScript.setTarget(puzzleObj.transform.Find("FloorPanel").gameObject);
            cameraMovementScript.zoomToTarget(puzScript.zoomDistance);
        }
        //If zoom has been deactivated
        else{
            cameraMovementScript.setTargetPlayer();
            cameraMovementScript.resetCameraZoom();
        }
    }

}
