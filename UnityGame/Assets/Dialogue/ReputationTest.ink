/*
Use the # symbol to choices to indicate to the script that the choice affects reputation
This will be done as 
#Reputation:[Integer]
[Integer] is a place holder for any positive or negative integer
This integer value will be added to the player's reputation value
+1 Reputation is equal to a 1% increase in base income
-1 Reputation is equal to a 1% decrease in base income
*/

-->main


=== main ===
Hello player...
Would you say that you are a good person? #Test:A
    * [Yes] #Reputation:10
        Thats good. #Test:B
        
    * [No] #Reputation: -10
        Oh...
    
- Anyway, talk to you later!
    
-> END
