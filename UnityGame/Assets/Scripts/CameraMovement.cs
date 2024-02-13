using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject player;

    public float timeToLockOn; //Once the player stops moving, this is how long it will take the camera to center the player onto the screen

    public Vector2 initialVel, accel;

    public float moveTimeInterval;
    public float timer;

    private IEnumerator currentMove;
    // Start is called before the first frame update
    void Start()
    {
        timeToLockOn = 1.0f;
        //initialVel = 0.0f;
        timer = 0.0f;
        moveTimeInterval = 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        goToTarget((Vector2)player.transform.position);
    }

    public Vector2 getDistanceFromTarget(Vector2 v){
        return v - (Vector2) this.gameObject.transform.position;
    }

    public void goToTarget(Vector2 v){
        Vector2 vel = getDistanceFromTarget(v) * 2 / timeToLockOn;
        Vector2 accel = -vel;
        if (currentMove != null){
            StopCoroutine(currentMove);
        }
        StartCoroutine(currentMove = move(vel, accel));

    }

    private IEnumerator move(Vector2 vel, Vector2 accel){
        initialVel = vel;
        this.accel = accel;
        //While velocity is not 0
        //velocity approaches 0
        //Meaning Accel + --> vel -
        //Meaning Accel - --> vel +
        //If accel is positive, multi = 1
        //If accel is negative, multi = -1
        float xNeg = accel.x/Mathf.Abs(accel.x);
        float yNeg = accel.y/Mathf.Abs(accel.y);
        while (
            (xNeg*vel.x < 0 || yNeg*vel.y < 0) //Accel is positive
        ){
            transform.Translate(vel * moveTimeInterval);
            vel -= accel * moveTimeInterval;
            


            if (xNeg*vel.x > 0){
                vel.x = 0;
            }
            if (yNeg*vel.y > 0){
                vel.y = 0;
            }
            yield return new WaitForSeconds(moveTimeInterval);
        }
        StopCoroutine(currentMove);
    }

    
}
