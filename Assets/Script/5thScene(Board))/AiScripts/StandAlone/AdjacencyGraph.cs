using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Int32 = System.Int32;

public class AdjacencyGraph
{
	public enum Player { PlayerOne = 0, PlayerTwo }

	//list of all the possible moves of each remaining gameObject
	public Dictionary<int, List<BoardState>> A = new Dictionary<int, List<BoardState>>();

	public void addVertex(int v, BoardState board)
	{
		if (!A.ContainsKey(v))
			A[v] = new List<BoardState>();
		A[v].Add(board);
	}

	//overload
	public void addVertex(int v, List<BoardState> boards)
	{
		if (!A.ContainsKey(v))
			A[v] = new List<BoardState>();
		for (int i = 0; i < boards.Count; i++)
			A[v].Add(boards[i]);
	}

	public List<BoardState> possibleMoves(int v)
	{
		if (A.ContainsKey(v))
			return A[v];
		else
			return null;
	}
	public void clearAllList( )
	{
		A.Clear();
	}

	public static double[] minMax(BoardState boardState, int depth, double alpha, double beta, int playerType)
	{
		//|| d >= (BoardState.COL * BoardState.ROW) - turns
		if (depth == 0)
		{
			return new double[2] { boardState.EvalScore , 0};
		}

		if (playerType == (int)Player.PlayerTwo)
		{
			double[] alpBet = { Int32.MinValue , 0};
			/*if (winningMove(ref board, PLAYER))
			{
				return alpBet;
			}*/
			//own
			BoardState temp = new BoardState();
			BoardState.copyBoardState(boardState, temp);
			List<BoardState> newBoards = AIAgent.exploreNextMoves(temp, boardState.WhoseTurnToMove);
			for (int i = 0; i < newBoards.Count; i++)
			{
				BoardState temp1 = new BoardState();
				BoardState.copyBoardState(newBoards[i], temp1);
				BoardEvaluator boardEval = new BoardEvaluator(temp1);
				newBoards[i].EvalScore = boardEval.evaluate();
			}

			for (int c = 0; c < newBoards.Count; c++)
			{ // for each possible move
				BoardState copyBoard = new BoardState();
				BoardState.copyBoardState(newBoards[c], copyBoard);
				double score = minMax(copyBoard, depth - 1, alpha, beta, (int)Player.PlayerOne)[0]; // find move based on that new board state
				if(score > alpBet[0])
                {
					alpBet = new double[2] { score, (double) c};
				}
				alpha = alpha > alpBet[0] ? alpha : alpBet[0];
				if (beta <= alpha) { break; }
			}
			Debug.LogError(alpBet[0]);
			return alpBet; // return best possible move
		}
		else
		{
			double[] alpBet = { Int32.MaxValue, 0 };

			//own
			BoardState temp = new BoardState();
			BoardState.copyBoardState(boardState, temp);
			List<BoardState> newBoards = AIAgent.exploreNextMoves(temp, boardState.WhoseTurnToMove);
			for (int i = 0; i < newBoards.Count; i++)
			{
				BoardState temp1 = new BoardState();
				BoardState.copyBoardState(newBoards[i], temp1);
				BoardEvaluator boardEval = new BoardEvaluator(temp1);
				newBoards[i].EvalScore = boardEval.evaluate();
			}

			for (int c = 0; c < newBoards.Count; c++)
			{ // for each possible move
				BoardState copyBoard = new BoardState();
				BoardState.copyBoardState(newBoards[c], copyBoard);
				double score = minMax(copyBoard, depth - 1, alpha, beta, (int)Player.PlayerTwo)[0]; // find move based on that new board state
				if (score < alpBet[0])
				{
					alpBet = new double[2] { score, (double)c };
				}
				beta = beta > alpBet[0] ? beta : alpBet[0];
				if (beta <= alpha) { break; }
			}
			return alpBet; // return best possible move
		}
	}
}
