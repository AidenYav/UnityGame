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
    private Dictionary<string, object> data = new Dictionary<string, object>();
    private bool dataLoaded = false, successfulLogin = false;

    private const string LEADERBOARD_ID = "Fastest_Times";

    // Start is called before the first frame update
    async void Start()
    {
        //Will probably add some internet check here
        try
		{
            await UnityServices.InitializeAsync();
            //await AuthenticationService.Instance.SignInAnonymouslyAsync();
            //Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}"); 
            //data = await CloudSaveService.Instance.Data.LoadAsync();
            //Tester data
            // var data = new Dictionary<string, object>{ { "MySaveKey123", "HelloWorld123" } };
            // await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            
            //----------------------------------Disabled for testing leaderboard, RE-ENABLE LATER------------------------------------------------
            //await LoadData();
            //Debug.Log("Successfully loaded data!");
            //StartCoroutine(PeriodicSave());

		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}
        
        
        // await LeaderboardAddScore();
        // await LeaderboardGetPlayerScore();
    }

    //--------------------------------------------Authenticator Sign In/Up----------------------------------------------------------------
    public async Task AnonymousLogin(){
        try{
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            await LoadData();
        }
        catch(Exception e){
            Debug.Log(e);
        }
    }
    public async Task SignUpWithUsernamePassword(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            await AuthenticationService.Instance.UpdatePlayerNameAsync(username);
            Debug.Log("SignUp is successful.");
            successfulLogin = true;

        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    public async Task SignInWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            //await AuthenticationService.Instance.UpdatePlayerNameAsync(username);
            Debug.Log("SignIn is successful.");
            successfulLogin = true;
            await LoadData();
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    public async Task AddUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.AddUsernamePasswordAsync(username, password);
            await AuthenticationService.Instance.UpdatePlayerNameAsync(username);
            Debug.Log("Username and password added.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
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

    public async Task<bool> SaveGame(){
        try{
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            return true;
        }
        catch{
            return false;
        }
    }

    public async Task LoadData(){
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
        //Debug.Log("Saving..." + ); 
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


    //------------------Leader Board Code---------------------------
    public async Task LeaderboardAddScore(/*string username,*/ double time)
    {
        //var metadata = new Dictionary<string, string>() { {"Username", username} };
        var playerEntry = await LeaderboardsService.Instance
            .AddPlayerScoreAsync(
                LEADERBOARD_ID,
                time/*,
                new AddPlayerScoreOptions { Metadata = metadata}*/);
        //Debug.Log(JsonConvert.SerializeObject(playerEntry));

    }
    public async Task LeaderboardGetPlayerScore()
    {
        var scoreResponse = await LeaderboardsService.Instance
            .GetPlayerScoreAsync(
                LEADERBOARD_ID/*,
                new GetPlayerScoreOptions { IncludeMetadata = true }*/);
        //Debug.Log(JsonConvert.SerializeObject(scoreResponse));
        //Debug.Log("Testing...\nUsernameMetaData: " + JsonConvert.DeserializeObject<Dictionary<string,string>>(scoreResponse.Metadata)["Username"]);
    }

    public async Task<LeaderboardScoresPage> GetScores()
    {
        var scoresResponse = await LeaderboardsService.Instance
            .GetScoresAsync(LEADERBOARD_ID);
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
        //https://docs.unity3d.com/Packages/com.unity.services.leaderboards@2.0/api/Unity.Services.Leaderboards.Models.LeaderboardScoresPage.html
        //https://docs.unity3d.com/Packages/com.unity.services.leaderboards@2.0/api/Unity.Services.Leaderboards.Models.LeaderboardScoresPage.Results.html
        //Debug.Log(scoresResponse.GetType());
        //scoresResponse.Results --> List<LeaderboardEntry>
        //LeaderboardEntry.PlayerName
        //LeaderboardEntry.Score
        return scoresResponse;
        
    }
}
