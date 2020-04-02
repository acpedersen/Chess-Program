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
    public class Knight : AbstractPiece
    {
        ///Variables
        private static readonly int VALUE = 3;

        ///Constructor
        public Knight(ColorEnum color, Location location)
            : base(color, location)
        {
            whiteVersion = Chess_Program.Properties.Resources.WhiteKnight;
            blackVersion = Chess_Program.Properties.Resources.BlackKnight;
        }
        public Knight(AbstractPiece piece)
            : base(piece)
        {
            whiteVersion = Chess_Program.Properties.Resources.WhiteKnight;
            blackVersion = Chess_Program.Properties.Resources.BlackKnight;
        }

        ///Methods
        //Overidden with Knight piece logic
        public override MoveList getPieceMoves(GameBoard board, Location startLocation, ColorEnum color)
        {
            MoveList moves = new MoveList();

            for (int i = 0; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j += 2)
                {
                    for (int k = -1; k <= 1; k += 2)
                    {
                        int xShift = (1 + i) * j, yShift = (2 - i) * k;
                        Location poss = startLocation.Shift(xShift, yShift);
                        if (board.locationOnBoard(poss))
                        {
                            if(!board.pieceAtLocation(poss))
                            {
                                Move newMove = new Move(startLocation, poss);
                                newMove.AddNeededEmtpyLocations(poss.Copy());
                                moves.Add(newMove);
                            }
                            else if(board.getPiece(poss).color != color)
                            {
                                Move attack = new Move(startLocation, poss);
                                attack.AddAttackedLocation(poss);

                                moves.Add(attack);
                            }
                        }
                    }
                }
            }

            return moves;
        }
    }
}
