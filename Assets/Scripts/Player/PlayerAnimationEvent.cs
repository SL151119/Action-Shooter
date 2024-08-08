using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    [SerializeField] private PlayerWeaponVisuals _visualController;
    [SerializeField] private PlayerWeaponController _weaponController;

    public void ReloadIsOver()
    {
        _visualController.MaximizeRigWeight();
        _weaponController.CurrentWeapon().RefillBullets();

        _weaponController.SetWeaponReady(true);
    }

    public void ReturnRig()
    {
        _visualController.MaximizeRigWeight();
        _visualController.MaximizeLeftHandWeight();
    }

    public void WeaponEquipingIsOver()
    {
        _weaponController.SetWeaponReady(true);
    }

    public void SwitchOnWeaponModel() => _visualController.SwitchOnCurrentWeaponModel();
}
