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
    [SerializeField] private TMP_Text turnCountText;

    [Header("Scoreboard (Portrait)")]
    [SerializeField] private TMP_Text portraitXWinsText;
    [SerializeField] private TMP_Text portraitOWinsText;
    [SerializeField] private TMP_Text portraitCurrentTurnText;
    [SerializeField] private TMP_Text portraitTurnCountText;

    [Header("Win Overlay")]
    [SerializeField] private GameObject winOverlay;
    [SerializeField] private TMP_Text winResultText;
    [SerializeField] private Button playAgainButton;

    [Header("Win Overlay (Portrait)")]
    [SerializeField] private GameObject portraitWinOverlay;
    [SerializeField] private TMP_Text portraitWinResultText;
    [SerializeField] private Button portraitPlayAgainButton;

    [Header("Win Line")]
    [SerializeField] private WinLineAnimator winLineAnimator;

    private Dictionary<Vector2Int, Tile> _tileMap;
    private List<List<Vector2Int>> _winLines;
    private GameState _state;
    private int _xWins;
    private int _oWins;
    private int _moveCount;
    private float _matchStartTime;

    private void SetXWins(string v)         { playerXWinsText.text = v;     if (portraitXWinsText)         portraitXWinsText.text = v; }
    private void SetOWins(string v)         { playerOWinsText.text = v;     if (portraitOWinsText)         portraitOWinsText.text = v; }
    private void SetCurrentTurn(string v)   { currentTurnText.text = v;     if (portraitCurrentTurnText)   portraitCurrentTurnText.text = v; }
    private void SetTurnCount(string v)     { turnCountText.text = v;       if (portraitTurnCountText)     portraitTurnCountText.text = v; }
    private void SetWinOverlayActive(bool v){ winOverlay.SetActive(v);      if (portraitWinOverlay != null) portraitWinOverlay.SetActive(v); }
    private void SetWinResultText(string v) { winResultText.text = v;       if (portraitWinResultText != null) portraitWinResultText.text = v; }

    private void Awake()
    {
        playAgainButton.onClick.AddListener(PlayAgain);
        if (portraitPlayAgainButton != null) portraitPlayAgainButton.onClick.AddListener(PlayAgain);
        SetXWins("0");
        SetOWins("0");
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
        UpdateTurnDisplay();
        SetWinOverlayActive(false);
        winLineAnimator.Hide();
        foreach (var tile in tiles) tile.Reset();
        TransitionTo(GameState.PlayerXTurn);
    }

    private void TransitionTo(GameState newState)
    {
        _state = newState;
        SetCurrentTurn(newState == GameState.PlayerXTurn ? "PLAYING: X" : "PLAYING: O");
    }

    public void OnTileClicked(Tile tile)
    {
        if (_state == GameState.GameOver || tile.State != TileState.Empty) return;
        var mark = _state == GameState.PlayerXTurn ? TileState.X : TileState.O;
        tile.SetMark(mark);
        _moveCount++;
        UpdateTurnDisplay();
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
            SetXWins(_xWins.ToString());
            SetWinResultText("PLAYER X WON");
        }
        else
        {
            _oWins++;
            SetOWins(_oWins.ToString());
            SetWinResultText("PLAYER O WON");
        }
        GameStats.RecordGame(winner, duration);
        StartCoroutine(WinSequence(winLine, winner));
    }

    private IEnumerator WinSequence(List<Vector2Int> winLine, TileState winner)
    {
        yield return StartCoroutine(winLineAnimator.Animate(winLine, winner, _tileMap));
        AudioManager.Instance?.PlayWin();
        SetWinOverlayActive(true);
    }

    private void HandleDraw()
    {
        _state = GameState.GameOver;
        float duration = Time.time - _matchStartTime;
        SetWinResultText("DRAW");
        GameStats.RecordGame(TileState.Empty, duration);
        SetWinOverlayActive(true);
    }

    private void UpdateTurnDisplay()
    {
        SetTurnCount($"TURN : {Mathf.Min(_moveCount + 1, tiles.Count)}");
    }

    private void PlayAgain() => BeginMatch();
}
