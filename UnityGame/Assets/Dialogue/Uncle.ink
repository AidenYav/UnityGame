INCLUDE GlobalVariables.ink


{UncleScene1 : -> Scene1 | -> Default}

=== Scene1 ===
    Uncle: Oh is <i>you</i>! What a surprise. What were you doing in there…?
        * Talking with the mayor…
    - Uncle: About…?
        * Grandma left me a building.
    - Uncle: <i>WHAT? THAT WRETCHED OLD WOMAN!</i> Ah hem. Well, <color=orange>{name}</color>, you know your grandma was very… sick in her last days. She must’ve made a grave mistake filling out the will!
        *Grandma made sure that...
        *...the mayor knew it was mine…
    - Uncle: Well... running a business or even living in that building takes an awful lot of time and money. Are you even prepared?
        *Yes. #Reputation:5
    - Uncle: How naive. If you don’t have at least <color=green>{tavernCost}</color>, that building is mine.
    ~   UncleScene1 = false
-> END

=== Default ===
    Uncle: That building will be mine...
-> END