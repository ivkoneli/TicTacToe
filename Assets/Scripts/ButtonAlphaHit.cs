using UnityEngine;
using UnityEngine.UI;

public class ButtonAlphaHit : MonoBehaviour
{
    [SerializeField] private float threshold = 0.1f;

    private void Awake()
    {
        foreach (var img in GetComponentsInChildren<Image>(true))
            img.alphaHitTestMinimumThreshold = threshold;
    }
}
