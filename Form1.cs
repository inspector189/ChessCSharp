using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess
{
    public partial class Form1 : Form
    {
        private Chessboard Board = new Chessboard(Tools.BoardSize, Tools.SquareSize);
        private AIPlayer aiPlayer;
        public Form1()
        {
            InitializeComponent();
            InitializeBoard();
            InitializePieces();
            aiPlayer = new AIPlayer(2, Piece.Black);
        }
        private void InitializeBoard()
        {
            foreach (Panel square in Board.squares)
            {
                Controls.Add(square);
            }
        }
        private void InitializePieces(string layout = Piece.DefaultStartingPosition)
        {
            Board.FillBoard(layout);
            foreach (Piece.PiecePanelPair piece in Board.pieces)
            {
                if (piece.panel != null)
                {
                    Controls.Add(piece.panel);
                    piece.panel.MouseDown += Piece_MouseDown;
                    piece.panel.MouseMove += Piece_MouseMove;
                    piece.panel.MouseUp += Piece_MouseUp;
                    piece.panel.BringToFront();
                }
            }
        }
        private void Piece_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (Panel marker in Board.validMoveMarkers)
            {
                Controls.Remove(marker);
            }
            Board.validMoveMarkers.Clear();

            if (sender is Panel panel)
            {
                Board.selectedPiece = panel;
                Board.originalLocation = panel.Location;
                for (int row = 0; row < Tools.BoardSize; row++)
                {
                    for (int col = 0; col < Tools.BoardSize; col++)
                    {
                        if (Board.pieces[row, col].panel == panel)
                        {
                            int pieceType = Board.pieces[row, col].number;
                            if((pieceType & Board.currentPlayer) == 0)
                            {
                                return;
                            }
                            Board.selectedRow = row;
                            Board.selectedCol = col;
                            List<Point> validMoves = Board.GetValidMovesForPiece(row, col);
                            HighlightValidMoves(row, col);
                            
                            this.Refresh();
                            return;
                        }
                    }
                }
            }
            Board.selectedPiece = null;
        }
        private void Piece_MouseMove(object sender, MouseEventArgs e)
        {
            if (Board.selectedPiece != null && e.Button == MouseButtons.Left)
            {
                Board.selectedPiece.Location = new Point(Board.selectedPiece.Location.X + e.X - (Tools.SquareSize / 2), Board.selectedPiece.Location.Y + e.Y - (Tools.SquareSize / 2));
            }
        }
        private async void Piece_MouseUp(object sender, MouseEventArgs e)
        {
            if (Board.selectedPiece != null)
            {
                int newCol = (int)Math.Round((float)Board.selectedPiece.Location.X / (float)Tools.SquareSize, 0, MidpointRounding.AwayFromZero);
                int newRow = (int)Math.Round((float)Board.selectedPiece.Location.Y / (float)Tools.SquareSize, 0, MidpointRounding.AwayFromZero);
  
                if (newRow >= 0 && newRow < Tools.BoardSize && newCol >= 0 && newCol < Tools.BoardSize)
                {
                    int pieceType = Board.pieces[Board.selectedRow, Board.selectedCol].number;

                    if(Board.IsValidMove(Board.selectedRow, Board.selectedCol, newRow, newCol, pieceType))
                    {
                        if (Board.pieces[newRow, newCol].panel != null)
                        {
                            Controls.Remove(Board.pieces[newRow, newCol].panel);
                            Board.pieces[newRow, newCol] = new Piece.PiecePanelPair();
                        }
                      
                        Board.selectedPiece.Location = new Point(newCol * Tools.SquareSize + (Tools.Margin / 2), newRow * Tools.SquareSize + (Tools.Margin / 2));
                        Board.pieces[newRow, newCol] = Board.pieces[Board.selectedRow, Board.selectedCol];
                        Board.pieces[Board.selectedRow, Board.selectedCol] = new Piece.PiecePanelPair();
                        Board.SwitchPlayer();
           

                        if (Board.currentPlayer == Piece.Black)
                        {
                            await Task.Delay(2000);
                            MakeAIMove();
                        }
                    }
                    else
                    {
                        Board.selectedPiece.Location = Board.originalLocation;
                    }
    
                }
            }
        }
        private void MakeAIMove()
        {
            Move bestMove = aiPlayer.GetBestMove(Board);
            int pieceType = Board.pieces[Board.selectedRow, Board.selectedCol].number;
            if (bestMove == null)
            {
                return;
            }

            if (bestMove.StartRow == bestMove.TargetRow && bestMove.StartCol == bestMove.TargetCol)
            {
                return;
            }

            Panel aiPiece = Board.pieces[bestMove.StartRow, bestMove.StartCol].panel;

            if (aiPiece == null)
            {
                return;
            }

            if (Board.pieces[bestMove.TargetRow, bestMove.TargetCol].panel != null)
            {
                Controls.Remove(Board.pieces[bestMove.TargetRow, bestMove.TargetCol].panel);
                Board.pieces[bestMove.TargetRow, bestMove.TargetCol] = new Piece.PiecePanelPair();
            }

            aiPiece.Location = new Point(bestMove.TargetCol * Tools.SquareSize + (Tools.Margin / 2),
                                         bestMove.TargetRow * Tools.SquareSize + (Tools.Margin / 2));

            Board.pieces[bestMove.TargetRow, bestMove.TargetCol] = new Piece.PiecePanelPair
            {
                panel = aiPiece,
                number = Board.pieces[bestMove.StartRow, bestMove.StartCol].number
            };

            Board.pieces[bestMove.StartRow, bestMove.StartCol] = new Piece.PiecePanelPair();

            aiPiece.BringToFront();
            this.Refresh();

            Board.SwitchPlayer();
        }
        public void HighlightValidMoves(int row, int col)
        {
            List<Point> validMoves = Board.GetValidMovesForPiece(row, col);

            foreach (Point move in validMoves)
            {
                // Tworzymy nowy Panel do narysowania kółka
                if (Board.IsValidMove(row, col, move.X, move.Y, Board.pieces[row, col].number))
                {
                    // Tworzymy nowy Panel do narysowania kółka
                    Panel marker = new Panel
                    {
                        Size = new Size(Tools.SquareSize / 3, Tools.SquareSize / 3), // Małe kółko
                        Location = new Point(move.Y * Tools.SquareSize + Tools.SquareSize / 3, move.X * Tools.SquareSize + Tools.SquareSize / 3),
                        BackColor = Color.Red, // Kolor kółka
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    Controls.Add(marker); // Dodajemy marker do formularza
                    marker.BringToFront();
                    // Pamiętamy o markerze, żeby go później usunąć, gdy klikniemy inną figurę
                    Board.validMoveMarkers.Add(marker);
                }
            }
        }
    }
}
