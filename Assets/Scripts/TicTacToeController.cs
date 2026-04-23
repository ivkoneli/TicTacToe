using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameState { PlayerXTurn, PlayerOTurn, GameOver }

public class TicTacToeController : MonoBehaviour
{
    [Header("Board")]
    [SerializeField] private List<Tile> tiles;
    [SerializeField] private int boardSize = 3;

    [Header("Theme Sprites")]
    [SerializeField] private Sprite[] xThemeSprites; // 0=Red 1=Green 2=Yellow
    [SerializeField] private Sprite[] oThemeSprites;

    [Header("Scoreboard")]
    [SerializeField] private TMP_Text playerXWinsText;
    [SerializeField] private TMP_Text playerOWinsText;
    [SerializeField] private TMP_Text currentTurnText;

    [Header("Win Overlay")]
    [SerializeField] private GameObject winOverlay;
    [SerializeField] private TMP_Text winResultText;
    [SerializeField] private Button playAgainButton;

    [Header("Win Line")]
    [SerializeField] private WinLineAnimator winLineAnimator;

    private Dictionary<Vector2Int, Tile> _tileMap;
    private List<List<Vector2Int>> _winLines;
    private GameState _state;
    private int _xWins;
    private int _oWins;
    private int _moveCount;
    private float _matchStartTime;

    private void Awake()
    {
        playAgainButton.onClick.AddListener(PlayAgain);
        playerXWinsText.text = "0";
        playerOWinsText.text = "0";
        InitializeBoard();
        ApplyTheme();
        BeginMatch();
    }

    private void InitializeBoard()
    {
        _tileMap = new Dictionary<Vector2Int, Tile>();
        for (int i = 0; i < tiles.Count; i++)
        {
            var coord = new Vector2Int(i % boardSize, i / boardSize);
            tiles[i].Initialize(coord, this);
            _tileMap[coord] = tiles[i];
        }
        _winLines = BuildWinLines(boardSize);
    }

    private void ApplyTheme()
    {
        if (xThemeSprites == null || xThemeSprites.Length == 0) return;
        int theme = Mathf.Clamp(PlayerPrefs.GetInt("SelectedTheme", 0), 0, xThemeSprites.Length - 1);
        var x = xThemeSprites[theme];
        var o = oThemeSprites[theme];
        foreach (var tile in tiles)
            tile.SetTheme(x, o);
    }

    // Generates all rows, columns, and diagonals for an NxN board.
    private static List<List<Vector2Int>> BuildWinLines(int n)
    {
        var lines = new List<List<Vector2Int>>();
        for (int r = 0; r < n; r++)
        {
            var row = new List<Vector2Int>();
            for (int c = 0; c < n; c++) row.Add(new Vector2Int(c, r));
            lines.Add(row);
        }
        for (int c = 0; c < n; c++)
        {
            var col = new List<Vector2Int>();
            for (int r = 0; r < n; r++) col.Add(new Vector2Int(c, r));
            lines.Add(col);
        }
        var mainDiag = new List<Vector2Int>();
        for (int i = 0; i < n; i++) mainDiag.Add(new Vector2Int(i, i));
        lines.Add(mainDiag);
        var antiDiag = new List<Vector2Int>();
        for (int i = 0; i < n; i++) antiDiag.Add(new Vector2Int(n - 1 - i, i));
        lines.Add(antiDiag);
        return lines;
    }

    private void BeginMatch()
    {
        _moveCount = 0;
        _matchStartTime = Time.time;
        winOverlay.SetActive(false);
        winLineAnimator.Hide();
        foreach (var tile in tiles) tile.Reset();
        TransitionTo(GameState.PlayerXTurn);
    }

    private void TransitionTo(GameState newState)
    {
        _state = newState;
        currentTurnText.text = newState == GameState.PlayerXTurn ? "PLAYING: X" : "PLAYING: O";
    }

    public void OnTileClicked(Tile tile)
    {
        if (_state == GameState.GameOver || tile.State != TileState.Empty) return;
        var mark = _state == GameState.PlayerXTurn ? TileState.X : TileState.O;
        tile.SetMark(mark);
        _moveCount++;
        var winLine = GetWinLine(tile.Coordinates, mark);
        if (winLine != null)
            HandleWin(mark, winLine);
        else if (_moveCount == tiles.Count)
            HandleDraw();
        else
            TransitionTo(_state == GameState.PlayerXTurn ? GameState.PlayerOTurn : GameState.PlayerXTurn);
    }

    private List<Vector2Int> GetWinLine(Vector2Int lastPlaced, TileState mark)
    {
        foreach (var line in _winLines)
        {
            if (!line.Contains(lastPlaced)) continue;
            bool allMatch = true;
            foreach (var coord in line)
            {
                if (!_tileMap.TryGetValue(coord, out var t) || t.State != mark)
                { allMatch = false; break; }
            }
            if (allMatch) return line;
        }
        return null;
    }

    private void HandleWin(TileState winner, List<Vector2Int> winLine)
    {
        _state = GameState.GameOver;
        float duration = Time.time - _matchStartTime;
        if (winner == TileState.X)
        {
            _xWins++;
            playerXWinsText.text = _xWins.ToString();
            winResultText.text = "PLAYER X WON";
        }
        else
        {
            _oWins++;
            playerOWinsText.text = _oWins.ToString();
            winResultText.text = "PLAYER O WON";
        }
        GameStats.RecordGame(winner, duration);
        StartCoroutine(WinSequence(winLine, winner));
    }

    private IEnumerator WinSequence(List<Vector2Int> winLine, TileState winner)
    {
        yield return StartCoroutine(winLineAnimator.Animate(winLine, winner, _tileMap));
        AudioManager.Instance?.PlayWin();
        winOverlay.SetActive(true);
    }

    private void HandleDraw()
    {
        _state = GameState.GameOver;
        float duration = Time.time - _matchStartTime;
        winResultText.text = "DRAW";
        GameStats.RecordGame(TileState.Empty, duration);
        winOverlay.SetActive(true);
    }

    private void PlayAgain() => BeginMatch();
}
