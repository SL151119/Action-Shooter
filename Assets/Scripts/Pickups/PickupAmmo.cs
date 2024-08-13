using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AmmoData
{
    public WeaponType _weaponType;
    [Range(10, 100)] public int _minAmount;
    [Range(10, 100)] public int _maxAmount;
}

public enum AmmoBoxType
{
    SmallAmmoBox = 0,
    BigAmmoBox = 1,
}

public class PickupAmmo : Interactable
{
    [SerializeField] private AmmoBoxType _boxType;

    [SerializeField] private List<AmmoData> _smallAmmoBox;
    [SerializeField] private List<AmmoData> _bigAmmoBox;

    [SerializeField] private GameObject[] _boxModels;

    private void Start()
    {
        SetupBoxModels();
    }

    public override void Interaction()
    {
        List<AmmoData> currentAmmoList = _smallAmmoBox;

        if (_boxType == AmmoBoxType.BigAmmoBox)
        {
            currentAmmoList = _bigAmmoBox;
        }

        foreach (AmmoData ammo in currentAmmoList)
        {
            Weapon weapon = _weaponController.WeaponInSlots(ammo._weaponType);

            AddBulletsToWeapon(weapon, GetBulletsAmount(ammo));
        }

        ObjectPool.instance.ReturnObject(gameObject);
    }

    private int GetBulletsAmount(AmmoData ammoData)
    {
        float min = Mathf.Min(ammoData._minAmount, ammoData._maxAmount);
        float max = Mathf.Max(ammoData._minAmount, ammoData._maxAmount);

        float randomAmmoAmount = Random.Range(min, max);

        return Mathf.RoundToInt(randomAmmoAmount);
    }

    private void AddBulletsToWeapon(Weapon weapon, int amount)
    {
        if (weapon == null)
        {
            return;
        }

        weapon.totalReserveAmmo += amount;
    }

    private void SetupBoxModels()
    {
        for (int i = 0; i < _boxModels.Length; i++)
        {
            _boxModels[i].SetActive(false);

            if (i == ((int)_boxType))
            {
                _boxModels[i].SetActive(true);
                UpdateMeshAndMaterial(_boxModels[i].GetComponent<MeshRenderer>());
            }
        }
    }
}
