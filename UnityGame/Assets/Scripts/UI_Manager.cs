using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    // This could utilize a List, but then it would be difficult to use/read in the code
    public GameObject startScreen, menu, objectives, creditScreen; 
    private TransitionScript transitionScript;
    // Start is called before the first frame update
    void Start()
    {
        transitionScript = GameObject.Find("Main Camera").GetComponent<TransitionScript>();
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
        transitionScript.FancyFadeIn();
    }

    public void ClickedCredits(){
        Deactivate(startScreen);
        Activate(creditScreen);
    }

    public void ReturnToMenu(){ //This might be changed in the future
        Deactivate(creditScreen);
        Activate(startScreen);
    }

    public void ClickedHelp(){
        Deactivate(menu);
        //Activate(help);
    }

    public void ClickedResume(){
        Deactivate(menu);
    }


}
