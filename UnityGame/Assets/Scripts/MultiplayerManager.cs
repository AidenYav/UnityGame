using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    private bool isMultiplayerActivated = true;

    private Camera cam1,cam2;
    private GameObject player2;

    private GameObject multiplayerBoarder;

    private PuzzleManager puzzleManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        cam1 = GameObject.Find("Main Camera").GetComponent<Camera>();
        cam2 = GameObject.Find("Camera2").GetComponent<Camera>();
        player2 = GameObject.Find("Player2");
        multiplayerBoarder = GameObject.Find("Canvas").transform.Find("MultiplayerUI").gameObject;
        puzzleManagerScript = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
    }

    // Update is called once per frame

    public void MultiplayerToggle(){
        isMultiplayerActivated = !isMultiplayerActivated;
        puzzleManagerScript.SetPuzzleType();
    }

    //Activates Multiplayer Visual Effects
    public void ActivateMultiplayer(){
        //Activate the second player
        player2.SetActive(true);
        //Set Main Camera (cam1) to be split screen with:
        // Viewport Rect: X,Y = 0, 0 ; W,H = 0.5, 1
        
        cam1.rect = new Rect(0,0,0.5f,1);
        //Enable Camera 2
        cam2.gameObject.SetActive(true);
        multiplayerBoarder.SetActive(true);
    }


    //Deactivates Multiplayer Visual Effects
    public void DeactivateMultiplayer(){
        //Deactivate the second player
        player2.SetActive(false);
        //Set Main Camera (cam1) to disable split screen with:
        // Viewport Rect: X,Y = 0, 0 ; W,H = 1, 1
        cam1.rect = new Rect(0,0,1,1);
        //Disable Camera 2
        cam2.gameObject.SetActive(false);
        multiplayerBoarder.SetActive(false);
    }

    public bool GetIsMultiplayerActivated(){
        return isMultiplayerActivated;
    }
}
