using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Animator _animator;

    private void Start()
    {
        _player.Controls.Character.Fire.performed += context => Shoot();
    }

    private void Shoot()
    {
        _animator.SetTrigger("Fire");
    }
}
