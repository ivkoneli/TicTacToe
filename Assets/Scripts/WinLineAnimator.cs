using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLineAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer lineRenderer;
    [SerializeField] private float animDuration = 0.6f;

    [Header("X Line Sprites (0=Red 1=Green 2=Yellow)")]
    [SerializeField] private Sprite[] xLineSprites;

    [Header("O Line Sprites (0=Red 1=Green 2=Yellow)")]
    [SerializeField] private Sprite[] oLineSprites;

    private void Awake() => lineRenderer.enabled = false;

    public void Hide() => lineRenderer.enabled = false;

    public IEnumerator Animate(List<Vector2Int> winLine, TileState winner, Dictionary<Vector2Int, Tile> tileMap)
    {
        var startPos = tileMap[winLine[0]].transform.position;
        var endPos   = tileMap[winLine[winLine.Count - 1]].transform.position;
        var center = (startPos + endPos) * 0.5f;
        var dir    = endPos - startPos;

        int theme = Mathf.Clamp(PlayerPrefs.GetInt("SelectedTheme", 0), 0, 2);
        lineRenderer.sprite = winner == TileState.X ? xLineSprites[theme] : oLineSprites[theme];

        float worldAngle  = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float spriteAngle = worldAngle - 90f;
        float targetScale = dir.magnitude / lineRenderer.sprite.bounds.size.y;

        float posZ = lineRenderer.transform.position.z;
        lineRenderer.transform.rotation   = Quaternion.Euler(0f, 0f, spriteAngle);
        lineRenderer.transform.localScale  = new Vector3(targetScale, 0f, 1f);
        lineRenderer.transform.position   = new Vector3(startPos.x, startPos.y, posZ);
        lineRenderer.enabled = true;

        AudioManager.Instance?.PlayLineDrawn();

        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / animDuration));
            var pos = startPos + (Vector3)((Vector2)dir * (t * 0.5f));
            lineRenderer.transform.position   = new Vector3(pos.x, pos.y, posZ);
            lineRenderer.transform.localScale  = new Vector3(targetScale, t * targetScale, 1f);
            yield return null;
        }

        lineRenderer.transform.position   = new Vector3(center.x, center.y, posZ);
        lineRenderer.transform.localScale  = new Vector3(targetScale, targetScale, 1f);
    }
}
