using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Animator _animator;

    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _weaponPoint;
    [SerializeField] private float _bulletSpeed;

    [SerializeField] private Transform _weaponHolder;

    private void Start()
    {
        _player.Controls.Character.Fire.performed += context => Shoot();
    }

    private void Shoot()
    {
        GameObject newBullet = 
            Instantiate(_bulletPrefab, _weaponPoint.position, Quaternion.LookRotation(_weaponPoint.forward));

        newBullet.GetComponent<Rigidbody>().velocity = BulletDirection() * _bulletSpeed;

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
