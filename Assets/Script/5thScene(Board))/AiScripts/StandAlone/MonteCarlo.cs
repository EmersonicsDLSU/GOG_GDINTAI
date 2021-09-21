using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonteCarlo
{
    public enum Player { PlayerOne = 0, PlayerTwo }
    public static void run_simulation(BoardState board, int maxMoves)
    {
        BoardState mainState = new BoardState();
        BoardState.copyBoardState(board, mainState);

        BoardState state = new BoardState();
        BoardState.copyBoardState(board, state);


        AdjacencyGraph visited_states = new AdjacencyGraph();
        List<BoardState> states_copy = new List<BoardState>();

        int player = board.WhoseTurnToMove;

        bool expand = true;
        bool winner = false;

        for (int i = 0; i < maxMoves; i++)
        {
            BoardState.copyBoardState(Board.next_state(state), state);
            states_copy.Add(state);

            if (expand && !containsState(player, state))
            {
                expand = false;
                mainState.plays[player] = null;
                mainState.wins[player] = null;
            }

            visited_states.addVertex(player, state);
            player = state.WhoseTurnToMove;

            if (isWinner(state, player))
                break;
        }

        /*for (int i = 0; i < length; i++)
        {

        }*/
    }
    private static bool containsState(int whoseTurnToMove, BoardState boardState)
    {
        //if(boardState.plays.A[whoseTurnToMove].Contains(boardState))
            return true;
        return false;
    }
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
}

