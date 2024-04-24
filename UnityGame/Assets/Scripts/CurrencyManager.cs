using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* A high level class designed to solely manage
 * the player's currency
*/

public class CurrencyManager : MonoBehaviour
{
    private CloudSaveScript saveScript;
    [SerializeField] private int playerCash; //Although money is typically a decimal/double, lets keep all money as a whole number
    [SerializeField] private int playerReputation; //This will affect the player's cash multiplier

    [SerializeField] private TextMeshProUGUI cashCount, reputationStat;
    // Start is called before the first frame update
    void Start()
    {
        saveScript = GameObject.Find("GameManager").GetComponent<CloudSaveScript>();
        
        //StartCoroutine(CheckForData());
        //playerCash = 0; //Default is 0. Will change once a save system is implemented.
    }

    private IEnumerator CheckForData(){
        Debug.Log("Awaiting Data");
        yield return new WaitUntil(() => saveScript.GetDataLoaded());
        Debug.Log("Data loaded!");
        //Initializes the data into the script
        playerCash = 0;
        playerReputation = 0;
        if (saveScript.GetValue("Cash") != null){
            playerCash = int.Parse(saveScript.GetValue("Cash").ToString());
        }
        if (saveScript.GetValue("Reputation") != null){
            playerReputation = int.Parse(saveScript.GetValue("Reputation").ToString());
        }
        Debug.Log("Cash initialized at: "+playerCash);
        Debug.Log("Reputation initialized at: "+playerReputation);
        cashCount.text = "Cash: $" + playerCash;
        reputationStat.text = "Reputation: " + playerReputation;
    }


    //Changes the player's current currency count
    //Amount can be positive or negative
    public void ChangeMoney(int amount){
        //Amount is calculated as reputation being a percent out of 100
        //Reputation is clamped between [-99,100]
        amount = (int) Mathf.Floor(amount * (1 + playerReputation / 100f));
        StartCoroutine(MoneyCounterAnimation(playerCash, playerCash + amount));
        //Actual player data is handled here
        playerCash += amount;
        saveScript.AddValue("Cash",playerCash);
        //Leaderboard changes...?
        //saveScript.[LeadereboardCumalativeUpdate](Mathf.Max(amount,0));
    }

    //Only for animation purposes, does not impact actual playerCash data
    public IEnumerator MoneyCounterAnimation(int startAmount, int targetAmount){
        while(startAmount < targetAmount){
            startAmount++;
            cashCount.text = "Cash: $" + startAmount;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public int GetReputaiton(){
        return playerReputation;
    }

    //Adjust reputation accordingly
    public void ChangeReputation(int rep){
        //Reputation is restricted to the range of [-99,100]
        this.playerReputation = Mathf.Clamp(this.playerReputation + rep, -99, 100);
        if(playerReputation >= 0){
            reputationStat.text = "Reputation: <color=green>"+playerReputation+"</color>";
        }
        else{
            reputationStat.text ="Reputation: <color=red>"+playerReputation+"</color>";
        }
        saveScript.AddValue("Reputation",playerReputation);
    }

    //Getter method for the amount of cash the player has.
    public int GetMoney(){
        return playerCash;
    }

    public void ResyncCashData(){
        StartCoroutine(CheckForData());
    }
}
