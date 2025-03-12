using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObstacle : MonoBehaviour
{
    
    private PlayerInteraction interactionScript;
    public float pushDistance;
    private float pushTime = 1f;
    private bool isMoving = false;
    private Vector2 direction;

    private Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        interactionScript = GameObject.Find("GameManager").GetComponent<PlayerInteraction>();
        rigidBody = this.GetComponentInParent<Rigidbody2D>();
    }

    public void BeginMoving(){
        if(isMoving){
            return;
        }
        StartCoroutine(Push(direction));
    }
    
    public IEnumerator Push(Vector2 direction){
        isMoving = true;
        rigidBody.AddForce(direction*pushDistance*rigidBody.mass, ForceMode2D.Impulse);
        yield return new WaitForSeconds(pushTime);
        isMoving = false;
    }

    //Original function to move the object. Replaced with a rigid-body force system.
    // public IEnumerator Move(Vector2 direction){
    //     float curTime = 0f;
    //     Vector3 start = this.transform.position;
    //     Vector2 target = pushDistance * direction + (Vector2) start;
    //     isMoving = true;
    //     while (curTime < 1){
    //         this.transform.position = new Vector3(Mathf.Lerp(start.x, target.x,Mathf.Sqrt(curTime)),
    //                                             Mathf.Lerp(start.y, target.y,Mathf.Sqrt(curTime)) , 0);
    //         curTime += Time.deltaTime / pushTime;
    //         yield return null;
    //     }
    //     isMoving = false;
    // }


    private void OnTriggerEnter2D(Collider2D other){
        if (other.gameObject.tag == "Player"){
            interactionScript.canPushObject(this.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other){
        if (other.gameObject.tag == "Player"){
            direction = IdentifyDirection(other.gameObject.transform);
            if (!isMoving){
                interactionScript.canPushObject(this.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if (other.gameObject.tag == "Player"){
            interactionScript.cannotPushObject();
        }
    }

    //Identifies what direction the obsticle should be pushed
    private Vector2 IdentifyDirection(Transform other){
        //Produces a normalized direction for the object to travel
        Vector2 result = (Vector2) (this.transform.position - other.position);
        //If the player is closer to the sides of the object (Left and Right Sides)
        if (Mathf.Abs(result.x) > Mathf.Abs(result.y)){
            result = new Vector2(result.x,0);
        }
        //If the player is closer  to the top and botom of the object (Top and Bottom);
        else{
            result = new Vector2(0,result.y);
        }
        result.Normalize();
        return result;
    }
}
