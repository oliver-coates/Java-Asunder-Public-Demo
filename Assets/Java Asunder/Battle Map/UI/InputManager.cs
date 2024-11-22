using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = _camera.ScreenToWorldPoint(Input.mousePosition);
            position.y = 0;

        }
    }
}
