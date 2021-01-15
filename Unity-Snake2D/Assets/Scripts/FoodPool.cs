using System.Collections.Generic;
using UnityEngine;

public class FoodPool : MonoBehaviour
{
    #region Properties
    public static FoodPool SharedInstance { get; private set; }                 // Singleton.
    [SerializeField] private List<GameObject> _PooledObjects;                   // Pool object list.
    [SerializeField] private GameObject _FoodPrefab;                            // Food prefab.
    [SerializeField] private List<Sprite> _FoodSpriteList;                      // Food's sprite list.
    [SerializeField] private int _AmountToPool = 30;                            // Total object is available in the pool.
    #endregion

    #region Methods
    private void Awake()
    {
        if (SharedInstance == null || SharedInstance != this)
        {
            SharedInstance = this;
        }
    }

    private void Start()
    {
        _PooledObjects = new List<GameObject>();
        for (int i = 0; i < _AmountToPool; i++)
        {
            GameObject obj = Instantiate(_FoodPrefab, transform);
            obj.SetActive(false);
            _PooledObjects.Add(obj);
        }
    }

    public GameObject GetFoodObject()
    {
        foreach (GameObject food in _PooledObjects)
        {
            if (food.activeInHierarchy == false)
            {
                SpriteRenderer spriteRenderer = food.GetComponent<SpriteRenderer>();
                int randIndex = Random.Range(1, _FoodSpriteList.Count) - 1;
                spriteRenderer.sprite = _FoodSpriteList[randIndex];
                food.transform.SetParent(null);
                food.SetActive(true);
                return food;
            }
        }
        return null;
    }

    public void ReturnFoodToPool(GameObject food)
    {
        if (_PooledObjects.Contains(food))
        {
            food.SetActive(false);
            food.transform.SetParent(transform);
        }
    }
    #endregion
}
