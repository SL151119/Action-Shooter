using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject _bulletImpactFX;
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private MeshRenderer _meshRenderer;

    private Vector3 _startPosition;
    private float _flyDistance;

    private bool _bulletDisabled;

    private void Update()
    {
        FadeTrailIfNeeded();
        DisableBulletIfNeeded();
        ReturnToPoolfNeeded();
    }

    private void DisableBulletIfNeeded()
    {
        if (Vector3.Distance(_startPosition, transform.position) > _flyDistance && !_bulletDisabled)
        {
            _collider.enabled = false;
            _meshRenderer.enabled = false;
            _bulletDisabled = true;
        }
    }

    private void ReturnToPoolfNeeded()
    {
        if (_trailRenderer.time < 0)
        {
            ObjectPool.instance.ReturnBulletToPool(gameObject);
        }
    }

    private void FadeTrailIfNeeded()
    {
        if (Vector3.Distance(_startPosition, transform.position) > _flyDistance - 1.5f)
        {
            _trailRenderer.time -= 2 * Time.deltaTime;
        }
    }

    public void BulletSetup(float flyDistance)
    {
        _collider.enabled = true;
        _meshRenderer.enabled = true;
        _bulletDisabled = false;
        _trailRenderer.time = 0.25f;
        _startPosition = transform.position;
        this._flyDistance = flyDistance + 0.5f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        CreateImpactFx(collision);
        ObjectPool.instance.ReturnBulletToPool(gameObject);
    }

    private void CreateImpactFx(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];

            GameObject newImpactFX =
                Instantiate(_bulletImpactFX, contact.point, Quaternion.LookRotation(contact.normal));

            Destroy(newImpactFX, 1f);
        }
    }
}
