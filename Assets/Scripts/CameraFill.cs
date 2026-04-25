using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class CameraFill : MonoBehaviour
{
    private SpriteRenderer _sr;

    void Awake() => _sr = GetComponent<SpriteRenderer>();

    void LateUpdate() => Fit();

    void Fit()
    {
        if (_sr == null) _sr = GetComponent<SpriteRenderer>();
        if (_sr?.sprite == null) return;
        var cam = Camera.main;
        if (cam == null) return;
        float worldH = cam.orthographicSize * 2f;
        float worldW = worldH * ((float)Screen.width / Screen.height);
        var s = _sr.sprite.bounds.size;
        transform.localScale = new Vector3(worldW / s.x, worldH / s.y, transform.localScale.z);
    }
}
