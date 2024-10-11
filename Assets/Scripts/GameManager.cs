using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject randomResultPanel;
    public TextMeshProUGUI randomResultText;
    public Button randomResultConfirmButton;

    public GameObject pauseMenuPanel; // 暂停菜单面板
    public Button resumeButton;       // 继续游戏按钮
    public Button returnButton;       // 返回主菜单按钮
    public Button quitButton;         // 退出游戏按钮


    public GameObject victoryPanel; // 胜利面板
    public TextMeshProUGUI victoryText; // 显示谁获胜的文本
    public Button restartButton; // 重新开始按钮

    public GameObject[] gridCells; 
    private string[,] board = new string[3, 3];
                
    private bool isPlayerTurn;
    private bool gamePaused = false;
    private bool gameStarted = false; // 标记游戏是否已经开始
    private bool gameEnded = false;

    private string selectedDifficulty; // 保存选择的难度

    public AudioSource backgroundMusicSource;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

        backgroundMusicSource.Play();
        // 从 GameSettings 中获取难度
        selectedDifficulty = GameSettings.SelectedDifficulty;
        Debug.Log(selectedDifficulty);
        // 初始化棋盘状态
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                board[i, j] = "";
            }
        }

        // 随机选择先手
        isPlayerTurn = Random.value > 0.5f;
        ShowRandomResultPanel();

        // 为确认按钮设置点击事件
        randomResultConfirmButton.onClick.AddListener(OnConfirmRandomResult);

        // 初始化每个格子的逻辑
        for (int i = 0; i < gridCells.Length; i++)
        {
            GridCell cell = gridCells[i].GetComponent<GridCell>();
            cell.Initialize(new Vector2Int(i / 3, i % 3)); 
            cell.SetPlayerTurn(isPlayerTurn);
        }

        

        // 为暂停菜单按钮设置点击事件
        resumeButton.onClick.AddListener(ResumeGame);
        returnButton.onClick.AddListener(ReturnToStartMenu);
        quitButton.onClick.AddListener(QuitGame);

        // 先隐藏胜利面板
        victoryPanel.SetActive(false);

        // 为按钮添加点击事件
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    // 显示随机选择先手的结果面板
    void ShowRandomResultPanel()
    {
        randomResultPanel.SetActive(true);
        if (isPlayerTurn)
        {
            randomResultText.text = "Player First!";
        }
        else
        {
            randomResultText.text = "AI First!";
        }
        DisableBoardInteraction();  // 禁用棋格交互
    }

    // 确认随机先手结果，开始游戏
    void OnConfirmRandomResult()
    {
        randomResultPanel.SetActive(false); // 隐藏随机结果面板
        gameStarted = true;

        // 如果电脑先手，立即执行AI操作
        if (!isPlayerTurn)
        {
            // 根据难度选择AI行为
            if (selectedDifficulty == "Hard")
            {
                StartCoroutine(HardAI_Move());
            }
            else
            {
                StartCoroutine(SimpleAI_Move());
            }
        }
        EnableBoardInteraction();  // 启用棋格交互
    }

    void Update()
    {
        // 检查是否按下Esc键，打开/关闭暂停菜单
        if (gameStarted && Input.GetKeyDown(KeyCode.Escape) && !gameEnded)
        {
            TogglePauseMenu();
        }
        if (gamePaused)
        {
            DisableBoardInteraction();  // 暂停时禁用棋格交互
        }
        else if (!gamePaused && gameStarted)
        {
            EnableBoardInteraction();   // 恢复时启用棋格交互
        }
    }

    // 切换暂停菜单的显示状态
    void TogglePauseMenu()
    {
        gamePaused = !gamePaused;
        pauseMenuPanel.SetActive(gamePaused);

        if (gamePaused)
        {
            Time.timeScale = 0; // 暂停游戏时间
        }
        else
        {
            Time.timeScale = 1; // 恢复游戏时间
        }
    }

    // 继续游戏
    public void ResumeGame()
    {
        TogglePauseMenu(); // 关闭暂停菜单
    }

    // 返回主菜单
    public void ReturnToStartMenu()
    {
        Time.timeScale = 1; // 恢复时间流动
        SceneManager.LoadScene("StartScene");
    }

    // 退出游戏
    public void QuitGame()
    {
        Application.Quit();
    }

    // 重新开始游戏
    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 重新加载当前场景
    }

    // 禁用棋格交互
    void DisableBoardInteraction()
    {
        foreach (GameObject cell in gridCells)
        {
            Collider cellCollider = cell.GetComponent<Collider>();
            if (cellCollider != null)
            {
                cellCollider.enabled = false;  // 禁用棋格的Collider
            }
        }
    }

    // 启用棋格交互
    void EnableBoardInteraction()
    {
        foreach (GameObject cell in gridCells)
        {
            Collider cellCollider = cell.GetComponent<Collider>();
            if (cellCollider != null)
            {
                cellCollider.enabled = true;  // 启用棋格的Collider
            }
        }
    }

    // 玩家移动逻辑
    public void PlayerMove(Vector2Int gridPosition)
    {
        if (gameEnded) return;

        // 更新棋盘状态
        board[gridPosition.x, gridPosition.y] = "O";

        // 检查胜负
        if (CheckWin("O"))
        {
            EndGame("Player win!");
        }
        else if (IsBoardFull())
        {
            EndGame("Draw!");
        }
        else
        {
            // 切换到电脑回合
            isPlayerTurn = false;
            SetPlayerTurnForCells(false);
            // 根据难度选择AI行为
            if (selectedDifficulty == "Hard")
            {
                StartCoroutine(HardAI_Move());
            }
            else
            {
                StartCoroutine(SimpleAI_Move());
            }
        }
    }

    // AI移动逻辑（简单随机）
    IEnumerator SimpleAI_Move()
    {
        yield return new WaitForSeconds(1); // 模拟思考时间

        List<GridCell> availableCells = new List<GridCell>();
        foreach (GameObject cellObj in gridCells)
        {
            GridCell cell = cellObj.GetComponent<GridCell>();
            if (cell.IsEmpty())
            {
                availableCells.Add(cell);
            }
        }

        if (availableCells.Count > 0)
        {
            GridCell randomCell = availableCells[Random.Range(0, availableCells.Count)];
            randomCell.SetAICross();

            // 更新棋盘状态
            Vector2Int gridPosition = randomCell.GetGridPosition();
            board[gridPosition.x, gridPosition.y] = "X";

            // 检查胜负
            if (CheckWin("X"))
            {
                EndGame("AI Win!");
            }
            else if (IsBoardFull())
            {
                EndGame("Draw!");
            }
            else
            {
                // 切换到玩家回合
                isPlayerTurn = true;
                SetPlayerTurnForCells(true);
            }
        }
    }

    // Hard难度AI的移动逻辑
    IEnumerator HardAI_Move()
    {
        yield return new WaitForSeconds(1); // 模拟AI思考时间

        // 使用Minimax算法找到最佳位置
        Vector2Int bestMove = FindBestMove();

        // 更新棋盘
        board[bestMove.x, bestMove.y] = "X";
        gridCells[bestMove.x * 3 + bestMove.y].GetComponent<GridCell>().SetAICross(); // 显示叉

        // 检查胜负
        if (CheckWin("X"))
        {
            EndGame("AI Win!");
        }
        else if (IsBoardFull())
        {
            EndGame("Draw!");
        }
        else
        {
            isPlayerTurn = true; // 切换到玩家回合
            SetPlayerTurnForCells(true);
        }
    }

    // Minimax算法的实现
    Vector2Int FindBestMove()
    {
        int bestScore = int.MinValue;
        Vector2Int bestMove = new Vector2Int(-1, -1);

        // 遍历棋盘，找到所有空位置
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] == "")
                {
                    // 模拟在该位置下“叉”
                    board[i, j] = "X";

                    // 计算该移动的得分
                    int moveScore = Minimax(board, 0, false);

                    // 还原棋盘
                    board[i, j] = "";

                    // 如果该移动的得分比当前最佳得分更高，更新最佳移动
                    if (moveScore > bestScore)
                    {
                        bestScore = moveScore;
                        bestMove = new Vector2Int(i, j);
                    }
                }
            }
        }
        return bestMove;
    }

    // Minimax算法
    int Minimax(string[,] boardState, int depth, bool isMaximizing)
    {
        // 检查是否有人胜利
        if (CheckWin("X")) return 10 - depth; // AI胜利，返回正分
        if (CheckWin("O")) return depth - 10; // 玩家胜利，返回负分
        if (IsBoardFull()) return 0;          // 平局

        if (isMaximizing)
        {
            int bestScore = int.MinValue;

            // 尝试每一个空格子
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (boardState[i, j] == "")
                    {
                        // 模拟AI下“叉”
                        boardState[i, j] = "X";

                        // 递归计算分数
                        int score = Minimax(boardState, depth + 1, false);

                        // 还原棋盘
                        boardState[i, j] = "";

                        bestScore = Mathf.Max(score, bestScore);
                    }
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;

            // 尝试每一个空格子
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (boardState[i, j] == "")
                    {
                        // 模拟玩家下“圈”
                        boardState[i, j] = "O";

                        // 递归计算分数
                        int score = Minimax(boardState, depth + 1, true);

                        // 还原棋盘
                        boardState[i, j] = "";

                        bestScore = Mathf.Min(score, bestScore);
                    }
                }
            }
            return bestScore;
        }
    }


    // 检查是否有玩家获胜
    bool CheckWin(string symbol)
    {
        // 行列和对角线检测
        for (int i = 0; i < 3; i++)
        {
            if (board[i, 0] == symbol && board[i, 1] == symbol && board[i, 2] == symbol)
                return true;
            if (board[0, i] == symbol && board[1, i] == symbol && board[2, i] == symbol)
                return true;
        }

        if (board[0, 0] == symbol && board[1, 1] == symbol && board[2, 2] == symbol)
            return true;
        if (board[0, 2] == symbol && board[1, 1] == symbol && board[2, 0] == symbol)
            return true;

        return false;
    }

    // 检查棋盘是否已满
    bool IsBoardFull()
    {
        foreach (string cell in board)
        {
            if (cell == "")
                return false;
        }
        return true;
    }

    // 结束游戏
    void EndGame(string resultText)
    {
        gameEnded = true;
       
        victoryPanel.SetActive(true);
        DisableBoardInteraction();  // 暂停时禁用棋格交互
        victoryText.text = resultText; // 显示胜利信息
    }

    // 设置所有格子的玩家回合状态
    void SetPlayerTurnForCells(bool isPlayerTurn)
    {
        foreach (GameObject cellObj in gridCells)
        {
            GridCell cell = cellObj.GetComponent<GridCell>();
            cell.SetPlayerTurn(isPlayerTurn);
        }
    }
}
