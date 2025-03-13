using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleEnterances : MonoBehaviour
{

    private PuzzleManager puzzleManager;

    private UI_Manager uiScript;

    public int levelNumber = 0;

    // Start is called before the first frame update
    void Start()
    {
        puzzleManager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
        uiScript = GameObject.Find("Canvas").GetComponent<UI_Manager>();
    }

    private void OnTriggerEnter2D(Collider2D other){

        if (other.gameObject.tag == "Player"){
            puzzleManager.SetPuzzle(levelNumber - 1);
            uiScript.PlayMinigameAgain();
        }
    }
}
