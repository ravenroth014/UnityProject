//  PlayerController.cs
//  By Atid Puwatnuttasit

using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    #region Inspector Properties

    [Header("Bullet Setting")]
    [SerializeField] private float _FireRate = 0.5f;

    [Header("Character Setting")] 
    [SerializeField] private CharacterType _CharacterType;
    [SerializeField] private float speed = 15F;

    #endregion

    #region Private Properties

    private Vector3 _moveDirection = Vector3.zero;
    private float _currentFireTime;

    private bool _isInvincible;                                 // State that to prevent killing during waiting mode.

    #endregion

    #region Events

    public static event Action OnPlayerDied;                    // Event request when player is dead.

    #endregion

    #region Methods

    #region Unity Callback Methods

    private void Start()
    {
        _currentFireTime = _FireRate;
    }

    private void FixedUpdate()
    {
        // Player control and Shoot bullet available checking.
        if(GameManager.Instance.IsGamePause == false)
        {
            PlayerControl();
            ShootBullet();
        }
    }

    // Subscribe event.
    private void OnEnable()
    {
        GameManager.OnGameRetry += GameManager_OnGameRetry;

        UIManager.OnUIGameReadyToPlay += UiManager_OnUiGameReadyToPlay;
        UIManager.OnUIGameReadyToRetry += UiManager_OnUiGameReadyToRetry;
    }

    // Unsubscribe event.
    private void OnDisable()
    {
        GameManager.OnGameRetry -= GameManager_OnGameRetry;

        UIManager.OnUIGameReadyToPlay -= UiManager_OnUiGameReadyToPlay;
        UIManager.OnUIGameReadyToRetry -= UiManager_OnUiGameReadyToRetry;
    }

    #endregion

    #region Update Methods
    
    /// <summary>
    /// Call this method to update player damage.
    /// </summary>
    /// <param name="caller">Enemy object.</param>
    /// <param name="onComplete">Callback method when this action is complete</param>
    public void TakeDamage(object caller, UnityAction onComplete)
    {
        if (_isInvincible)
        {
            onComplete?.Invoke();
            return;
        }

        if (caller.GetType() != typeof(BulletController)
            && caller.GetType() != typeof(LaserController)
            && caller.GetType() != typeof(BlueController)
            && caller.GetType() != typeof(RedController)
            && caller.GetType() != typeof(GreenController))
            
        {
            onComplete?.Invoke();
            return;
        }

        onComplete?.Invoke();
        OnPlayerDied?.Invoke();
        CharacterPool.Instance.ReturnToPool(this.gameObject);
    }

    #endregion

    #region Action Methods

    /// <summary>
    /// Call this method to move the character.
    /// </summary>
    private void PlayerControl()
    {
        if (Input.GetKey(KeyCode.RightArrow)){
            transform.position += Vector3.right * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow)){
            transform.position += Vector3.left* speed * Time.deltaTime;
        }

        if (transform.position.x < GameManager.Instance.MinOffSet.position.x) transform.position = GameManager.Instance.MinOffSet.position;
        else if (transform.position.x > GameManager.Instance.MaxOffSet.position.x) transform.position = GameManager.Instance.MaxOffSet.position;
    }

    /// <summary>
    /// Call this method to shoot bullet to enemy.
    /// </summary>
    private void ShootBullet()
    {
        if (Input.GetKey(KeyCode.Space) && _currentFireTime >= _FireRate)
        {
            _currentFireTime = 0;
            GameObject bullet = BulletPool.SharedInstance.GetBulletObject();
            BulletController bulletScript = bullet.GetComponent<BulletController>();
            bulletScript.Init(_CharacterType, transform.position);
        }
        else
        {
            _currentFireTime += Time.deltaTime;
        }
    }

    #endregion

    #region Event Methods

    /// <summary>
    /// Call this method when the retry request is granted, turn off invincible mode.
    /// </summary>
    private void UiManager_OnUiGameReadyToRetry()
    {
        _isInvincible = false;
    }

    /// <summary>
    /// Call this method when the game request is ready, turn off invincible mode.
    /// </summary>
    private void UiManager_OnUiGameReadyToPlay()
    {
        _isInvincible = false;
    }

    /// <summary>
    /// Call this method when requesting game retry, turn on invincible mode.
    /// </summary>
    private void GameManager_OnGameRetry()
    {
        _isInvincible = true;
    }

    #endregion

    #endregion
}
