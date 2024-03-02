using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/* Naming Conventions information:
https://learn.microsoft.com/en-us/previous-versions/dotnet/netframework-4.0/ms229043(v=vs.100)?redirectedfrom=MSDN
This will be helpful for identifying methods, variables, classes, etc.
Primarily follows PascalCase conventions.
*/

public class PlayerInteraction : MonoBehaviour
{

    private DialogueManager dialogueScript;
    public GameObject interactButton;
    private bool pushObject;
    private GameObject pushableObject;
    // Start is called before the first frame update
    void Start()
    {
        dialogueScript = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
        pushObject = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(pushObject && Input.GetKeyDown(KeyCode.E)){
            InteractButtonOnPress();
        }
    }

    public void InteractButtonOnPress(){
        //If the dialogue is activatable
        if (dialogueScript.getCanInteract()){
            //Run the activate dialogue
            dialogueScript.ActivateDialogue();
        }
        //Otherwise, this button can be reused for other interactions
        //If this button is in range of other
        else if (pushObject){
            SetButtonVisability(false);
            pushableObject.GetComponent<MovableObstacle>().BeginMoving();
        }
        
    }


    public void canPushObject(GameObject obj){
        pushObject = true;
        pushableObject = obj;
        interactButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "E - Push";
        SetButtonVisability(pushObject);
    }

    public void cannotPushObject(){
        pushObject = false;
        pushableObject = null;
        SetButtonVisability(pushObject);
    }

    public void SetButtonVisability(bool visable){
        interactButton.SetActive(visable);
    }
    
}
