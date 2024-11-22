using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAngleRestrictor : MonoBehaviour
{
    [SerializeField] private bool _restrictorEnabled;
    public bool restrictorEnabled
    {
        get
        {
            return _restrictorEnabled;
        }	
    }

    [Header("Gun cannot shoot between these two angles:")]
    [SerializeField] private float _minAngle = 130f;
    [SerializeField] private float _maxAngle = 230f;

    public bool IsGunAllowedToFire(float localRotation)
    {
        float min = _minAngle;
        float max = _maxAngle;

        return (localRotation < min) || (localRotation > max);
    }


    #if UNITY_EDITOR
    [SerializeField] private float _gizmosSize = 2f;

    public void OnDrawGizmosSelected()
    {
        int numPoints = 64;

        Color canShootColor;
        Color cannotShootColor;
        if (_restrictorEnabled)
        {
            canShootColor = Color.green;
            cannotShootColor = Color.red;
        }
        else
        {
            canShootColor = Color.green * 0.33f;
            cannotShootColor = Color.red * 0.33f;
        }

        Vector3 pointA;
        Vector3 pointB;

        float z = transform.position.z;
        float localRotationZ = transform.localEulerAngles.z * Mathf.Deg2Rad;

        // Point zero starts at local rotation + 0
        pointB = transform.position + FindPointOnCircle(localRotationZ + 0, _gizmosSize, z);

        for (int pointIndex = 1; pointIndex <= numPoints; pointIndex++)
        {
            pointA = pointB;

            float angle = (pointIndex / (float)numPoints) * 360f * Mathf.Deg2Rad;

            if (IsGunAllowedToFire(angle * Mathf.Rad2Deg))
            {
                Gizmos.color = canShootColor;
            }
            else
            {
                Gizmos.color = cannotShootColor;
            }

            Vector3 pointOnCircle = FindPointOnCircle(localRotationZ + angle, _gizmosSize, z);    
            pointB = transform.position + pointOnCircle;
        
            Gizmos.DrawLine(pointA, pointB);
        }
    }

    private Vector3 FindPointOnCircle(float angle, float size, float z)
    {
        float x = Mathf.Sin(angle) * size;
        float y = Mathf.Cos(angle) * size;

        return new Vector3(x, y, z);
    }
    #endif

}
