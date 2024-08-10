using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private Weapon_Data _weaponData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerWeaponController weaponController))
        {
            weaponController.PickupWeapon(_weaponData);
        }
    }
}
