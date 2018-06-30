using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerShotScript : MonoBehaviour {

    public Vector2 velocity { get; set; }
    public int moveDirX { get; set; }
    public float moveSpeed = 2.0f;
    public float shotTimer = 2.0f;
    public float stunLockTime = 2.0f;
    public bool isPiercing = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (shotTimer > 0)
        { 
            // Calculate the velocity
            velocity = new Vector2(moveSpeed * Time.deltaTime * moveDirX, 0);

            //Move the shot
            transform.Translate(velocity);

            // Reduce the shot timer
            shotTimer -= Time.deltaTime;
        }
        else
        {
            // Destroy shot after X period of time
            Destroy(this.gameObject);
        }
    
    }
}
