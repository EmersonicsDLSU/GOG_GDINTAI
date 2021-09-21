using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position
{
    public enum Player { PlayerOne = 0, PlayerTwo }

    public static string TAG = "Position";

    /*{ "Flag", "Spy", "Private", "Sergeant", "2nd Lieutenant", "1st Lieutenant", 
        "Captain", "Major", "Lieutenant Colonel", "Colonel", "One-Star General", 
        "Two-Star General", "Three-Star General", "Four-Star General", "Five-Star General"} 1 - 15 piece index */
    private int pieceID = 0; //1 - 21 
    private int realValue = 0; //piece index - 1-15 flag,spy, .. 5 general
    private int pieceValue = 0; //piece value 1 - 15; 15-spy, 1-flag
    private int row = -1;
    private int column = -1;
    private bool isVisible = false;
    private int playerIndex = 0; //0 - player 1, 1 - player 2


    public static bool isTheSamePos(Position pos1, Position pos2)
    {
        if (pos1.Row == pos2.Row && pos1.Column == pos2.Column)
        {
            return true;
        }
        else
            return false;
    }

    public static void copyPosition(Position ps1, Position copy)
    {
        if (ps1 == null)
            return;
        copy.PieceID = ps1.PieceID;
        copy.PieceValue = ps1.PieceValue;
        copy.RealValue = ps1.RealValue;
        copy.Row = ps1.Row;
        copy.Column = ps1.Column;
        copy.PlayerIndex = ps1.PlayerIndex;
    }
    public bool IsVisible
    {
        get { return isVisible; }
        set { isVisible = value; }
    }

    public int PieceID
    {
        get { return pieceID; }
        set { pieceID = value; }
    }
    public int RealValue
    {
        get { return realValue; }
        set { realValue = value; }
    }
    public int PieceValue
    {
        get { return pieceValue; }
        set { pieceValue = value; }
    }
    public int Row
    {
        get { return row; }
        set { row = value; }
    }
    public int Column
    {
        get { return column; }
        set { column = value; }
    }
    public int PlayerIndex
    {
        get { return playerIndex; }
        set { playerIndex = value; }
    }
}
