using System;
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
                        minigameResult, help, timer, cashCount;

    public GameObject signUp, loginChoices, leaderboardPage;
    private TransitionScript transitionScript;
    private GameObject player;
    private PuzzleManager puzzleScript;

    private CloudSaveScript saveScript;

    private Movement_2D moveScript;

    private CurrencyManager currencyScript;

    private bool signingUp = false;//False for login procedure, true for sign in procedure
    // Start is called before the first frame update

    List<TextMeshProUGUI> leaderNames = new List<TextMeshProUGUI>();
    List<TextMeshProUGUI> leaderScores = new List<TextMeshProUGUI>();
    void Start()
    {
        transitionScript = GameObject.Find("Main Camera").GetComponent<TransitionScript>();
        puzzleScript = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
        saveScript = GameObject.Find("DataManager").GetComponent<CloudSaveScript>();
        currencyScript = GameObject.Find("CurrencyManager").GetComponent<CurrencyManager>();
        player = GameObject.Find("Player");
        moveScript = player.GetComponent<Movement_2D>();
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
        moveScript.SetCanMove(true);
    }

    public void ClickedCredits(){
        Deactivate(startScreen);
        Activate(creditScreen);
    }

    //Helper function for transitions to move players between locations
    //Without the use of teleportation objects (which may overcomplicate things)
    private IEnumerator MovePlayer(Vector3 pos){
        player.GetComponent<Movement_2D>().SetCanMove(false);
        transitionScript.FancyFadeOut();
        yield return new WaitForSeconds(2);
        player.transform.position = pos;
        transitionScript.FancyFadeIn();
        yield return new WaitForSeconds(2);
        player.GetComponent<Movement_2D>().SetCanMove(true);
    }
    //-------------------------------------------------UI Functions/Methods-----------------------------------------------------------
    public void ReturnToMenu(){ //This might be changed in the future
        Deactivate(creditScreen);
        Activate(startScreen);
    }

    public void ClickedHelp(){
        Deactivate(menu);
        Deactivate(cashCount);
        //Activate(help);
    }

    public void ClickedResume(){
        Deactivate(menu);
        Deactivate(cashCount);
        moveScript.SetCanMove(true);
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
        string password2 = signUp.transform.GetChild(2).gameObject.GetComponent<TMP_InputField>().text;
        TextMeshProUGUI errorBox = signUp.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();
        
        if (signingUp){
            //Displays potential sign-up issues
            errorBox.text = SignUpErrors(username,password1,password2);
            //Checks if there are potential issues with sign up
            //Ignores the obvious issue of a username being already taken
            //This is an issue, but not a large one as there should be a virtually infinate number of possible usernames still available considering
            //The game is still in development
            if (errorBox.text == ""){
                errorBox.text = await saveScript.SignUpWithUsernamePassword(username, password1);
                //This is for saving a guset account into a real account
                //By default, it will save nothing on a completely new account
                //But will save pre-existing guest data if they are creating an account
                saveScript.ManualSave(); 
                
            }
        }
        //If the player is not signing up, they must be logging in
        else{
            //If data is loaded (On a guset account), and the player logs in,
            //First signout of the guest account and then login with the players credentials
            if(saveScript.GetDataLoaded()){
                saveScript.SetDataLoaded(false);
                currencyScript.ResyncCashData();
            }
            errorBox.text = await saveScript.SignInWithUsernamePasswordAsync(username, password1);
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

    //Helper methods because username/password debugging has so many restrictions
    private string SignUpErrors(string username, string password1, string password2){
        string errors = "";
        if(username.Length<3)               errors += "Username must have at least 3 characters\n";
        else if(username.Length>20)         errors += "Usernames cannot have more than 20 characters\n";
        if(password1 != password2)          errors += "Passwords do not match\n";
        if(password1.Length < 8)            errors += "Password must be at least 8 characters\n";
        if(!ContainsUppercase(password1))   errors += "Password must contain an uppercase letter\n";
        if(!ContainsLowercase(password1))   errors += "Password must contain an lowercase letter\n";
        if(!ContainsDigit(password1))       errors += "Password must contain a numerical digit\n";
        if(!ContainsSpecialChar(password1)) errors += "Password must contain a symbol\n";
        return errors;
    }

    private bool ContainsUppercase(string s){
        foreach(char c in s){
            if(Char.IsUpper(c)){
                return true;
            }
        }
        return false;
    }

    private bool ContainsLowercase(string s){
        foreach(char c in s){
            if(Char.IsLower(c)){
                return true;
            }
        }
        return false;
    }

    private bool ContainsDigit(string s){
        foreach(char c in s){
            if(Char.IsDigit(c)){
                return true;
            }
        }
        return false;
    }
    private bool ContainsSpecialChar(string s){
        foreach(char c in s){
            if(!Char.IsLetterOrDigit(c)){
                return true;
            }
        }
        return false;
    }

    public void BackToLoginChoices(){
        Activate(loginChoices);
        Deactivate(signUp);
    }

    public void BackToStartScreen(){
        Activate(startScreen);
        Deactivate(loginChoices);
    }

    public void MenuToLoginChoices(){
        Activate(loginChoices);
        Deactivate(menu);
        Deactivate(cashCount);
        if(saveScript.GetSuccessfulLogin()){
            saveScript.SignOut();
            saveScript.SetSuccessfulLogin(false);
        }
    }

    public void LoginOptions(int choice){
        /*0 --> Login/sign in
         *1 --> Sign up
         *2 --> Play as Guest
        */
        if (choice == 0){
            Activate(signUp);
            //Retrieves the button under the sign-up page to set as "Sign In"
            signUp.transform.GetChild(4).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Sign In";
            Deactivate(signUp.transform.GetChild(2).gameObject);
            signingUp = false;
        }
        else if(choice == 1){
            Activate(signUp);
            //Retrieves the button under the sign-up page to set as "Create Account!"
            signUp.transform.GetChild(4).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Create Account!";
            Activate(signUp.transform.GetChild(2).gameObject);
            signingUp = true;
        }
        else{
            //Load player into game
            saveScript.SetDataLoaded(true); //So the game can load
            StartCoroutine(WaitForDataLoading());

        }
        currencyScript.ResyncCashData();
        Deactivate(loginChoices);
    }

    public async void LoadLeaderBoard(){
        Activate(leaderboardPage);
        Deactivate(menu);
        Deactivate(cashCount);
        LeaderboardScoresPage page = await saveScript.GetScores();
        //Fill in the names and scores into the texts
        for(int i=0; i<page.Total; i++){ //Page total should always be 5 or less
            //Extra text effect to identify players on the leaderboard
            string playerIdentifier = page.Results[i].PlayerId.ToString() == saveScript.GetPlayerId() ? "<color=\"green\">" : "";
            //Update the leaderboard
            leaderNames[i].text = (i+1) + ". " + playerIdentifier + page.Results[i].PlayerName.Substring(0,page.Results[i].PlayerName.Length-5);
            leaderScores[i].text = playerIdentifier + page.Results[i].Score.ToString() + " Seconds";
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
        Activate(cashCount);
    }

    public void OpenInGameMenu(){
        Activate(menu);
        Activate(cashCount);
        moveScript.SetCanMove(false);
        //If the player is currently in a puzzle, enable the UI button for restarting the puzzle
        if (puzzleScript.GetInPuzzle()){
            Activate(menu.transform.Find("Restart Puzzle").gameObject);
        }
        else{
            Deactivate(menu.transform.Find("Restart Puzzle").gameObject);
        }
        if (saveScript.GetSuccessfulLogin()){
            Activate(menu.transform.Find("Save").gameObject);
            Activate(menu.transform.Find("Leaderboard").gameObject);
            menu.transform.Find("Login_SignUp/Text").GetComponent<TextMeshProUGUI>().text = "Sign Out";
        }
        else{
            Deactivate(menu.transform.Find("Save").gameObject);
            Deactivate(menu.transform.Find("Leaderboard").gameObject);
            menu.transform.Find("Login_SignUp/Text").GetComponent<TextMeshProUGUI>().text = "Login/Sign Up";
        }
    }

    public void RestartPuzzle(){
        Deactivate(menu);
        Deactivate(cashCount);
        puzzleScript.RestartPuzzle();
    }

}
