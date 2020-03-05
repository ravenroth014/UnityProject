//  BlueController.cs
//  By Atid Puwatnuttasit

using UnityEngine;

public class BlueController : BaseEnemyController
{
    #region Private Properties

    private Transform _target;                          // Pursue target.
    private Vector3 _direction;                         // Pursue direction.

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

    #region Update Methods

    /// <summary>
    /// Call this method when the enemy get the damage from player.
    /// </summary>
    /// <param name="caller">Bullet controller.</param>
    public override void TakeDamage(object caller)
    {
        base.TakeDamage(caller);

        // If enemy's health reach 0, remove this enemy from the game.
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

            // If it move out of the camera, move it to the other side of the camera and move back to it's formation.
            InvisibleType visibleResult = GameManager.Instance.CheckObjectVisible(transform.position);
            if (visibleResult == InvisibleType.BottomInvisible)
            {
                _target = null;
                Transform refPos = GameManager.Instance.GetScreenTeleportPos(visibleResult);
                transform.position = new Vector3(transform.position.x, refPos.position.y, transform.position.z);
                _enemyState = EnemyStates.PathFormation;
                _currentIndex = 0;
            }
        }
    }

    #endregion

    #region Event Methods

    /// <summary>
    /// Call this method when the regroup request is occured.
    /// </summary>
    protected override void GameManager_OnRegroupEnemy()
    {
        _enemyState = EnemyStates.PathFormation;
        _currentIndex = 0;
    }

    #endregion

    #endregion
}
