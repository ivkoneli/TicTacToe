using UnityEngine;
using UnityEngine.EventSystems;

public class UiSound : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private bool playOnEnable;
    [SerializeField] private bool playOnClick;

    private void OnEnable()
    {
        if (playOnEnable) AudioManager.Instance?.PlayWhoosh();
    }

    public void OnPointerDown(PointerEventData _)
    {
        if (playOnClick) AudioManager.Instance?.PlayClick();
    }
}
