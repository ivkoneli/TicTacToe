using UnityEngine;

public class PopupSound : MonoBehaviour
{
    private void OnEnable() => AudioManager.Instance?.PlayWhoosh();
}
