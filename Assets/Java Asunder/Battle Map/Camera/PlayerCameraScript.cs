using System.Collections;
using System.Collections.Generic;
using Ships;
using UnityEngine;

public class PlayerCameraScript : MonoBehaviour
{
    #region Magic numbers

    public const float SPOTTING_SIZE = 500; 
    public const float GUNNERY_SIZE_TUNER = 0.75f;

    public const float GUNNERY_SIZE_MIN = 75f;
    public const float GUNNERY_SIZE_MAX = 10000f;

    #endregion

    [Header("State:")]
    [SerializeField] private CameraFocusType _currentFocusType;

    [Header("References:")]
    [SerializeField] private DeveloperCameraScript _developerCamera;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private AudioListener _audioListener;
    private float _camSize;

    private float _zOffset;

    private float _cameraSizeTarget;
    private Vector3 _targetPosition;

    private float _movementTightness;

    #region Initialisation & Destruction
    private void Awake()
    {
        GameMaster.OnBattleEnd += EnableListener;
        GameMaster.OnBattleStart += DisableListener;

        CameraFocusManager.OnChanged += SetFocusType;
    }

    private void OnDestroy()
    {
        GameMaster.OnBattleEnd -= EnableListener;
        GameMaster.OnBattleStart -= DisableListener;

        CameraFocusManager.OnChanged -= SetFocusType;
    }

    /// <summary>
    /// Sets up stats from the developer camera.
    /// Makes it so that this mirrors the dev cameras stats.
    /// This just makes it so we don't have to change both cameras settings.
    /// </summary>
    public void Setup(float tightness, float zPos)
    {
        _movementTightness = tightness;
        _zOffset = zPos;
    }

    #endregion

    private void Update()
    {
        if (GameMaster.BattleUnderway)
        {
            DetermineTargetPositionAndSizeFromFocusType();
            FOVUpdate();
            MovementUpdate();
        }
        
    }

    #region Target And Position from focus type

    private void SetFocusType(CameraFocusType cameraFocusType)
    {
        _currentFocusType = cameraFocusType;
    }

    private void DetermineTargetPositionAndSizeFromFocusType()
    {
        switch (_currentFocusType)
        {
            case CameraFocusType.FreeCam:
                ResolveFreeCamFocus();
                return;
            
            case CameraFocusType.ShipOverview:
                ResolveShipOverviewFocus();
                return;
            
            case CameraFocusType.Spotting:
                ResolveSpottingFocus();
                return;
            
            case CameraFocusType.Gunnery:
                ResolveGunneryFocus();
                return;
            
            case CameraFocusType.Target:
                ResolveTargetFocus();
                return;
            
            default:
                Debug.LogError($"Recieved unknown focus type: {_currentFocusType}");
                return;
        }
    }

    private void SetTargetPositionAndSize(float cameraSizeAmount, Vector3 targetPos)
    {
        // Set position (clamp the z)
        Vector3 targetPosZClamped = targetPos;
        targetPosZClamped.z = _zOffset;
        _targetPosition = targetPosZClamped;

        // Set the size
        _cameraSizeTarget = cameraSizeAmount;
    }
    #endregion

    #region Focus Types
    private void ResolveFreeCamFocus()
    {
        float size = _developerCamera.size;
        Vector3 position = _developerCamera.transform.position;

        SetTargetPositionAndSize(size, position); 
    }

    private void ResolveShipOverviewFocus()
    {
        // Get the size from the ship's type (e.g. Destroyer)
        float size = SessionMaster.PlayerShip.shipClass.shipType.cameraViewSize;;
        Vector3 position = SessionMaster.PlayerShip.instance.transform.position;

        SetTargetPositionAndSize(size, position);
    }

    private void ResolveSpottingFocus()
    {
        float size = SPOTTING_SIZE;
        Vector3 position = SessionMaster.PlayerShip.instance.transform.position;

        SetTargetPositionAndSize(size, position);
    }

    private void ResolveGunneryFocus()
    {
        ShipInstance targetShip = SessionMaster.PlayerShip.instance.target;
        if (targetShip == null)
        {
            ResolveSpottingFocus();
            return;
        }

        // Position should be mid-point between the two ships
        Vector3 playerPos = SessionMaster.PlayerShip.instance.transform.position;
        Vector3 enemyPos = targetShip.transform.position;

        Vector3 position = Vector3.Lerp(playerPos, enemyPos, 0.5f);

        // The size should scale up with the distance betwen the two ships,
        // so that both are visible
        float distanceBetweenPositions = Vector3.Distance(playerPos, enemyPos);
        float size = GUNNERY_SIZE_TUNER * distanceBetweenPositions;

        size = Mathf.Clamp(size, GUNNERY_SIZE_MIN, GUNNERY_SIZE_MAX);

        SetTargetPositionAndSize(size, position);
    }

    private void ResolveTargetFocus()
    {
        ShipInstance targetShip = SessionMaster.PlayerShip.instance.target;
        if (targetShip == null)
        {
            ResolveSpottingFocus();
            return;
        }

        // Get size from ship type (e.g. Destroyer)
        float size = targetShip.shipData.shipClass.shipType.cameraViewSize;
        Vector3 position = targetShip.transform.position;

        SetTargetPositionAndSize(size, position);
    }
    #endregion

    #region Size and positon lerping
    public void FOVUpdate()
    {
        _camSize = Mathf.Lerp(_camSize, _cameraSizeTarget, Time.deltaTime * 2f);
        _playerCamera.orthographicSize = _camSize;
    }

    private void MovementUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _movementTightness);
    }
    #endregion

    #region Audio
    private void DisableListener()
    {
        _audioListener.enabled = false;
    }

    private void EnableListener()
    {
        _audioListener.enabled = true;
    }
    #endregion
}

