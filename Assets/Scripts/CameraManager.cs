using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    private CinemachineFramingTransposer _transposer;

    [Header("Camera Distance")]
    [SerializeField] private bool _canChangeCameraDistance;
    [SerializeField] private float _distanceChangeRate;
    private float _targetCameraDistance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Update()
    {
        UpdateCameraDistance();

    }

    private void UpdateCameraDistance()
    {
        if (_canChangeCameraDistance == false)
        {
            return;
        }

        float currentDistance = _transposer.m_CameraDistance;

        if (Mathf.Abs(_targetCameraDistance - currentDistance) < 0.1f)
        {
            return;
        }

        _transposer.m_CameraDistance = 
            Mathf.Lerp(currentDistance, _targetCameraDistance, _distanceChangeRate * Time.deltaTime);
    }

    public void ChangeCameraDistance(float distance)
    {
        _targetCameraDistance = distance;
    }
}
