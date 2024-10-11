using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StartMenu : MonoBehaviour
{
    public TextMeshProUGUI difficultyText;
    public Button leftArrowButton;
    public Button rightArrowButton;
    public Button startButton;
    public Button exitButton;

    private string[] difficulties = { "Easy", "Hard" };
    private int currentDifficultyIndex = 0;

    public AudioSource backgroundMusicSource;

    void Start()
    {
        backgroundMusicSource.Play();

        // 更新难度文本
        UpdateDifficultyText();

        // 添加按钮事件监听器
        leftArrowButton.onClick.AddListener(OnLeftArrowClicked);
        rightArrowButton.onClick.AddListener(OnRightArrowClicked);
        startButton.onClick.AddListener(OnStartButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    void UpdateDifficultyText()
    {
        difficultyText.text = difficulties[currentDifficultyIndex];
    }

    void OnLeftArrowClicked()
    {
        currentDifficultyIndex--;
        if (currentDifficultyIndex < 0)
        {
            currentDifficultyIndex = difficulties.Length - 1;
        }
        UpdateDifficultyText();
    }

    void OnRightArrowClicked()
    {
        currentDifficultyIndex++;
        if (currentDifficultyIndex >= difficulties.Length)
        {
            currentDifficultyIndex = 0;
        }
        UpdateDifficultyText();
    }

    void OnStartButtonClicked()
    {
        // 保存选择的难度
        GameSettings.SelectedDifficulty = difficulties[currentDifficultyIndex];

        // 加载游戏场景
        SceneManager.LoadScene("MainScene");
    }

    void OnExitButtonClicked()
    {
        // 退出游戏
        Application.Quit();
    }
}
