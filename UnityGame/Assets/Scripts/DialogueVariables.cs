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
    public DialogueVariables(/*TextAsset loadGlobalsJSON*/ string globalsFilePath){
        Debug.Log(globalsFilePath);
        //globalVariablesStory = new Story(loadGlobalsJSON.text);
        string inkFileContents = File.ReadAllText(globalsFilePath);
        Ink.Compiler compiler = new Ink.Compiler(inkFileContents);
        globalVariablesStory = compiler.Compile();

        //Initialize Dictionary
        variables = new Dictionary<string, Ink.Runtime.Object>();
        Debug.Log(globalVariablesStory.variablesState);
        foreach(string name in globalVariablesStory.variablesState){
            Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
            Debug.Log(name + " = " + value.ToString());
        }
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

    private void VariablesToStory(Story story){
        Debug.Log("Hello!");
        foreach(KeyValuePair<string, Ink.Runtime.Object> variable in variables){
            Debug.Log("Updating variable "+variable.Key+ " to "+variable.Value);
            story.variablesState.SetGlobal(variable.Key, variable.Value);
            Debug.Log("Variable is now...");
            Ink.Runtime.Object value = story.variablesState.GetVariableWithName(variable.Key);
            Debug.Log(variable.Key + " = " + value.ToString());
        }
        Debug.Log("Story Info:");
    }
}
