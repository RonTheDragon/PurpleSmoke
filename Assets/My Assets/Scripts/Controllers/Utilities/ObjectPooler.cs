using UnityEngine;
using System.Collections.Generic;

public class ObjectPooler<T> : MonoBehaviour where T : Component
{
    [SerializeField] private List<Pool<T>> _pools;
    // Dictionary to store pools for different types
    private Dictionary<string, Queue<T>> _objectPools = new Dictionary<string, Queue<T>>();


    public void CreateAllPools()
    {
        foreach (Pool<T> pool in _pools)
        {
            CreatePool(pool.Tag,pool.Prefab,pool.Amount);
        }
    }

    // Create a new pool for a specific type
    public void CreatePool(string tag,T prefab, int size)
    {
        if (!_objectPools.ContainsKey(tag))
        {
            _objectPools[tag] = new Queue<T>();

            for (int i = 0; i < size; i++)
            {
                T obj = Instantiate(prefab,transform);
                obj.gameObject.SetActive(false);
                _objectPools[tag].Enqueue(obj);
            }
        }
        else
        {
            Debug.LogWarning("Pool for " + tag + " already exists!");
        }
    }

    // Get an object from the pool
    public T GetObject(string tag)
    {
        if (_objectPools.ContainsKey(tag) && _objectPools[tag].Count > 0)
        {
            T obj = _objectPools[tag].Dequeue();
            _objectPools[tag].Enqueue(obj );
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            Debug.LogWarning("Pool for " + tag + " is empty or does not exist!");
            return null;
        }
    }

    // Spawn an object from the pool pretending like Instantiate
    public T SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (_objectPools.ContainsKey(tag) && _objectPools[tag].Count > 0)
        {
            T obj = GetObject(tag);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            return obj;
        }
        else
        {
            Debug.LogWarning("Pool for " + tag + " is empty or does not exist!");
            return null;
        }
    }

    [System.Serializable]
    public class Pool<TT>
    {
        public string Tag;
        public TT Prefab;
        public int Amount;
    }
}
