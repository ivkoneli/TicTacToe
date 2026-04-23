using UnityEngine;

public static class GameStats
{
    static int   GetI(string k, int   d) => PlayerPrefs.GetInt(k, d);
    static float GetF(string k, float d) => PlayerPrefs.GetFloat(k, d);
    static void  SetI(string k, int   v) { PlayerPrefs.SetInt(k, v);   PlayerPrefs.Save(); }
    static void  SetF(string k, float v) { PlayerPrefs.SetFloat(k, v); PlayerPrefs.Save(); }

    public static int   TotalGames    { get => GetI("TotalGames",    0);  set => SetI("TotalGames",    value); }
    public static int   Player1Wins   { get => GetI("Player1Wins",   0);  set => SetI("Player1Wins",   value); }
    public static int   Player2Wins   { get => GetI("Player2Wins",   0);  set => SetI("Player2Wins",   value); }
    public static int   Draws         { get => GetI("Draws",         0);  set => SetI("Draws",         value); }
    public static float TotalDuration { get => GetF("TotalDuration", 0f); set => SetF("TotalDuration", value); }

    public static float AverageDuration => TotalGames > 0 ? TotalDuration / TotalGames : 0f;

    public static void RecordGame(TileState result, float duration)
    {
        TotalGames++;
        TotalDuration += duration;
        if      (result == TileState.X) Player1Wins++;
        else if (result == TileState.O) Player2Wins++;
        else                            Draws++;
    }
}
