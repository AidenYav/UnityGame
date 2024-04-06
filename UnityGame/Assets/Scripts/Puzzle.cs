using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* Instance Level script for each puzzle
 * Each puzzle MUST have a start and end point
*/

public class Puzzle : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject start, end;
    private UI_Manager uiScript;
    private CurrencyManager currencyScript;
    private PuzzleManager puzzleScript;
    private CloudSaveScript saveScript;
    private TextMeshProUGUI timerUI, resultUI;
    [SerializeField] private double time; //Serialized for monitoring purposes
    private GameObject obsticles;
    private Vector3[] initialObsticlePosition;
    private IEnumerator timer; //Timer Object for each puzzle
    void Start()
    {
        start = transform.Find("StartPoint").gameObject;
        end = transform.Find("EndPoint").gameObject;
        obsticles = transform.Find("ObsticleObjects").gameObject;
        initialObsticlePosition = new Vector3[obsticles.transform.childCount];
        initializeObsticles();

        uiScript = GameObject.Find("Canvas").GetComponent<UI_Manager>();
        currencyScript = GameObject.Find("CurrencyManager").GetComponent<CurrencyManager>();
        puzzleScript = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
        saveScript = GameObject.Find("DataManager").GetComponent<CloudSaveScript>();

        time = 0;
        timerUI = uiScript.timer.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
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
    public void ReachedEnd(){
        //Activate the congradulations Screen
        
        if (timer != null){
            StopTimer();
        
            uiScript.MinigameEnd();
            string result = time < 60 ? "Success" : "Failed";
            
            double personalBest = time;//Default best is the current time
            //But reassign if the best time is not null
            if(saveScript.GetValue("BestTime") != null){
                personalBest = double.Parse(saveScript.GetValue("BestTime").ToString());
            }
            //If the new time is better than the previous personal best, update the data base.
            //While this if statement is slightly redundant, it can be used in the future to
            //Create some sort of bonus effect when achieving a new personal best.
            if(personalBest <= time){
                personalBest = time;
                //Truncating the data to reduce the number of bytes used when saving data.
                saveScript.AddValue("BestTime", Math.Floor(personalBest * 100)/100 );

                //If the player is properly logged in
                if (saveScript.GetSuccessfulLogin()){
                    saveScript.LeaderboardAddScore(Math.Floor(personalBest * 100)/100);
                }
            }


            int earnings = (int) Math.Floor(1000/time);
            resultUI.text = String.Format("Result: {0}\nPersonal Best: {1} \nTime: {2} \nEarnings: {3:C}", result, FormatTime(personalBest), FormatTime(time), earnings);
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
        return start.transform.position;
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
}
