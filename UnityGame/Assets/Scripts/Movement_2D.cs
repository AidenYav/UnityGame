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
    enum direction{
        
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    direction playerDirection = direction.DOWN;

    SpriteRenderer spriteRender;

    // Start is called before the first frame update
    void Start()
    {
        spriteRender = GetComponent<SpriteRenderer>();
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

        
        if (vertical != 0){
            if (vertical > 0){
                playerDirection = direction.UP;
            }
            else{
                playerDirection = direction.DOWN;
            }
        }
        else if (horizontal != 0){
            if (horizontal > 0) {
                playerDirection = direction.RIGHT;
            }
            else{
                playerDirection = direction.LEFT;
            }
        }

        changeSpritDirection(playerDirection);
    }

    private void changeSpritDirection(direction dir){
        //Colors are placeholders for sprites
        switch (dir){
            //Set sprite facing Left
            case direction.LEFT:
                spriteRender.color = Color.magenta;
                break;

            //Set sprite facing Right
            case direction.RIGHT:
                spriteRender.color = Color.yellow;
                break;

            //Set sprite faicng Up
            case direction.UP:
                spriteRender.color = Color.red;
                break;

            //Set Sprite facing Down
            case direction.DOWN:
                spriteRender.color = Color.cyan;
                break;
        }
    }


}
