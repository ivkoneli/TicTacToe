using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class LayoutSwitcher : MonoBehaviour
{
    [SerializeField] private RectTransform landscapeRoot;
    [SerializeField] private RectTransform portraitRoot;
    // When true: same GameObjects move between roots (MainMenu — same layout, different scale).
    // When false: two distinct layouts toggled by active state (GameScene).
    [SerializeField] private bool sharedContent;
    [SerializeField] private GameObject landscapeBackground;
    [SerializeField] private GameObject portraitBackground;

    private bool _wasPortrait;

    private void Start() => Apply(IsPortrait());

    private void Update()
    {
        // In edit mode, skip sharedContent mode to avoid moving objects unintentionally
        if (!Application.isPlaying && sharedContent) return;
        bool portrait = IsPortrait();
        if (portrait != _wasPortrait) Apply(portrait);
    }

    private static bool IsPortrait() => Screen.width < Screen.height;

    private void Apply(bool portrait)
    {
        _wasPortrait = portrait;
        if (portraitRoot == null) return;

        if (sharedContent)
        {
            var source = portrait ? landscapeRoot : portraitRoot;
            var dest   = portrait ? portraitRoot   : landscapeRoot;

            var kids = new List<Transform>();
            foreach (Transform t in source) kids.Add(t);
            foreach (var k in kids) k.SetParent(dest, false);
        }

        landscapeRoot.gameObject.SetActive(!portrait);
        portraitRoot.gameObject.SetActive(portrait);
        if (landscapeBackground != null) landscapeBackground.SetActive(!portrait);
        if (portraitBackground != null) portraitBackground.SetActive(portrait);
    }
}
