using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerShotScript : MonoBehaviour {

    public Vector3 velocity { get; set; }
    public int moveDirX = 1;
    public float moveSpeed = 2.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        velocity = new Vector2(moveSpeed * Time.deltaTime * moveDirX, 0);

        //Move the object
        transform.Translate(velocity);
    }
}
