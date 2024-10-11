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

    public GameObject pauseMenuPanel; // ��ͣ�˵����
    public Button resumeButton;       // ������Ϸ��ť
    public Button returnButton;       // �������˵���ť
    public Button quitButton;         // �˳���Ϸ��ť


    public GameObject victoryPanel; // ʤ�����
    public TextMeshProUGUI victoryText; // ��ʾ˭��ʤ���ı�
    public Button restartButton; // ���¿�ʼ��ť

    public GameObject[] gridCells; 
    private string[,] board = new string[3, 3];
                
    private bool isPlayerTurn;
    private bool gamePaused = false;
    private bool gameStarted = false; // �����Ϸ�Ƿ��Ѿ���ʼ
    private bool gameEnded = false;

    private string selectedDifficulty; // ����ѡ����Ѷ�

    public AudioSource backgroundMusicSource;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

        backgroundMusicSource.Play();
        // �� GameSettings �л�ȡ�Ѷ�
        selectedDifficulty = GameSettings.SelectedDifficulty;
        Debug.Log(selectedDifficulty);
        // ��ʼ������״̬
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                board[i, j] = "";
            }
        }

        // ���ѡ������
        isPlayerTurn = Random.value > 0.5f;
        ShowRandomResultPanel();

        // Ϊȷ�ϰ�ť���õ���¼�
        randomResultConfirmButton.onClick.AddListener(OnConfirmRandomResult);

        // ��ʼ��ÿ�����ӵ��߼�
        for (int i = 0; i < gridCells.Length; i++)
        {
            GridCell cell = gridCells[i].GetComponent<GridCell>();
            cell.Initialize(new Vector2Int(i / 3, i % 3)); 
            cell.SetPlayerTurn(isPlayerTurn);
        }

        

        // Ϊ��ͣ�˵���ť���õ���¼�
        resumeButton.onClick.AddListener(ResumeGame);
        returnButton.onClick.AddListener(ReturnToStartMenu);
        quitButton.onClick.AddListener(QuitGame);

        // ������ʤ�����
        victoryPanel.SetActive(false);

        // Ϊ��ť��ӵ���¼�
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    // ��ʾ���ѡ�����ֵĽ�����
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
        DisableBoardInteraction();  // ������񽻻�
    }

    // ȷ��������ֽ������ʼ��Ϸ
    void OnConfirmRandomResult()
    {
        randomResultPanel.SetActive(false); // �������������
        gameStarted = true;

        // ����������֣�����ִ��AI����
        if (!isPlayerTurn)
        {
            // �����Ѷ�ѡ��AI��Ϊ
            if (selectedDifficulty == "Hard")
            {
                StartCoroutine(HardAI_Move());
            }
            else
            {
                StartCoroutine(SimpleAI_Move());
            }
        }
        EnableBoardInteraction();  // ������񽻻�
    }

    void Update()
    {
        // ����Ƿ���Esc������/�ر���ͣ�˵�
        if (gameStarted && Input.GetKeyDown(KeyCode.Escape) && !gameEnded)
        {
            TogglePauseMenu();
        }
        if (gamePaused)
        {
            DisableBoardInteraction();  // ��ͣʱ������񽻻�
        }
        else if (!gamePaused && gameStarted)
        {
            EnableBoardInteraction();   // �ָ�ʱ������񽻻�
        }
    }

    // �л���ͣ�˵�����ʾ״̬
    void TogglePauseMenu()
    {
        gamePaused = !gamePaused;
        pauseMenuPanel.SetActive(gamePaused);

        if (gamePaused)
        {
            Time.timeScale = 0; // ��ͣ��Ϸʱ��
        }
        else
        {
            Time.timeScale = 1; // �ָ���Ϸʱ��
        }
    }

    // ������Ϸ
    public void ResumeGame()
    {
        TogglePauseMenu(); // �ر���ͣ�˵�
    }

    // �������˵�
    public void ReturnToStartMenu()
    {
        Time.timeScale = 1; // �ָ�ʱ������
        SceneManager.LoadScene("StartScene");
    }

    // �˳���Ϸ
    public void QuitGame()
    {
        Application.Quit();
    }

    // ���¿�ʼ��Ϸ
    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // ���¼��ص�ǰ����
    }

    // ������񽻻�
    void DisableBoardInteraction()
    {
        foreach (GameObject cell in gridCells)
        {
            Collider cellCollider = cell.GetComponent<Collider>();
            if (cellCollider != null)
            {
                cellCollider.enabled = false;  // ��������Collider
            }
        }
    }

    // ������񽻻�
    void EnableBoardInteraction()
    {
        foreach (GameObject cell in gridCells)
        {
            Collider cellCollider = cell.GetComponent<Collider>();
            if (cellCollider != null)
            {
                cellCollider.enabled = true;  // ��������Collider
            }
        }
    }

    // ����ƶ��߼�
    public void PlayerMove(Vector2Int gridPosition)
    {
        if (gameEnded) return;

        // ��������״̬
        board[gridPosition.x, gridPosition.y] = "O";

        // ���ʤ��
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
            // �л������Իغ�
            isPlayerTurn = false;
            SetPlayerTurnForCells(false);
            // �����Ѷ�ѡ��AI��Ϊ
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

    // AI�ƶ��߼����������
    IEnumerator SimpleAI_Move()
    {
        yield return new WaitForSeconds(1); // ģ��˼��ʱ��

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

            // ��������״̬
            Vector2Int gridPosition = randomCell.GetGridPosition();
            board[gridPosition.x, gridPosition.y] = "X";

            // ���ʤ��
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
                // �л�����һغ�
                isPlayerTurn = true;
                SetPlayerTurnForCells(true);
            }
        }
    }

    // Hard�Ѷ�AI���ƶ��߼�
    IEnumerator HardAI_Move()
    {
        yield return new WaitForSeconds(1); // ģ��AI˼��ʱ��

        // ʹ��Minimax�㷨�ҵ����λ��
        Vector2Int bestMove = FindBestMove();

        // ��������
        board[bestMove.x, bestMove.y] = "X";
        gridCells[bestMove.x * 3 + bestMove.y].GetComponent<GridCell>().SetAICross(); // ��ʾ��

        // ���ʤ��
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
            isPlayerTurn = true; // �л�����һغ�
            SetPlayerTurnForCells(true);
        }
    }

    // Minimax�㷨��ʵ��
    Vector2Int FindBestMove()
    {
        int bestScore = int.MinValue;
        Vector2Int bestMove = new Vector2Int(-1, -1);

        // �������̣��ҵ����п�λ��
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] == "")
                {
                    // ģ���ڸ�λ���¡��桱
                    board[i, j] = "X";

                    // ������ƶ��ĵ÷�
                    int moveScore = Minimax(board, 0, false);

                    // ��ԭ����
                    board[i, j] = "";

                    // ������ƶ��ĵ÷ֱȵ�ǰ��ѵ÷ָ��ߣ���������ƶ�
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

    // Minimax�㷨
    int Minimax(string[,] boardState, int depth, bool isMaximizing)
    {
        // ����Ƿ�����ʤ��
        if (CheckWin("X")) return 10 - depth; // AIʤ������������
        if (CheckWin("O")) return depth - 10; // ���ʤ�������ظ���
        if (IsBoardFull()) return 0;          // ƽ��

        if (isMaximizing)
        {
            int bestScore = int.MinValue;

            // ����ÿһ���ո���
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (boardState[i, j] == "")
                    {
                        // ģ��AI�¡��桱
                        boardState[i, j] = "X";

                        // �ݹ�������
                        int score = Minimax(boardState, depth + 1, false);

                        // ��ԭ����
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

            // ����ÿһ���ո���
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (boardState[i, j] == "")
                    {
                        // ģ������¡�Ȧ��
                        boardState[i, j] = "O";

                        // �ݹ�������
                        int score = Minimax(boardState, depth + 1, true);

                        // ��ԭ����
                        boardState[i, j] = "";

                        bestScore = Mathf.Min(score, bestScore);
                    }
                }
            }
            return bestScore;
        }
    }


    // ����Ƿ�����һ�ʤ
    bool CheckWin(string symbol)
    {
        // ���кͶԽ��߼��
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

    // ��������Ƿ�����
    bool IsBoardFull()
    {
        foreach (string cell in board)
        {
            if (cell == "")
                return false;
        }
        return true;
    }

    // ������Ϸ
    void EndGame(string resultText)
    {
        gameEnded = true;
       
        victoryPanel.SetActive(true);
        DisableBoardInteraction();  // ��ͣʱ������񽻻�
        victoryText.text = resultText; // ��ʾʤ����Ϣ
    }

    // �������и��ӵ���һغ�״̬
    void SetPlayerTurnForCells(bool isPlayerTurn)
    {
        foreach (GameObject cellObj in gridCells)
        {
            GridCell cell = cellObj.GetComponent<GridCell>();
            cell.SetPlayerTurn(isPlayerTurn);
        }
    }
}
