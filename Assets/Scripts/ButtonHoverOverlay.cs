using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverOverlay : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData _) => AudioManager.Instance?.PlayClick();
}
