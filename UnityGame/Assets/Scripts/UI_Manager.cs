using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* A Class meant to contain all code related to
 * what should happen following a button press.
*/

public class UI_Manager : MonoBehaviour
{
    // This could utilize a List, but then it would be difficult to use/read in the code
    public GameObject startScreen, menu, objectives, creditScreen,
                        minigameResult, help, timer;
    private TransitionScript transitionScript;
    private GameObject player;
    private PuzzleManager puzzleScript;
    // Start is called before the first frame update
    void Start()
    {
        transitionScript = GameObject.Find("Main Camera").GetComponent<TransitionScript>();
        puzzleScript = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
        player = GameObject.Find("Player");
        Activate(startScreen);
    }

    //Helper functions to help with readability
    private void Activate(GameObject obj){
        obj.SetActive(true);
    }

    private void Deactivate(GameObject obj){
        obj.SetActive(false);
    }


    //Clicker Functions - Likely will add more to these later on
    public void ClickedPlay(){
        Deactivate(startScreen);
        transitionScript.FancyFadeIn();
    }

    public void ClickedCredits(){
        Deactivate(startScreen);
        Activate(creditScreen);
    }

    //Helper function for transitions to move players between locations
    //Without the use of teleportation objects (which may overcomplicate things)
    private IEnumerator MovePlayer(Vector3 pos){
        transitionScript.FancyFadeOut();
        yield return new WaitForSeconds(2);
        player.transform.position = pos;
        transitionScript.FancyFadeIn();
    }
    //-------------------------------------------------UI Functions/Methods-----------------------------------------------------------
    public void ReturnToMenu(){ //This might be changed in the future
        Deactivate(creditScreen);
        Activate(startScreen);
    }

    public void ClickedHelp(){
        Deactivate(menu);
        //Activate(help);
    }

    public void ClickedResume(){
        Deactivate(menu);
    }

    public void MinigameEnd(){
        Activate(minigameResult);
        Deactivate(timer);
    }

    public void MinigameStart(){
        Activate(timer);
        //StartCoroutine(MovePlayer(puzzleScript.GetCurrentPuzzle().transform.GetComponent<Puzzle>().GetStartPosition()));
    }

    public void MinigameStop(){
        Deactivate(timer);
    }

    public void LeaveMiniGame(){
        Deactivate(minigameResult);
        //Should place them outside of the building/tavern or something? not sure where we should place them yet.
        StartCoroutine(MovePlayer(new Vector3(0,0,0)));
    }

    public void PlayMinigameAgain(){
        Deactivate(minigameResult);
        GameObject newPuzzle = puzzleScript.RandomizePuzzle();
        StartCoroutine(MovePlayer(newPuzzle.transform.GetComponent<Puzzle>().GetStartPosition()));
    }


}
