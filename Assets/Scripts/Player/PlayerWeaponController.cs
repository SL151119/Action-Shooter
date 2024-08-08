using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private const float REFERENCE_BULLET_SPEED = 20f; //this is default speed from which our mass formula is derived

    [SerializeField] private Player _player;
    [SerializeField] private Animator _animator;

    [SerializeField] private Weapon _currentWeapon;

    [Header("Bullet Details")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _bulletSpeed;

    [SerializeField] private Transform _weaponPoint;
    [SerializeField] private Transform _weaponHolder;

    [Header("Inventory")]
    [SerializeField] private int _maxSlots = 2;
    [SerializeField] private List<Weapon> _weaponSlots;

    private void Start()
    {
        AssingInputEvents();

        Invoke(nameof(EquipStartingWeapon), 0.1f);
    }

    private void EquipStartingWeapon()
    {
        EquipWeapon(0);
    }

    private void EquipWeapon(int weaponIndex)
    {
        _currentWeapon = _weaponSlots[weaponIndex];

        _player.WeaponVisuals.PlayWeaponEquipAnimation();
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

    private void Shoot()
    {
        if (_currentWeapon.CanShoot() == false)
        {
            return;
        }

        GameObject newBullet = 
            Instantiate(_bulletPrefab, _weaponPoint.position, Quaternion.LookRotation(_weaponPoint.forward));

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / _bulletSpeed;
        rbNewBullet.velocity = BulletDirection() * _bulletSpeed;

        Destroy(newBullet, 10);

        _animator.SetTrigger("Fire");
    }

    public Vector3 BulletDirection()
    {
        Transform aim = _player.Aim.Aim();

        Vector3 direction = (aim.position - _weaponPoint.position).normalized;

        if (_player.Aim.CanAimPrecisely() == false && _player.Aim.Target() == null)
        {
            direction.y = 0;
        }

        //_weaponHolder.LookAt(_aim);
        //_weaponPoint.LookAt(_aim); // TODO: find a better place for it

        return direction;
    }

    public bool HasOnlyOneWeapon() => _weaponSlots.Count <= 1;

    public Transform WeaponPoint() => _weaponPoint;
    public Weapon CurrentWeapon() => _currentWeapon;
    public Weapon BackupWeapon()
    {
        foreach(Weapon weapon in _weaponSlots)
        {
            if(weapon != _currentWeapon)
            {
                return weapon;
            }
        }

        return null;
    }

    private void AssingInputEvents()
    {
        PlayerControls controls = _player.Controls;

        controls.Character.Fire.performed += context => Shoot();

        controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += context => EquipWeapon(1);

        controls.Character.DropCurrentWeapon.performed += context => DropWeapon();

        controls.Character.Reload.performed += context =>
        {
            if (_currentWeapon.CanReload())
            {
                _player.WeaponVisuals.PlayReloadAnimation();
            }
        };
    }
}
