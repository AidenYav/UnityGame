INCLUDE GlobalVariables.ink


{LevelIntroScene : -> Intro | -> LevelSelect}

=== Intro ===
    Hello {name}!
    Next to me is the building you inherited.
    The Mayor has told me to help you learn a couple of basics.
    You can enter this door right here to enter a <color=blue>puzzle</color>.
    When ever you would like to change the <color=blue>puzzle level</color>, just talk to me again!
    ~ LevelIntroScene = false
-> END

=== LevelSelect ===
    Hello again {name}!
    Please select a puzzle you would like to play:
        *Level 1 #Level:1
        *Level 2 #Level:2
        *Level 3 #Level:3
    - Okay {name}! Just enter the door next to me to play!
-> END