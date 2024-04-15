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
    public int playerCash; //Although money is typically a decimal/double, lets keep all money as a whole number
    private int reputation; //This will affect the player's cash multiplier

    [SerializeField] private TextMeshProUGUI cashCount;
    // Start is called before the first frame update
    void Start()
    {
        saveScript = GameObject.Find("DataManager").GetComponent<CloudSaveScript>();
        
        //StartCoroutine(CheckForData());
        //playerCash = 0; //Default is 0. Will change once a save system is implemented.
    }

    private IEnumerator CheckForData(){
        Debug.Log("Awaiting Data");
        yield return new WaitUntil(() => saveScript.GetDataLoaded());
        Debug.Log("Data loaded!");
        //Initializes the data into the script
        playerCash = 0;
        if (saveScript.GetValue("Cash") != null){
            playerCash = int.Parse(saveScript.GetValue("Cash").ToString());
        }
        Debug.Log("Value initialized at: "+playerCash);
        cashCount.text = "Cash: $" + playerCash;
    }


    //Changes the player's current currency count
    //Amount can be positive or negative
    public void ChangeMoney(int amount){
        amount = (int) Mathf.Floor(amount*reputation/100f);
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
        return reputation;
    }

    public void ChangeReputation(int rep){
        this.reputation += rep;
    }

    //Getter method for the amount of cash the player has.
    public int GetMoney(){
        return playerCash;
    }

    public void ResyncCashData(){
        StartCoroutine(CheckForData());
    }
}
