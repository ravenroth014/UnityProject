//  GreenController.cs
//  By Atid Puwatnuttasit

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GreenController : BaseEnemyController
{
    #region Static Value

    private static readonly string _laserIdleState = "IDLE";
    private static readonly string _laserOnState = "LASER_ON";
    private static readonly string _laserOffState = "LASER_OFF";

    #endregion

    #region Inspector Properties

    [Header("Enemy Setting")]
    [SerializeField] private Animator _Animator;
    [SerializeField] private SpriteRenderer _Renderer;
    [SerializeField] private Sprite _StartForm;                         // Enemy first's sprite form.
    [SerializeField] private Sprite _HalfForm;                          // Enemy second's sprite form.
    [SerializeField] private LaserController _LaserController;          // Laser controller.

    #endregion

    #region Private Properties

    private bool _isShooting;
    private Coroutine _currentAction;               // Current active co-routine action.

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
    public override void Init(Path path, FormationController enemyFormation, GameData data, EnemyType enemyType)
    {
        base.Init(path, enemyFormation, data, enemyType);
        _Renderer.sprite = _StartForm;
        _isShooting = false;
        _LaserController.Init(this, CharacterType.Enemy);
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
            _Animator.Play(_laserIdleState);
            _enemyFormation.RemoveEnemy(this);
            CharacterPool.Instance.ReturnToPool(this.gameObject);
            OnDiedAction(_enemyType);
        }
        else if (_currentHealth == 1)
        {
            _Renderer.sprite = _HalfForm;
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
        // Else, move toward to the last known player's spot, then shoot the laser beam.
        else
        {
            if (!_isShooting)
            {
                _isShooting = true;
                transform.rotation = _defaultDirection;
                _currentAction = StartCoroutine(ShootLaser(
                    () =>
                    {
                        _currentAction = StartCoroutine(LeaveScreen());
                    })
                );
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
        if (_currentAction != null)
        {
            StopCoroutine(_currentAction);
            _currentAction = null;
        }
        _Animator.Play(_laserIdleState);
        _enemyState = EnemyStates.PathFormation;
        _currentIndex = 0;
        _isShooting = false;
    }

    #endregion

    #region Co-routine Methods

    /// <summary>
    /// Call this method to do laser beam animation.
    /// </summary>
    /// <param name="onComplete">Callback method when action is complete.</param>
    /// <returns></returns>
    private IEnumerator ShootLaser(UnityAction onComplete)
    {
        _Animator.Play(_laserOnState);
        yield return new WaitForSeconds(1.5f);
        _Animator.Play(_laserOffState);
        yield return new WaitForSeconds(1.5f);
        _Animator.Play(_laserIdleState);

        onComplete?.Invoke();
    }

    /// <summary>
    /// Call this method to leave the screen and move to the other side of the screen.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LeaveScreen()
    {
        InvisibleType visibleResult = GameManager.Instance.CheckObjectVisible(transform.position);

        while (visibleResult == InvisibleType.Visible)
        {
            transform.position += Vector3.down * Speed * Time.deltaTime;
            visibleResult = GameManager.Instance.CheckObjectVisible(transform.position);
            yield return new WaitForEndOfFrame();
        }

        Transform refPos = GameManager.Instance.GetScreenTeleportPos(visibleResult);
        transform.position = new Vector3(transform.position.x, refPos.position.y, transform.position.z);
        _enemyState = EnemyStates.PathFormation;
        _currentIndex = 0;
        _isShooting = false;
    }

    #endregion

    #endregion
}
