//  CharacterPool.cs
//  By Atid Puwatnuttasit

using System.Collections.Generic;
using UnityEngine;

public class CharacterPool : MonoBehaviour
{
    #region Public Properties

    public static CharacterPool Instance { get; private set; }              // Singleton instance.

    #endregion

    #region Inspector Properties

    [Header("Character Pooling Setting.")]
    [SerializeField] private GameObject _PlayerToPool;                          // Player prefab.
    [SerializeField] private int _PlayerAmountToPool = 1;                       // Max player object.
    [SerializeField] private GameObject _BlueEnemyToPool;                       // Blue prefab.
    [SerializeField] private int _BlueEnemyAmountToPool = 20;                   // Max blue object.
    [SerializeField] private GameObject _RedEnemyToPool;                        // Red prefab.
    [SerializeField] private int _RedEnemyAmountToPool = 16;                    // Max red object.
    [SerializeField] private GameObject _GreenEnemyToPool;                      // Green prefab.
    [SerializeField] private int _GreenEnemyAmountToPool = 4;                   // Max green object.

    #endregion

    #region Private Properties

    private List<GameObject> _PooledPlayer;                         // Player's pool list.
    private List<GameObject> _PooledBlueEnemy;                      // Blue's pool list.
    private List<GameObject> _PooledRedEnemy;                       // Red's pool list.
    private List<GameObject> _PooledGreenEnemy;                     // Green's pool list.

    #endregion

    #region Methods

    #region Unity Callback Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        Init();
    }

    #endregion

    #region Init Methods

    /// <summary>
    /// Call this method to generate all pool item from the beginning.
    /// </summary>
    private void Init()
    {
        _PooledPlayer = new List<GameObject>();
        _PooledBlueEnemy = new List<GameObject>();
        _PooledRedEnemy = new List<GameObject>();
        _PooledGreenEnemy = new List<GameObject>();

        for (int i = 0; i < _PlayerAmountToPool; i++)
        {
            GameObject obj = Instantiate(_PlayerToPool, transform);
            obj.SetActive(false);
            _PooledPlayer.Add(obj);
        }

        for (int i = 0; i < _BlueEnemyAmountToPool; i++)
        {
            GameObject obj = Instantiate(_BlueEnemyToPool, transform);
            obj.SetActive(false);
            _PooledBlueEnemy.Add(obj);
        }

        for (int i = 0; i < _RedEnemyAmountToPool; i++)
        {
            GameObject obj = Instantiate(_RedEnemyToPool, transform);
            obj.SetActive(false);
            _PooledRedEnemy.Add(obj);
        }

        for (int i = 0; i < _GreenEnemyAmountToPool; i++)
        {
            GameObject obj = Instantiate(_GreenEnemyToPool, transform);
            obj.SetActive(false);
            _PooledGreenEnemy.Add(obj);
        }
    }

    #endregion

    #region Get From Pool Methods

    /// <summary>
    /// Get Player game object from pool.
    /// </summary>
    /// <returns></returns>
    public GameObject GetPlayerObject()
    {
        foreach (GameObject player in _PooledPlayer)
        {
            if (player.activeInHierarchy == false)
            {
                player.transform.SetParent(null);
                player.SetActive(true);
                return player;
            }
        }
        return null;
    }

    /// <summary>
    /// Get Green game object from pool.
    /// </summary>
    /// <returns></returns>
    public GameObject GetGreenEnemyObject()
    {
        foreach (GameObject greenEnemyObject in _PooledGreenEnemy)
        {
            if (greenEnemyObject.activeInHierarchy == false)
            {
                greenEnemyObject.transform.SetParent(null);
                greenEnemyObject.SetActive(true);
                return greenEnemyObject;
            }
        }
        return null;
    }

    /// <summary>
    /// Get Blue game object from pool.
    /// </summary>
    /// <returns></returns>
    public GameObject GetBlueEnemyObject()
    {
        foreach (GameObject blueEnemyObject in _PooledBlueEnemy)
        {
            if (blueEnemyObject.activeInHierarchy == false)
            {
                blueEnemyObject.transform.SetParent(null);
                blueEnemyObject.SetActive(true);
                return blueEnemyObject;
            }
        }
        return null;
    }

    /// <summary>
    /// Get Red game object from pool.
    /// </summary>
    /// <returns></returns>
    public GameObject GetRedEnemyObject()
    {
        foreach (GameObject redEnemyObject in _PooledRedEnemy)
        {
            if (redEnemyObject.activeInHierarchy == false)
            {
                redEnemyObject.transform.SetParent(null);
                redEnemyObject.SetActive(true);
                return redEnemyObject;
            }
        }
        return null;
    }

    #endregion

    #region Return Pool

    /// <summary>
    /// Call this method to return game object to pool.
    /// </summary>
    /// <param name="character">Character object.</param>
    public void ReturnToPool(GameObject character)
    {
        if (_PooledPlayer.Contains(character)
            || _PooledBlueEnemy.Contains(character)
            || _PooledRedEnemy.Contains(character)
            || _PooledGreenEnemy.Contains(character))
        {
            character.SetActive(false);
            character.transform.SetParent(this.transform);
        } 
    }

    #endregion

    #endregion
}
