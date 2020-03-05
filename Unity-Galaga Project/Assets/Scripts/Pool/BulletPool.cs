//  BulletPool.cs
//  By Atid Puwatnuttasit

using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    #region Properties

    public static BulletPool SharedInstance { get; private set; }               // Singleton instance.
    [SerializeField] private List<GameObject> _PooledObjects;                   // Pool list object.
    [SerializeField] private GameObject _BulletToPool;                          // Bullet prefab.
    [SerializeField] private int _AmountToPool = 30;                            // Max amount of bullet in this pool.

    #endregion

    #region Methods

    #region Unity Callback Methods

    private void Awake()
    {
        if (SharedInstance == null)
        {
            SharedInstance = this;
        }
    }

    // Generate all item to the pool at the beginning.
    private void Start()
    {
        _PooledObjects = new List<GameObject>();
        for (int i = 0; i < _AmountToPool; i++)
        {
            GameObject obj = Instantiate(_BulletToPool, transform);
            obj.SetActive(false);
            _PooledObjects.Add(obj);
        }
    }

    #endregion

    #region Get and Return Methods

    /// <summary>
    /// Call this method to get bullet from the pool.
    /// </summary>
    /// <returns></returns>
    public GameObject GetBulletObject()
    {
        foreach (GameObject bullet in _PooledObjects)
        {
            if (bullet.activeInHierarchy == false)
            {
                bullet.transform.SetParent(null);
                bullet.SetActive(true);
                return bullet;
            }
        }
        return null;
    }

    /// <summary>
    /// Call this method to return bullet to the pool.
    /// </summary>
    /// <param name="bullet">Bullet.</param>
    public void ReturnBulletToPool(GameObject bullet)
    {
        if (_PooledObjects.Contains(bullet))
        {
            bullet.SetActive(false);
            bullet.transform.SetParent(transform);
        }
    }

    #endregion

    #endregion
}
