using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* A high level class designed to solely manage
 * the player's currency
*/

public class CurrencyManager : MonoBehaviour
{

    public int playerCash; //Although money is typically a decimal/double, lets keep all money as a whole number

    [SerializeField] private TextMeshProUGUI cashCount;
    // Start is called before the first frame update
    void Start()
    {
        playerCash = 0; //Default is 0. Will change once a save system is implemented.
    }

    //Changes the player's current currency count
    //Amount can be positive or negative
    public void ChangeMoney(int amount){
        playerCash += amount;
        //Add animations for this to appear?
        cashCount.text = "Cash: " + playerCash; //Do we want to name the currency as Cash? Points? Coins? TBD.
    }

    //Getter method for the amount of cash the player has.
    public int getMoney(){
        return playerCash;
    }
}