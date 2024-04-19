using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_2D : MonoBehaviour
{

    /*
    Notes: Input Manager setting of Gravity was set as 10
    This is to prevent the vector.normalize() function from
    causing the player to continue to move and lose control
    of their character.
    */
    public float moveSpeed = 10;

    public enum PlayerControls { WASD, Arrows }
    public PlayerControls playerControls = PlayerControls.WASD;
    public Animator animator;

    private static bool canMove;

    private static MultiplayerManager multiplayerManager;

    // Start is called before the first frame update
    void Start()
    {
        canMove = false;
        multiplayerManager = GameObject.Find("GameManager").GetComponent<MultiplayerManager>();
        if (gameObject.name == "Player"){
            playerControls = PlayerControls.WASD;
        }
        else{
            playerControls = PlayerControls.Arrows;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Moves Players in the desired direction on the 2D Plane
        int horizontal = GetHorizontal();
        int vertical = GetVertical();
        Vector2 vector = new Vector2( horizontal , vertical );
        vector.Normalize(); //Prevents players from going faster when moving diagonally
        //If canMove is true, move the playerrrr
        if (canMove) transform.Translate(vector * moveSpeed * Time.deltaTime);

        //Vertical animation movement is prioritized
        //If the player is moving in the vertical direction, play the forward/backward animation
        if (vertical != 0){
            animator.SetBool("isMoving",true); //This is to prevent the idle/standstill animation from triggering
            animator.SetFloat("speedX",0); //This is to prevent the side-to-side animations from triggering
            animator.SetFloat("speedY",vertical); //This parameter is the driver for the forward/backward animations
        }
        //If the player is moving only in a horizontal direction, play the left/right animation
        else if (horizontal != 0){
            animator.SetBool("isMoving",true); //This is to prevent the idle/standstill animation from triggering
            animator.SetFloat("speedX",horizontal); //This parameter is the driver for the left/right animations
            animator.SetFloat("speedY",0); //This is to prevent the forward/backward animations from triggering
        }
        //If the player is not moving, resest all animator parameter values to defaults
        else{
            animator.SetBool("isMoving",false); //This is to prevent the idle/standstill animation from triggering
            animator.SetFloat("speedX",0);
            animator.SetFloat("speedY",0);
        }


    }

    public static void SetCanMove(bool move){
        canMove = move;
    }

    private int GetVertical(){
        if ((playerControls == PlayerControls.WASD && Input.GetKey(KeyCode.W)) ||
                (playerControls == PlayerControls.Arrows && Input.GetKey(KeyCode.UpArrow)))
            {
                return 1;
            }
        else if ((playerControls == PlayerControls.WASD && Input.GetKey(KeyCode.S)) ||
                    (playerControls == PlayerControls.Arrows && Input.GetKey(KeyCode.DownArrow)))
            {
                return -1;
            }
        return 0;
    }

    private int GetHorizontal(){
        if ((playerControls == PlayerControls.WASD && Input.GetKey(KeyCode.D)) ||
                (playerControls == PlayerControls.Arrows && Input.GetKey(KeyCode.RightArrow)))
            {
                return 1;
            }
        else if ((playerControls == PlayerControls.WASD && Input.GetKey(KeyCode.A)) ||
                    (playerControls == PlayerControls.Arrows && Input.GetKey(KeyCode.LeftArrow)))
            {
                return -1;
            }
        return 0;
    }
}
