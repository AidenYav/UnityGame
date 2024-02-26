using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
Credits to Sloan Kelly for program inspiration.
https://www.youtube.com/watch?v=p60ISYgo8xA
*/


public class TransitionScript : MonoBehaviour
{
    public Shader shader;

    private Material material;

    private float radius;

    public float FADE_IN = 2f;

    public float FADE_OUT = 0f;
    float horizontal, vertical;
    public GameObject player;
    private Camera cam;

    void Start()
    {
        horizontal = Screen.width;
        vertical = Screen.height;
        cam = GetComponent<Camera>();
        material = new Material(shader);
        radius = FADE_OUT;
        UpdateShader();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination){
        Graphics.Blit(source, destination, material);
    }

    private void UpdateShader(){
        //Updates Screen width and height
        horizontal = Screen.width;
        vertical = Screen.height;
        float radiusSpeed = Mathf.Max(horizontal, vertical);
        //Updates shader values
        material.SetFloat("_Radius",radius);
        material.SetFloat("_RadiusSpeed",radiusSpeed);
        material.SetFloat("_Horizontal", horizontal);
        material.SetFloat("_Vertical", vertical);
        UpdatePosition(player);
    }

    //Updates the position of the player, or target of transition animation
    private void UpdatePosition(GameObject target){
        Vector3 pos = cam.WorldToScreenPoint(target.transform.position);
        //_PosX and _PosY must be a value between [0,1]
        material.SetFloat("_PosX",pos.x/horizontal);
        material.SetFloat("_PosY",pos.y/vertical);
    }
    /* Code for a basic linear transition
    // Function manages coroutines of the Fade transition

    private void Fade(float end){
        //This prevents multiple coroutines from running
        if (transition != null){
            StopCoroutine(transition);
        }
        //Start the new coroutine containing the transition animation
        transition = Fade(radius,end);
        StartCoroutine(transition);
    }

    //Helper functin for the actual transition animation
    private IEnumerator Fade(float start, float end){
        float time = 0f;
        UpdateShader();

        while (time < transitionDuration){
            radius = Mathf.Lerp(start, end, time);
            time += Time.deltaTime;
            UpdateShader();
            yield return null;
        }
    }
    */

    //Pre-Hard-Coded animation with values to create the animation
    public void FancyFadeOut(){
        StopAllCoroutines();
        IEnumerator c3 = FancyFade( 0.15f , FADE_OUT , 0.5f , null);
        IEnumerator c2 = FancyFade( 0.1f , 0.15f , 0.5f ,c3);
        StartCoroutine(FancyFade( radius , 0.1f , 1f , c2));
    }

    //Pre-Hard-Coded animation with values to create the animation
    public void FancyFadeIn(){
        StopAllCoroutines();
        IEnumerator c3 = FancyFade( 0.1f , FADE_IN , 1f , null);
        IEnumerator c2 = FancyFade( 0.15f , 0.1f , 0.5f , c3 );
        StartCoroutine(FancyFade( FADE_OUT , 0.15f , 0.5f, c2 ));
    }


    //
    //The nextCoroutine is used to begin the next portion of the animation in the transition sequence
    //Fades the animation from the start radius to the end radius in the set amount of time
    /**@start           - Start Radius
    *  @end             - End Radius
    *  @duration        - Time in seconds for this to animate 
    *  @nextCoroutine   - The coroutine intended to run after the current one is complete, or null if no other coroutine to run.
    */
    private IEnumerator FancyFade(float start, float end, float duration, IEnumerator nextCoroutine){

        float time = 0f;
        //Updates shader data
        UpdateShader();
        //time can be thought of as a percentage, [0, 100] or [0, 1]
        while (time < 1f){
            radius = Mathf.Lerp(start, end, time);
            time += Time.deltaTime / duration; //Time is calculted as a percentage of time passed versus duration
            //Update the shader again
            UpdateShader();
            yield return null;
        }
        //Starts the next coroutine in the sequence
        if (nextCoroutine != null){
            StartCoroutine(nextCoroutine);
        }
    }



}
