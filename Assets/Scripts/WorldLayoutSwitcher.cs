using UnityEngine;

public class WorldLayoutSwitcher : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform table;
    [SerializeField] private Transform background;

    [SerializeField] private float landscapeOrthoSize = 5f;
    [SerializeField] private float portraitOrthoSize  = 8.13f;

    [SerializeField] private Vector3 landscapeTablePosition = new Vector3(0f, -0.81f, 0f);
    [SerializeField] private Vector3 portraitTablePosition  = new Vector3(0f, -0.30f, 0f);

    private bool _wasPortrait;

    private void Start() => Apply(IsPortrait());

    private void Update()
    {
        bool portrait = IsPortrait();
        if (portrait != _wasPortrait) Apply(portrait);
    }

    private static bool IsPortrait() => Screen.width < Screen.height;

    private void Apply(bool portrait)
    {
        _wasPortrait = portrait;
        cam.orthographicSize = portrait ? portraitOrthoSize : landscapeOrthoSize;
        table.position       = portrait ? portraitTablePosition : landscapeTablePosition;
        RefreshBackground();
    }

    private void RefreshBackground()
    {
        float h = cam.orthographicSize * 2f + 1f;
        float w = h * cam.aspect + 1f;
        background.localScale = new Vector3(w, h, 1f);
    }
}
