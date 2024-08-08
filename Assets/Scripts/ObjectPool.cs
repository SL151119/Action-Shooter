using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private int _poolSize = 50;

    private Queue<GameObject> _bulletPool;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _bulletPool = new Queue<GameObject>();

        CreateInitialPool();
    }

    public GameObject GetBulletFromPool()
    {
        if (_bulletPool.Count == 0)
        {
            CreateNewBullet();
        }

        GameObject bulletToGet = _bulletPool.Dequeue();
        bulletToGet.SetActive(true);
        bulletToGet.transform.parent = null;

        return bulletToGet;
    }

    public void ReturnBulletToPool(GameObject bullet)
    {
        bullet.SetActive(false);
        _bulletPool.Enqueue(bullet);
        bullet.transform.parent = transform;
    }

    private void CreateInitialPool()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            CreateNewBullet();
        }
    }

    private void CreateNewBullet()
    {
        GameObject newBullet = Instantiate(_bulletPrefab, transform);
        newBullet.SetActive(false);
        _bulletPool.Enqueue(newBullet);
    }
}
