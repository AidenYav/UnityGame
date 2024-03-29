using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Leaderboards.Models;
/* A Class meant to contain all code related to
 * what should happen following a button press.
*/

public class UI_Manager : MonoBehaviour
{
    // This could utilize a List, but then it would be difficult to use/read in the code
    public GameObject startScreen, menu, objectives, creditScreen,
                        minigameResult, help, timer;

    public GameObject signUp, loginChoices, leaderboardPage;
    private TransitionScript transitionScript;
    private GameObject player;
    private PuzzleManager puzzleScript;

    private CloudSaveScript saveScript;

    private bool signingUp = false;//False for login procedure, true for sign in procedure
    // Start is called before the first frame update

    List<TextMeshProUGUI> leaderNames = new List<TextMeshProUGUI>();
    List<TextMeshProUGUI> leaderScores = new List<TextMeshProUGUI>();
    void Start()
    {
        transitionScript = GameObject.Find("Main Camera").GetComponent<TransitionScript>();
        puzzleScript = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
        saveScript = GameObject.Find("DataManager").GetComponent<CloudSaveScript>();
        player = GameObject.Find("Player");
        Activate(startScreen);
        
        foreach (Transform child in leaderboardPage.transform.GetChild(0)) leaderNames.Add(child.gameObject.GetComponent<TextMeshProUGUI>());
        foreach (Transform child in leaderboardPage.transform.GetChild(1)) leaderScores.Add(child.gameObject.GetComponent<TextMeshProUGUI>());
    }

    //This is really bad that there are a ton of keyboard checks scatter through the scripts
    //I'll probaby fix this if there is time in the future
    //There will probably be new script that would just manage all keyboard inputs
    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (menu.activeSelf){
                ClickedResume();
            }
            else{
                OpenInGameMenu();
            }
        }
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
        Activate(loginChoices);
        //StartCoroutine(WaitForDataLoading());
        
    }
    private IEnumerator WaitForDataLoading(){
        yield return new WaitUntil(() => saveScript.GetDataLoaded());
        transitionScript.FancyFadeIn();
    }

    public void ClickedCredits(){
        Deactivate(startScreen);
        Activate(creditScreen);
    }

    //Helper function for transitions to move players between locations
    //Without the use of teleportation objects (which may overcomplicate things)
    private IEnumerator MovePlayer(Vector3 pos){
        player.GetComponent<Movement_2D>().setCanMove(false);
        transitionScript.FancyFadeOut();
        yield return new WaitForSeconds(2);
        player.transform.position = pos;
        transitionScript.FancyFadeIn();
        yield return new WaitForSeconds(2);
        player.GetComponent<Movement_2D>().setCanMove(true);
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
        puzzleScript.SetInPuzzle(false);
        //Should place them outside of the building/tavern or something? not sure where we should place them yet.
        StartCoroutine(MovePlayer(new Vector3(0,0,0)));
    }

    public void PlayMinigameAgain(){
        Deactivate(minigameResult);
        GameObject newPuzzle = puzzleScript.RandomizePuzzle(puzzleScript.GetCurrentPuzzle());
        StartCoroutine(MovePlayer(newPuzzle.transform.GetComponent<Puzzle>().GetStartPosition()));
    }
    #pragma warning disable CS4014
    public async void CreateAccount(){
        
        //check username validity
        string username = signUp.transform.GetChild(0).gameObject.GetComponent<TMP_InputField>().text;
        //check password validity
        string password1 = signUp.transform.GetChild(1).gameObject.GetComponent<TMP_InputField>().text;
        //A proper login system would be nice to have, especially if this was for a real game.
        //Maybe come back to this issue if there is extra time to develop this.
        //string password2 = signUp.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text;
        //TextMeshProUGUI errorBox = string username = signUp.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();
        if(username.Length < 3 || password1.Length < 3){
            Debug.Log("Username/Password too short");
            return;
        }
        if (signingUp){
            await saveScript.SignUpWithUsernamePassword(username, password1);
        }else{
            await saveScript.SignInWithUsernamePasswordAsync(username, password1);
        }

        //Send values to CloudSaveScript to create an account
        if (saveScript.GetSuccessfulLogin()){
            Deactivate(signUp);
            StartCoroutine(WaitForDataLoading());
        }
        else{
            Debug.Log("Error with " + (signingUp ? "Account Creation" : "Login"));
        }
        
        
    }

    public void LoginOptions(int choice){
        /*0 --> Login/sign in
         *1 --> Sign up
         *2 --> Play as Guest
        */
        if (choice == 0){
            Activate(signUp);
            signUp.transform.GetChild(4).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Sign In";
            signingUp = false;
        }
        else if(choice == 1){
            Activate(signUp);
            signUp.transform.GetChild(4).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Create Account!";
            signingUp = true;
        }
        else{
            saveScript.AnonymousLogin();
            //Load player into game
            StartCoroutine(WaitForDataLoading());

        }
        Deactivate(loginChoices);
    }

    public async void LoadLeaderBoard(){
        Activate(leaderboardPage);
        Deactivate(menu);
        LeaderboardScoresPage page = await saveScript.GetScores();
        //Fill in the names and scores into the texts
        for(int i=0; i<page.Total; i++){
            leaderNames[i].text = (i+1) + ". " + page.Results[i].PlayerName.Substring(0,page.Results[i].PlayerName.Length-5);
            leaderScores[i].text =  page.Results[i].Score.ToString() + " Seconds";
        }
        //Remove extra text boxes
        for(int i=page.Total; i<leaderNames.Count; i++){
            leaderNames[i].text = "";
            leaderScores[i].text = "";
        }
    }
    #pragma warning restore CS4014

    public void CloseLeaderboard(){
        Deactivate(leaderboardPage);
        Activate(menu);
    }

    public void OpenInGameMenu(){
        Activate(menu);
        //If the player is currently in a puzzle, enable the UI button for restarting the puzzle
        if (puzzleScript.GetInPuzzle()){
            Activate(menu.transform.GetChild(menu.transform.childCount-1).gameObject);
        }
        else{
            Deactivate(menu.transform.GetChild(menu.transform.childCount-1).gameObject);
        }
    }

    public void RestartPuzzle(){
        Deactivate(menu);
        puzzleScript.RestartPuzzle();
    }
}
