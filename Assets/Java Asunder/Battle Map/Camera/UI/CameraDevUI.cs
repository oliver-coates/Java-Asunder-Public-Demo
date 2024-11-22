using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraDevUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _cameraStatusText;

    private void Awake()
    {
        CameraFocusManager.OnChanged += CameraFocusTypeChanged;
    }

    private void OnDestroy()
    {
        CameraFocusManager.OnChanged -= CameraFocusTypeChanged;
    }

    private void CameraFocusTypeChanged(CameraFocusType cameraFocusType)
    {
        _cameraStatusText.text = $"Camera: <b>{cameraFocusType.ToString()}</b>";
    }
}
