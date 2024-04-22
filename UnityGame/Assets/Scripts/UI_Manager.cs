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

    //-----------------------------UI Object Variables--------------------------------------

    // This could utilize a List, but then it would be difficult to use/read in the code
    public GameObject startScreen, menu, objectives, creditScreen,
                        minigameResult, help, timer, cashCount;

    public GameObject signUp, loginChoices, leaderboardPage;
    public TMP_FontAsset retroFont, liberationSansFont;

    //---------------------------------Other Scripts--------------------------------------
    private PuzzleManager puzzleScript;
    private CloudSaveScript saveScript;
    private CurrencyManager currencyScript;
    private TransitionScript transitionScript, transitionScript2;
    private MultiplayerManager multiplayerManager;

    //--------------------Player data related variables-----------------------------
    private bool signingUp = false;//False for login procedure, true for sign in procedure
    // Start is called before the first frame update

    List<TextMeshProUGUI> leaderNames = new List<TextMeshProUGUI>();
    List<TextMeshProUGUI> leaderScores = new List<TextMeshProUGUI>();

    //--------------------Player related variables-----------------------------
    private GameObject player, player2;

    void Start()
    {
        transitionScript = GameObject.Find("Main Camera").GetComponent<TransitionScript>();
        transitionScript2 = GameObject.Find("Camera2").GetComponent<TransitionScript>();
        multiplayerManager = GameObject.Find("GameManager").GetComponent<MultiplayerManager>();
        puzzleScript = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
        saveScript = GameObject.Find("GameManager").GetComponent<CloudSaveScript>();
        currencyScript = GameObject.Find("GameManager").GetComponent<CurrencyManager>();
        player = GameObject.Find("Player");
        player2 = GameObject.Find("Player2");
        SetAllFont(this.transform, retroFont);

        Activate(startScreen);
        
        foreach (Transform child in leaderboardPage.transform.Find("Names")) leaderNames.Add(child.gameObject.GetComponent<TextMeshProUGUI>());
        foreach (Transform child in leaderboardPage.transform.Find("Score")) leaderScores.Add(child.gameObject.GetComponent<TextMeshProUGUI>());
    }

    //Sets all UI elements to a uniform font.
    //@parent - This and all it's children will use the new font
    //@newFont - The font asset file which is inputted in editor on the Canvas Object
    private void SetAllFont(Transform parent,TMP_FontAsset newFont){
        for(int i = 0; i<parent.childCount; i++){
            //Debug.Log(parent.GetChild(i));
            //Recursively loops through the hiearchy
            SetAllFont(parent.GetChild(i),newFont);
        }
        //If the object is a TMP text box, set the new font
        var tmp = parent.GetComponent<TextMeshProUGUI>();
        if (tmp != null){
            tmp.font = newFont;
        }
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
        Movement_2D.SetCanMove(true);
    }

    public void ClickedCredits(){
        Deactivate(startScreen);
        Activate(creditScreen);
    }

    //Helper function for transitions to move players between locations
    //Without the use of teleportation objects (which may overcomplicate things)
    //leavePuzzle = player is going from puzzle to open world
    //!leavePuzzle = player is going from open world to puzzle = enterPuzzle
    private IEnumerator MovePlayer(Vector3 pos1, Vector3 pos2, bool leavePuzzle){
        Movement_2D.SetCanMove(false); //Deactivate all movement
        
        //If the player has multiplayer on AMD is trying to leave the puzzle (implying they are currently in the puzzle),
        //Then fade both characters out before moving player 1 and deactivating player 2
        transitionScript.FancyFadeOut();
        if(multiplayerManager.GetIsMultiplayerActivated() && leavePuzzle){
            //Assumes Camera2 and Player2 are activated
            transitionScript2.FancyFadeOut();
            yield return new WaitForSeconds(2);//Wait for the transition to complete
            multiplayerManager.DeactivateMultiplayer(); //Deactivate multiplayer
        }else{
            yield return new WaitForSeconds(2);
        }

        player.transform.position = pos1;
        
        if (multiplayerManager.GetIsMultiplayerActivated() && !leavePuzzle){
            player2.transform.position = pos2;
        }
        transitionScript.FancyFadeIn();
        if(multiplayerManager.GetIsMultiplayerActivated() && !leavePuzzle){
            multiplayerManager.ActivateMultiplayer();
            //yield return new WaitForSeconds(2);
            transitionScript2.FancyFadeIn();
        }
        else{
            yield return new WaitForSeconds(2);
        }
        Movement_2D.SetCanMove(true);
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
        Movement_2D.SetCanMove(true);
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
        puzzleScript.GetCurrentPuzzle().GetComponent<Puzzle>().ResetObsticles();
        //Should place them outside of the building/tavern or something? not sure where we should place them yet.
        StartCoroutine(MovePlayer(new Vector3(0,0,0), new Vector3(0,0,0), true));
    }

    public void PlayMinigameAgain(){
        Deactivate(minigameResult);
        GameObject newPuzzle = puzzleScript.RandomizePuzzle(/*puzzleScript.GetCurrentPuzzleIndex()*/);
        //These variables are less efficient, however this makes it easier to read the code
        Vector3 pos1 = newPuzzle.transform.GetComponent<Puzzle>().GetStartPosition();

        Vector3 pos2 = multiplayerManager.GetIsMultiplayerActivated() ? newPuzzle.transform.GetComponent<Puzzle>().GetStartPosition2() : new Vector3(0,0,0);
        
        StartCoroutine(MovePlayer(pos1, pos2, false));
    }
    #pragma warning disable CS4014
    public async void CreateAccount(){
        
        //check username validity
        string username = signUp.transform.Find("Username").gameObject.GetComponent<TMP_InputField>().text;
        //check password validity
        string password1 = signUp.transform.Find("Password1").gameObject.GetComponent<TMP_InputField>().text;
        //A proper login system would be nice to have, especially if this was for a real game.
        //Maybe come back to this issue if there is extra time to develop this.
        string password2 = signUp.transform.Find("Password2").gameObject.GetComponent<TMP_InputField>().text;
        TextMeshProUGUI errorBox = signUp.transform.Find("Errors").gameObject.GetComponent<TextMeshProUGUI>();
        
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
            //This also means all player data on the guest account is cleared
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

    //Accepts the submitted username, password, and the password confirmation
    //Ensures that account details fit specific criteria regarding account creation
    //And returns a string of error messages of what the player can/cannot do.
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

    //Checks that the string contains an uppercase character
    private bool ContainsUppercase(string s){
        foreach(char c in s){
            if(Char.IsUpper(c)){
                return true;
            }
        }
        return false;
    }

    //Checks that the string contains an lowercase character
    private bool ContainsLowercase(string s){
        foreach(char c in s){
            if(Char.IsLower(c)){
                return true;
            }
        }
        return false;
    }

    ///Checks that the string contains an numerical digit
    private bool ContainsDigit(string s){
        foreach(char c in s){
            if(Char.IsDigit(c)){
                return true;
            }
        }
        return false;
    }

    //Checks that the string contains a symbol character
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
        Deactivate(creditScreen);//This was also used for some reason?
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
        //We need to keep this font because it enables lower/capital case characters to be visable, which is critical for login information
        signUp.transform.Find("Username").GetComponent<TMP_InputField>().fontAsset = liberationSansFont;
        signUp.transform.Find("Password1").GetComponent<TMP_InputField>().fontAsset = liberationSansFont;
        signUp.transform.Find("Password2").GetComponent<TMP_InputField>().fontAsset = liberationSansFont;

        //Login Option
        if (choice == 0){
            Activate(signUp);
            //Retrieves the button under the sign-up page to set as "Sign In"
            signUp.transform.Find("CreateAccount/Wood/Button Container/Button/Text").gameObject.GetComponent<TextMeshProUGUI>().text = "Sign In";
            //Updates other elements in the UI to properly match the context of the situation
            signUp.transform.Find("CreateNewAccountText").GetComponent<TextMeshProUGUI>().text = "Log in to your account";
            signUp.transform.Find("UsernamePrompt").GetComponent<TextMeshProUGUI>().text = "Please Enter Your Username:";
            signUp.transform.Find("PasswordPrompt").GetComponent<TextMeshProUGUI>().text = "Please Enter Your Password:";
            Deactivate(signUp.transform.Find("Password2").gameObject);
            signingUp = false;
        }
        //Create new account Option
        else if(choice == 1){
            Activate(signUp);
            //Retrieves the button under the sign-up page to set as "Create Account!"
            signUp.transform.Find("CreateAccount/Wood/Button Container/Button/Text").gameObject.GetComponent<TextMeshProUGUI>().text = "Create Account!";
            //Updates other elements in the UI to properly match the context of the situation
            signUp.transform.Find("CreateNewAccountText").GetComponent<TextMeshProUGUI>().text = "Create a new account";
            signUp.transform.Find("UsernamePrompt").GetComponent<TextMeshProUGUI>().text = "Please Create a Username:";
            signUp.transform.Find("PasswordPrompt").GetComponent<TextMeshProUGUI>().text = "Please Create a Password:";
            Activate(signUp.transform.Find("Password2").gameObject);
            signingUp = true;
        }
        //Guest Option
        else{
            //Load player into game
            saveScript.SetDataLoaded(true); //So the game can load
            StartCoroutine(WaitForDataLoading());

        }
        currencyScript.ResyncCashData();
        Deactivate(loginChoices);
        Deactivate(transitionScript2.gameObject); //Cam2 was activated by default so the object could be properly initalized, but it needs to be deactivated before loading in the game
        Deactivate(player2); //Player 2 should always be deactivated when starting the game since multiplayer won't be activated
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
        Movement_2D.SetCanMove(false);
        //If the player is currently in a puzzle, enable the UI button for restarting the puzzle
        if (puzzleScript.GetInPuzzle()){
            Activate(menu.transform.Find("Restart Puzzle").gameObject);
        }
        else{
            Deactivate(menu.transform.Find("Restart Puzzle").gameObject);
        }
        //If the player is logged in (and connected to unity services)
        if (saveScript.GetSuccessfulLogin()){
            //Enable their options to view leaderboards and save the game
            Activate(menu.transform.Find("Save").gameObject);
            Activate(menu.transform.Find("Leaderboard").gameObject);
            menu.transform.Find("Login_SignUp/Text").GetComponent<TextMeshProUGUI>().text = "Sign Out";
        }
        //Otherwise, if they are not logged in
        else{
            //Disable their ability to save or view leaderboard (since these require a connection to unity services)
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
