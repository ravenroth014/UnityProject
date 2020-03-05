//  RedController.cs
//  By Atid Puwatnuttasit

using UnityEngine;

public class RedController : BaseEnemyController
{
    #region Private Properties

    private float RateFire = 1f;                        // Red's missile rate fire.
    private int _baseBullet;                            // Base bullet number that can fire in one diving.
    private int _bulletCount;                           // Current available bullet number on that diving.
    private float _lastShotTime;
    private Transform _target;
    private Vector3 _direction;

    #endregion

    #region Methods

    #region Unity Callback Methods

    // Subscribe event.
    protected override void OnEnable()
    {
        base.OnEnable();

        GameManager.OnRegroupEnemy += GameManager_OnRegroupEnemy;
    }

    // Unsubscribe event.
    protected override void OnDisable()
    {
        base.OnEnable();

        GameManager.OnRegroupEnemy -= GameManager_OnRegroupEnemy;
    }

    #endregion

    #region Init Methods

    /// <summary>
    /// Call this method to initialize this enemy's data.
    /// </summary>
    /// <param name="path">Start paths</param>
    /// <param name="enemyFormation">Enemy main formation.</param>
    /// <param name="data">Current game data</param>
    /// <param name="enemyType">Enemy type</param>
    public override void Init(Path path, FormationController enemyFormation, GameData data, EnemyType enmEnemyType)
    {
        base.Init(path, enemyFormation, data, enmEnemyType);
        _baseBullet = data.Bullet;
        _bulletCount = _baseBullet;
        _lastShotTime = RateFire;
    }

    #endregion

    #region Update Methods

    /// <summary>
    /// Call this method when the enemy get the damage from player.
    /// </summary>
    /// <param name="caller">Bullet controller.</param>
    public override void TakeDamage(object caller)
    {
        base.TakeDamage(caller);

        if (_currentHealth <= 0)
        {
            _currentIndex = 0;
            _enemyFormation.RemoveEnemy(this);
            CharacterPool.Instance.ReturnToPool(this.gameObject);
            OnDiedAction(_enemyType);
        }
    }

    /// <summary>
    /// Call this method to let the enemy makes next action.
    /// </summary>
    protected override void EnemyDecisionMaking()
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
                // Do nothing
            }
                break;
            case EnemyStates.Diving:
            {
                Diving();
            }
                break;
        }
    }

    /// <summary>
    /// Call this method to reset data of this character.
    /// </summary>
    private void ResetData()
    {
        _enemyState = EnemyStates.PathFormation;
        _currentIndex = 0;
        _bulletCount = _baseBullet;
        _lastShotTime = RateFire;
    }

    #endregion

    #region Action Methods

    /// <summary>
    /// Call this method to do diving action to the player
    /// </summary>
    private void Diving()
    {
        // Unregistered from formation.
        _enemyFormation.RemoveEnemy(this);

        // If it's still on path.
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
        // Else, move toward to the last known player's spot.
        else
        {
            if (_target == null)
            {
                _target = GameManager.Instance.PlayerObject.transform;
                EnemyRotation(_target.position);
                _direction = _target.position - transform.position;
                _direction.Normalize();
            }

            transform.position += _direction * Speed * Time.deltaTime;

            InvisibleType visibleResult = GameManager.Instance.CheckObjectVisible(transform.position);
            if (visibleResult == InvisibleType.BottomInvisible)
            {
                _target = null;
                Transform refPos = GameManager.Instance.GetScreenTeleportPos(visibleResult);
                transform.position = new Vector3(transform.position.x, refPos.position.y, transform.position.z);
                ResetData();
            }
        }

        // Shoot the bullet when it's ready.
        if (_lastShotTime <= 0)
        {
            ShootBullet();
        }
        else
        {
            _lastShotTime -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Call this method to shoot bullet to player.
    /// </summary>
    private void ShootBullet()
    {
        if (_bulletCount > 0)
        {
            GameObject bullet = BulletPool.SharedInstance.GetBulletObject();
            BulletController bulletScript = bullet.GetComponent<BulletController>();
            bulletScript.Init(_characterType, transform.position);
            _bulletCount--;
            _lastShotTime = RateFire;
        }
    }

    #endregion

    #region Event Methods

    /// <summary>
    /// Call this method when the regroup request is occured.
    /// </summary>
    protected override void GameManager_OnRegroupEnemy()
    {
        ResetData();
    }

#endregion

    #endregion
    
}
