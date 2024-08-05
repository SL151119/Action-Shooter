using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisuals : MonoBehaviour
{
    [SerializeField] private Transform[] _weaponTransforms;
    [SerializeField] private Animator _animator;

    [Header("Left Hand IK")]
    [SerializeField] private Transform _leftHandIK_Target;
    [SerializeField] private TwoBoneIKConstraint _leftHandIK;
    [SerializeField] private float _leftHandIK_WeightIncreaseRate = 2.75f;
    private bool _shouldIncrease_LeftHandIKWeight;

    [Header("Rig")]
    [SerializeField] private Rig _rig;
    [SerializeField] private float _rigWeightIncreaseRate = 2.75f;
    private bool _shouldIncrease_RigWeight;

    private Transform _currentWeapon;
    private bool _isGrabbingWeapon;

    private void Start()
    {
        SwitchOn(_weaponTransforms[0]);
    }

    private void Update()
    {
        CheckWeaponSwitch();

        if (Input.GetKeyDown(KeyCode.R) && _isGrabbingWeapon == false)
        {
            _animator.SetTrigger("Reload");
            ReduceRigWeight();
        }

        UpdateRigWeight();
        UpdateLeftHandIKWeight();
    }

    private void UpdateLeftHandIKWeight()
    {
        if (_shouldIncrease_LeftHandIKWeight)
        {
            _leftHandIK.weight += _leftHandIK_WeightIncreaseRate * Time.deltaTime;

            if (_leftHandIK.weight >= 1)
            {
                _shouldIncrease_LeftHandIKWeight = false;
            }
        }
    }

    private void UpdateRigWeight()
    {
        if (_shouldIncrease_RigWeight)
        {
            _rig.weight += _rigWeightIncreaseRate * Time.deltaTime;

            if (_rig.weight >= 1)
            {
                _shouldIncrease_RigWeight = false;
            }
        }
    }

    private void ReduceRigWeight()
    {
        _rig.weight = 0.15f;
    }

    private void PlayWeaponGrabAnimation(GrabType grabType)
    {
        _leftHandIK.weight = 0;
        ReduceRigWeight();
        _animator.SetFloat("WeaponGrabType", (float)grabType);
        _animator.SetTrigger("WeaponGrab");
        SetBusyGrabbingWeaponTo(true);
    }

    public void SetBusyGrabbingWeaponTo(bool busy)
    {
        _isGrabbingWeapon = busy;
        _animator.SetBool("BusyGrabbingWeapon", _isGrabbingWeapon);
    }

    public void MaximizeRigWeight() => _shouldIncrease_RigWeight = true;

    public void MaximizeLeftHandWeight() => _shouldIncrease_LeftHandIKWeight = true;

    private void SwitchOn(Transform gunTransform)
    {
        SwitchOffGuns();

        gunTransform.gameObject.SetActive(true);
        _currentWeapon = gunTransform;

        AttachLeftHand();
    }

    private void SwitchOffGuns()
    {
        foreach (var guns in _weaponTransforms)
        {
            guns.gameObject.SetActive(false);
        }
    }

    private void AttachLeftHand()
    {
        Transform targetTransform = _currentWeapon.GetComponentInChildren<LeftHandTargetTransform>().transform;

        _leftHandIK_Target.localPosition = targetTransform.localPosition;
        _leftHandIK_Target.localRotation = targetTransform.localRotation;
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < _animator.layerCount; i++)
        {
            _animator.SetLayerWeight(i, 0);
        }

        _animator.SetLayerWeight(layerIndex, 1);
    }

    private void CheckWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchOn(_weaponTransforms[0]);
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchOn(_weaponTransforms[1]);
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchOn(_weaponTransforms[2]);
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchOn(_weaponTransforms[3]);
            SwitchAnimationLayer(2);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchOn(_weaponTransforms[4]);
            SwitchAnimationLayer(3);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }
    }
}

public enum GrabType
{
    SideGrab = 0,
    BackGrab = 1
}