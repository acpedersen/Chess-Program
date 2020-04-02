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
    public class Pawn : AbstractPiece
    {
        ///Variables
        private static readonly int VALUE = 1;

        ///Constructor
        public Pawn(ColorEnum color, Location location)
            : base(color, location)
        {
            whiteVersion = Chess_Program.Properties.Resources.WhitePawn;
            blackVersion = Chess_Program.Properties.Resources.BlackPawn;
        }
        public Pawn(AbstractPiece piece)
            : base(piece)
        {
            whiteVersion = Chess_Program.Properties.Resources.WhitePawn;
            blackVersion = Chess_Program.Properties.Resources.BlackPawn;
        }

        ///Methods
        //Overidden with Pawn piece logic
        public override MoveList getPieceMoves(GameBoard board, Location startLocation, ColorEnum color)
        {
            MoveList moves = new MoveList();

            int forward = getYForward();
            Location forwardLoc = startLocation.Shift(0, forward);
            if (board.locationOnBoard(forwardLoc) && !board.pieceAtLocation(forwardLoc))
            {
                Move newMove = new Move(startLocation, forwardLoc);
                newMove.AddNeededEmtpyLocations(forwardLoc.Copy());
                moves.Add(newMove);
                

                //Needs to be able to do the first forward in order to do the second one
                Location doubleForwardLoc = forwardLoc.Shift(0, forward);
                if (board.locationOnBoard(doubleForwardLoc) && !board.pieceAtLocation(doubleForwardLoc) && !getHasMoved())
                {
                    Move doubleForwardNewMove = new Move(startLocation, doubleForwardLoc);
                    doubleForwardNewMove.AddNeededEmtpyLocations(forwardLoc.Copy());
                    doubleForwardNewMove.AddNeededEmtpyLocations(doubleForwardLoc.Copy());
                    moves.Add(doubleForwardNewMove);
                }
            }
            

            //Two attacks
            for (int i = -1; i <=1; i+=2)
            {
                Location sideAttack = startLocation.Shift(i, forward);
                if (board.locationOnBoard(sideAttack) && board.pieceAtLocation(sideAttack) && board.getPiece(sideAttack).color != color)
                {
                    Move sideAttackMove = new Move(startLocation, sideAttack);
                    sideAttackMove.AddAttackedLocation(sideAttack);
                    moves.Add(sideAttackMove);
                }
            }
            
            return moves;
        }

        //Makes the Pawn into a queen if it has reached the end of the board
        public override void afterMoveAction(GameBoard board)
        {
            Location forwardLoc = location.Shift(0, getYForward());
            if (!board.locationOnBoard(forwardLoc))
            {
                //Perform the queen me
                Queen upgrade = new Queen(this);
                board.removePiece(this);
                board.addPiece(upgrade);
            }
        }
    }
}
