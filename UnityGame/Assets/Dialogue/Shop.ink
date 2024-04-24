INCLUDE GlobalVariables.ink

{ShopStartScene: -> StartingOut | -> Shop}



=== StartingOut ===
    Towns Person: Greetings! You must be the new arrival!
    * Yes #Reputation:5
        Shopkeeper: I’m the Shopkeeper, I have a supplies shop in the village! I’d be willing to help you with anything supplies-related at any time. We need new faces around this village… definitely needs a revival!
    * Whatever #Reputation:-5
    - Shopkeeper: Well here's some advice... you see that cat over there?
    Shopkeeper: Go talk to them, they can help you earn some money.
    Shopkeeper: When you have enough money, you can come talk to me again!
    ~ ShopStartScene = false
-> END

=== Shop ===
Shopkeeper: Hello! Looks like you’ve got the hang of things around here.
    *I need some help...
    *...gathering the rest of my materials
    -Shopkeeper: Oh! Perhaps you can find the supplies you need here! What do you need?
    *I need some wood and tools
    -Okay! That will be <color=green>$100</color>.
    [Wow! That’s a lot of money! Use this chance to practice your negotiation skills and develop your business leadership skills!]
    *[Negotiate] #Reputation:5
    -You: I’m interested in purchasing this item, but I was hoping we could discuss the price a bit. 
    [Good job! Assertiveness and preparation are important in negotiation. Your initial request is assertive and implies you have done research and planning.]
    Shopkeeper: Why of course! I’m open to negotiation, what price were you thinking?
    You: Well, after looking around and considering my budget, I was hoping we could work out something closer to <color=green>$50</color>.
    Shopkeeper: Hmm, but I’ve priced my items according to their value and market demand. However, because of our good relationship, I’m willing to be flexible! How about we meet halfway at <color=green>$75</color>?
    Shopkeeper: I appreciate your willingness to negotiate and understand the value of the item, but I still need to be mindful of my budget. Would you consider  <color=green>$60</color>?
    [Nice problem-solving! This is evident in both parties’ willingness to explore different options and find a solution that meets both their needs.]
    Shopkeeper: I understand. Let me think about it for a moment… Okay, I’m willing to agree to <color=green>$65</color>.
    [Emotional intelligence is demonstrated by the Shopkeeper’s understanding of your budget constraints and your acknowledgment of the seller’s position. This helps to maintain a positive atmosphere and facilitates compromise. Nice job!]
    You: Thank you for considering my offer! That is manageable!
    Shopkeeper: Splendid my friend, I’m glad we could come to an agreement! Let’s finalize the deal.
    [Wow! Great job exercising your negotiation tactics and becoming a future business leader! You employed them through initial offers, counteroffers, and concessions strategically to influence both the negotiation and outcomes.]
    *Finalize transaction #Reputation:20 #Money:-65
-> END