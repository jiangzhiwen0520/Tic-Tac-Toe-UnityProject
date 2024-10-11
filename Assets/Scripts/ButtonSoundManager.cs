using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSoundManager : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioSource audioSource; // ��ť����ƵԴ
    public AudioClip hoverClip; // ��ͣ��Ч
    public AudioClip clickClip; // �����Ч

    // �������ͣ�ڰ�ť��ʱ������Ч
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySound(hoverClip);
    }

    // �������ťʱ������Ч
    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySound(clickClip);
    }

    // ������Ч
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
