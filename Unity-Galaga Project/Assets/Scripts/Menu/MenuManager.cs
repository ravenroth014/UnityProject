//  MenuManager.cs
//  By Atid Puwatnuttasit

using System;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    #region Public Properties

    public static MenuManager Instance { get; private set; }                // Singleton instance.
    public MenuScene CurrentMenuScene => _currentMenuScene;                 // Current active menu scene.
    public GameData Data => _data;                                          // Default game data.
    #endregion

    #region Private Properties

    private MenuScene _currentMenuScene = MenuScene.MainMenu;
    private GameData _data;
    #endregion

    #region Events

    public static event Action OnOpenGame;                                          // Event activate on open game.
    
    public static event Action<int> OnChangeMainMenuChoice;                         // Event activate when change option on main menu.
    public static event Action<int> OnChangeGameOverMenuChoice;                     // Event activate when change option on game over menu.
    public static event Action<int> OnChangeVerticalCustomMenuChoice;               // Event activate when select vertical option on custom game menu.
    public static event Action<int> OnChangeHorizontalCustomMenuChoice;             // Event activate when select horizontal option on custom game menu.

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

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        // Menu action.
        if (GameManager.Instance.IsGameActive == false)
        {
            switch (_currentMenuScene)
            {
                case MenuScene.MainMenu:
                    OnMainMenuSelection();
                    break;
                case MenuScene.CustomMenu:
                    OnCustomMenuSelection();
                    break;
                case MenuScene.GameOver:
                    OnGameOverMenuSelection();
                    break;
            }
        }
    }

    // Subscribe event.
    private void OnEnable()
    {
        UIManager.OnChangeMenuScene += UiManager_OnChangeMenuScene;
    }

    // Unsubscribe event.
    private void OnDisable()
    {
        UIManager.OnChangeMenuScene -= UiManager_OnChangeMenuScene;
    }

    #endregion

    #region Init Methods

    /// <summary>
    /// Call this method to init this instance.
    /// </summary>
    private void Init()
    {
        _currentMenuScene = MenuScene.MainMenu;
        _data = new GameData();

        OnOpenGame?.Invoke();
    }

    #endregion

    #region Menu Methods

    /// <summary>
    /// Call this method to manage main menu.
    /// </summary>
    private void OnMainMenuSelection()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnChangeMainMenuChoice?.Invoke(-1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnChangeMainMenuChoice?.Invoke(1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            UIManager.Instance.OpenNextMenu();
        }
    }

    /// <summary>
    /// Call this method to manage custom game menu.
    /// </summary>
    private void OnCustomMenuSelection()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnChangeVerticalCustomMenuChoice?.Invoke(-1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnChangeVerticalCustomMenuChoice?.Invoke(1);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnChangeHorizontalCustomMenuChoice?.Invoke(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnChangeHorizontalCustomMenuChoice?.Invoke(1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            UIManager.Instance.OpenNextMenu();
        }
    }

    /// <summary>
    /// Call this method to manage game over menu.
    /// </summary>
    private void OnGameOverMenuSelection()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnChangeGameOverMenuChoice?.Invoke(-1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnChangeGameOverMenuChoice?.Invoke(1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            UIManager.Instance.OpenNextMenu();
        }
    }

    #endregion

    #region Event Methods

    /// <summary>
    /// Call this method when the current main scene is changed.
    /// </summary>
    /// <param name="obj">Update current menu scene.</param>
    private void UiManager_OnChangeMenuScene(MenuScene obj)
    {
        _currentMenuScene = obj;
    }

    #endregion

    #endregion

}
