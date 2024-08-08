using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject _bulletImpactFX;
    private Rigidbody rb => GetComponent<Rigidbody>();

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
