using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSoundManager : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioSource audioSource; // 按钮的音频源
    public AudioClip hoverClip; // 悬停音效
    public AudioClip clickClip; // 点击音效

    // 当鼠标悬停在按钮上时播放音效
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySound(hoverClip);
    }

    // 当点击按钮时播放音效
    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySound(clickClip);
    }

    // 播放音效
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
