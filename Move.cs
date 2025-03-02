using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Move
    {
        public int StartRow { get; }
        public int StartCol { get; }
        public int TargetRow { get; }
        public int TargetCol { get; }
        public int Piece { get; }
        public Move(int startRow, int startCol, int targetRow, int targetCol, int piece)
        {
            StartRow = startRow;
            StartCol = startCol;
            TargetRow = targetRow;
            TargetCol = targetCol;
            Piece = piece;
        }
    }
}
