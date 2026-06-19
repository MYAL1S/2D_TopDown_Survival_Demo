using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraTargetBinder : MonoBehaviour
{
    private static CameraTargetBinder _instance;

    public static CameraTargetBinder Instance => _instance;

    private CinemachineVirtualCamera _virtualCamera;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void OnDestroy()
    {
        if (_instance != null) 
            _instance = null;
    }

    public void SetTarget(Transform target)
    {
        _virtualCamera.Follow = target;
        _virtualCamera.LookAt = target;
    }

    public void ClearTarget()
    {
        _virtualCamera.Follow = null;
        _virtualCamera.LookAt = null;
    }
}
