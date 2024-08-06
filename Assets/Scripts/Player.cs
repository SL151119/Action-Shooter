using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerAim _aim;
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerWeaponController _weaponController;

    private PlayerControls _controls;

    public PlayerAim Aim => _aim;
    public PlayerControls Controls => _controls;
    public PlayerMovement Movement => _movement;
    public PlayerWeaponController WeaponController => _weaponController;

    private void Awake()
    {
        _controls = new PlayerControls();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }
}
