using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
This script will procedurally generate an idle tycoon UI.
*/


public class IdleTycoonScript : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Data class used to organize and manage the various tycoon data.
    private class TycoonObject{
        //Data Fields
        private string name;
        private int level;
        private int baseIncome;
        private int timeIncrament;//Do we want time incraments to be the same? different? TBD
        //private IEnumerator coroutine;

        //Constructor
        public TycoonObject(string name, int level, int baseIncome){
            this.name = name;
            this.level = level;
            this.baseIncome = baseIncome;

            timeIncrament = 10;
            //coroutine = null;
        }

        //Setter Methods
        public void SetName(string name){
            this.name = name;
        }
        
        public void SetLevel(int level){
            this.level = level;
        }

        public void SetBaseIncome(int baseIncome){
            this.baseIncome = baseIncome;
        }
        //Getter Methods
        public string GetName(){
            return name;
        }

        public int GetLevel(){
            return level;
        }

        public int GetBaseIncome(){
            return baseIncome;
        }

        // public void InitializeTycoon(){
        //     if(coroutine != null){
        //         StopCoroutine(coroutine);
        //     }
        //     coroutine = GenerateIncome();
        //     StartCoroutine(coroutine);
        // }

        public int UpgradeCost(){
            return baseIncome * level * level;
        }

        public string Upgrade(){
            //Successfully Upgrade Tycoon
            /*if(MoneyScript.GetMoney() > UpgradeCost()) {
                MoneyScript.ChangeMoney(-UpgradeCost());
                return "<color=green>Successfully Upgraded " + name + "!";
            }*/
            //Failed to Upgrade Tycoon
            /*else{
                return "<color=red>Insufficient Funds! " + "Requires $" + (UpgradeCost() - MoneyScript.GetMoney()) + "more to upgrade.";
            }
            */
            return "";
        }
        private IEnumerator GenerateIncome(){
            //Should run indefinately, or at least while the player is on the tycoon screen.
            //This while true can be changed to a different statement later.
            while(true){
                //Every certain segment of time, give the player income
                float time = 0f;
                float resetTime = timeIncrament/level;
                while(time < resetTime){
                    //Incrament the time
                    time += Time.deltaTime;
                    //This would update some sort of UI indicator to show how close the player is to 
                    //Recieving their money
                    yield return null;
                }
                //Add player cash here
                //MoneyScript.ChangeMoney(baseIncome * level);
            }
        }
    }
}
