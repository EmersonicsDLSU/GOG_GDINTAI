using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Board
{
    public enum Player { PlayerOne = 0, PlayerTwo }
    public static bool isWinner(BoardState boardState, int player)
    {
        Position posFlag = new Position();

        if ((int)Player.PlayerOne == player)
        {
            posFlag = boardState.findPositionAt((int)Player.PlayerOne, 1);
            if (posFlag.Row == 0)
                return true;
        }
        else if ((int)Player.PlayerTwo == player)
        {
            posFlag = boardState.findPositionAt((int)Player.PlayerTwo, 1);
            if (posFlag.Row == 7)
                return true;
        }
        return false;
    }

    public static BoardState next_state(BoardState boardState)
    {
        while(true)
        {
            Random rand = new Random();
            int randNum = rand.Next(0, boardState.getPositionList(boardState.WhoseTurnToMove).Count);
            Position position = new Position();
            Position.copyPosition(boardState.getPositionAt(boardState.WhoseTurnToMove, randNum), position);
            int row = position.Row;
            int column = position.Column;

            BoardState expandedBoardState = new BoardState();

            int randMove = rand.Next(0, 4);

            //0-up, 1-down, 2-left, 3-right
            switch (randMove)
            {
                case 0:
                    //going up
                    if (checkMoves(boardState, row - 1, column, boardState.WhoseTurnToMove))
                    {
                        expandedBoardState.moveUnit = 0;
                        return expandedBoardState = generateNewChildStateForMove(boardState, position, position.PieceID, position.PieceValue, row - 1, column, boardState.WhoseTurnToMove, position.RealValue);
                    }
                    break;
                case 1:
                    //going down
                    if (checkMoves(boardState, row + 1, column, boardState.WhoseTurnToMove))
                    {
                        expandedBoardState.moveUnit = 1;
                        return expandedBoardState = generateNewChildStateForMove(boardState, position, position.PieceID, position.PieceValue, row + 1, column, boardState.WhoseTurnToMove, position.RealValue);
                    }
                    break;
                case 2:
                    //going left
                    if (checkMoves(boardState, row, column - 1, boardState.WhoseTurnToMove))
                    {
                        expandedBoardState.moveUnit = 2;
                        return expandedBoardState = generateNewChildStateForMove(boardState, position, position.PieceID, position.PieceValue, row, column - 1, boardState.WhoseTurnToMove, position.RealValue);
                    }
                    break;
                case 3:
                    //going right
                    if (checkMoves(boardState, row, column + 1, boardState.WhoseTurnToMove))
                    {
                        expandedBoardState.moveUnit = 3;
                        return expandedBoardState = generateNewChildStateForMove(boardState, position, position.PieceID, position.PieceValue, row, column + 1, boardState.WhoseTurnToMove, position.RealValue);
                    }
                    break;
            }
        }

    }

    private static bool checkMoves(BoardState boardState, int rowIndex, int colIndex, int player)
    {
        if (rowIndex >= BoardState.MAX_ROW || rowIndex < 0)
        {
            return false;
        }
        if (colIndex >= BoardState.MAX_COL || colIndex < 0)
        {
            return false;
        }
        //if there's the same player piece
        if (boardState.isThereAPosition(player, rowIndex, colIndex))
        {
            return false;
        }
        return true;
    }

    private static BoardState generateNewChildStateForMove(BoardState boardState, Position position, int pieceId, int pieceValue, int newRow, int newCol, int player, int realV)
    {
        BoardState newBoardState = new BoardState();
        BoardState.copyBoardState(boardState, newBoardState);

        Position newPosition = new Position();
        int opposite = player == 0 ? 1 : 0;

        newBoardState.WhoseTurnToMove = opposite;

        newPosition.PieceID = pieceId;
        newPosition.PieceValue = pieceValue;
        newPosition.RealValue = realV;
        newPosition.Row = newRow;
        newPosition.Column = newCol;
        newPosition.PlayerIndex = player;
        Position.copyPosition(position, newBoardState.sourcePosition);
        newBoardState.deletePosition(position, player);
        newBoardState.addFromPosList(player, newPosition);

        Position traversePiece = new Position();
        Position.copyPosition(boardState.getPositionAtPlace(newRow, newCol), traversePiece);

        Position.copyPosition(traversePiece, newBoardState.positionEaten);
        Position.copyPosition(newPosition, newBoardState.movePosition);



        if (newPosition.RealValue > traversePiece.RealValue && traversePiece.RealValue > 0)
        {
            newBoardState.eatenState = 0;
        }
        else if (newPosition.RealValue < traversePiece.RealValue && traversePiece.RealValue > 0)
        {
            newBoardState.eatenState = 1;
        }
        else if (newPosition.RealValue == traversePiece.RealValue && traversePiece.RealValue > 0)
        {
            newBoardState.eatenState = 2;
        }

        return newBoardState;
    }
}
