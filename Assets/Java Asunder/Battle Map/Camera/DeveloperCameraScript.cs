using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeveloperCameraScript : MonoBehaviour
{
    [SerializeField] private PlayerCameraScript _playerCamera;
    [SerializeField] private Camera _devCamera;

    [Header("Field of View settings:")]
    [SerializeField] private float _minSize = 100;
    [SerializeField] private float _maxSize = 500;
    [SerializeField] private float _scrollSensitivity = 10;
    private float _cameraSizeLerp;
    public float size
    {
        get
        {
            return Mathf.Lerp(_minSize, _maxSize, _cameraSizeLerp);
        }
    }

    [Header("Movement Settings:")]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _movementTightness;
    private Vector3 _targetPosition;


    private void Start()
    {
        _cameraSizeLerp = 0.5f;
    
        _targetPosition = transform.position;

        _playerCamera.Setup(_movementTightness, transform.position.z);
    }

    private void Update()
    {
        FOVUpdate();
        MovementUpdate();
    }

    private void FOVUpdate()
    {
        _cameraSizeLerp -= Input.mouseScrollDelta.y * _scrollSensitivity * Time.deltaTime;
        _cameraSizeLerp = Mathf.Clamp(_cameraSizeLerp, 0, 1f);

        _devCamera.orthographicSize = Mathf.Lerp(_minSize, _maxSize, _cameraSizeLerp);
    }


    private void MovementUpdate()
    {
        float speedMultiplier = 1 + _cameraSizeLerp;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speedMultiplier *= 3f;
        }

        _targetPosition.x += Input.GetAxis("Horizontal") * Time.deltaTime * _movementSpeed * speedMultiplier;
        _targetPosition.y += Input.GetAxis("Vertical") * Time.deltaTime * _movementSpeed * speedMultiplier;
        _targetPosition.z = -500;

        transform.position = _targetPosition;
    }
}
