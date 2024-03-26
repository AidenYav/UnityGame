using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
public class CloudSaveScript : MonoBehaviour
{
    private Dictionary<string, object> data = new Dictionary<string, object>();
    private bool dataLoaded = false;

    // Start is called before the first frame update
    async void Start()
    {
        //Will probably add some internet check here
        try
		{
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}"); 
            //data = await CloudSaveService.Instance.Data.LoadAsync();
            //Tester data
            // var data = new Dictionary<string, object>{ { "MySaveKey123", "HelloWorld123" } };
            // await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            
            await LoadData();
            Debug.Log("Successfully loaded data!");
            StartCoroutine(PeriodicSave());

		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}
        dataLoaded = true;
    }


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
    }

    //These warnings described potential issues with not using the await keyword for SaveGame();
    #pragma warning disable CS4014
    public IEnumerator PeriodicSave(){
        Debug.Log("Starting Coroutine");
        while(true){
            //Automatically saves the game every 3 minutes
            yield return new WaitForSeconds(180);
            Debug.Log("Saving");
            SaveGame();

        }
    }

    //Likely linked to a button
    
    public void ManualSave(){
        StopAllCoroutines();//Stops current Periodic Save
        SaveGame(); //Saves the game
        StartCoroutine(PeriodicSave()); //Starts a new Periodic Save
    }
    #pragma warning restore CS4014

    //Simple getter method for other scripts to refer to
    //This will allow other scripts to wait for data loading to be completed
    public bool GetDataLoaded(){
        return dataLoaded;
    }



}
