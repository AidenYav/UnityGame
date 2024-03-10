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

    public GameObject currentPuzzle;
    // Start is called before the first frame update
    void Start()
    {
        RandomizePuzzle();
    }

    //Randomly selects a pre-built puzzle to use
    public GameObject RandomizePuzzle(){
        currentPuzzle = puzzles[Random.Range(0,puzzles.Length)];
        return currentPuzzle;
    }

    //This ensures that the previous puzzle is not selected again
    public GameObject RandomizePuzzle(GameObject prev){
        //The available puzzle set must contain at least 2 puzzles
        //If that is true, this will continously 
        while (puzzles.Length > 1 && currentPuzzle != prev){
            currentPuzzle = puzzles[Random.Range(0,puzzles.Length)];
        }
        return currentPuzzle;
    }

    //Returns the current puzzle object
    public GameObject GetCurrentPuzzle(){
        return currentPuzzle;
    }
}
