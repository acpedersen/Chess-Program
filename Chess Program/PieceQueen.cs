﻿/*
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
    public class Queen : AbstractPiece
    {
        ///Variables
        private static readonly int VALUE = 9;

        ///Constructor
        public Queen(ColorEnum color, Location location)
            : base(color, location)
        {
            whiteVersion = Chess_Program.Properties.Resources.WhiteQueen;
            blackVersion = Chess_Program.Properties.Resources.BlackQueen;
        }
        public Queen(AbstractPiece piece)
            : base(piece)
        {
            whiteVersion = Chess_Program.Properties.Resources.WhiteQueen;
            blackVersion = Chess_Program.Properties.Resources.BlackQueen;
        }

        ///Methods
        //Overidden with Queen piece logic
        public override MoveList getPieceMoves(GameBoard board, Location startLocation, ColorEnum color)
        {
            MoveList moves = new MoveList();

            Location local;
            for (int i = 0; i < 8; i++)
            {
                local = new Location(startLocation);


                IList<Location> previousEmpty = new List<Location>();
                while (true)
                {
                    local = local.Shift(Location.directionIndex(i));

                    //Conditional for stopping the array
                    if (!board.locationOnBoard(local))
                    {
                        break;
                    }
                    if (board.pieceAtLocation(local))
                    {
                        if (board.getPiece(local).color != color)
                        {
                            Move attack = new Move(startLocation, local);
                            attack.AddAttackedLocation(local);

                            foreach (Location empty in previousEmpty)
                            {
                                attack.AddNeededEmtpyLocations(empty.Copy());
                            }

                            moves.Add(attack);
                        }

                        break;
                    }
                    
                    previousEmpty.Add(local);

                    Move nonAttack = new Move(startLocation, local);

                    //Includes the end location as it is non attack
                    foreach (Location empty in previousEmpty)
                    {
                        nonAttack.AddNeededEmtpyLocations(empty.Copy());
                    }

                    moves.Add(nonAttack);
                }
            }
            
            return moves;
        }
    }
}
