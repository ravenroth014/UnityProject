//  BaseEnemyController.cs
//  By Atid Puwatnuttasit

using System;
using UnityEngine;

public class BaseEnemyController : MonoBehaviour
{
    #region Static Value

    protected static readonly Quaternion _defaultDirection = Quaternion.Euler(0, 0, 0);
    protected static readonly float _reachOffset = 0.4f;
    protected static readonly float _reachFormationOffset = 0.001f;
    protected static readonly float _angleOffset = 90f;
    protected static readonly float _doubleSpeed = 3f;
    protected static readonly string _wallTagName = "Wall";
    protected static readonly string _PlayerTagName = "Player";

    #endregion

    #region Public Properties
    
    public int EnemyID => _enemyID;                                     // Enemy ID on its formation.
    public float Speed => _speed;                                       // Enemy's speed.

    public Path Path => _path;                                          // Enemy's path for path following logic.
    public FormationController EnemyFormation => _enemyFormation;       // Registered formation.

    public EnemyStates CurrentState => _enemyState;                     // Action state.

    #endregion

    #region Protected Properties

    protected int _enemyID;

    protected Path _path;
    protected FormationController _enemyFormation;

    protected EnemyStates _enemyState;
    protected CharacterType _characterType;                             // Character type.
    protected EnemyType _enemyType;                                     // Enemy type.

    protected int _currentIndex;                                        // Current path index.
    protected int _currentHealth;                                       // Current health.
    protected bool _isInit;                                             // Initialization state.
    protected float _speed;     

    protected bool _isHit = false;                                      // Hit state.

    #endregion

    #region Inspector Properties

    [Header("Enemy Setting")]
    [SerializeField] protected int _BaseHealth;                         // Base health.

    #endregion

    #region Events

    public static event Action<EnemyType> OnDeductEnemy;                // Enemy deduction event.
    public static event Action<EnemyType> OnEnemyDied;                  // Enemy died event.

    #endregion

    #region Methods

    #region Unity Callback Methods

    protected void Update()
    {
        // If this instance is initialized and the game is not over, Do the action.
        if(_isInit && GameManager.Instance.IsGameActive && GameManager.Instance.IsGameOver == false)
            EnemyDecisionMaking();
    }

    // Subscribe event.
    protected virtual void OnEnable()
    {
        GameManager.OnRemoveAllCharacter += GameManager_OnRemoveAllCharacter;
    }

    // Unsubscribe event.
    protected virtual void OnDisable()
    {
        GameManager.OnRemoveAllCharacter -= GameManager_OnRemoveAllCharacter;
    }

    // Impact checking.
    protected virtual void OnTriggerEnter(Collider col)
    {
        // Failed safe to prevent multiple calling time.
        if (_isHit) return;

        // If the shooter is "Enemy" and the receiver is "Player", Deduct player health.
        if (col.tag == _PlayerTagName && _characterType == CharacterType.Enemy)
        {
            _isHit = true;
            col.GetComponent<PlayerController>().TakeDamage(this
                , () => _isHit = false);
        }
    }

    #endregion

    #region Init Method

    /// <summary>
    /// Call this method to initialize this enemy's data.
    /// </summary>
    /// <param name="path">Start paths</param>
    /// <param name="enemyFormation">Enemy main formation.</param>
    /// <param name="data">Current game data</param>
    /// <param name="enemyType">Enemy type</param>
    public virtual void Init(Path path, FormationController enemyFormation, GameData data, EnemyType enemyType)
    {
        _path = path;
        _enemyID = enemyFormation.GetNewEnemyID();
        _currentHealth = _BaseHealth;
        _enemyFormation = enemyFormation;
        _enemyState = EnemyStates.PathFollowing;
        _characterType = CharacterType.Enemy;
        _enemyType = enemyType;
        _speed = data.Speed;

        _isInit = true;
        _isHit = false;
    }

    #endregion

    #region Action Methods

    /// <summary>
    /// Call this method to do path following action.
    /// </summary>
    protected void PathFollowing()
    {
        if (_currentIndex < _path.BezierPathList.Count)
        {
            float distance = Vector3.Distance(_path.BezierPathList[_currentIndex], transform.position);
            transform.position = Vector3.MoveTowards(transform.position, _path.BezierPathList[_currentIndex],_speed * Time.deltaTime);
            EnemyRotation(_path.BezierPathList[_currentIndex]);

            if (distance <= _reachOffset)
            {
                _currentIndex++;
            }
        }
        else
        {
            _currentIndex = 0;
            _enemyState = EnemyStates.PathFormation;
        }
    }

    /// <summary>
    /// Call this method to moving into formation zone.
    /// </summary>
    protected void FormationFollowing()
    {
        transform.position = Vector3.MoveTowards(transform.position, _enemyFormation.GetPosition(_enemyID), _speed * Time.deltaTime);
        EnemyRotation(_enemyFormation.GetPosition(_enemyID));

        if (Vector3.Distance(transform.position, _enemyFormation.GetPosition(_enemyID)) <= _reachFormationOffset)
        {
            _enemyFormation.AddEnemy(this);
            transform.SetParent(_enemyFormation.transform);
            transform.rotation = _defaultDirection;
            _enemyState = EnemyStates.Idle;
        }
    }

    #endregion

    #region Update Methods

    /// <summary>
    /// Call this method to let the enemy makes next action.
    /// </summary>
    protected virtual void EnemyDecisionMaking()
    {
        switch (_enemyState)
        {
            case EnemyStates.PathFollowing:
            {
                PathFollowing();
            }
                break;
            case EnemyStates.PathFormation:
            {
                FormationFollowing();
            }
                break;
            case EnemyStates.Idle:
            {
                // Do nothing.
            }
                break;
        }
    }

    /// <summary>
    /// Call this method to set new path.
    /// </summary>
    /// <param name="caller">Registered formation</param>
    /// <param name="path">New path</param>
    public void UpdatePath(FormationController caller, Path path)
    {
        if (caller != _enemyFormation) return;
        _path = path;
    }

    /// <summary>
    /// Call this method to update enemy state.
    /// </summary>
    /// <param name="caller">Formation parent.</param>
    /// <param name="state">Enemy state.</param>
    public void UpdateState(FormationController caller, EnemyStates state)
    {
        if (caller != _enemyFormation) return;
        _enemyState = state;
    }

    /// <summary>
    /// Call this method when the enemy get the damage from player.
    /// </summary>
    /// <param name="caller">Bullet controller.</param>
    public virtual void TakeDamage(object caller)
    {
        if (caller.GetType() != typeof(BulletController)) return;
        _currentHealth--;
    }

    /// <summary>
    /// Call this method to set the enemy rotation.
    /// </summary>
    /// <param name="target">Target position</param>
    protected void EnemyRotation(Vector3 target)
    {
        float x = target.x - transform.position.x;
        float y = target.y - transform.position.y;
        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - _angleOffset, Vector3.forward);
    }

    /// <summary>
    /// Call this method to notify the game manager that this unit is dead.
    /// </summary>
    /// <param name="enemyType">Enemy type</param>
    protected void OnDiedAction(EnemyType enemyType)
    {
        _isInit = false;
        OnDeductEnemy?.Invoke(enemyType);
        OnEnemyDied?.Invoke(enemyType);
    }

    #endregion

    #region Event Methods

    /// <summary>
    /// Call this method when the event of removing all game character is occured.
    /// </summary>
    private void GameManager_OnRemoveAllCharacter()
    {
        // Return this instance to the pool.
        CharacterPool.Instance.ReturnToPool(this.gameObject);
    }

    /// <summary>
    /// Call this method to regroup the enemy unit to the formation
    /// </summary>
    protected virtual void GameManager_OnRegroupEnemy()
    {
        // Not used in base class.
        // Implement for each of enemy type on super class.
    }

    #endregion

    #endregion
}
