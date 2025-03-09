using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleButton : MonoBehaviour
{

    private GameObject button, door, shadow;
    private Color buttonColor, shadedColor;
    private bool isPressed = false;

    private int numOfObjectsInside = 0;

    private BoxCollider2D doorCollider;
    private SpriteRenderer doorRender;




    // Start is called before the first frame update
    void Start()
    {
        button = transform.gameObject;
        door = transform.parent.transform.Find("Door").gameObject;
        doorCollider = door.GetComponent<BoxCollider2D>();
        doorRender = door.GetComponent<SpriteRenderer>();

        shadow = transform.Find("Shadow").gameObject;
        buttonColor = transform.Find("Button").gameObject.GetComponent<SpriteRenderer>().color;
        shadedColor = new Color(buttonColor.r, buttonColor.g, buttonColor.b, 0.5f);
        updateDoor();
    }

    public void OnTriggerEnter2D(Collider2D other){
        numOfObjectsInside++;
        isPressed = true;
        updateDoor();
    }

    public void OnTriggerExit2D(Collider2D other){
        numOfObjectsInside--;
        if(numOfObjectsInside == 0){
            isPressed = false;
        }
        updateDoor();
    }

    public void updateDoor(){
        if(isPressed){
            doorCollider.enabled = false;
            doorRender.color = shadedColor;
            shadow.SetActive(false);
        }else{
            doorCollider.enabled = true;
            doorRender.color = buttonColor;
            shadow.SetActive(true);
        }
    }

    public void reset(){
        isPressed = false;
        numOfObjectsInside = 0;
        updateDoor();
    }




}
