using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconBounce : MonoBehaviour
{

    public float amplitude = 0.2f;
    public float period = 1.0f;
    public float offset = 0;
    private GameObject icon;

    private Vector3 localPos;

    // Start is called before the first frame update
    void Start()
    {
        // if (icon == null){
        //     icon = transform.gameObject;
        // }
        icon = transform.gameObject;
        localPos = icon.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        icon.transform.position = localPos + new Vector3(0, amplitude * Mathf.Sin(period * Time.time * Mathf.PI + offset),0);
    }
}
