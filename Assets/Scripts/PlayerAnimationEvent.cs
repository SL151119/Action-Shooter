using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    [SerializeField] private PlayerWeaponVisuals _visualController;

    public void ReloadIsOver()
    {
        _visualController.MaximizeRigWeight();
    }

    public void ReturnRig()
    {
        _visualController.MaximizeRigWeight();
        _visualController.MaximizeLeftHandWeight();
    }

    public void WeaponGrabIsOver()
    {
        _visualController.SetBusyGrabbingWeaponTo(false);
    }
}
