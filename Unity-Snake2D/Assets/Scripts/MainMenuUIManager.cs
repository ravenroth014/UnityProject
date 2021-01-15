using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuUIManager : MonoBehaviour
{
    #region Inspector Properties

    [Header("UI Setting")]
    [SerializeField] private Button _StartButton;                           // Start UI button.
    [SerializeField] private Button _SecondStartButton;                     // Start UI button on how to play panel.
    [SerializeField] private Button _HowToPlayButton;                       // How to play UI button.
    [SerializeField] private CanvasGroup _HowToPlayPanel;                   // How to play UI panel.
    [SerializeField] private Button _BackButton;                            // Close how to play UI panel button.

    #endregion

    #region Methods
    /// <summary>
    /// Use the Start Method to initialize the UI in Main menu scene.
    /// </summary>
    private void Start()
    {
        _StartButton.onClick.RemoveAllListeners();
        _StartButton.onClick.AddListener(() => { SceneController.Instance.LoadScene(SceneController.Instance.GamePlaySceneName); });

        _SecondStartButton.onClick.RemoveAllListeners();
        _SecondStartButton.onClick.AddListener(() => { SceneController.Instance.LoadScene(SceneController.Instance.GamePlaySceneName); });

        _HowToPlayButton.onClick.RemoveAllListeners();
        _HowToPlayButton.onClick.AddListener(OpenHowToPlay);

        _BackButton.onClick.RemoveAllListeners();
        _BackButton.onClick.AddListener(CloseHowToPlay);

        // Set default highscore for the first time playing.
        if (!PlayerPrefs.HasKey(GameManager.HighScoreString))
        {
            PlayerPrefs.SetInt(GameManager.HighScoreString, 1000);
        }
    }

    /// <summary>
    /// Call this method to open How to play panel.
    /// </summary>
    private void OpenHowToPlay()
    {
        _HowToPlayPanel.gameObject.SetActive(true);
        _HowToPlayPanel.DOFade(1, 1).OnComplete(() => {
            _SecondStartButton.enabled = true;
            _BackButton.enabled = true;
        });
    }

    /// <summary>
    /// Call this method to close How to play panel.
    /// </summary>
    private void CloseHowToPlay()
    {
        _HowToPlayPanel.DOFade(0, 1).OnComplete(() => {
            _SecondStartButton.enabled = false;
            _BackButton.enabled = false;
            _HowToPlayPanel.gameObject.SetActive(false); 
        });
    }

    #endregion
}
