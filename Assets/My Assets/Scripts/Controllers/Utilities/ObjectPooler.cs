using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPooler<T> : MonoBehaviour where T : MonoBehaviour
{

    [SerializeField] private List<PoolGroup<T>> _poolGroups;

    private Dictionary<string, Queue<T>> _pools = new Dictionary<string, Queue<T>>();


    public void SpawnAllPools()
    {
        foreach (PoolGroup<T> item in _poolGroups)
        {
            foreach (Pool<T> i in item.Group)
            {
                _pools.Add(i.Tag, new Queue<T>());
                for (int j = 0; j < i.StartAmount; j++)
                {
                    T newObj = Instantiate(i.Object, transform.position, transform.rotation, transform);
                    _pools[i.Tag].Enqueue(newObj);
                    newObj.gameObject.SetActive(false);
                }
            }
        }
    }

    private T GetObjectFromPools(string tag)
    {
        foreach (PoolGroup<T> item in _poolGroups)
        {
            foreach (Pool<T> i in item.Group)
            {
                if (i.Tag == tag)
                {
                    return i.Object;
                }
            }
        }
        return null;
    }

    private T SpawnObject(T obj, string tag, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        _pools[tag].Enqueue(obj);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.SetParent(parent);
        obj.gameObject.SetActive(true);
        return obj;
    }

    public T SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (_pools.ContainsKey(tag))
        {
            T obj = _pools[tag].Dequeue();
            return SpawnObject(obj, tag, position, rotation, parent);
        }
        print($"{tag} does not exist");
        return null;
    }

    public T CreateOrSpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (_pools.ContainsKey(tag))
        {
            T obj = null;
            for (int i = 0; i < _pools[tag].Count; i++)
            {
                obj = _pools[tag].Dequeue();
                if (obj.gameObject.activeSelf)
                {
                    _pools[tag].Enqueue(obj);
                    obj = null;
                }
                else
                {
                    break;
                }
            }
            if (obj == null)
            {
                T objToSpawn = GetObjectFromPools(tag);
                obj = Instantiate(objToSpawn, transform.position, transform.rotation, transform);
            }

            return SpawnObject(obj, tag, position, rotation, parent);
        }
        else
        {
            print($"{tag} does not exist");
            return null;
        }
    }

    [System.Serializable]
    protected class Pool<t>
    {
        public string Tag;
        public t Object;
        public int StartAmount;
    }

    [System.Serializable]
    protected class PoolGroup<Ts>
    {
        public string Name;
        public List<Pool<Ts>> Group;
    }
}