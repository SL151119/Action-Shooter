using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerControls _controls;

    public PlayerControls Controls => _controls;

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
