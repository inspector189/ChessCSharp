using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class AIPlayer
    {
        private int maxDepth;
        private int playerColor;
        public AIPlayer(int depth, int color)
        {
            maxDepth = depth;
            playerColor = color;
        }
        public Move GetBestMove(Chessboard board)
        {
            int bestValue = int.MinValue;
            Move bestMove = null;

            var possibleMoves = board.GetAllPossibleMoves(playerColor);
            if (!possibleMoves.Any())
            {
                return null;
            }

            foreach (var move in possibleMoves)
            {
                Chessboard tempBoard = board.Clone();     
                tempBoard.MakeMove(move);
                int moveValue = Minimax(tempBoard, maxDepth, int.MinValue, int.MaxValue, false);

                if (moveValue > bestValue)
                {
                    bestValue = moveValue;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        private int Minimax(Chessboard board, int depth, int alpha, int beta, bool isMaximizing)
        {
            if(depth == 0)
            {
                return EvaluateBoard(board);
            }
            if(isMaximizing)
            {
                int maxEval = int.MinValue;
                foreach(var move in board.GetAllPossibleMoves(playerColor))
                {
                    Chessboard tempBoard = board.Clone();
                    tempBoard.MakeMove(move);
                    int eval = Minimax(tempBoard, depth - 1, alpha, beta, false);
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if(beta <= alpha)
                    {
                        break;
                    }
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach(var move in board.GetAllPossibleMoves(-playerColor))
                {
                    Chessboard tempBoard = board.Clone();
                    tempBoard.MakeMove(move);
                    int eval = Minimax(tempBoard, depth - 1, alpha, beta, true);
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if(beta <= alpha)
                    {
                        break;
                    }
                }
                return minEval;
            }
        }
        private int EvaluateBoard(Chessboard board)
        {
            int whiteScore = 0, blackScore = 0;

            foreach (var piece in board.pieces)
            {
                int pieceValue = Piece.GetPieceValue(piece.number);
                if ((piece.number & Piece.White) != 0)
                    whiteScore += pieceValue;
                else
                    blackScore += pieceValue;
            }

            int evaluation = whiteScore - blackScore;

            return evaluation * (playerColor == Piece.White ? 1 : -1);
        }

    }
}
