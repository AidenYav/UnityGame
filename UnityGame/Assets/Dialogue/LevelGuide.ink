INCLUDE GlobalVariables.ink


{LevelIntroScene : -> Intro | -> LevelSelect}

=== Intro ===
    Cat?: Meow\~ Hello {name}!
    Cat: Next to me is the building you inherited.
    Cat: The Mayor has told me to help you learn a couple of basics.
    Cat: You can enter this door right here using <color=\#961B96> W A S D </color> to enter a <color=blue>puzzle</color>.
    Cat: I do suggest that you play level 1 first to help you learn some of the controls!
    
    ~ LevelIntroScene = false
-> END

=== LevelSelect ===
    Cat: Hello again {name}!
    Do you need help finding a level? I can help give you some directions if you would like!
    *Level 2
        Cat: You can go <color=green>South</color> and then <color=green>East</color> to the <color=brown>Brown house</color> with a <color=blue>Blue roof</color>. You should see a <color=orange>Number 2</color> on it!
    *Level 3
    Cat: You can go <color=green>North</color> to the <color=red>Red house</color> with a <color=green>Green roof</color>. You should see a <color=yellow>Number 3</color> on it!
    *Other Levels
        Here are some other levels I can help you find!
        **Level 4
        Cat: You can go <color=green>East</color> and then all the way<color=green>North</color> to the <color=yellow>Yellow house</color> with a <color=blue>Blue roof</color>. You should see a <color=green>Number 4</color> on it!
        **Level 5
        Cat: You can go <color=green>East</color> and then slightly <color=green>North</color> to the <color=white>White house</color> with a <color=red>Red roof</color>. You should see a <color=blue>Number 5</color> on it!
        **Goodbye!
    -Cat: Have fun {name}!
-> END