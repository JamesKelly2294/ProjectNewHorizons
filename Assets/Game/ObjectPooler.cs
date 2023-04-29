using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int amountToPool;
    public bool shouldExpand = true;
}

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    public Dictionary<string, List<GameObject>> pooledObjects;
    public List<ObjectPoolItem> itemsToPool;
    private Dictionary<string, ObjectPoolItem> itemsToPoolLookup;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pooledObjects = new Dictionary<string, List<GameObject>>();
        itemsToPoolLookup = new Dictionary<string, ObjectPoolItem>();
        foreach (ObjectPoolItem item in itemsToPool)
        {
            itemsToPoolLookup[item.objectToPool.tag] = item;
            pooledObjects[item.objectToPool.tag] = new List<GameObject>();
            for (int i = 0; i < item.amountToPool; i++)
            {
                var obj = Instantiate(item.objectToPool);
                obj.transform.parent = transform;
                obj.SetActive(false);
                pooledObjects[item.objectToPool.tag].Add(obj);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetPooledObject(string tag)
    {
        if(!itemsToPoolLookup.ContainsKey(tag)) { return null; }

        var pooledObjectsForTag = pooledObjects[tag];
        var item = itemsToPoolLookup[tag];

        for (int i = 0; i < pooledObjectsForTag.Count; i++)
        {
            if (!pooledObjectsForTag[i].activeInHierarchy)
            {
                return pooledObjectsForTag[i];
            }
        }

        if (item.shouldExpand)
        {
            var obj = Instantiate(item.objectToPool);
            obj.transform.parent = transform;
            obj.SetActive(false);
            pooledObjectsForTag.Add(obj);
            return obj;
        } else
        {
            return null;
        }
    }

    IEnumerator ReturnObjectToPoolAfterDelayHelper(GameObject go, float time)
    {
        yield return new WaitForSeconds(time);
        go.SetActive(false); // referencing 'self' as a local variable is probably not needed, but not wrong, either.
    }

    public void ReturnObjectToPoolAfterDelay(GameObject go, float time)
    {
        StartCoroutine(ReturnObjectToPoolAfterDelayHelper(go, time));
    }
}