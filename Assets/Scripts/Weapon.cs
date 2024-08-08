using UnityEngine;

public enum WeaponType
{
    Pistol = 0,
    Revolver = 1,
    AutoRifle = 2,
    Shotgun = 3,
    SniperRifle = 4,
}

[System.Serializable] //Class visible in the inspector
public class Weapon
{
    public WeaponType weaponType;

    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    [Range(1f, 3f)]
    public float reloadSpeed = 1f; //how fast character reloads weapon
    [Range(1f, 3f)]
    public float equipmentSpeed = 1f; //how fast character equips weapon

    public bool CanShoot()
    {
        return HaveEnoughBullets();
    }

    private bool HaveEnoughBullets()
    {
        if (bulletsInMagazine > 0)
        {
            bulletsInMagazine--;
            return true;
        }

        return false;
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

