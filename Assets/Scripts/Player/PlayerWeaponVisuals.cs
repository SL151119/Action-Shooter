using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisuals : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Player _player;

    [SerializeField] private WeaponModel[] _weaponModels;
    [SerializeField] private BackupWeaponModel[] _backupWeaponModels;

    [Header("Left Hand IK")]
    [SerializeField] private Transform _leftHandIK_Target;
    [SerializeField] private TwoBoneIKConstraint _leftHandIK;
    [SerializeField] private float _leftHandIK_WeightIncreaseRate = 2.75f;
    private bool _shouldIncrease_LeftHandIKWeight;

    [Header("Rig")]
    [SerializeField] private Rig _rig;
    [SerializeField] private float _rigWeightIncreaseRate = 2.75f;
    private bool _shouldIncrease_RigWeight;

    private bool _isEquipingWeapon;

    private void Update()
    {
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

    public void MaximizeLeftHandWeight() => _shouldIncrease_LeftHandIKWeight = true;
    public void MaximizeRigWeight() => _shouldIncrease_RigWeight = true;

    private void ReduceRigWeight()
    {
        _rig.weight = 0.15f;
    }

    private void AttachLeftHand()
    {
        Transform targetTransform = CurrentWeaponModel()._leftHandHoldPoint;

        _leftHandIK_Target.localPosition = targetTransform.localPosition;
        _leftHandIK_Target.localRotation = targetTransform.localRotation;
    }

    public void PlayWeaponEquipAnimation()
    {
        EquipWeaponType equipType = CurrentWeaponModel()._equipType;

        float equipmentSpeed = _player.WeaponController.CurrentWeapon().equipmentSpeed;

        _leftHandIK.weight = 0;
        ReduceRigWeight();
        _animator.SetTrigger("EquipWeapon");
        _animator.SetFloat("EquipSpeed", equipmentSpeed);
        _animator.SetFloat("EquipWeaponType", (float)equipType);

        SetBusyEquipingWeaponTo(true);
    }

    public void PlayReloadAnimation()
    {
        if (_isEquipingWeapon)
        {
            return;
        }

        float reloadSpeed = _player.WeaponController.CurrentWeapon().reloadSpeed;

        _animator.SetFloat("ReloadSpeed", reloadSpeed);
        _animator.SetTrigger("Reload");
        ReduceRigWeight();
    }

    public void SetBusyEquipingWeaponTo(bool busy)
    {
        _isEquipingWeapon = busy;
        _animator.SetBool("BusyEquipingWeapon", _isEquipingWeapon);
    }

    public void SwitchOnCurrentWeaponModel()
    {
        int animationIndex = (int)CurrentWeaponModel()._holdType;

        SwitchOffWeaponModels();

        SwitchOffBackupWeaponModels();

        if (_player.WeaponController.HasOnlyOneWeapon() == false)
        {
            SwitchOnBackupWeaponModel();
        }

        SwitchAnimationLayer(animationIndex);

        CurrentWeaponModel().gameObject.SetActive(true);

        AttachLeftHand();
    }

    public void SwitchOffWeaponModels()
    {
        foreach (WeaponModel weapons in _weaponModels)
        {
            weapons.gameObject.SetActive(false);
        }
    }

    public void SwitchOnBackupWeaponModel()
    {
        WeaponType weaponType = _player.WeaponController.BackupWeapon().weaponType;

        foreach (BackupWeaponModel backupModel in _backupWeaponModels)
        {
            if (backupModel._weaponType == weaponType)
            {
                backupModel.gameObject.SetActive(true);
            }
        }
    }

    private void SwitchOffBackupWeaponModels()
    {
        foreach (BackupWeaponModel backupModel in _backupWeaponModels)
        {
            backupModel.gameObject.SetActive(false);
        }
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < _animator.layerCount; i++)
        {
            _animator.SetLayerWeight(i, 0);
        }

        _animator.SetLayerWeight(layerIndex, 1);
    }

    public WeaponModel CurrentWeaponModel()
    {
        WeaponModel weaponModel = null;
        WeaponType weaponType = _player.WeaponController.CurrentWeapon().weaponType;

        for (int i = 0; i < _weaponModels.Length; i++)
        {
            if (_weaponModels[i]._weaponType == weaponType)
            {
                weaponModel = _weaponModels[i];
            }
        }

        return weaponModel;
    }
}