using UnityEngine;

public class GridCell : MonoBehaviour
{
    public Material transparentMaterial; // �ո��Ĭ�ϲ���
    public Material highlightMaterial;   // �������ʱ�ĸ�������
    public Material CircleMaterial; // ���Ȧ�Ĳ���
    public Material CrossMaterial;      // ���Բ�Ĳ���

    private Renderer cellRenderer;  // ��ǰ���ӵ���Ⱦ��
    private bool isEmpty = true;    // �����жϸø��Ƿ�Ϊ��
    private bool isPlayerTurn;      // �����ж��Ƿ���һغ�
    private Vector2Int gridPosition; // ���̸��ӵ�λ��

    public AudioSource audioSource; // ������ƵԴ
    public AudioClip hoverClip; // ��ͣ��Ч
    public AudioClip clickClip; // �����Ч

    public void Initialize(Vector2Int position)
    {
        cellRenderer = GetComponent<Renderer>();
        gridPosition = position;
        SetTransparent(); // ��ʼ��Ϊ��
    }

    public bool IsEmpty()
    {
        return isEmpty;
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    // ����������ڸ�����ʱ
    private void OnMouseEnter()
    {
        if (isEmpty)
        {
            PlaySound(hoverClip);
            SetHighlight();
        }
    }

    // ������뿪����ʱ
    private void OnMouseExit()
    {
        if (isEmpty)
        {
            SetTransparent();
        }
    }

    // �����������ʱ
    private void OnMouseDown()
    {
        if (isEmpty && isPlayerTurn)
        {
            PlaySound(clickClip);
            // ����¡�Ȧ��
            SetPlayerCircle();
            isEmpty = false;

            // ֪ͨGameManager����������
            GameManager.Instance.PlayerMove(gridPosition);
        }
    }

    // ����͸�����
    public void SetTransparent()
    {
        cellRenderer.material = transparentMaterial;
    }

    // ���ø������
    public void SetHighlight()
    {
        cellRenderer.material = highlightMaterial;
    }

    // ������ҡ�Ȧ��ͼ��
    public void SetPlayerCircle()
    {
        cellRenderer.material = CircleMaterial;
    }

    // ���õ��ԡ��桱ͼ��
    public void SetAICross()
    {
        cellRenderer.material = CrossMaterial;
        isEmpty = false; // һ���������壬�ø���Ϊ��
    }

    // ��GameManager���Ƶ�ǰ�Ƿ�Ϊ��һغ�
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


