INCLUDE GlobalVariables.ink


{LevelIntroScene : -> Intro | -> LevelSelect}

=== Intro ===
    Cat?: Meow\~ Hello {name}!
    Cat: Next to me is the building you inherited.
    Cat: The Mayor has told me to help you learn a couple of basics.
    Cat: You can enter this door right here using <color=\#961B96> W A S D </color> to enter a <color=blue>puzzle</color>.
    Cat: When ever you would like to change the <color=blue>puzzle level</color>, just talk to me again!
    Cat: I do suggest that you play level 1 first to help you learn some of the controls!
    
    ~ LevelIntroScene = false
-> END

=== LevelSelect ===
    Cat: Hello again {name}!
    Please select a puzzle you would like to play:
        *Level 1 #Level:1
        *Level 2 #Level:2
        *Level 3 #Level:3
    - Okay {name}! Just enter the door next to me to play!
-> END