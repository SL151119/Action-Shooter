using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private Weapon _weapon;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerWeaponController weaponController))
        {
            weaponController.PickupWeapon(_weapon);
        }
    }
}
