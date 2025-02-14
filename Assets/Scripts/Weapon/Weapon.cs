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
    public ShootType shootType;
    public int BulletsPerShot { get; private set; }

    private float _defaultFireRate;
    public float fireRate = 1; //bullets per second
    private float _lastShootTime;

    private bool _burstAvailable;
    public bool burstActive;

    private int _burstBulletsPerShot;
    private float _burstFireRate;
    public float BurstFireDelay { get; private set; }

    [Header("Magazine Details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    public float ReloadSpeed { get; private set; } //how fast character reloads weapon
    public float EquipmentSpeed { get; private set; } //how fast character equips weapon

    public float WeaponDistance { get; private set; }
    public float CameraDistance { get; private set; }

    [Header("Spread")]
    private float _baseSpread = 1;
    private float _maximumSpread = 3;
    private float _currentSpread = 2;

    private float _spreadIncreaseRate = 0.15f;

    private float _lastSpreadUpdateTime;
    private float _spreadCooldown = 1;

    public Weapon_Data WeaponData { get; private set; } // serves as default weapon data

    public Weapon(Weapon_Data weaponData)
    {
        bulletsInMagazine = weaponData.bulletsInMagazine;
        magazineCapacity = weaponData.magazineCapacity;
        totalReserveAmmo = weaponData.totalReserveAmmo;

        fireRate = weaponData.fireRate;
        weaponType = weaponData.weaponType;

        BulletsPerShot = weaponData.bulletsPerShot;
        shootType = weaponData.shootType;

        _burstAvailable = weaponData.burstAvailable;
        burstActive = weaponData.burstActive;
        _burstBulletsPerShot = weaponData.burstBulletsPerShot;
        _burstFireRate = weaponData.burstFireRate;
        BurstFireDelay = weaponData.burstFireDelay;

        _baseSpread = weaponData.baseSpread;
        _maximumSpread = weaponData.maximumSpread;

        _spreadIncreaseRate = weaponData.spreadIncreaseRate;

        ReloadSpeed = weaponData.reloadSpeed;
        EquipmentSpeed = weaponData.equipmentSpeed;
        WeaponDistance = weaponData.weaponDistance;
        CameraDistance = weaponData.cameraDistance;

        _defaultFireRate = fireRate;

        this.WeaponData = weaponData;
    }

    public bool BurstActivated()
    {
        if (weaponType == WeaponType.Shotgun)
        {
            BurstFireDelay = 0;
            return true;
        }

        return burstActive;
    }

    public void ToogleBurst()
    {
        if (_burstAvailable == false)
        {
            return;
        }

        burstActive = !burstActive;

        if (burstActive)
        {
            BulletsPerShot = _burstBulletsPerShot;
            fireRate = _burstFireRate;
        }
        else
        {
            BulletsPerShot = 1;
            fireRate = _defaultFireRate;
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
        if (Time.time > _lastShootTime + 1 / fireRate)
        {
            _lastShootTime = Time.time;
            return true;
        }

        return false;
    }

    private bool HaveEnoughBullets()
    {
        return bulletsInMagazine > 0 && bulletsInMagazine >= BulletsPerShot; // 2nd condition to prevent negative bullets count
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

