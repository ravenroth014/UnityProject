//  GameManager.cs
//  By Atid Puwatnuttasit

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region Static Value

    // Random logic for enemy attack.
    private static readonly int _minAttacker = 2;
    private static readonly int _maxAttacker = 4;

    private static readonly float _minAttackWaitTime = 1f;
    private static readonly float _maxAttackWaitTime = 3f;

    #endregion

    #region Inspector Properties

    [Header("Enemy Formation")] 
    [SerializeField] private List<FormationController> _formationList;

    [Header("Wave Setting")]
    [SerializeField] private List<Wave> WaveList = new List<Wave>();

    [Header("Player Ref")] 
    [SerializeField] private Transform _PlayerStartPos;
    [SerializeField] private Transform _MinOffSet;
    [SerializeField] private Transform _MaxOffSet;

    [Header("Wall Ref")]
    [SerializeField] private Transform _TopWallObject;
    [SerializeField] private Transform _BottomWallObject;

    [Header("Enemy Score")] 
    [SerializeField] private int _BlueScore = 100;
    [SerializeField] private int _RedScore = 200;
    [SerializeField] private int _GreenScore = 300;

    #endregion

    #region Public Properties

    public float EnemySpawnInterval; // Interval Between ship spawns
    public float WaveSpawnInterval;

    public static GameManager Instance;

    public Transform MinOffSet => _MinOffSet;
    public Transform MaxOffSet => _MaxOffSet;

    public GameObject PlayerObject => _playerGameObject;

    public bool IsGameActive => _isGameActive;
    public bool IsGamePause => _isPause;
    public bool IsGameOver => _isGameOver;

    public int TotalActiveEnemy => GetTotalActiveEnemy();
    public GameData Data => _data;

    #endregion
    
    #region Private Properties

    private int _enemyCount;
    private int _currentScore;
    private int _highScore;
    private int _totalLife;

    private bool _isGameActive = false;
    private bool _isPause = true;
    private bool _isGameOver = false;
    private bool _isAllSpawned = false;
    private bool _isAttackAble = false;

    private GameObject _playerGameObject;
    private GameData _data;

    private Coroutine _action;

    #endregion

    #region Event

    public static event Action OnPreStartGame;                          // Event request ui on pre start game.
    public static event Action<int> OnScoreUpdate;                      // Event request ui score update.
    public static event Action<int> OnUpdatePlayerLife;                 // Event request ui life update.
    public static event Action OnStageClear;                            // Event request on stage clear.
    public static event Action OnGameOver;                              // Event request on game over.
    public static event Action OnGameRetry;                             // Event request on game retry.
    public static event Action OnRemoveAllCharacter;                    // Event request to remove all characters.
    public static event Action OnRegroupEnemy;                          // Event request enemy to regroup to its formation.

    #endregion

    #region Methods

    #region Unity Callback Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Subscribe event.
    private void OnEnable()
    {
        PlayerController.OnPlayerDied += PlayerController_OnPlayerDied;
        BaseEnemyController.OnEnemyDied += BaseEnemyController_OnOnEnemyDied;

        UIManager.OnInGameSceneActive += UiManager_OnInGameSceneActive;
        UIManager.OnUIGameReadyToRetry += UiManager_OnUiGameReadyToRetry;
        UIManager.OnUIGameReadyToPlay += UiManager_OnGameStart;
        UIManager.OnResetGameScene += UiManager_OnResetGameScene;
        UIManager.OnRespawnPlayer += UiManager_OnRespawnPlayer;
    }

    // Unsubscribe event.
    private void OnDisable()
    {
        PlayerController.OnPlayerDied -= PlayerController_OnPlayerDied;
        BaseEnemyController.OnEnemyDied -= BaseEnemyController_OnOnEnemyDied;
        
        UIManager.OnInGameSceneActive -= UiManager_OnInGameSceneActive;
        UIManager.OnUIGameReadyToRetry -= UiManager_OnUiGameReadyToRetry;
        UIManager.OnUIGameReadyToPlay -= UiManager_OnGameStart;
        UIManager.OnResetGameScene -= UiManager_OnResetGameScene;
        UIManager.OnRespawnPlayer -= UiManager_OnRespawnPlayer;
    }

    private void Update()
    {
        // Attack logic
        if (_isAllSpawned && _isAttackAble)
            OnAttackPlayer();
    }

    #endregion

    #region Init Methods

    /// <summary>
    /// Call this method to init game data in current game session.
    /// </summary>
    /// <param name="data">Game data.</param>
    private void GameInit(GameData data)
    {
        _data = new GameData(data);
        _totalLife = _data.PlayerLife;
        _highScore = GameData.HighScore;

        SpawnPlayer();
    }

    #endregion

    #region Spawn Enemy Methods

    /// <summary>
    /// Call this method to spawn player into game session.
    /// </summary>
    private void SpawnPlayer()
    {
        _playerGameObject = CharacterPool.Instance.GetPlayerObject();
        _playerGameObject.transform.position = _PlayerStartPos.position;
    }

    /// <summary>
    /// Call this routine method to spawn wave of enemies.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnWaves()
    {
        int currentWave = 0;

        while (currentWave < WaveList.Count)
        {
            for (int i = 0; i < WaveList[currentWave].FirstEnemyList.Count; i++)
            {
                Vector3 startPath = WaveList[currentWave].FirstPath.BezierPathList[0];
                Path currentPath = WaveList[currentWave].FirstPath;
                FormationController currentFormationController = GetFormation(WaveList[currentWave].FirstEnemyList[i]);

                GameObject newFly = GetCharacter(WaveList[currentWave].FirstEnemyList[i]);
                newFly.transform.SetParent(this.transform);
                newFly.transform.position = startPath;

                BaseEnemyController enemyController = newFly.GetComponent<BaseEnemyController>();

                enemyController.Init(currentPath
                    , currentFormationController
                    , _data
                    , WaveList[currentWave].FirstEnemyList[i]
                );


                if (WaveList[currentWave].SecondEnemyList != null && WaveList[currentWave].SecondEnemyList.Count > 0)
                {
                    startPath = WaveList[currentWave].SecondPath.BezierPathList[0];
                    currentPath = WaveList[currentWave].SecondPath;
                    currentFormationController = GetFormation(WaveList[currentWave].SecondEnemyList[i]);

                    newFly = GetCharacter(WaveList[currentWave].SecondEnemyList[i]);
                    newFly.transform.SetParent(this.transform);
                    newFly.transform.position = startPath;

                    enemyController = newFly.GetComponent<BaseEnemyController>();

                    enemyController.Init(currentPath
                        , currentFormationController
                        , _data
                        , WaveList[currentWave].FirstEnemyList[i]
                    );

                }

                yield return new WaitForSeconds(EnemySpawnInterval);
            }

            yield return new WaitForSeconds(WaveSpawnInterval);
            currentWave++;
        }

        _isAllSpawned = true;
        _isAttackAble = true;
    }

    /// <summary>
    /// Call this method to start enemy spawn routine. 
    /// </summary>
    private void StartSpawn()
    {
        StartCoroutine(SpawnWaves());
        CancelInvoke("StartSpawn");
    }

    #endregion

    #region Get Data Methods

    /// <summary>
    /// Call this method to get enemy game object data from pool.
    /// </summary>
    /// <param name="type">Enemy type</param>
    /// <returns></returns>
    private GameObject GetCharacter(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Blue:
                return CharacterPool.Instance.GetBlueEnemyObject();
            case EnemyType.Red:
                return CharacterPool.Instance.GetRedEnemyObject();
            case EnemyType.Green:
                return CharacterPool.Instance.GetGreenEnemyObject();
            default:
                return null;
        }
    }

    /// <summary>
    /// Call this method to get formation.
    /// </summary>
    /// <param name="type">Enemy type.</param>
    /// <returns></returns>
    private FormationController GetFormation(EnemyType type)
    {
        if (_formationList == null || _formationList.Count == 0)
            return null;

        foreach (FormationController formation in _formationList)
        {
            if (formation.EnemyType == type) return formation;
        }

        return null;
    }

    /// <summary>
    /// Call this method to get current active enemy number.
    /// </summary>
    /// <returns></returns>
    private int GetTotalActiveEnemy()
    {
        int count = 0;
        foreach (FormationController formation in _formationList)
        {
            count += formation.TotalEnemy;
        }

        return count;
    }

    #endregion

    #region Event Methods

    /// <summary>
    /// Call this event method to init game data.
    /// </summary>
    /// <param name="data">Game data.</param>
    private void UiManager_OnInGameSceneActive(GameData data)
    {
        _isGameActive = true;
        _isGameOver = false;
        _isPause = true;
        _isAllSpawned = false;
        _isAttackAble = false;
        GameInit(data);

        OnPreStartGame?.Invoke();
    }

    /// <summary>
    /// Call this method to start game.
    /// </summary>
    private void UiManager_OnGameStart()
    {
        _isPause = false;
        Invoke("StartSpawn", 3f);
    }

    /// <summary>
    /// Call this method when player is dead
    /// </summary>
    private void PlayerController_OnPlayerDied()
    {
        _totalLife--;
        if (_totalLife < 0)
            _totalLife = 0;
        OnUpdatePlayerLife?.Invoke(_totalLife);
        OnRegroupEnemy?.Invoke();
        
        if(_action != null)
            StopCoroutine(_action);

        if (_totalLife <= 0)
        {
            _isGameActive = false;
            _isPause = true;
            _isGameOver = true;
            _isAttackAble = false;

            if (_currentScore > _highScore)
            {
                _highScore = _currentScore;
                GameData.UpdateHighScore(this, _highScore);
            }

            OnGameOver?.Invoke();
        }
        else
        {
            _isAttackAble = false;
            OnGameRetry?.Invoke();
        }
    }

    /// <summary>
    /// Call this method when enemy is downed.
    /// </summary>
    /// <param name="enemyType">Enemy type.</param>
    private void BaseEnemyController_OnOnEnemyDied(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Blue:
                _currentScore += _BlueScore;
                break;
            case EnemyType.Red:
                _currentScore += _RedScore;
                break;
            case EnemyType.Green:
                _currentScore += _GreenScore;
                break;
        }
        
        OnScoreUpdate?.Invoke(_currentScore);
        if (_isPause == false && TotalActiveEnemy <= 0 && _isAllSpawned)
        {
            _isPause = true;
            OnStageClear?.Invoke();
        }
    }

    /// <summary>
    /// Call this method to reset game scene.
    /// </summary>
    private void UiManager_OnResetGameScene()
    {
        _isAllSpawned = false;
        OnRemoveAllCharacter?.Invoke();
    }

    /// <summary>
    /// Call this method to respawn player.
    /// </summary>
    private void UiManager_OnRespawnPlayer()
    {
        SpawnPlayer();
    }

    /// <summary>
    /// Call this method to set game ready to attack player.
    /// </summary>
    private void UiManager_OnUiGameReadyToRetry()
    {
        _isAttackAble = true;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Call this method to check if that object leave camera or not.
    /// </summary>
    /// <param name="position">Position</param>
    /// <returns></returns>
    public InvisibleType CheckObjectVisible(Vector3 position)
    {
        if (_BottomWallObject.position.y > position.y)
            return InvisibleType.BottomInvisible;
        if (_TopWallObject.position.y < position.y)
            return InvisibleType.TopInvisible;
        return InvisibleType.Visible;
    }

    /// <summary>
    /// Call this method to get shift point.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Transform GetScreenTeleportPos(InvisibleType type)
    {
        if (type == InvisibleType.BottomInvisible)
            return _TopWallObject;
        else
            return null;
    }

    #endregion

    #region Attack Methods

    /// <summary>
    /// Call this method to let the formation order its enemy to attack player.
    /// </summary>
    private void OnAttackPlayer()
    {
        if (!_isAttackAble) return;
        _isAttackAble = false;
        int totalRandom = Random.Range(_minAttacker, _maxAttacker);

        int ruleOne = _formationList[0].TotalEnemy;
        int ruleTwo = ruleOne + _formationList[1].TotalEnemy;
        int ruleThree = TotalActiveEnemy;

        for (int i = 0; i < totalRandom; i++)
        {
            int random = Random.Range(1, TotalActiveEnemy);

            if (random <= ruleOne)
            {
                _formationList[0].AttackPlayer(this);
            }
            else if(random > ruleOne && random <= ruleTwo)
            {
                _formationList[1].AttackPlayer(this);
            }
            else if(random < ruleThree)
            {
                _formationList[2].AttackPlayer(this);
            }
        }

        float randTime = Random.Range(_minAttackWaitTime, _maxAttackWaitTime);
        _action = StartCoroutine(DelayTime(randTime, () =>
        {
            _isAttackAble = true;
            _action = null;
        }));
    }

    #endregion

    #region Co-routine Methods

    /// <summary>
    /// Call this method to delay action for specific time.
    /// </summary>
    /// <param name="time">Specific time.</param>
    /// <param name="onComplete">Delegate method when action is complete.</param>
    /// <returns></returns>
    private IEnumerator DelayTime(float time, UnityAction onComplete)
    {
        yield return new WaitForSeconds(time);
        onComplete?.Invoke();
    }

    #endregion

    #endregion
}

/// <summary>
/// Game Data Class
/// </summary>
public class GameData
{
    #region Static Value

    private static readonly string HighScoreKey = "HighScore";

    public static readonly int MinLife = 1;
    public static readonly int MaxLife = 8;

    public static readonly float MinSpeed = 10;
    public static readonly float MaxSpeed = 15;

    public static readonly int MinBullet = 2;
    public static readonly int MaxBullet = 10;

    public static readonly float DefaultEnemyFireRate = 0.35f;

    #endregion

    #region Properties
    public static int HighScore { get; private set; }

    public int PlayerLife { get; private set; }
    public float Speed { get; private set; }
    public int Bullet { get; private set; }
    public float EnemyFireRate { get; private set; }

    #endregion

    #region Constructor

    public GameData()
    {
        if (PlayerPrefs.HasKey(HighScoreKey) == false)
        {
            PlayerPrefs.SetInt(HighScoreKey, 10000);
        }

        PlayerLife = 3;
        HighScore = PlayerPrefs.GetInt(HighScoreKey);
        Speed = MinSpeed;
        Bullet = MinBullet;
        EnemyFireRate = DefaultEnemyFireRate;
    }

    public GameData(GameData data)
    {
        PlayerLife = data.PlayerLife;
        Speed = data.Speed;
        Bullet = data.Bullet;
        EnemyFireRate = data.EnemyFireRate;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Call this method to update data.
    /// </summary>
    /// <param name="data">Game data.</param>
    public void UpdateData(GameData data)
    {
        PlayerLife = data.PlayerLife;
        Speed = data.Speed;
        Bullet = data.Bullet;
    }

    /// <summary>
    /// Call this method to update data.
    /// </summary>
    /// <param name="life">life data.</param>
    /// <param name="speed">speed data.</param>
    /// <param name="bullet">bullet data.</param>
    public void UpdateData(int life, float speed, int bullet)
    {
        PlayerLife = life;
        Speed = speed;
        Bullet = bullet;
    }

    /// <summary>
    /// Call this method to update high-score data.
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="highScore"></param>
    public static void UpdateHighScore(object caller, int highScore)
    {
        if (caller.GetType() != typeof(GameManager)) return;

        if (highScore > HighScore)
            HighScore = highScore;
        PlayerPrefs.SetInt(HighScoreKey, highScore);
    }

    #endregion
}
