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

    private CloudSaveScript saveScript;

    public DialogueVariables(string globalsFilePath, CloudSaveScript saveScript){
        this.saveScript = saveScript;
        //Initialize Dictionary
        variables = new Dictionary<string, Ink.Runtime.Object>();
        LoadData(globalsFilePath);
        //Debug.Log(globalVariablesStory.variablesState);
        foreach(string name in globalVariablesStory.variablesState){
            Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
            Debug.Log(name + " = " + value.ToString());
        }

        //Cloud Save Integration
        
        
    }
    

    public void LoadData(string globalsFilePath){
        //Loads the defaults in the inkfile using the file path
        string inkFileContents = File.ReadAllText(globalsFilePath);
        Ink.Compiler compiler = new Ink.Compiler(inkFileContents);
        globalVariablesStory = compiler.Compile();

        //Loads player data into the ink file, if it exists
        if (saveScript.GetValue("StoryData") != null){
            Debug.Log("Loading player story data...");
            globalVariablesStory.state.LoadJson(saveScript.GetValue("StoryData").ToString());
        }
        else{
            Debug.Log("Nothing to load at the moment");
        }
    }

    private void SaveData(){
        //Syncs up all variable data to the global story
        VariablesToStory(globalVariablesStory);
        saveScript.AddValue("StoryData", globalVariablesStory.state.ToJson());
        
        // foreach(KeyValuePair<string, Ink.Runtime.Object> variable in variables){
        //     //saveScript.AddValue("Story" + variable.Key, JsonConvert.SerializeObject(variable.Value));
        // }
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
        //For each key-value pair in the dictionary, sync it into the Ink file
        foreach(KeyValuePair<string, Ink.Runtime.Object> variable in variables){
            Debug.Log(variable.Value);
            story.variablesState.SetGlobal(variable.Key, variable.Value);
            Debug.Log(variable.Key + " = " + variable.Value.ToString());
        }
        Debug.Log("Story Info:");
    }
}
