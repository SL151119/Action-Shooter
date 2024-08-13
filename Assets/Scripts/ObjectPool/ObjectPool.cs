using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] private int _poolSize = 10;

    private Dictionary<GameObject, Queue<GameObject>> _poolDictionary =
        new Dictionary<GameObject, Queue<GameObject>>();

    [Header("To Initialize")]
    [SerializeField] private GameObject _weaponPickup;
    [SerializeField] private GameObject _ammoPickup;

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
        InitializeNewPool(_weaponPickup);
        InitializeNewPool(_ammoPickup);
    }

    public GameObject GetObject(GameObject prefab)
    {
        if (_poolDictionary.ContainsKey(prefab) == false)
        {
            InitializeNewPool(prefab);
        }

        if (_poolDictionary[prefab].Count == 0)
        {
            CreateNewObject(prefab);
        }

        GameObject objectToGet = _poolDictionary[prefab].Dequeue();
        objectToGet.SetActive(true);
        objectToGet.transform.parent = null;

        return objectToGet;
    }

    public void ReturnObject(GameObject objectToReturn, float delay = 0.001f)
    {
        DelayReturn(objectToReturn, delay).Forget();
    }

    private async UniTask DelayReturn(GameObject objectToReturn, float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));

        ReturnToPool(objectToReturn);
    }

    private void ReturnToPool(GameObject objectToReturn)
    {
        GameObject originalPrefab = objectToReturn.GetComponent<PooledObject>().originalPrefab;
        
        objectToReturn.SetActive(false);
        objectToReturn.transform.parent = transform;

        _poolDictionary[originalPrefab].Enqueue(objectToReturn);
    }

    private void InitializeNewPool(GameObject prefab)
    {
        _poolDictionary[prefab] = new Queue<GameObject>();

        for (int i = 0; i < _poolSize; i++)
        {
            CreateNewObject(prefab);
        }
    }

    private void CreateNewObject(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab, transform);
        newObject.AddComponent<PooledObject>().originalPrefab = prefab;

        newObject.SetActive(false);
        _poolDictionary[prefab].Enqueue(newObject);
    }
}
