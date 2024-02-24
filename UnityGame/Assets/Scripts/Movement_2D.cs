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

    //Old variables for recording character direction
    // enum direction{
    //     LEFT,
    //     RIGHT,
    //     UP,
    //     DOWN
    // }
    //direction playerDirection = direction.DOWN;


    public Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Moves Players in the desired direction on the 2D Plane
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 vector = new Vector2( horizontal , vertical );
        vector.Normalize(); //Prevents players from going faster when moving diagonally
        transform.Translate(vector * moveSpeed * Time.deltaTime);

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

        //Old version of changing sprite art
    // private void changeSpritDirection(direction dir){
    //     //Colors are placeholders for sprites
    //     switch (dir){
    //         //Set sprite facing Left
    //         case direction.LEFT:
    //             spriteRender.color = Color.magenta;
    //             break;

    //         //Set sprite facing Right
    //         case direction.RIGHT:
    //             spriteRender.color = Color.yellow;
    //             break;

    //         //Set sprite faicng Up
    //         case direction.UP:
    //             spriteRender.color = Color.red;
    //             break;

    //         //Set Sprite facing Down
    //         case direction.DOWN:
    //             spriteRender.color = Color.cyan;
    //             break;
    //     }
    // }


}
