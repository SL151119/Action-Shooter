using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private const float REFERENCE_BULLET_SPEED = 20f; //this is default speed from which our mass formula is derived

    [SerializeField] private Player _player;

    [SerializeField] private Weapon _currentWeapon;
    private bool _isWeaponReady;
    private bool _isShooting;

    [Header("Bullet Details")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _bulletSpeed;

    [SerializeField] private Transform _weaponHolder;

    [Header("Inventory")]
    [SerializeField] private int _maxSlots = 2;
    [SerializeField] private List<Weapon> _weaponSlots;

    private void Start()
    {
        AssingInputEvents();

        Invoke(nameof(EquipStartingWeapon), 0.1f);
    }

    private void Update()
    {
        if (_isShooting)
        {
            Shoot();
        }
    }

    private void EquipStartingWeapon()
    {
        EquipWeapon(0);
    }

    private void EquipWeapon(int weaponIndex)
    {
        if (weaponIndex >= _weaponSlots.Count)
        {
            return;
        }

        SetWeaponReady(false);

        _currentWeapon = _weaponSlots[weaponIndex];

        _player.WeaponVisuals.PlayWeaponEquipAnimation();

        CameraManager.instance.ChangeCameraDistance(_currentWeapon.cameraDistance);
    }

    public void PickupWeapon(Weapon newWeapon)
    {
        if (_weaponSlots.Count >= _maxSlots)
        {
            Debug.Log("No Slots");
            return;
        }

        _weaponSlots.Add(newWeapon);
        _player.WeaponVisuals.SwitchOnBackupWeaponModel();
    }

    private void DropWeapon()
    {
        if (HasOnlyOneWeapon())
        {
            return;
        }

        _weaponSlots.Remove(_currentWeapon);

        EquipWeapon(0);
    }

    public void SetWeaponReady(bool ready) => _isWeaponReady = ready;
    public bool WeaponReady() => _isWeaponReady;

    private async UniTask BurstFire()
    {
        SetWeaponReady(false);

        for (int i = 1; i <= _currentWeapon.bulletsPerShot; i++)
        {
            FireSingleBullet();

            await UniTask.Delay(TimeSpan.FromSeconds(_currentWeapon.burstFireDelay)); // Use UniTask's delay for async waiting

            if (i >= _currentWeapon.bulletsPerShot)
            {
                SetWeaponReady(true);
            }
        }
    }

    private void Shoot()
    {
        if (WeaponReady() == false)
        {
            return;
        }

        if (_currentWeapon.CanShoot() == false)
        {
            return;
        }

        _player.WeaponVisuals.PlayFireAnimation();

        if (_currentWeapon._shootType == ShootType.Single)
        {
            _isShooting = false;
        }

        if (_currentWeapon.BurstActivated() == true)
        {
            BurstFire().Forget();
            return;
        }

        FireSingleBullet();
    }

    private void FireSingleBullet()
    {
        _currentWeapon.bulletsInMagazine--;

        GameObject newBullet = ObjectPool.instance.GetBulletFromPool();

        newBullet.transform.position = WeaponPoint().position;
        newBullet.transform.rotation = Quaternion.LookRotation(WeaponPoint().forward);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        bulletScript.BulletSetup(_currentWeapon.weaponDistance);

        Vector3 bulletsDirection = _currentWeapon.ApplySpread(BulletDirection());

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / _bulletSpeed;
        rbNewBullet.velocity = bulletsDirection * _bulletSpeed;
    }

    private void Reload()
    {
        SetWeaponReady(false);
        _player.WeaponVisuals.PlayReloadAnimation();
    }

    public Vector3 BulletDirection()
    {
        Transform aim = _player.Aim.Aim();

        Vector3 direction = (aim.position - WeaponPoint().position).normalized;

        if (_player.Aim.CanAimPrecisely() == false && _player.Aim.Target() == null)
        {
            direction.y = 0;
        }

        return direction;
    }

    public bool HasOnlyOneWeapon() => _weaponSlots.Count <= 1;
    public Weapon WeaponInSlots(WeaponType weaponType)
    {
        foreach (Weapon weapon in _weaponSlots)
        {
            if (weapon.weaponType == weaponType)
            {
                return weapon;
            }
        }

        return null;
    }

    public Transform WeaponPoint() => _player.WeaponVisuals.CurrentWeaponModel()._weaponPoint;
    public Weapon CurrentWeapon() => _currentWeapon;

    private void AssingInputEvents()
    {
        PlayerControls controls = _player.Controls;

        controls.Character.Fire.performed += context => _isShooting = true;
        controls.Character.Fire.canceled += context => _isShooting = false;

        controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += context => EquipWeapon(1);
        controls.Character.EquipSlot3.performed += context => EquipWeapon(2);
        controls.Character.EquipSlot4.performed += context => EquipWeapon(3);
        controls.Character.EquipSlot5.performed += context => EquipWeapon(4);

        controls.Character.DropCurrentWeapon.performed += context => DropWeapon();

        controls.Character.Reload.performed += context =>
        {
            if (_currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };

        controls.Character.ToogleWeaponMode.performed += context => _currentWeapon.ToogleBurst();
    }
}
