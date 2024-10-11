using UnityEngine;

public class GridCell : MonoBehaviour
{
    public Material transparentMaterial; // 空格的默认材质
    public Material highlightMaterial;   // 鼠标悬浮时的高亮材质
    public Material CircleMaterial; // 玩家圈的材质
    public Material CrossMaterial;      // 电脑叉的材质

    private Renderer cellRenderer;  // 当前格子的渲染器
    private bool isEmpty = true;    // 用于判断该格是否为空
    private bool isPlayerTurn;      // 用于判定是否玩家回合
    private Vector2Int gridPosition; // 棋盘格子的位置

    public AudioSource audioSource; // 棋格的音频源
    public AudioClip hoverClip; // 悬停音效
    public AudioClip clickClip; // 点击音效

    public void Initialize(Vector2Int position)
    {
        cellRenderer = GetComponent<Renderer>();
        gridPosition = position;
        SetTransparent(); // 初始化为空
    }

    public bool IsEmpty()
    {
        return isEmpty;
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    // 当鼠标悬浮在格子上时
    private void OnMouseEnter()
    {
        if (isEmpty)
        {
            PlaySound(hoverClip);
            SetHighlight();
        }
    }

    // 当鼠标离开格子时
    private void OnMouseExit()
    {
        if (isEmpty)
        {
            SetTransparent();
        }
    }

    // 当鼠标点击格子时
    private void OnMouseDown()
    {
        if (isEmpty && isPlayerTurn)
        {
            PlaySound(clickClip);
            // 玩家下“圈”
            SetPlayerCircle();
            isEmpty = false;

            // 通知GameManager玩家完成下棋
            GameManager.Instance.PlayerMove(gridPosition);
        }
    }

    // 设置透明外观
    public void SetTransparent()
    {
        cellRenderer.material = transparentMaterial;
    }

    // 设置高亮外观
    public void SetHighlight()
    {
        cellRenderer.material = highlightMaterial;
    }

    // 设置玩家“圈”图案
    public void SetPlayerCircle()
    {
        cellRenderer.material = CircleMaterial;
    }

    // 设置电脑“叉”图案
    public void SetAICross()
    {
        cellRenderer.material = CrossMaterial;
        isEmpty = false; // 一旦电脑下棋，该格不再为空
    }

    // 让GameManager控制当前是否为玩家回合
    public void SetPlayerTurn(bool isPlayerTurn)
    {
        this.isPlayerTurn = isPlayerTurn;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}


