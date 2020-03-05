//  BulletController.cs
//  By Atid Puwatnuttasit

using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    #region Static Value

    private static readonly Vector3 _playerDefaultDirection = Vector3.up;

    private static readonly Quaternion _playerDefaultQuaternion = Quaternion.AngleAxis(0, Vector3.forward);
    private static readonly Quaternion _enemyDefaultQuaternion = Quaternion.AngleAxis(180, Vector3.forward);

    private static readonly float _bulletLifeCycleTime = 2.5f;

    private static readonly string _enemyTagName = "Enemy";
    private static readonly string _PlayerTagName = "Player";
    private static readonly string _WallTagName = "Wall";

    #endregion

    #region Inspector Properties

    [Header("Bullet Setting")]
    [SerializeField] private float _Speed = 20;
    

    #endregion

    #region Private Properties

    private bool _isInit;
    private bool _isHit;
    private CharacterType _shooter;
    private Coroutine _lifeCycleCoroutine;
    private Vector3 _direction;

    #endregion

    #region Methods

    #region Unity Callback Methods

    private void FixedUpdate()
    {
        // Failed safe for uninitiated data for this object.
        if (!_isInit) return;
        
        // Start Co-routine bullet cycle.
        // So the bullet will return to the pool when the the life cycle is end.
        if (_lifeCycleCoroutine == null)
        {
            _lifeCycleCoroutine = StartCoroutine(LifeCycleRoutine());
        }

        // Update bullet position.
        ShotMoving();
    }

    // Impact checking.
    private void OnTriggerEnter(Collider col)
    {
        // Failed safe to prevent multiple calling time.
        if (_isHit) return;

        // If the shooter is "Player" and the receiver is "Enemy", Deduct enemy health.
        if (col.tag == _enemyTagName && _shooter == CharacterType.Player)
        {
            // Set failed safe.
            _isHit = true;

            // Deduct enemy health. 
            col.GetComponent<BaseEnemyController>().TakeDamage(this);

            // Stop bullet life cycle and return it to the pool.
            StopCoroutine(_lifeCycleCoroutine);
            DestroyItself();
        }
        // If the shooter is "Enemy" and the receiver is "Player", Deduct player health.
        else if (col.tag == _PlayerTagName && _shooter == CharacterType.Enemy)
        {
            // Set failed safe.
            _isHit = true;

            // Deduct player health. 
            col.GetComponent<PlayerController>().TakeDamage(this, null);

            // Stop bullet life cycle and return it to the pool.
            StopCoroutine(_lifeCycleCoroutine);
            DestroyItself();
        }
        // If the receiver is wall, return to the pool.
        else if (col.tag == _WallTagName)
        {
            // Set failed safe.
            _isHit = true;

            // Stop bullet life cycle and return it to the pool.
            StopCoroutine(_lifeCycleCoroutine);
            DestroyItself();
        }
    }

    #endregion

    #region Init Methods

    /// <summary>
    /// Call this method to initialize this bullet object.
    /// </summary>
    /// <param name="shooter">Shooter</param>
    /// <param name="startPos">Start position</param>
    public void Init(CharacterType shooter, Vector3 startPos)
    {
        _shooter = shooter;
        switch (_shooter)
        {
            case CharacterType.Player:
                _direction = _playerDefaultDirection;
                transform.rotation = _playerDefaultQuaternion;
                break;
            case CharacterType.Enemy:
                _direction = startPos - GameManager.Instance.PlayerObject.transform.position;
                _direction.Normalize();
                transform.rotation = _enemyDefaultQuaternion;
                break;
        }

        transform.position = startPos;
        _isInit = true;
        _isHit = false;
    }

    #endregion

    #region Update Data

    /// <summary>
    /// Call this method to update bullet position.
    /// </summary>
    private void ShotMoving()
    {
        transform.Translate(_direction * Time.deltaTime * _Speed);
    }

    /// <summary>
    /// Call this method to reset this bullet and return it to the pool.
    /// </summary>
    private void DestroyItself()
    {
        _isHit = false;
        _lifeCycleCoroutine = null;
        BulletPool.SharedInstance.ReturnBulletToPool(this.gameObject);
    }

    #endregion

    #region Co-Routine Methods

    /// <summary>
    /// Call this method to run the bullet life cycle.
    /// If it's done, return the bullet to the pool.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LifeCycleRoutine()
    {
        yield return new WaitForSeconds(_bulletLifeCycleTime);
        DestroyItself();
    }

    #endregion

    #endregion
}
