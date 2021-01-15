using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Static Data
    public static readonly string HighScoreString = "Highscore";                        // Highscore string for playerpref.
    #endregion 

    #region Private Properties
    private static GameManager _instance;                                               // Singleton instance.
    private bool _isStart;                                                              // Check game start state.
    private bool _isOver;                                                               // Check game over state.
    private int _currentScore;                                                          // Current score.
    private int _currentBaseScore;                                                      // Current base score for sum up with the current score.
    private int _MultiplyScoreCount;                                                    // Use to determine when to increase the current base score.
    private int _lastHighScore;                                                         // Previous highscore.
    #endregion

    #region Public Properties
    public static GameManager Instance => _instance;                                    
    public bool IsStart => _isStart;                                                    
    public bool IsOver => _isOver;
    public float SpeedRate => _speedRate;
    public int LastHighscore => _lastHighScore;
	#endregion

	#region Inspector Properties

    [Header("Stage Object Setting")]
	[SerializeField] private Transform _SnakeParent;                                    
	[SerializeField] private Transform _LeftWall;
	[SerializeField] private Transform _RightWall;
	[SerializeField] private Transform _TopWall;
	[SerializeField] private Transform _BottomWall;

    [Header("Stage Level Setting")]
    [SerializeField] private float _speedRate;                                          // Speed rate for increasing snake's speed.
    [SerializeField] private int _baseScore;                                            // Base score for each of snake's eating.
    [SerializeField] private int _baseMultiplyScore;                                    // The rule for changing up the snake's speed and score.
    #endregion

    #region Events

    public static Action OnSpeedRateChange;                                             
    public static Action OnGameOver;
    public static Action OnGameInit;
    #endregion

    #region Methods

    #region Unity Callback Methods
    private void Awake()
    {
        if(_instance == null || _instance != this)
        {
            _instance = this;
        }
    }

    /// <summary>
    /// Initialize this class in this start method.
    /// </summary>
    private void Start()
    {
        Init();
	}

    /// <summary>
    /// Subscribe event.
    /// </summary>
    private void OnEnable()
    {
        GameplayUIManager.OnInitFinish += GameStart;
        SnakeController.OnEatenFood += Spawn;
    }

    /// <summary>
    /// Unsubscribe event.
    /// </summary>
    private void OnDisable()
    {
        GameplayUIManager.OnInitFinish -= GameStart;
        SnakeController.OnEatenFood -= Spawn;
    }
    #endregion

    #region Initialize Methods
    /// <summary>
    /// Call this method to initialize gameplay data.
    /// </summary>
    private void Init()
    {
        _isStart = false;
        _isOver = false;
        _currentScore = 0;

        _currentBaseScore = _baseScore;
        _MultiplyScoreCount = _baseMultiplyScore;
        _lastHighScore = PlayerPrefs.GetInt(HighScoreString);

        OnGameInit?.Invoke();
    }

    /// <summary>
    /// Call this method when the game is ready to start.
    /// </summary>
    private void GameStart()
    {
        _isStart = true;
        Spawn();
    }

    #endregion

    #region Game Control Methods
    /// <summary>
    /// Call this method to spawn the food for snake.
    /// </summary>
    private void Spawn()
	{
		bool isClearSpawn = true;

		int x = (int)UnityEngine.Random.Range(_LeftWall.position.x + 1, _RightWall.position.x - 1);
		int y = (int)UnityEngine.Random.Range(_BottomWall.position.y + 1, _TopWall.position.y - 1);
		Vector2 foodPos = new Vector2(x, y);

        // In this loop, will identify if the generated position is available to spawn or not
        // If the position has been taken by heroes, it has to look for new one.
        for (var i = 0; i < _SnakeParent.childCount; i++)
        {
            Vector2 heroPos = _SnakeParent.GetChild(i).position;

            if (foodPos == heroPos)
            {
                isClearSpawn = false;
                break;
            }
        }

        // Check clear to spawn condition.
        if (isClearSpawn)
        {
            GameObject food = FoodPool.SharedInstance.GetFoodObject();
            food.transform.position = foodPos;
        }
        else
            Spawn();
    }

    public void UpdateScore(object caller)
    {
        if (caller.GetType() != typeof(SnakeSensor)) return;

        _MultiplyScoreCount--;

        if(_MultiplyScoreCount <= 0)
        {
            _currentBaseScore += _baseScore;
            _MultiplyScoreCount = _baseMultiplyScore;
            OnSpeedRateChange?.Invoke();
        }

        _currentScore += _currentBaseScore;
        GameplayUIManager.Instance.UpdateScore(this, _currentScore);
    }

    public void GameOver(object caller)
    {
        if (caller.GetType() != typeof(SnakeSensor))
            return;

        _isOver = true;

        if (_currentScore > _lastHighScore)
            PlayerPrefs.SetInt(HighScoreString, _currentScore);

        OnGameOver?.Invoke();
    }

    #endregion

    #endregion
}
