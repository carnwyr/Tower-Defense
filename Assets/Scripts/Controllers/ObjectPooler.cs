﻿using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[System.Serializable]
public class ObjectPoolItem
{
    public GameObject ObjectToPool;
    public int AmountToPool;
}

public class ObjectPooler : MonoBehaviour, IObjectPooler
{
    public event Action<GameObject> NewObjectCreated;

    public List<ObjectPoolItem> itemsToPool;

    private List<GameObject> pooledObjects;
    private Dictionary<string, GameObject> parents = new Dictionary<string, GameObject>();

    public void Init()
    {
        pooledObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in itemsToPool)
        {
            var parent = new GameObject(item.ObjectToPool.tag);
            parents.Add(item.ObjectToPool.tag, parent);
            for (int i = 0; i < item.AmountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item.ObjectToPool);
                obj.transform.SetParent(parent.transform);
                obj.SetActive(false);
                pooledObjects.Add(obj);
                NewObjectCreated?.Invoke(obj);
            }
        }
    }

    public GameObject GetPooledObject(string tag)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].CompareTag(tag))
            {
                return pooledObjects[i];
            }
        }

        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.ObjectToPool.CompareTag(tag))
            {
                GameObject obj = (GameObject)Instantiate(item.ObjectToPool);
                obj.transform.SetParent(parents[item.ObjectToPool.tag].transform);
                obj.SetActive(false);
                pooledObjects.Add(obj);
                NewObjectCreated?.Invoke(obj);
                return obj;
            }
        }
        return null;
    }
    public List<GameObject> GetSeveral(string tag, int number)
    {
        List<GameObject> objects = new List<GameObject>();

        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].CompareTag(tag))
            {
                objects.Add(pooledObjects[i]);
                if (objects.Count == number)
                {
                    return objects;
                }
            }
        }
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.ObjectToPool.CompareTag(tag))
            {
                var missingObjects = number - objects.Count;
                for (int i = 0; i < missingObjects; i++)
                {
                    GameObject obj = (GameObject)Instantiate(item.ObjectToPool);
                    obj.transform.SetParent(parents[item.ObjectToPool.tag].transform);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                    objects.Add(obj);
                    NewObjectCreated?.Invoke(obj);
                }
            }
        }
        return objects;
    }

    public void HideByTag(string tag)
    {
        foreach (var obj in pooledObjects)
        {
            if (obj.activeInHierarchy && obj.CompareTag(tag))
            {
                obj.SetActive(false);
            }
        }
    }

    public List<GameObject> GetFullList(string tag)
    {
        return pooledObjects.Where(x => x.CompareTag(tag)).ToList();
    }
}
