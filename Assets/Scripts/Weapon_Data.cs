using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon System/WeaponData")]
public class Weapon_Data : ScriptableObject
{
    public string weaponName;

    [Header("Magazine Details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    [Header("Regular Shot")]
    public ShootType shootType;
    public int bulletsPerShot = 1;
    public float fireRate;

    [Header("Burst Shot")]
    public bool burstAvailable;
    public bool burstActive;
    public int burstBulletsPerShot;
    public float burstFireRate;
    public float burstFireDelay = 0.1f;

    [Header("Weapon Spread")]
    public float baseSpread = 1;
    public float maximumSpread = 3;
    public float spreadIncreaseRate = 0.15f;

    [Header("Weapon Generics")]
    public WeaponType weaponType;
    [Range(1, 3)]
    public float reloadSpeed = 1;
    [Range(1, 3)]
    public float equipmentSpeed = 1;
    [Range(4,8)] 
    public float weaponDistance = 4;
    [Range(4, 8)]
    public float cameraDistance = 6;
}
