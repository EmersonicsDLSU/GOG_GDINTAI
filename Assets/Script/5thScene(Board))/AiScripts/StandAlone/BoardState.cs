using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardState 
{
    public enum Player { PlayerOne = 0, PlayerTwo}

    public static string TAG = "BoardState";
    public static int MAX_ROW = 8;
    public static int MAX_COL = 9;

    public Dictionary<int, BoardState> plays;
    public Dictionary<int, BoardState> wins;

    private float MAX_INFINITY = 999999.0f;

    private List<Position> playerOnePosition = new List<Position>();
    private List<Position> playerTwoPosition = new List<Position>();

    private int whoseTurnToMove; //0 - player1, 1 - player2

    private List<double> evalScoreList = new List<double>();
    private double evalScore;

    public Position movePosition = new Position();
    public Position sourcePosition = new Position();
    public int eatenState = -1; //0 - opposing piece Eaten, 1 - own piece eaten, 2 - both piece dead

    public int moveUnit = 0; //(0-left, 1-right, 2-up, 3-down)

    public Position positionEaten = new Position();

    private int winCount = 0;


    public int MoveUnit
    {
        get { return moveUnit; }
        set { moveUnit = value; }
    }
    public double EvalScore
    {
        get { return evalScore; }
        set { evalScore = value; }
    }

    public static void copyBoardState(BoardState bs1, BoardState copy)
    {
        foreach (var item in bs1.getPositionList(0))
        {
            copy.getPositionList(0).Add(item);
        }
        foreach (var item in bs1.getPositionList(1))
        {
            copy.getPositionList(1).Add(item);
        }
        copy.WhoseTurnToMove = bs1.whoseTurnToMove;
        copy.MoveUnit = bs1.MoveUnit;
        copy.EvalScore = bs1.EvalScore;
        Position.copyPosition(bs1.positionEaten, copy.positionEaten);
        Position.copyPosition(bs1.movePosition, copy.movePosition);
        Position.copyPosition(bs1.sourcePosition, copy.sourcePosition);
    }
    public Position findPositionByID(int player, int pieceID)
    {
        Position pos = new Position();
        if (player == 1)
        {
            foreach (var item in playerTwoPosition)
            {
                if (item.PieceID == pieceID)
                {
                    Position.copyPosition(item, pos);
                    return pos;
                }
            }
        }
        if (player == 0)
        {
            foreach (var item in playerOnePosition)
            {
                if (item.PieceID == pieceID)
                {
                    Position.copyPosition(item, pos);
                    return pos;
                }
            }
        }
        return pos;
    }

    public Position findPositionAt(int player, int pieceValue)
    {
        Position pos = new Position();
        if(player == 1)
        {
            foreach (var item in playerTwoPosition)
            {
                if(item.RealValue == pieceValue)
                {
                    Position.copyPosition(item, pos);
                    return pos;
                }
            }
        }
        if (player == 0)
        {
            foreach (var item in playerOnePosition)
            {
                if (item.RealValue == pieceValue)
                {
                    Position.copyPosition(item, pos);
                    return pos;
                }
            }
        }
        return pos;
    }

    public BoardState addEvalScoreAndGetBest()
    {
        //Debug.Log("size: " + this.playerTwoPosition.Count);

        BoardState temp = new BoardState();
        copyBoardState(this, temp);

        List<BoardState> newBoards = AIAgent.exploreNextMoves(temp, whoseTurnToMove);

        for (int i = 0; i < newBoards.Count; i++)
        {
            BoardState temp1 = new BoardState();
            copyBoardState(newBoards[i], temp1);
            BoardEvaluator boardEval = new BoardEvaluator(temp1);
            this.evalScoreList.Add(boardEval.evaluate());
        }

        //Debug.Log("BOARDSIZE: " + newBoards.Count);

        double bestMove = -MAX_INFINITY;
        int index = 0;

        for (int i = 0; i < evalScoreList.Count; i++)
        {
            if (evalScoreList[i] > bestMove)
            {
                bestMove = evalScoreList[i];
                index = i;
            }
        }

        foreach (var item in evalScoreList)
        {
            //Debug.LogError("EvalScore: " + item);
        }

        return newBoards[index]; //return the best move
    }

    private void deletePositions(BoardState boardState)
    {
        if (boardState.eatenState == 0)
        {
            boardState.deletePosition(boardState.positionEaten, (int)Player.PlayerOne);
        }
        else if (boardState.eatenState == 1)
        {
            boardState.deletePosition(boardState.movePosition, (int)Player.PlayerTwo);
        }
        else if (boardState.eatenState == 2)
        {
            boardState.deletePosition(boardState.positionEaten, (int)Player.PlayerOne);
            boardState.deletePosition(boardState.movePosition, (int)Player.PlayerTwo);
        }
    }

    public void deletePosition(Position pos, int player)
    {
        if (player == (int)Player.PlayerOne)
        {
            foreach (var item in this.getPositionList(0))
            {
                if(item.Row == pos.Row && item.Column == pos.Column)
                {
                    this.getPositionList(0).Remove(item);
                    break;
                }
            }
        }
        else
        {
            foreach (var item in this.getPositionList(1))
            {
                if (item.Row == pos.Row && item.Column == pos.Column)
                {
                    this.getPositionList(1).Remove(item);
                    break;
                }
            }
        }
    }

    public void deletePosition(int pieceId, int pieceValue, int player)
    {
        if (player == (int)Player.PlayerOne)
        {
            foreach (var pos in playerOnePosition)
            {
                if (pos.PieceID == pieceId && pos.PieceValue == pieceValue)
                {
                    playerOnePosition.Remove(pos);
                    return;
                }
            }
        }
        else
        {
            foreach (var pos in playerTwoPosition)
            {
                if (pos.PieceID == pieceId && pos.PieceValue == pieceValue)
                {
                    playerTwoPosition.Remove(pos);
                    return;
                }
            }
        }
    }

    public bool isThereAPosition(int player, int row, int col)
    {
        if (player == (int)Player.PlayerOne)
        {
            foreach (var pos in playerOnePosition)
            {
                if(pos.Row == row && pos.Column == col)
                {
                    return true;
                }
            }
        }
        else
        {
            foreach (var pos in playerTwoPosition)
            {
                if (pos.Row == row && pos.Column == col)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public int WhoseTurnToMove
    {
        get { return whoseTurnToMove; }
        set { whoseTurnToMove = value; }
    }

    public Position getPositionAtPlace(int row, int col)
    {
        foreach (var pos in playerOnePosition)
        {
            if (pos.Row == row && pos.Column == col)
            {
                Position newPos = new Position();
                Position.copyPosition(pos, newPos);
                return newPos;
            }
        }
        foreach (var pos in playerTwoPosition)
        {
            if (pos.Row == row && pos.Column == col)
            {
                Position newPos = new Position();
                Position.copyPosition(pos, newPos);
                return newPos;
            }
        }
        return null;
    }

    public Position getPositionAt(int player, int i)
    {
        if (player == (int)Player.PlayerOne)
        {
            Position newPos = new Position();
            Position.copyPosition(playerOnePosition[i], newPos);
            return newPos;
        }
        else
        {
            Position newPos = new Position();
            Position.copyPosition(playerTwoPosition[i], newPos);
            return newPos;
        }
    }
    public List<Position> getPositionList(int player)
    {
        if (player == (int)Player.PlayerOne)
        {
            return playerOnePosition;
        }
        else
        {
            return playerTwoPosition;
        }
    }

    public int getPositionSize(int player)
    {
        if (player == (int)Player.PlayerOne)
        {
            return playerOnePosition.Count;
        }
        else
        {
            return playerTwoPosition.Count;
        }
    }

    public void addFromPosList(int player, Position position)
    {
        if(player == (int)Player.PlayerOne)
        {
            playerOnePosition.Add(position);
        }
        else
        {
            playerTwoPosition.Add(position);
        }
    }
    public void addFromPosList(int player, List<Position> positions)
    {
        if (player == (int)Player.PlayerOne)
        {
            foreach (var position in positions)
            {
                playerOnePosition.Add(position);
            }
        }
        else
        {
            foreach (var position in positions)
            {
                playerOnePosition.Add(position);
            }
        }
    }
    public void addFromScoList(double eval)
    {
        evalScoreList.Add(eval);
    }
    public void addFromScoList(List<double> evals)
    {
        foreach (var eval in evals)
        {
            evalScoreList.Add(eval);
        }
    }
    public void incWinCount()
    {
        winCount++;
    }
}
