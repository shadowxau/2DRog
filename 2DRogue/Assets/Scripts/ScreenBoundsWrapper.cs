using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBoundsWrapper : MonoBehaviour {

    public float topEdge = 0.0f;
    public float bottomEdge = 0.0f;
    public float leftEdge = 0.0f;
    public float rightEdge = 0.0f;
    public float vertBuffer = 12.0f;         // buffer allows object to disappear offscreen before appearing on the other side
    public float horBuffer = 0.5f;
    public float camDistance;
    Camera cam;

    // Use this for initialization
    void Start () {
        cam = Camera.main;
        camDistance = cam.transform.position.z + transform.position.z;

        leftEdge = cam.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, camDistance)).x;
        rightEdge = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, camDistance)).x;
        topEdge = cam.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, camDistance)).y;
        bottomEdge = cam.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, camDistance)).y;

	}

    void FixedUpdate()
    {
        if (transform.position.x < leftEdge - horBuffer)
        {
            transform.position = new Vector3(rightEdge + horBuffer, transform.position.y, transform.position.z);
        }    
        
        if (transform.position.x > rightEdge + horBuffer)
        {
            transform.position = new Vector3(leftEdge - horBuffer, transform.position.y, transform.position.z);
        }

        if (transform.position.y > topEdge + vertBuffer)
        {
            transform.position = new Vector3(transform.position.x, bottomEdge - vertBuffer, transform.position.z);
        }

        if (transform.position.y < bottomEdge - vertBuffer)
        {
            transform.position = new Vector3(transform.position.x, topEdge + vertBuffer, transform.position.z);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
