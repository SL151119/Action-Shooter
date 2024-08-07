using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private const float REFERENCE_BULLET_SPEED = 20f; //this is default speed from which our mass formula is derived

    [SerializeField] private Player _player;
    [SerializeField] private Animator _animator;

    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _bulletSpeed;

    [SerializeField] private Transform _weaponPoint;
    [SerializeField] private Transform _weaponHolder;

    private void Start()
    {
        _player.Controls.Character.Fire.performed += context => Shoot();
    }

    private void Shoot()
    {
        GameObject newBullet = 
            Instantiate(_bulletPrefab, _weaponPoint.position, Quaternion.LookRotation(_weaponPoint.forward));

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / _bulletSpeed;
        rbNewBullet.velocity = BulletDirection() * _bulletSpeed;

        Destroy(newBullet, 10);

        _animator.SetTrigger("Fire");
    }

    public Vector3 BulletDirection()
    {
        Transform aim = _player.Aim.Aim();

        Vector3 direction = (aim.position - _weaponPoint.position).normalized;

        if (_player.Aim.CanAimPrecisely() == false && _player.Aim.Target() == null)
        {
            direction.y = 0;
        }

        //_weaponHolder.LookAt(_aim);
        //_weaponPoint.LookAt(_aim); // TODO: find a better place for it

        return direction;
    }

    public Transform WeaponPoint() => _weaponPoint;
}
