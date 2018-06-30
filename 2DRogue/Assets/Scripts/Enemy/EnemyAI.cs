using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyAI : MonoBehaviour {

    Enemy enemy;

    public enum MoveType
    {
        ConstantMovement,
        LerpBetweenPoints,
        MoveForTime,
        Chase,
        Flee
    }

    public MoveType AIMoveType;

    public enum StartMoveDir
    {
        Left,
        Right
    }

    public StartMoveDir StartDirection;

    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;

    public bool isCyclic;
    public float timeToWait = 1.0f;
    public float timeToMove = 2.0f;

    int fromWaypointIndex;
    float percentBetweenWaypoints;
    float waitTimer = 0.0f;
    float moveTimer = 0.0f;

    int movementDir = 1;

    // Use this for initialization
    void Start () {
        enemy = GetComponent<Enemy>();

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }

        if (StartDirection == StartMoveDir.Left)
        {
            movementDir = -1;
        }
        else if (StartDirection == StartMoveDir.Right)
        {
            movementDir = 1;
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (enemy.enemyCanMove == false)
        {
            enemy.SetDirectionalInput(new Vector2(0, 0));
        }
        else
        {
            if (AIMoveType == MoveType.LerpBetweenPoints)
            {
                Vector2 directionalInput = CalculateEnemyMoveBetweenPoints();
                enemy.SetDirectionalInput(directionalInput);
            }

            if (AIMoveType == MoveType.MoveForTime)
            {
                Vector2 directionalInput = CalculateEnemyMoveOverTime();
                enemy.SetDirectionalInput(directionalInput);
            }

            if (AIMoveType == MoveType.ConstantMovement)
            {
                Vector2 directionalInput = CalculateConstantMove();
                enemy.SetDirectionalInput(directionalInput);
            }
        }
	}

    Vector2 CalculateConstantMove()
    {
        return new Vector2(movementDir, 0);
    }

    Vector2 CalculateEnemyMoveOverTime()
    {
        if (Time.time < waitTimer)
        {
            return Vector2.zero;    // don't move
        }
    
        if (Time.time >= moveTimer)
        {
            movementDir = -movementDir;
            waitTimer = Time.time + timeToWait;
            moveTimer = Time.time + timeToWait + timeToMove;
        }

        return new Vector2(movementDir, 0);
    }


    Vector3 CalculateEnemyMoveBetweenPoints()
    {
        if (Time.time < waitTimer)
        {
            return Vector3.zero;                                                //stop moving
        }

        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * enemy.moveSpeed / distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        //float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);        // apply easing

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], 1);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;

            if (!isCyclic)                                                      // cycle back and forth through waypoints
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
            waitTimer = Time.time + timeToWait;                                // reset move timer
        }

        return newPos - transform.position;
    }

    void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.blue;
            float size = .3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }
}
