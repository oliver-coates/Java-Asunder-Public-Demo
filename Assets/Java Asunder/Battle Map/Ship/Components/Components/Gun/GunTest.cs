using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTest : MonoBehaviour
{
    public Transform target;

    public float turnSpeed;


    public Transform turret;

    void Start()
    {
        
    }

    void Update()
    {

        Vector3 dirToTarget = (target.position - transform.position).normalized;

        float dotRight = Vector3.Dot(dirToTarget, turret.right);
        float dotForward = Vector3.Dot(dirToTarget, turret.up);

        int turnDirection = 0;
        float deadzone = 0.05f;

        if (dotRight > deadzone)
        {
            turnDirection = -1;
        }
        else if (dotRight < (-deadzone))
        {
            turnDirection = 1;
        }
        if (dotForward < (-1 + deadzone))
        {
            turnDirection = 1;
        }

        Debug.Log($"Right {dotRight} - Forward {dotForward} - TurnDirection {turnDirection}");

        turret.Rotate(0, 0, turnDirection * turnSpeed * Time.deltaTime);
    }
}
