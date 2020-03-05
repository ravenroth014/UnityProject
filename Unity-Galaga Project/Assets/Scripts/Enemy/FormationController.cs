//  FormationController.cs
//  By Atid Puwatnuttasit

using System.Collections.Generic;
using UnityEngine;

public class FormationController : MonoBehaviour
{
    #region Inspector Properties

    [Header("Formation Setting")]
    [SerializeField] private int _GridSizeX = 10;
    [SerializeField] private int _GridSizeY = 2;

    [SerializeField] private float _GridOffSetX = 1f;
    [SerializeField] private float _GridOffSetY = 1f;

    [SerializeField] private float _MaxOffset = -3;
    [SerializeField] private float _MinOffset = -9;

    [SerializeField] private float _Speed = 1f;
    [SerializeField] private int _Divider = 4;

    [Header("Diving Path Setting")] 
    [SerializeField] private Path _LeftDivePath;
    [SerializeField] private Path _RightDivePath;

    [Header("Other Setting")] 
    [SerializeField] private bool _ShowFormation = true;
    [SerializeField] private EnemyType _EnemyType;

    #endregion

    #region Private Properties

    private List<Vector3> _gridList = new List<Vector3>();              // The positions for the registered enemy.
    private List<BaseEnemyController> _enemyControllers;                // the registered enemy's instance.

    private float _curPosX;
    private Vector3 _startPos;
    private int _direction = -1;
    private int _count = 0;

    private int _activeCount;                                           // Enemy active number.
    #endregion

    #region Public Properties

    public int TotalEnemy => _activeCount;
    public EnemyType EnemyType => _EnemyType;

    #endregion

    #region Methods

    #region Unity Callback Methods

    private void Awake()
    {
        _enemyControllers = new List<BaseEnemyController>();
    }

    private void Start()
    {
        _startPos = transform.position;
        _curPosX = transform.position.x;

        CreateGrid();
    }

    private void Update()
    {
        UpdateFormationPos();
    }

    // Subscribe event.
    private void OnEnable()
    {
        GameManager.OnStageClear += GameManager_OnStageClear;

        UIManager.OnResetGameScene += UiManager_OnResetGameScene;
        BaseEnemyController.OnDeductEnemy += BaseEnemyController_OnEnemyDied;
    }

    // Unsubscribe event.
    private void OnDisable()
    {
        GameManager.OnStageClear -= GameManager_OnStageClear;

        UIManager.OnResetGameScene -= UiManager_OnResetGameScene;
        BaseEnemyController.OnDeductEnemy -= BaseEnemyController_OnEnemyDied;
    }

    // Debug Method.
    private void OnDrawGizmos()
    {
        if (!_ShowFormation) return;

        int index = 0;

        CreateGrid();
        foreach (Vector3 position in _gridList)
        {
            Gizmos.DrawWireSphere(GetPosition(index++), 0.1f);
        }
    }

    #endregion

    #region Generate Methods

    /// <summary>
    /// Call this method to create grid position for enemy.
    /// </summary>
    private void CreateGrid()
    {
        _gridList.Clear();

        int num = 0;

        for (int i = 0; i < _GridSizeX; i++)
        {
            for (int j = 0; j < _GridSizeY; j++)
            {
                float x = (_GridOffSetX + _GridOffSetX * 2 * (num/_Divider)) * Mathf.Pow(-1, num%2+1);
                float y = _GridOffSetY * (num % _Divider / 2);
                Vector3 vector3 = new Vector3(x, y, 0);
                num++;

                _gridList.Add(vector3);
            }
        }
    }

    #endregion

    #region Get Methods

    /// <summary>
    /// Call this method to get the position for the specific enemy.
    /// </summary>
    /// <param name="index">Enemy ID.</param>
    /// <returns></returns>
    public Vector3 GetPosition(int index)
    {
        return transform.position + _gridList[index];
    }

    /// <summary>
    /// Call this method to get new enemy ID.
    /// </summary>
    /// <returns></returns>
    public int GetNewEnemyID()
    {
        _activeCount++;
        return _count++;
    }

    #endregion

    #region Action Methods

    /// <summary>
    /// Call this method to order the enemy in this formation to attack player.
    /// </summary>
    /// <param name="caller">Game manager</param>
    public void AttackPlayer(object caller)
    {
        if (caller.GetType() != typeof(GameManager)) return;

        if (_enemyControllers.Count > 0)
        {
            int index = _enemyControllers[0].EnemyID;
            _enemyControllers[0].UpdatePath(this, index % 2 == 0 ? _LeftDivePath : _RightDivePath);

            _enemyControllers[0].UpdateState(this, EnemyStates.Diving);
            _enemyControllers[0].transform.SetParent(null);
            _enemyControllers.RemoveAt(0);
        }
    }

    #endregion

    #region Update Methods

    /// <summary>
    /// Call this method to register enemy instance into this formation.
    /// </summary>
    /// <param name="enemyController">Enemy instance</param>
    public void AddEnemy(BaseEnemyController enemyController)
    {
        _enemyControllers.Add(enemyController);
    }

    /// <summary>
    /// Call this method to remove enemy instance from this formation.
    /// </summary>
    /// <param name="enemyController">Enemy instance</param>
    public void RemoveEnemy(BaseEnemyController enemyController)
    {
        if (_enemyControllers.Contains(enemyController))
            _enemyControllers.Remove(enemyController);
    }

    /// <summary>
    /// Call this method to reset data in this formation.
    /// </summary>
    private void OnReset()
    {
        _count = 0;
        _activeCount = 0;
        _enemyControllers.Clear();
    }

    /// <summary>
    /// Call this method to update formation's position.
    /// </summary>
    private void UpdateFormationPos()
    {
        _curPosX += Time.deltaTime * _Speed * _direction;
        if (_curPosX >= _MaxOffset)
        {
            _direction *= -1;
        }
        else if (_curPosX <= _MinOffset)
        {
            _direction *= -1;
        }
        transform.position = new Vector3(_curPosX, _startPos.y, _startPos.z);
    }

    #endregion

    #region Event Methods

    /// <summary>
    /// Call this method when the stage is cleared.
    /// </summary>
    private void GameManager_OnStageClear()
    {
        OnReset();
    }

    /// <summary>
    /// Call this method when scene reset is requested.
    /// </summary>
    private void UiManager_OnResetGameScene()
    {
        OnReset();
    }

    /// <summary>
    /// Call this method when the dead enemy is occured, deduct the current active member number in this formation. 
    /// </summary>
    /// <param name="obj"></param>
    private void BaseEnemyController_OnEnemyDied(EnemyType obj)
    {
        if(_EnemyType == obj)
            _activeCount--;
    }

    #endregion

    #endregion
}
