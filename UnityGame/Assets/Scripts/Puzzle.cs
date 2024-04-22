using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/* Instance Level script for each puzzle
 * Each puzzle MUST have a start and end point
*/

public class Puzzle : MonoBehaviour
{
    //--------------------Other Scripts-----------------------------
    private UI_Manager uiScript;
    private CurrencyManager currencyScript;
    private PuzzleManager puzzleScript;
    private CloudSaveScript saveScript;

    //--------------------Puzzle Related Variables-----------------------------
    private GameObject[] startAndEndPoints = new GameObject[3]; //Start and End Points
    private TextMeshProUGUI timerUI, resultUI; //UI Objects
    private GameObject obsticles; //An empty object acting as a folder for all obstical objects
    private Vector3[] initialObsticlePosition; //An array containing all obstical initalized positions
    private double time;
    private IEnumerator timer; //Timer Object for each puzzle

    [SerializeField] private double puzzleCashMultiplier, puzzleTimeLimit;
    [SerializeField] private bool multiplayerCompatible;
    void Start()
    {
        startAndEndPoints[0] = transform.Find("StartPoint").gameObject;
        startAndEndPoints[1] = transform.Find("EndPoint").gameObject;
        if(multiplayerCompatible){
            startAndEndPoints[2] = transform.Find("StartPoint2").gameObject;
        }
        obsticles = transform.Find("ObsticleObjects").gameObject;
        initialObsticlePosition = new Vector3[obsticles.transform.childCount];
        initializeObsticles();

        uiScript = GameObject.Find("Canvas").GetComponent<UI_Manager>();
        currencyScript = GameObject.Find("GameManager").GetComponent<CurrencyManager>();
        puzzleScript = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
        saveScript = GameObject.Find("GameManager").GetComponent<CloudSaveScript>();

        time = 0;
        timerUI = uiScript.timer.transform.Find("TimerText").GetComponent<TextMeshProUGUI>();
        resultUI = uiScript.minigameResult.transform.Find("Backboard/ResultText").GetComponent<TextMeshProUGUI>();
    }

    public IEnumerator UpdateTimer(){
        //We can decrease the frequency of updating the timer if we want
        time = 0;//Resets the timer each time
        while(true){
            time += Time.deltaTime;

            yield return null;
            //Update timer text
            timerUI.text = FormatTime(time);
        }
    }

    //t is time in second
    public string FormatTime(double t){
        int min = (int) (t/60);
        int sec = (int) Math.Floor(t%60);
        int centSec =  (int) Math.Floor((t%1) * 100);
        return String.Format("{0}:{1:d2}:{2:d2}",min,sec,centSec);
    }

    #pragma warning disable CS4014
    //Activate the congratulations Screen
    public void ReachedEnd(){
        if (timer != null){
            StopTimer(); //Stops timer

            //Activates the UI for 
            uiScript.MinigameEnd();
            //If the player can complete the puzzle under the specified time, they succeed.
            string result = time < puzzleTimeLimit ? "<color=green>Success</color>" : "<color=red>Failed</color>";

            string stringTime = FormatTime(time);
            //Personal Best Logic
            double personalBest = time;//Default best is the current time
            //But reassign if the best time is not null
            if(saveScript.GetValue("BestTime") != null){
                Debug.Log("BestTime reassigning personalBest");
                personalBest = double.Parse(saveScript.GetValue("BestTime").ToString());
                Debug.Log(personalBest);
            }else{
                Debug.Log("BestTime Does Not Exist.");
            }
            //If the new time is better than the previous personal best, update the data base.
            //While this if statement is slightly redundant, it can be used in the future to
            //Create some sort of bonus effect when achieving a new personal best.
            if(time <= personalBest){
                //If the player achieves a new best time, color their current time green (White by default)
                stringTime = "<color=green>" + stringTime + "</color>";

                personalBest = time;
                //Truncating the data to reduce the number of bytes used when saving data.
                saveScript.AddValue("BestTime", Math.Floor(personalBest * 100)/100 );
                
                //If the player is properly logged in
                if (saveScript.GetSuccessfulLogin()){
                    saveScript.LeaderboardAddScore(Math.Floor(personalBest * 100)/100);
                }
            }


            int earnings = (int) (Math.Floor(1000/time)*puzzleCashMultiplier);
            resultUI.text = String.Format("Result: {0}\nPersonal Best: {1} \nTime: {2} \nEarnings: {3:C}", result, FormatTime(personalBest), stringTime, earnings);
            currencyScript.ChangeMoney(earnings);
        }   
        else{
            Debug.Log("Timer was never started to begin with.");
            time = -1; //To clearly show something is wrong
        }
    }
    #pragma warning restore CS4014
    public void StartPuzzle(){
        puzzleScript.SetInPuzzle(true);
        uiScript.MinigameStart();
        //Stops previous timer coroutines
        StopTimer();
        timer = UpdateTimer();
        StartCoroutine(timer);
    }

    public Vector3 GetStartPosition(){
        return startAndEndPoints[0].transform.position;
    }

    public Vector3 GetStartPosition2(){
        if (multiplayerCompatible){
            return startAndEndPoints[2].transform.position;
        }
        Debug.Log("Position 2 does not exist in puzzle: "+gameObject.name);
        return new Vector3(0,0,0);
    }

    public void ResetObsticles(){

        //Just in case something goes wrong
        if (initialObsticlePosition.Length != obsticles.transform.childCount){
            Debug.Log("Something is wrong, saved positions and obstical counts do not match.\n # of postions: "
             + initialObsticlePosition + "\n # of obsticles: " + obsticles.transform.childCount);
            return;
        }


        //Resets the puzzle to it's original state
        for(int i=0; i < obsticles.transform.childCount; i++){
            obsticles.transform.GetChild(i).transform.position = initialObsticlePosition[i];
        }
    }

    public void initializeObsticles(){
        //Just in case something goes wrong
        if (initialObsticlePosition.Length != obsticles.transform.childCount){
            Debug.Log("Something is wrong, saved positions and obstical counts do not match.\n # of postions: "
             + initialObsticlePosition + "\n # of obsticles: " + obsticles.transform.childCount);
            return;
        }
        
        for(int i=0; i < initialObsticlePosition.Length; i++){
            initialObsticlePosition[i] = obsticles.transform.GetChild(i).transform.position;
        }
    }

    public void StopTimer(){
        if (timer != null){
            StopCoroutine(timer);
            timer = null;
        }
    }

    public bool GetMultiplayerCompatible(){
        return multiplayerCompatible;
    }
}

