using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/* Naming Conventions information:
https://learn.microsoft.com/en-us/previous-versions/dotnet/netframework-4.0/ms229043(v=vs.100)?redirectedfrom=MSDN
This will be helpful for identifying methods, variables, classes, etc.
Primarily follows PascalCase conventions.
*/

/* REDUNDANT ------------------ REPLACED BY GAME MANAGER -------------------
This was originally intended to be the script for NPC interactions, however GameManager.cs covers this issue.
Thus makin this script useless at the moment.
*/
public class PlayerInteraction : MonoBehaviour
{
    private bool dialogueActive;
    public GameObject interactButton;
    private Button button;

    private static bool canInteract;
    // Start is called before the first frame update
    void Start()
    {
        button = interactButton.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dialogueActive && Input.GetKeyDown(KeyCode.E)){
            //Initialize dialogue
            BeginDialogue();
        }
    }

    //To Test user interactions
    public void BeginDialogue(){
        Debug.Log("Hi");
        dialogueActive = true;
    }

    public void OnClick(){
        BeginDialogue();
        button.interactable = false;
        //interactButton.transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>.text = "";
    }




    //Static variables for interact button
    public static void SetCanInteract(bool interact){
        canInteract = interact;
    }
    
    public static bool GetInteractable(){
        return canInteract;
    }
}
