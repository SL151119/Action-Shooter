using System;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Player _player;
    private PlayerControls _controls;

    [Header("Aim Visual - Laser")]
    [SerializeField] private LineRenderer _aimLaser; //this component is on the weapon holder (child of a player)

    [Header("Aim Control")]
    [SerializeField] private Transform _aim;

    [SerializeField] private bool _isAimingPrecisely;
    [SerializeField] private bool _isLockingToTarget;

    [Header("Camera Control")]
    [SerializeField] private Transform _cameraTarget;
    [Range(0.5f,1f)]
    [SerializeField] private float _minCameraDistance = 1f;
    [Range(1f,3f)]
    [SerializeField] private float _maxCameraDistance = 3f;
    [Range(3f,5f)]
    [SerializeField] private float _cameraSensitivity = 3.5f;

    [Space]
    [SerializeField] private LayerMask _aimLayerMask;

    private Vector2 _mouseInput;
    private RaycastHit _lastKnownMouseHit;

    private void Start()
    {
        AssingInputEvents();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            _isAimingPrecisely = !_isAimingPrecisely;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            _isLockingToTarget = !_isLockingToTarget;
        }

        UpdateAimVisuals();
        UpdateAimPosition();
        UpdateCameraPosition();
    }

    private void UpdateAimVisuals()
    {
        _aimLaser.enabled = _player.WeaponController.WeaponReady();

        if (_aimLaser.enabled == false)
        {
            return;
        }

        WeaponModel weaponModel = _player.WeaponVisuals.CurrentWeaponModel();

        weaponModel.transform.LookAt(_aim);
        weaponModel._weaponPoint.LookAt(_aim);

        Transform weaponPoint = _player.WeaponController.WeaponPoint();
        Vector3 laserDirection = _player.WeaponController.BulletDirection();

        float _laserTipLength = 0.5f;
        float weaponDistance = _player.WeaponController.CurrentWeapon().WeaponDistance;

        Vector3 endPoint = weaponPoint.position + laserDirection * weaponDistance;
        
        if (Physics.Raycast(weaponPoint.position, laserDirection, out RaycastHit hit, weaponDistance))
        {
            endPoint = hit.point;
            _laserTipLength = 0;
        }

        _aimLaser.SetPosition(0, weaponPoint.position);
        _aimLaser.SetPosition(1, endPoint);
        _aimLaser.SetPosition(2, endPoint + laserDirection * _laserTipLength);
    }

    private void UpdateCameraPosition()
    {
        _cameraTarget.position =
                    Vector3.Lerp(_cameraTarget.position, TargetCameraPosition(), _cameraSensitivity * Time.deltaTime);
    }

    private Vector3 TargetCameraPosition()
    {
        float actualMaxCameraDistance = _player.Movement.MoveInput.y < -0.5f ? _minCameraDistance : _maxCameraDistance;

        Vector3 targetCameraPosition = GetMouseHitInfo().point;
        Vector3 aimDirection = (targetCameraPosition - transform.position).normalized;

        float distanceToTargetPosition = Vector3.Distance(transform.position, targetCameraPosition);
        float clapmedDistance = Mathf.Clamp(distanceToTargetPosition, _minCameraDistance, actualMaxCameraDistance);

        targetCameraPosition = transform.position + aimDirection * clapmedDistance;
        targetCameraPosition.y = transform.position.y + 1;

        return targetCameraPosition;
    }

    private void UpdateAimPosition()
    {
        Transform target = Target();

        if (target != null && _isLockingToTarget)
        {
            if (target.GetComponent<Renderer>() != null)
            {
                _aim.position = target.GetComponent<Renderer>().bounds.center;
            }
            else
            {
                _aim.position = target.position;
            }

            return;
        }

        _aim.position = GetMouseHitInfo().point;

        if (!_isAimingPrecisely)
        {
            _aim.position = new Vector3(_aim.position.x, transform.position.y + 1, _aim.position.z);
        }
    }
    public Transform Target()
    {
        Transform target = null;

        if (GetMouseHitInfo().transform.GetComponent<Target>() != null)
        {
            target = GetMouseHitInfo().transform;
        }

        return target;
    }

    public Transform Aim() => _aim;

    public bool CanAimPrecisely() => _isAimingPrecisely;

    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(_mouseInput);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _aimLayerMask))
        {
            _lastKnownMouseHit = hitInfo;
            return hitInfo;
        }

        return _lastKnownMouseHit;
    }

    private void AssingInputEvents()
    {
        _controls = _player.Controls;

        _controls.Character.Aim.performed += context => _mouseInput = context.ReadValue<Vector2>();
        _controls.Character.Aim.canceled += context => _mouseInput = Vector2.zero;
    }
}
