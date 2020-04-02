/*
 * Derived from AbstractPiece
 * Only used to create unique move logic for getPieceMoves
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_Program
{
    public class King : AbstractPiece
    {
        ///Variables
        private static readonly int VALUE = 1000;

        ///Constructor
        public King(ColorEnum color, Location location)
            : base(color, location)
        {
            whiteVersion = Chess_Program.Properties.Resources.WhiteKing;
            blackVersion = Chess_Program.Properties.Resources.BlackKing;
        }
        public King(AbstractPiece piece)
            : base(piece)
        {
            whiteVersion = Chess_Program.Properties.Resources.WhiteKing;
            blackVersion = Chess_Program.Properties.Resources.BlackKing;
        }

        ///Methods
        //Overidden with King piece logic
        //Does not include that a king cannot move to an attacked square, that is added in FilteringMoves
        public override MoveList getPieceMoves(GameBoard board, Location startLocation, ColorEnum color)
        {
            MoveList moves = new MoveList();

            for (int i = 0; i < 8; i++)
            {
                Location possible = startLocation.Shift(Location.directionIndex(i));
                if (board.locationOnBoard(possible))
                {// Cannot include the idea that a king cant move to an attack space as it links to the king and creates an infinite loop
                    //Needs to be filtered on the outside of the gameboard
                    if (!board.pieceAtLocation(possible))
                    {
                        Move newMove = new Move(startLocation, possible);
                        newMove.AddNeededEmtpyLocations(possible.Copy());
                        moves.Add(newMove);
                    }
                    else if(board.getPiece(possible).color != color)
                    {
                        Move attack = new Move(startLocation, possible);
                        attack.AddAttackedLocation(possible);
                        moves.Add(attack);
                    }
                }
            }
            
            return moves;
        }
    }
}
