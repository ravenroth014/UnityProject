using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameplayUIManager : MonoBehaviour
{
    #region Static Data
    private static readonly string NewHighscoreString = "New Highscore!!";                      // "Highscore" string for UI text.
    private static readonly string YourScoreString = "Your Score";                              // "Your score" string for UI text.
    #endregion

    #region Private Properties
    private static GameplayUIManager _instance;                                                 // Singleton instance.
    private bool _isNewHighscore;                                                               // For checking new highscore.
    #endregion

    #region Public Properties
    public static GameplayUIManager Instance => _instance;                                      // Public instance.
    #endregion

    #region Inspector Properties
    [Header("UI Setting")]
    [SerializeField] CanvasGroup _GameOverPanel;                                                // Gameover UI Panel.
    [SerializeField] TextMeshProUGUI _ScoreText;                                                // Score Text.
    [SerializeField] TextMeshProUGUI _HighScoreText;                                            // Highscore Text.
    [SerializeField] TextMeshProUGUI _LevelScoreText;                                           // Result Text.
    [SerializeField] TextMeshProUGUI _FinalScoreText;                                           // Final score Text.
    [SerializeField] TextMeshProUGUI _CountdownText;                                            // Countdown Text.
    [SerializeField] Button _ReplayButton;                                                      // Replay Button.
    [SerializeField] Button _MainMenuButton;                                                    // Main Menu Button.
    #endregion

    #region Event Properties
    public static event Action OnInitFinish;                                                    // On Init Finish event.
    #endregion

    #region Methods

    #region Unity Callback Methods
    private void Awake()
    {
        if (_instance == null || _instance != this)
            _instance = this;
    }

    /// <summary>
    /// Subscribe event.
    /// </summary>
    private void OnEnable()
    {
        GameManager.OnGameInit += InitUI;
        GameManager.OnGameOver += OnGameOver;
    }

    /// <summary>
    /// Unsubscribe event.
    /// </summary>
    private void OnDisable()
    {
        GameManager.OnGameInit -= InitUI;
        GameManager.OnGameOver -= OnGameOver;
    }

    #endregion

    #region Initialize Methods
    /// <summary>
    /// Call this method to initialize game play UI scene.
    /// </summary>
    private void InitUI()
    {
        _ScoreText.text = "0";
        _HighScoreText.text = GameManager.Instance.LastHighscore.ToString();
        _FinalScoreText.text = "0";
        _HighScoreText.text = GameManager.Instance.LastHighscore.ToString();
        _isNewHighscore = false;

        _ReplayButton.onClick.RemoveAllListeners();
        _ReplayButton.onClick.AddListener(() => { SceneController.Instance.LoadScene(SceneController.Instance.GamePlaySceneName); });

        _MainMenuButton.onClick.RemoveAllListeners();
        _MainMenuButton.onClick.AddListener(() => { SceneController.Instance.LoadScene(SceneController.Instance.MainMenuSceneName); });

        CountdownUI();
    }

    /// <summary>
    /// Call this method to countdown the gameplay.
    /// </summary>
    private void CountdownUI()
    {
        _CountdownText.gameObject.SetActive(true);
        _CountdownText.text = "3";
        _CountdownText.rectTransform.DOScale(new Vector2(3, 3), 1f).OnComplete(() =>
        {
            _CountdownText.text = "2";
            _CountdownText.rectTransform.localScale = Vector2.one;
            _CountdownText.rectTransform.DOScale(new Vector2(3, 3), 1f).OnComplete(() =>
            {
                _CountdownText.text = "1";
                _CountdownText.rectTransform.localScale = Vector2.one;
                _CountdownText.rectTransform.DOScale(new Vector2(3, 3), 1f).OnComplete(() =>
                {
                    _CountdownText.text = "START!!!";
                    _CountdownText.rectTransform.localScale = Vector2.one;
                    _CountdownText.rectTransform.DOScale(new Vector2(3, 3), 1f).OnComplete(() =>
                    {
                        OnInitFinish?.Invoke();
                        _CountdownText.gameObject.SetActive(false);
                    });
                });
            });
        });
    }

    #endregion

    #region Update Methods
    /// <summary>
    /// Call this method to show the Game Over panel.
    /// </summary>
    private void OnGameOver()
    {
        _FinalScoreText.text = _ScoreText.text;
        _LevelScoreText.text = _isNewHighscore ? NewHighscoreString : YourScoreString;
        _GameOverPanel.gameObject.SetActive(true);
        _GameOverPanel.DOFade(1, 1).OnComplete(() => {
            _ReplayButton.enabled = true;
            _MainMenuButton.enabled = true;
        });
    }

    /// <summary>
    /// Call this method to update score UI
    /// </summary>
    /// <param name="caller">Caller object who call this method</param>
    /// <param name="currentScore">Current score</param>
    public void UpdateScore(object caller, int currentScore)
    {
        if (caller.GetType() != typeof(GameManager)) return;
        
        _ScoreText.text = currentScore.ToString();
        if (currentScore > GameManager.Instance.LastHighscore)
        {
            _HighScoreText.text = currentScore.ToString();
            _isNewHighscore = true;
        }
    }

    #endregion

    #endregion
}
