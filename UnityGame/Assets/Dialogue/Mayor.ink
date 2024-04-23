INCLUDE GlobalVariables.ink

{MayorScene1 : -> Scene1_0 | -> Default }


=== Scene1_0 ===
    MAYOR: Oh! You must be <color=orange>{name}</color>, welcome to Fortune Falls! How was the trip?
        *Good! #Reputation:10
            MAYOR: Jolly!
        *Let's just get to the business, sir. #Reputation:-10
            MAYOR: Alright...
    - MAYOR: Well… firstly, my condolences for the death of your grandmother. She was truly beloved in our small town.
    MAYOR: But, oh! Yes! Silly me, she left a building in her will for you, she made sure I knew it was yours!
    MAYOR: Sadly, there is a price, but I believe in you. It’ll be <color=green>[amount of money]</color>. She hoped she would get well enough to make something of a tavern of it, but she never did…
    MAYOR: You see, this town has been dead for a while, but hopefully, this building will change things!
    * Of course! #Reputation: 5
    MAYOR: Splendid! What will you name your tavern <color=orange>{name}</color>?
    MAYOR: What an excellent name! This will surely get the villagers of Fortune Falls going! Feel free to explore the village and even see the building! It’s definitely in need of some care…
    ~ MayorScene1 = false
    
-> END

=== Default ===
    MAYOR: I hope you are enjoying your time in Fortune Falls!
-> END