using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System.IO;

/*
Credits to Shaped by Rain Studios aka. Trever Mock for this system.
https://www.youtube.com/watch?v=fA79neqH21s&list=PL3viUl9h9k78KsDxXoAzgQ1yRjhm7p8kl
*/

public class DialogueVariables
{

    public Dictionary<string, Ink.Runtime.Object> variables;
    private Story globalVariablesStory;
<<<<<<< Updated upstream
    public DialogueVariables(/*TextAsset loadGlobalsJSON*/ string globalsFilePath){
        Debug.Log(globalsFilePath);
        //globalVariablesStory = new Story(loadGlobalsJSON.text);
        string inkFileContents = File.ReadAllText(globalsFilePath);
        Ink.Compiler compiler = new Ink.Compiler(inkFileContents);
        globalVariablesStory = compiler.Compile();

        //Initialize Dictionary
        variables = new Dictionary<string, Ink.Runtime.Object>();
        Debug.Log(globalVariablesStory.variablesState);
=======

    private CloudSaveScript saveScript;

    public DialogueVariables(string globalsFilePath, CloudSaveScript saveScript){
        this.saveScript = saveScript;
        //Initialize Dictionary
        variables = new Dictionary<string, Ink.Runtime.Object>();
        //
        LoadData(globalsFilePath);
        //Debug.Log(globalVariablesStory.variablesState);
>>>>>>> Stashed changes
        foreach(string name in globalVariablesStory.variablesState){
            Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
            Debug.Log(name + " = " + value.ToString());
        }
<<<<<<< Updated upstream
=======

        //Cloud Save Integration
        
        
    }
    

    public void LoadData(string globalsFilePath){
        //Loads player data into the ink file.
        if (saveScript.GetValue("StoryData") != null){
            Debug.Log("Loading player story data...");
            globalVariablesStory.state.LoadJson(saveScript.GetValue("StoryData").ToString());

        }
        //Loads the defaults in the inkfile using the file path
        else{
            Debug.Log("Nothing to load at the moment");
            string inkFileContents = File.ReadAllText(globalsFilePath);
            Ink.Compiler compiler = new Ink.Compiler(inkFileContents);
            globalVariablesStory = compiler.Compile();
        }
    }

    private void SaveData(){
        //Syncs up all variable data to the global story
        VariablesToStory(globalVariablesStory);
        saveScript.AddValue("StoryData", globalVariablesStory.state.ToJson());
        
        // foreach(KeyValuePair<string, Ink.Runtime.Object> variable in variables){
        //     //saveScript.AddValue("Story" + variable.Key, JsonConvert.SerializeObject(variable.Value));
        // }
>>>>>>> Stashed changes
    }

    //Methods that will controll whether a story listener is active or not.
    public void StartListening(Story story){
        Debug.Log("Listening!");
        VariablesToStory(story);
        story.variablesState.variableChangedEvent += VariableChanged;
    }

    public void StopListening(Story story){
        story.variablesState.variableChangedEvent -= VariableChanged;
    }


    //This only updates the story variables into this script's dictionary, not actual values in the ink file
    private void VariableChanged(string name, Ink.Runtime.Object value){
        Debug.Log("Variable changed: " + name + " = " + value);
        if(variables.ContainsKey(name)){
            variables.Remove(name);
            variables.Add(name, value);
            Debug.Log("Replaced variable "+name+ " with "+value);
        }
    }

    //This syncs the Dictionary Data *into* the Ink File
    private void VariablesToStory(Story story){
<<<<<<< Updated upstream
        Debug.Log("Hello!");
        foreach(KeyValuePair<string, Ink.Runtime.Object> variable in variables){
            Debug.Log("Updating variable "+variable.Key+ " to "+variable.Value);
            story.variablesState.SetGlobal(variable.Key, variable.Value);
            Debug.Log("Variable is now...");
            Ink.Runtime.Object value = story.variablesState.GetVariableWithName(variable.Key);
            Debug.Log(variable.Key + " = " + value.ToString());
=======
        //For each key-value pair in the dictionary, sync it into the Ink file
        foreach(KeyValuePair<string, Ink.Runtime.Object> variable in variables){
            Debug.Log(variable.Value);
            story.variablesState.SetGlobal(variable.Key, variable.Value);
            Debug.Log(variable.Key + " = " + variable.Value.ToString());
>>>>>>> Stashed changes
        }
        Debug.Log("Story Info:");
    }
}
