using JetBrains.Annotations;
using UnityEngine;

public enum WeaponType
{
    Pistol = 0,
    Revolver = 1,
    AutoRifle = 2,
    Shotgun = 3,
    SniperRifle = 4,
}

public enum ShootType
{
    Single = 0,
    Auto = 1,
}

[System.Serializable] //Class visible in the inspector
public class Weapon
{
    public WeaponType weaponType;

    [Header("Shooting Specifics")]
    public ShootType _shootType;
    public int bulletsPerShot;
    public float defaultFireRate;
    [SerializeField] private float _fireRate = 1; //bullets per second
    private float _lastShootTime;

    [Header("Burst Fire")]
    public bool burstAvalible;
    public bool burstActive;
    public int burstBulletsPerShot;
    public float burstFireRate;
    public float burstFireDelay = 0.1f;

    [Header("Magazine Details")]
    public int bulletsInMagazine;
    [SerializeField] private int magazineCapacity;
    [SerializeField] private int totalReserveAmmo;

    [Range(1, 3)]
    public float reloadSpeed = 1; //how fast character reloads weapon
    [Range(1, 3)]
    public float equipmentSpeed = 1; //how fast character equips weapon
    [Range(2, 12)]
    public float weaponDistance = 4;
    [Range(3, 8)]
    public float cameraDistance = 6;

    [Header("Spread")]
    [SerializeField] private float _baseSpread = 1;
    [SerializeField] private float _maximumSpread = 3;
    private float _currentSpread = 2;

    [SerializeField] private float _spreadIncreaseRate = 0.15f;

    private float _lastSpreadUpdateTime;
    private float _spreadCooldown = 1;

    public bool BurstActivated()
    {
        if (weaponType == WeaponType.Shotgun)
        {
            burstFireDelay = 0;
            return true;
        }

        return burstActive;
    }

    public void ToogleBurst()
    {
        if (burstAvalible == false)
        {
            return;
        }

        burstActive = !burstActive;

        if (burstActive)
        {
            bulletsPerShot = burstBulletsPerShot;
            _fireRate = burstFireRate;
        }
        else
        {
            bulletsPerShot = 1;
            _fireRate = defaultFireRate;
        }
    }

    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        float randomizedValue = Random.Range(-_currentSpread, _currentSpread);

        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);

        return spreadRotation * originalDirection;
    }

    private void UpdateSpread()
    {
        if (Time.time > _lastSpreadUpdateTime + _spreadCooldown)
        {
            _currentSpread = _baseSpread;
        }
        else
        {
            IncreaseSpread();
        }

        _lastSpreadUpdateTime = Time.time;
    }

    private void IncreaseSpread()
    {
        _currentSpread = Mathf.Clamp(_currentSpread + _spreadIncreaseRate, _baseSpread, _maximumSpread);
    }

    public bool CanShoot() => HaveEnoughBullets() && ReadyToShoot();

    private bool ReadyToShoot()
    {
        if (Time.time > _lastShootTime + 1 / _fireRate)
        {
            _lastShootTime = Time.time;
            return true;
        }

        return false;
    }

    private bool HaveEnoughBullets()
    {
        return bulletsInMagazine > 0;
    }

    public bool CanReload()
    {
        if (bulletsInMagazine == magazineCapacity)
        {
            return false;
        }

        if (totalReserveAmmo > 0)
        {
            return true;
        }

        return false;
    }

    public void RefillBullets()
    {
        totalReserveAmmo += bulletsInMagazine; // this will add bullets in magazine to total amount of bullets

        int bulletsToReload = magazineCapacity;

        if (bulletsToReload > totalReserveAmmo)
        {
            bulletsToReload = totalReserveAmmo;
        }

        totalReserveAmmo -= bulletsToReload;
        bulletsInMagazine = bulletsToReload;

        if (totalReserveAmmo < 0)
        {
            totalReserveAmmo = 0;
        }
    }
}

