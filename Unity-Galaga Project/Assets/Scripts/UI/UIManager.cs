//  UIManager.cs
//  By Atid Puwatnuttasit

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    #region Static Value

    // Static string for menu.
    private static readonly string _readyString = "Ready?";
    private static readonly string _startString = "Start!!";
    private static readonly string _stageClearString = "Stage Clear!";
    private static readonly string _gameOverString = "GAME OVER";

    #endregion

    #region Inspector Properties

    [Header("Main Menu UI Elements")] 
    [SerializeField] private GameObject _MainMenuGameObject;
    [SerializeField] private List<MenuItemUI> _MainMenuList;

    [Header("Custom Menu UI Elements")]
    [SerializeField] private GameObject _CustomMenuGameObject;
    [SerializeField] private List<MenuItemUI> _CustomMenuList;

    [Header("Game Start Menu UI Elements")] 
    [SerializeField] private GameObject _GameStartMenuGameObject;
    [SerializeField] private TextMeshPro _ReadyText;

    [Header("Game Over UI Elements")] 
    [SerializeField] private GameObject _GameOverMenuGameObject;
    [SerializeField] private TextMeshPro _GameOverText;
    [SerializeField] private List<MenuItemUI> _GameOverMenuList;

    [Header("In Game UI Elements")] 
    [SerializeField] private TextMeshPro _HighScoreText;
    [SerializeField] private TextMeshPro _ScoreText;
    [SerializeField] private TextMeshPro _LifeText;

    #endregion

    #region Public Properties

    public static UIManager Instance { get; private set; }                      // Singleton Instance.

    #endregion

    #region Private Propeties

    private int _highScore;
    private int _currentVerticalMenuChoice;                         // Current selected vertical option.

    private int _lifeSetting;                                       // Current selected life value.
    private float _speedSetting;                                    // Current selected speed value.
    private int _bulletSetting;                                     // Current selected bullet value.
    private int _confirmMenuIndex;                                  // Current selected horizontal option (play/cancel).

    #endregion

    #region Events

    public static event Action<MenuScene> OnChangeMenuScene;                // Event request when change menu scene.
    public static event Action<GameData> OnInGameSceneActive;               // Event request when in-game active.
    public static event Action OnUIGameReadyToPlay;                         // Event request when ui is ready to play.
    public static event Action OnUIGameReadyToRetry;                        // Event request when retry is granted.
    public static event Action OnResetGameScene;                            // Event request when reset game scene.
    public static event Action OnRespawnPlayer;                             // Event request to spawn player.

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
        GameManager.OnPreStartGame += GameManager_OnPreStartGame;
        GameManager.OnScoreUpdate += GameManager_OnScoreUpdate;
        GameManager.OnUpdatePlayerLife += GameManager_OnUpdatePlayerLife;
        GameManager.OnStageClear += GameManager_OnStageClear;
        GameManager.OnGameRetry += GameManager_OnGameRetry;
        GameManager.OnGameOver += GameManager_OnGameOver;

        MenuManager.OnOpenGame += OnOpenGame_MainMenuInit;
        MenuManager.OnChangeMainMenuChoice += MenuManager_OnChangeMainMenuChoice;
        MenuManager.OnChangeGameOverMenuChoice += MenuManager_OnChangeGameOverMenuChoice;
        MenuManager.OnChangeVerticalCustomMenuChoice += MenuManager_OnChangeVerticalCustomMenuChoice;
        MenuManager.OnChangeHorizontalCustomMenuChoice += MenuManager_OnChangeHorizontalCustomMenuChoice;
    }

    // Unsubscribe event.
    private void OnDisable()
    {
        GameManager.OnPreStartGame -= GameManager_OnPreStartGame;
        GameManager.OnScoreUpdate -= GameManager_OnScoreUpdate;
        GameManager.OnUpdatePlayerLife -= GameManager_OnUpdatePlayerLife;
        GameManager.OnStageClear -= GameManager_OnStageClear;
        GameManager.OnGameRetry -= GameManager_OnGameRetry;
        GameManager.OnGameOver -= GameManager_OnGameOver;

        MenuManager.OnOpenGame -= OnOpenGame_MainMenuInit;
        MenuManager.OnChangeMainMenuChoice -= MenuManager_OnChangeMainMenuChoice;
        MenuManager.OnChangeGameOverMenuChoice -= MenuManager_OnChangeGameOverMenuChoice;
        MenuManager.OnChangeVerticalCustomMenuChoice -= MenuManager_OnChangeVerticalCustomMenuChoice;
        MenuManager.OnChangeHorizontalCustomMenuChoice -= MenuManager_OnChangeHorizontalCustomMenuChoice;
    }

    #endregion

    #region Events Methods

    /// <summary>
    /// Call this event method to update score.
    /// </summary>
    /// <param name="score">Score</param>
    private void GameManager_OnScoreUpdate(int score)
    {
        if (score > _highScore)
        {
            _highScore = score;
            _HighScoreText.text = _highScore.ToString();
        }

        _ScoreText.text = score.ToString();
    }

    /// <summary>
    /// Call this event method to update player life.
    /// </summary>
    /// <param name="life">Life</param>
    private void GameManager_OnUpdatePlayerLife(int life)
    {
        _LifeText.text = $"x {life - 1}";
    }

    /// <summary>
    /// Call this event method to init main menu ui.
    /// </summary>
    private void OnOpenGame_MainMenuInit()
    {
        _MainMenuGameObject.SetActive(true);
        _currentVerticalMenuChoice = 0;

        for (int i = 0; i < _MainMenuList.Count; i++)
        {
            if (i == _currentVerticalMenuChoice)
            {
                _MainMenuList[i].OnSelect(this);
            }
            else
            {
                _MainMenuList[i].OnDeSelect(this);
            }
        }

        OpenInit();
    }

    /// <summary>
    /// Call this event method to change menu option.
    /// </summary>
    /// <param name="obj">Option index.</param>
    private void MenuManager_OnChangeMainMenuChoice(int obj)
    {
        _currentVerticalMenuChoice += obj;
        if (_currentVerticalMenuChoice >= _MainMenuList.Count)
            _currentVerticalMenuChoice = 0;
        else if (_currentVerticalMenuChoice < 0)
            _currentVerticalMenuChoice = _MainMenuList.Count - 1;

        for (int i = 0; i < _MainMenuList.Count; i++)
        {
            if (i == _currentVerticalMenuChoice)
            {
                _MainMenuList[i].OnSelect(this);
            }
            else
            {
                _MainMenuList[i].OnDeSelect(this);
            }
        }
    }

    /// <summary>
    /// Call this event method to change vertical menu option.
    /// </summary>
    /// <param name="obj">Option index.</param>
    private void MenuManager_OnChangeVerticalCustomMenuChoice(int obj)
    {
        _currentVerticalMenuChoice += obj;
        if (_currentVerticalMenuChoice >= _CustomMenuList.Count)
            _currentVerticalMenuChoice = 0;
        else if (_currentVerticalMenuChoice < 0)
            _currentVerticalMenuChoice = _CustomMenuList.Count - 1;

        for (int i = 0; i < _CustomMenuList.Count; i++)
        {
            if (i == _currentVerticalMenuChoice)
            {
                _CustomMenuList[i].OnSelect(this);
            }
            else
            {
                _CustomMenuList[i].OnDeSelect(this);
            }

            if (i == _CustomMenuList.Count - 1 && i == _currentVerticalMenuChoice)
            {
                _CustomMenuList[i].UpdateHorizontalSelection(this, 0);
                _confirmMenuIndex = 0;
            }
            else if(i == _CustomMenuList.Count - 1 && i != _currentVerticalMenuChoice)
            {
                _CustomMenuList[i].UpdateHorizontalSelection(this, -1);
            }
        }
    }

    /// <summary>
    /// Call this event method to change horizontal menu option.
    /// </summary>
    /// <param name="obj">Option index.</param>
    private void MenuManager_OnChangeHorizontalCustomMenuChoice(int obj)
    {
        switch (_currentVerticalMenuChoice)
        {
            // Life
            case 0:
            {
                _lifeSetting += obj;
                if (_lifeSetting > GameData.MinLife && _lifeSetting < GameData.MaxLife)
                {
                    _CustomMenuList[(int) CustomMenuOrder.Life].SetDecreaseState(this, true);
                    _CustomMenuList[(int) CustomMenuOrder.Life].SetIncreaseState(this, true);
                }
                else if (_lifeSetting <= GameData.MinLife)
                {
                    _CustomMenuList[(int) CustomMenuOrder.Life].SetDecreaseState(this, false);
                    _CustomMenuList[(int) CustomMenuOrder.Life].SetIncreaseState(this, true);
                    _lifeSetting = GameData.MinLife;
                }
                else if (_lifeSetting >= GameData.MaxLife)
                {
                    _CustomMenuList[(int) CustomMenuOrder.Life].SetDecreaseState(this, true);
                    _CustomMenuList[(int) CustomMenuOrder.Life].SetIncreaseState(this, false);
                    _lifeSetting = GameData.MaxLife;
                }
                _CustomMenuList[(int) CustomMenuOrder.Life].UpdateValueText(this, _lifeSetting.ToString());
            }
                break;
            // Speed
            case 1:
            {
                _speedSetting += obj;
                if (_speedSetting > GameData.MinSpeed && _speedSetting < GameData.MaxSpeed)
                {
                    _CustomMenuList[(int) CustomMenuOrder.Speed].SetDecreaseState(this, true);
                    _CustomMenuList[(int) CustomMenuOrder.Speed].SetIncreaseState(this, true);
                }
                else if (_speedSetting <= GameData.MinSpeed)
                {
                    _CustomMenuList[(int) CustomMenuOrder.Speed].SetDecreaseState(this, false);
                    _CustomMenuList[(int) CustomMenuOrder.Speed].SetIncreaseState(this, true);
                    _speedSetting = GameData.MinSpeed;
                }
                else if (_speedSetting >= GameData.MaxSpeed)
                {
                    _CustomMenuList[(int) CustomMenuOrder.Speed].SetDecreaseState(this, true);
                    _CustomMenuList[(int) CustomMenuOrder.Speed].SetIncreaseState(this, false);
                    _speedSetting = GameData.MaxSpeed;
                }
                _CustomMenuList[(int) CustomMenuOrder.Speed]
                    .UpdateValueText(this, _speedSetting.ToString("n0"));
            }
                break;
            // Bullet
            case 2:
            {
                _bulletSetting += obj;
                if (_bulletSetting > GameData.MinBullet && _bulletSetting < GameData.MaxBullet)
                {
                    _CustomMenuList[(int) CustomMenuOrder.Bullet].SetDecreaseState(this, true);
                    _CustomMenuList[(int) CustomMenuOrder.Bullet].SetIncreaseState(this, true);
                }
                else if (_bulletSetting <= GameData.MinBullet)
                {
                    _CustomMenuList[(int) CustomMenuOrder.Bullet].SetDecreaseState(this, false);
                    _CustomMenuList[(int) CustomMenuOrder.Bullet].SetIncreaseState(this, true);
                    _bulletSetting = GameData.MinBullet;
                }
                else if (_bulletSetting >= GameData.MaxBullet)
                {
                    _CustomMenuList[(int) CustomMenuOrder.Bullet].SetDecreaseState(this, true);
                    _CustomMenuList[(int) CustomMenuOrder.Bullet].SetIncreaseState(this, false);
                    _bulletSetting = GameData.MaxBullet;
                }
                _CustomMenuList[(int) CustomMenuOrder.Bullet].UpdateValueText(this, _bulletSetting.ToString());

            }
                break;
            // Confirm Menu
            case 3:
            {
                _confirmMenuIndex += obj;
                if (_confirmMenuIndex >= _CustomMenuList[(int) CustomMenuOrder.ConfirmMenu].HorizontalIconCount)
                    _confirmMenuIndex = 0;
                else if (_currentVerticalMenuChoice < 0)
                    _currentVerticalMenuChoice =
                        _CustomMenuList[(int) CustomMenuOrder.ConfirmMenu].HorizontalIconCount - 1;
                _CustomMenuList[(int) CustomMenuOrder.ConfirmMenu].UpdateHorizontalSelection(this, _confirmMenuIndex);
            }
                break;
        }
    }

    /// <summary>
    /// Call this event method to change game over option.
    /// </summary>
    /// <param name="obj">Option index.</param>
    private void MenuManager_OnChangeGameOverMenuChoice(int obj)
    {
        _currentVerticalMenuChoice += obj;
        if (_currentVerticalMenuChoice >= _GameOverMenuList.Count)
            _currentVerticalMenuChoice = 0;
        else if (_currentVerticalMenuChoice < 0)
            _currentVerticalMenuChoice = _GameOverMenuList.Count - 1;

        for (int i = 0; i < _GameOverMenuList.Count; i++)
        {
            if (i == _currentVerticalMenuChoice)
            {
                _GameOverMenuList[i].OnSelect(this);
            }
            else
            {
                _GameOverMenuList[i].OnDeSelect(this);
            }
        }
    }

    /// <summary>
    /// Call this event method to show pre game start ui.
    /// </summary>
    private void GameManager_OnPreStartGame()
    {
        _GameStartMenuGameObject.gameObject.SetActive(true);
        _ReadyText.gameObject.SetActive(true);
        _ReadyText.text = _readyString;
        StartCoroutine(DelayTime(2f
            , delegate
            {
                _ReadyText.text = _startString;
                StartCoroutine(DelayTime(1f
                    , delegate
                    {
                        _GameStartMenuGameObject.SetActive(false);
                        _ReadyText.gameObject.SetActive(false);
                        OnUIGameReadyToPlay?.Invoke();
                    })
                );
            })
        );
    }

    /// <summary>
    /// Call this event method to show stage clear ui.
    /// </summary>
    private void GameManager_OnStageClear()
    {
        _GameStartMenuGameObject.gameObject.SetActive(true);
        _ReadyText.gameObject.SetActive(true);
        _ReadyText.text = _stageClearString;
        StartCoroutine(DelayTime(2f
            , GameManager_OnPreStartGame)
        );
    }

    /// <summary>
    /// Call this event method to show game retry ui.
    /// </summary>
    private void GameManager_OnGameRetry()
    {
        StartCoroutine(DelayTime(3f
            , () =>
            {
                _GameStartMenuGameObject.gameObject.SetActive(true);
                _ReadyText.gameObject.SetActive(true);
                _ReadyText.text = _readyString;
                OnRespawnPlayer?.Invoke();
                StartCoroutine(DelayTime(2f
                    , () =>
                    {
                        _ReadyText.text = _startString;
                        StartCoroutine(DelayTime(1f
                            , delegate
                            {
                                _GameStartMenuGameObject.SetActive(false);
                                _ReadyText.gameObject.SetActive(false);
                                OnUIGameReadyToRetry?.Invoke();
                            })
                        );
                    }));
            })
        );
    }

    /// <summary>
    /// Call this event method to show game over ui.
    /// </summary>
    private void GameManager_OnGameOver()
    {
        _GameOverMenuGameObject.gameObject.SetActive(true);
        _GameOverText.gameObject.SetActive(true);
        _GameOverText.text = _gameOverString;
        StartCoroutine(DelayTime(3f
            , OnGameOverMenuInit)
        );
    }

    #endregion

    #region Init Methods

    /// <summary>
    /// Call this method to init when first start open the game
    /// </summary>
    public void OpenInit()
    {
        GameData data = MenuManager.Instance.Data;

        _highScore = GameData.HighScore;
        _ScoreText.text = "0";
        _LifeText.text = $"x {data.PlayerLife - 1}";
        _HighScoreText.text = _highScore.ToString();
    }

    /// <summary>
    /// Call this method to update data ui.
    /// </summary>
    /// <param name="data"></param>
    public void GameUIInit(GameData data)
    {
        _highScore = GameData.HighScore;

        _ScoreText.text = "0";
        _LifeText.text = $"x {data.PlayerLife - 1}";
        _HighScoreText.text = _highScore.ToString();
    }

    /// <summary>
    /// Call this method to init custom game menu ui.
    /// </summary>
    private void OnCustomMenuInit()
    {
        GameData data = MenuManager.Instance.Data;

        _lifeSetting = data.PlayerLife;
        _speedSetting = data.Speed;
        _bulletSetting = data.Bullet;

        _CustomMenuList[(int)CustomMenuOrder.Life].UpdateValueText(this, _lifeSetting.ToString());
        _CustomMenuList[(int)CustomMenuOrder.Speed].UpdateValueText(this, _speedSetting.ToString("n0"));
        _CustomMenuList[(int)CustomMenuOrder.Bullet].UpdateValueText(this, _bulletSetting.ToString());

        #region Life Setting

        if (_lifeSetting > GameData.MinLife && _lifeSetting < GameData.MaxLife)
        {
            _CustomMenuList[(int)CustomMenuOrder.Life].SetDecreaseState(this, true);
            _CustomMenuList[(int)CustomMenuOrder.Life].SetIncreaseState(this, true);
        }
        else if (_lifeSetting <= GameData.MinLife)
        {
            _CustomMenuList[(int)CustomMenuOrder.Life].SetDecreaseState(this, false);
            _CustomMenuList[(int)CustomMenuOrder.Life].SetIncreaseState(this, true);
            _CustomMenuList[(int)CustomMenuOrder.Life].UpdateValueText(this, GameData.MinLife.ToString());
            _lifeSetting = GameData.MinLife;
        }
        else if(_lifeSetting >= GameData.MaxLife)
        {
            _CustomMenuList[(int)CustomMenuOrder.Life].SetDecreaseState(this, true);
            _CustomMenuList[(int)CustomMenuOrder.Life].SetIncreaseState(this, false);
            _CustomMenuList[(int)CustomMenuOrder.Life].UpdateValueText(this, GameData.MaxLife.ToString());
            _lifeSetting = GameData.MaxLife;
        }

        #endregion

        #region Speed Setting

        if (_speedSetting > GameData.MinSpeed && _speedSetting < GameData.MaxSpeed)
        {
            _CustomMenuList[(int)CustomMenuOrder.Speed].SetDecreaseState(this, true);
            _CustomMenuList[(int)CustomMenuOrder.Speed].SetIncreaseState(this, true);
        }
        else if (_speedSetting <= GameData.MinSpeed)
        {
            _CustomMenuList[(int)CustomMenuOrder.Speed].SetDecreaseState(this, false);
            _CustomMenuList[(int)CustomMenuOrder.Speed].SetIncreaseState(this, true);
            _CustomMenuList[(int)CustomMenuOrder.Speed].UpdateValueText(this, GameData.MinSpeed.ToString("n0"));
            _speedSetting = GameData.MinSpeed;
        }
        else if(_speedSetting >= GameData.MaxSpeed)
        {
            _CustomMenuList[(int)CustomMenuOrder.Speed].SetDecreaseState(this, true);
            _CustomMenuList[(int)CustomMenuOrder.Speed].SetIncreaseState(this, false);
            _CustomMenuList[(int)CustomMenuOrder.Speed].UpdateValueText(this, GameData.MaxSpeed.ToString("n0"));
            _speedSetting = GameData.MaxSpeed;
        }

        #endregion

        #region Bullet Setting

        if (_bulletSetting > GameData.MinBullet && _bulletSetting < GameData.MaxBullet)
        {
            _CustomMenuList[(int)CustomMenuOrder.Bullet].SetDecreaseState(this, true);
            _CustomMenuList[(int)CustomMenuOrder.Bullet].SetIncreaseState(this, true);
        }
        else if (_bulletSetting <= GameData.MinBullet)
        {
            _CustomMenuList[(int)CustomMenuOrder.Bullet].SetDecreaseState(this, false);
            _CustomMenuList[(int)CustomMenuOrder.Bullet].SetIncreaseState(this, true);
            _CustomMenuList[(int)CustomMenuOrder.Bullet].UpdateValueText(this, GameData.MinBullet.ToString());
            _bulletSetting = GameData.MinBullet;
        }
        else if(_bulletSetting >= GameData.MaxBullet)
        {
            _CustomMenuList[(int)CustomMenuOrder.Bullet].SetDecreaseState(this, true);
            _CustomMenuList[(int)CustomMenuOrder.Bullet].SetIncreaseState(this, false);
            _CustomMenuList[(int)CustomMenuOrder.Bullet].UpdateValueText(this, GameData.MaxBullet.ToString());
            _bulletSetting = GameData.MaxBullet;
        }

        #endregion

        _currentVerticalMenuChoice = 0;

        for (int i = 0; i < _CustomMenuList.Count - 1; i++)
        {
            if (i == _currentVerticalMenuChoice)
            {
                _CustomMenuList[i].OnSelect(this);
            }
            else
            {
                _CustomMenuList[i].OnDeSelect(this);
            }
        }

        _CustomMenuList[(int)CustomMenuOrder.ConfirmMenu].UpdateHorizontalSelection(this, -1);
    }

    /// <summary>
    /// Call this method to init game over menu ui.
    /// </summary>
    private void OnGameOverMenuInit()
    {
        _GameOverMenuGameObject.SetActive(true);
        _currentVerticalMenuChoice = 0;

        for (int i = 0; i < _GameOverMenuList.Count; i++)
        {
            _GameOverMenuList[i].SetTextActiveState(this, true);
            if (i == _currentVerticalMenuChoice)
            {
                _GameOverMenuList[i].OnSelect(this);
            }
            else
            {
                _GameOverMenuList[i].OnDeSelect(this);
            }
        }

        OnChangeMenuScene?.Invoke(MenuScene.GameOver);
    }

    #endregion

    #region Scene Methods

    /// <summary>
    /// Call this method to open next menu scene.
    /// </summary>
    public void OpenNextMenu()
    {
        MenuScene currentScene = MenuManager.Instance.CurrentMenuScene;

        switch (currentScene)
        {
            case MenuScene.MainMenu:
            {
                MenuScene selectOption = _MainMenuList[_currentVerticalMenuChoice].MenuScene;
                
                switch (selectOption)
                {
                    case MenuScene.CustomMenu:
                    {
                        OnCustomMenuInit();
                        _MainMenuGameObject.SetActive(false);
                        _CustomMenuGameObject.SetActive(true);
                        _GameStartMenuGameObject.SetActive(false);
                        _GameOverMenuGameObject.SetActive(false);
                        OnChangeMenuScene?.Invoke(selectOption);
                    }
                        break;
                    case MenuScene.Game:
                    {
                        _MainMenuGameObject.SetActive(false);
                        _CustomMenuGameObject.SetActive(false);
                        _GameStartMenuGameObject.SetActive(true);
                        _GameOverMenuGameObject.SetActive(false);
                        GameUIInit(MenuManager.Instance.Data);
                        OnChangeMenuScene?.Invoke(selectOption);
                        OnInGameSceneActive?.Invoke(MenuManager.Instance.Data);
                    }
                        break;
                }
            }
                break;
            case MenuScene.CustomMenu:
            {
                if (_currentVerticalMenuChoice != (int) CustomMenuOrder.ConfirmMenu) return;

                MenuScene selectOption = _CustomMenuList[(int) CustomMenuOrder.ConfirmMenu].MenuSceneList[_confirmMenuIndex];

                switch (selectOption)
                {
                    case MenuScene.MainMenu:
                    {
                        OnOpenGame_MainMenuInit();
                        _MainMenuGameObject.SetActive(true);
                        _CustomMenuGameObject.SetActive(false);
                        _GameStartMenuGameObject.SetActive(false);
                        _GameOverMenuGameObject.SetActive(false);
                        OnChangeMenuScene?.Invoke(selectOption);
                    }
                        break;
                    case MenuScene.Game:
                    {
                        GameData data = new GameData(MenuManager.Instance.Data);
                        data.UpdateData(_lifeSetting, _speedSetting, _bulletSetting);
                        _MainMenuGameObject.SetActive(false);
                        _CustomMenuGameObject.SetActive(false);
                        _GameStartMenuGameObject.SetActive(true);
                        _GameOverMenuGameObject.SetActive(false);
                        GameUIInit(data);
                        OnChangeMenuScene?.Invoke(selectOption);
                        OnInGameSceneActive?.Invoke(data);
                    }
                        break;
                }
            }
                break;
            case MenuScene.GameOver:
            {
                MenuScene selectOption = _GameOverMenuList[_currentVerticalMenuChoice].MenuScene;

                foreach (MenuItemUI menuItemUi in _GameOverMenuList)
                {
                    menuItemUi.OnDeSelect(this);
                }

                switch (selectOption)
                {
                    case MenuScene.MainMenu:
                    {
                        OnOpenGame_MainMenuInit();
                        _MainMenuGameObject.SetActive(true);
                        _CustomMenuGameObject.SetActive(false);
                        _GameStartMenuGameObject.SetActive(false);
                        _GameOverMenuGameObject.SetActive(false);
                        OnChangeMenuScene?.Invoke(selectOption);
                        OnResetGameScene?.Invoke();
                    }
                        break;
                    case MenuScene.Game:
                    {
                        _MainMenuGameObject.SetActive(false);
                        _CustomMenuGameObject.SetActive(false);
                        _GameStartMenuGameObject.SetActive(true);
                        _GameOverMenuGameObject.SetActive(false);
                        GameUIInit(GameManager.Instance.Data);
                        OnChangeMenuScene?.Invoke(selectOption);
                        OnInGameSceneActive?.Invoke(GameManager.Instance.Data);
                        OnResetGameScene?.Invoke();
                    }
                        break;
                }
            }
                break;
        }
    }

    #endregion

    #region Co-routine Methods

    /// <summary>
    /// Call this method to delay action for specific time.
    /// </summary>
    /// <param name="time">Define time.</param>
    /// <param name="onComplete">Callback method when action is complete.</param>
    /// <returns></returns>
    private IEnumerator DelayTime(float time, UnityAction onComplete)
    {
        yield return new WaitForSeconds(time);
        onComplete?.Invoke();
    }

    #endregion

    #endregion
}
