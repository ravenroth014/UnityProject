//  LaserController.cs
//  By Atid Puwatnuttasit

using UnityEngine;

public class LaserController : MonoBehaviour
{
    #region Static Value

    private static readonly float _bulletLifeCycleTime = 2.5f;
    private static readonly string _PlayerTagName = "Player";

    #endregion

    #region Private Properties

    private CharacterType _shooter;
    private bool _isHit;

    #endregion

    #region Methods

    #region Init Methods

    /// <summary>
    /// Call this method to initialize this laser instance.
    /// </summary>
    /// <param name="caller">Caller instance</param>
    /// <param name="shooter">Shooter type</param>
    public void Init(GreenController caller, CharacterType shooter)
    {
        // Failed safe check
        if (caller.GetType() != typeof(GreenController)) return;
        _shooter = shooter;
        _isHit = false;
    }

    #endregion

    #region Unity Callback Methods

    // Impact checking.
    private void OnTriggerEnter(Collider col)
    {
        // Failed safe to prevent multiple calling time.
        if (_isHit) return;

        // If the shooter is "Enemy" and the receiver is "Player", Deduct player health.
        if (col.tag == _PlayerTagName && _shooter == CharacterType.Enemy)
        {
            _isHit = true;
            col.GetComponent<PlayerController>().TakeDamage(this
                , () => _isHit = false);
        }
    }

    #endregion

    #endregion
}
