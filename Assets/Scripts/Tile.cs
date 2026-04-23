using UnityEngine;
using UnityEngine.UI;

public enum TileState { Empty, X, O }

[RequireComponent(typeof(Button))]
public class Tile : MonoBehaviour
{
    [SerializeField] private Image markerImage;
    [SerializeField] private Sprite xSprite;
    [SerializeField] private Sprite oSprite;

    public Vector2Int Coordinates { get; private set; }
    public TileState State { get; private set; }

    private TicTacToeController _controller;

    public void Initialize(Vector2Int coordinates, TicTacToeController controller)
    {
        Coordinates = coordinates;
        _controller = controller;
        GetComponent<Button>().onClick.AddListener(OnClicked);
        Reset();
    }

    public void SetTheme(Sprite x, Sprite o)
    {
        xSprite = x;
        oSprite = o;
    }

    public void SetMark(TileState mark)
    {
        State = mark;
        markerImage.sprite = mark == TileState.X ? xSprite : oSprite;
        markerImage.enabled = true;
        AudioManager.Instance?.PlayPop();
    }

    public void Reset()
    {
        State = TileState.Empty;
        markerImage.enabled = false;
    }

    private void OnClicked() => _controller.OnTileClicked(this);
}
