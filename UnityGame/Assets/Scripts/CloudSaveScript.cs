using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;

using Newtonsoft.Json;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
public class CloudSaveScript : MonoBehaviour
{
    //This is the dictionary storing all the data loaded
    private Dictionary<string, object> data;
    //These booleans signal to other scripts about how the current state of the player's account regarding account information
    private bool dataLoaded = false, successfulLogin = false;

    //Leaderboard ID, DO NOT CHANGE. This is an ID retrieved from the unity dashboard used to access Unity's leaderboard services
    //Will change when revamping Leaderboard system.
    private const string LEADERBOARD_ID = "Fastest_Times";

    // Start is called before the first frame update
    async void Start()
    {
        ClearData();//Initializes a default dictionary to store data
        //Will probably add some internet check here
        try
		{
            //Must be used to activate cloud-save services
            await UnityServices.InitializeAsync();
		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}
    }

    //--------------------------------------------Authenticator Sign In/Up----------------------------------------------------------------



    //SignOut will clear any current player data AND
    //Will sign out of the authenticator so the player can properly sign up/login
    public void SignOut(){
        try{
            ClearData(); //Clear any current player data
            AuthenticationService.Instance.SignOut();
        }
        catch (Exception e){
            Debug.Log(e);
        }
    }

    //Creates a new account using the passed username and password provided
    public async Task<string> SignUpWithUsernamePassword(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            await AuthenticationService.Instance.UpdatePlayerNameAsync(username); //Sets username to their account username
            Debug.Log("SignUp is successful.");
            successfulLogin = true;
            dataLoaded = true; //Normally would call LoadData(), but there is no data to load (because its a new account), so simply set data is loaded
            ManualSave();
            //If the player has a best time, add it to the leaderboard now that the player has an account
            if(GetValue("BestTime") != null){
                await LeaderboardAddScore(double.Parse(GetValue("BestTime").ToString()));
            }
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            // Debug.Log(ex.Message);
            // Debug.LogException(ex);
            return ex.Message;
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            // Debug.LogException(ex);
            return ex.Message;
        }
        return "";
    }

    //Logs in for the user using the username and password parameters
    public async Task<string> SignInWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            //await AuthenticationService.Instance.UpdatePlayerNameAsync(username);
            Debug.Log("SignIn is successful.");
            successfulLogin = true;
            ClearData(); //Login data should completely override any data. No matter what.
            await LoadData();
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            //Debug.LogException(ex);
            return ex.Message;
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            //Debug.LogException(ex);
            return ex.Message;
        }
        return "";
    }

    //-------------------------------------------------Data Manipulation Methods--------------------------------------------------------------
    public object GetValue(string keyName){
        //If the data field has yet to be initialized
        if (!data.ContainsKey(keyName)){
            //Normally would set a default value here, however, we don't know what data is being saved yet
            //Would likely be best that other scripts handle this save on their own case-by-case context
            return null;
        }
        else{
            return data[keyName];
        }
    }

    //Adds a new value or replaces it
    public void AddValue(string key, object value){
        if(key != null && value != null){
            data[key] = value;
        }
        else{
            Debug.Log("Error, attempted to save null key/value pairs!");
        }
        
    }

    //Saves the player's data
    public async Task<bool> SaveGame(){
        Debug.Log("Saving");
        try{
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            return true;
        }
        catch{
            return false;
        }
    }
    
    //Clears all data in the dictionary
    public void ClearData(){
        data = new Dictionary<string, object>();
    }

    public async Task LoadData(){
        //Clears any pre-existing data for the case of Guest --> Login
        //Login data should completely override any guest data.
        //ClearData();
        //Retrieves a current set of all data values currently stored
        //Using var because I don't know how/what to save these returned values as.
        var currentKeys = await CloudSaveService.Instance.Data.Player.ListAllKeysAsync();
        HashSet<string> currentKeySet = new HashSet<string>();
        for(int i=0; i<currentKeys.Count; i++){
            currentKeySet.Add(currentKeys[i].Key);
        }
        //Queries the cloud save service to return the data fields of the keys
        var data1 = await CloudSaveService.Instance.Data.Player.LoadAsync(currentKeySet);

        //Loads the data into a local dictonary that will be accessed by all scripts
        foreach(string key in data1.Keys){
            this.data.Add(key,data1[key].Value.GetAs<string>());
        }
        dataLoaded = true;
        ManualSave();
    }

    //These warnings described potential issues with not using the await keyword for SaveGame();
    #pragma warning disable CS4014
    public IEnumerator PeriodicSave(){
        //Debug.Log("Starting Coroutine");
        while(true){
            //Automatically saves the game every 3 minutes
            yield return new WaitForSeconds(180);
            //Debug.Log("Saving");
            SaveGame();

        }
    }

    //Likely linked to a button
    
    public async void ManualSave(){
        StopAllCoroutines();//Stops current Periodic Save
        await SaveGame();//Saves the game
        //Debug.Log("Saving..."); 
        StartCoroutine(PeriodicSave()); //Starts a new Periodic Save
    }
    #pragma warning restore CS4014

    //Simple getter method for other scripts to refer to
    //This will allow other scripts to wait for data loading to be completed
    public bool GetDataLoaded(){
        return dataLoaded;
    }

    public bool GetSuccessfulLogin(){
        return successfulLogin;
    }

    public void SetDataLoaded(bool data){
        dataLoaded = data;
    }

    public void SetSuccessfulLogin(bool login){
        successfulLogin = login;
    }

    public string GetPlayerId(){
        if (successfulLogin){
            return AuthenticationService.Instance.PlayerId.ToString();
        }
        return "";
    }
    //------------------Leader Board Code---------------------------
    //Adds the player's best time to the leaderboard
    public async Task LeaderboardAddScore(double time)
    {
        var playerEntry = await LeaderboardsService.Instance
            .AddPlayerScoreAsync(
                LEADERBOARD_ID,
                time);
    }

    //Returns a LeaderboardScoresPage object which can be used to 
    //read through the top 5 leaderboard data.
    public async Task<LeaderboardScoresPage> GetScores()
    {
        var scoresResponse = await LeaderboardsService.Instance
            .GetScoresAsync(LEADERBOARD_ID,
            new GetScoresOptions{Limit = 5});
        return scoresResponse;
        
    }
}
