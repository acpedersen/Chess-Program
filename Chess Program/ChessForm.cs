/*
 * @Author Adam Pedersen
 * 
 * Created 5/10/2019
 * 
 * This creates a chess game with a simulated board
 * When a piece is clicked it shows possible moves with regards to the fact that a move cannot put a king into check
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess_Program
{
    public partial class ChessForm : Form
    {
        //Variables
        private ColorEnum PlayerColor = ColorEnum.White;
        private bool aPlayerHasWon = false;

        ///Constructor
        public ChessForm()
        {
            InitializeComponent();
            Reset(null, null);
        }

        
        ///Methods
        
        //Selects and Deselects the possible moves of a location
        private void SelectMovePossibilites(Location local)
        {
            MoveList moves = gameBoard.getMoves(local);
            if (moves == null) return;
            foreach (Move move in moves)
            {
                Tile tile = gameBoard.getTile(move.endLocation);


                tile.overlay.BackgroundImage = Chess_Program.Properties.Resources.Targeting;
            }
        }
        private void DeselectMovePossibilites(Location local)
        {
            //Might stor the moves from last selection, less on the computer
            MoveList moves = gameBoard.getMoves(local);
            if (moves == null) return;
            foreach (Move move in moves)
            {
                Tile tile = gameBoard.getTile(move.endLocation);


                tile.overlay.BackgroundImage = null;
            }
        }
        //These methods are used when a Tile is selected in order to change the image of the overlay for user feedback
        public void SelectTile(Tile tile)
        {
            //Select tile
            tile.overlay.BackgroundImage = Chess_Program.Properties.Resources.Selected;
            //Select move possibilities
            SelectMovePossibilites(tile.location);
        }
        public void DeselectTile(Tile tile)
        {
            //Remove tile selection
            tile.overlay.BackgroundImage = null;
            //Remove move possibilities
            DeselectMovePossibilites(tile.location);
        }



        ///Events
        /*
         * Starts a new game of chess, resets the board and variables
         */
        private void Reset(object sender, EventArgs e)
        {
            PlayerColor = ColorEnum.White;
            aPlayerHasWon = false;
            lblPlayerTurn.Text = PlayerColor.ToString() + "'s Turn";
            lblWinText.Text = "";

            //Create a basic game of chess
            gameBoard.createNewBoard();

            ColorEnum color = ColorEnum.Black;
            for (int i = 0; i < 2; i++)
            {
                gameBoard.addPiece(new Rook(color, new Location(0, i * 7 - 0)));
                gameBoard.addPiece(new Knight(color, new Location(1, i * 7 - 0)));
                gameBoard.addPiece(new Bishop(color, new Location(2, i * 7 - 0)));
                gameBoard.addPiece(new King(color, new Location(3, i * 7 - 0)));
                gameBoard.addPiece(new Queen(color, new Location(4, i * 7 - 0)));
                gameBoard.addPiece(new Bishop(color, new Location(5, i * 7 - 0)));
                gameBoard.addPiece(new Knight(color, new Location(6, i * 7 - 0)));
                gameBoard.addPiece(new Rook(color, new Location(7, i * 7 - 0)));
                for (int j = 0; j < 8; j++)
                {
                    gameBoard.addPiece(new Pawn(color, new Location(j, Math.Abs(i * 7 - 1))));
                }

                color = ColorEnum.White;
            }
            gameBoard.RefreshPieceImage();
        }

        /*
         * Handles the GameBoard Tile Selecting event and creates the neccasary feedback for players to see what moves they can make and what they have selected
         */
        public void TileSelecting(GameBoard sender, TileSelectingEventArgs e)
        {
            if (aPlayerHasWon)
            {
                e.CancelEvent();
                return;
            }
            //The selected tile must be either a piece or a move of the previously selected piece

            if (sender.getSelectedTile() != null && sender.getSelectedTile() == e.tile)//Reselected the previous tile
            {//Deselect the previous move and prevent the event


                DeselectTile(sender.getSelectedTile());
                sender.DeselectTile();


                e.CancelEvent();
                return;
            }
            else if (sender.getSelectedTile() != null && sender.getSelectedPiece() != null && sender.getSelectedMoves().Contains(sender.getSelectedLocation(), e.location))//Selecting a move from the previous selected piece
            {//Execute the move and clean up the GUI
                //Removes the gui elements
                DeselectTile(sender.getSelectedTile());
                

                //Execute the planned move on the board
                if (gameBoard.movePiece(sender.getSelectedLocation(), e.location))
                {
                    //Get ready for next move
                    sender.DeselectTile();
                }
                else
                {//Moving failed, reset to last position
                    SelectTile(sender.getSelectedTile());
                }

                e.CancelEvent();
                return;
            }
            else if (sender.pieceAtLocation(e.location) && sender.getPiece(e.location).color == PlayerColor)//Selecting a new piece
            {//Deselect the previous tile
                if (sender.getSelectedTile() != null)
                {
                    DeselectTile(sender.getSelectedTile());
                }
            }
            else
            {//Else invalid selection
                e.CancelEvent();
                return;
            }
        }

        /*
         * Shows that a tile has been selected
         */
        public void TileSelected(GameBoard sender, TileSelectedEventArgs e)
        {
            SelectTile(e.tile);
        }

        /*
         * Modifies the moves that come from GameBoard.getMoves() by handling the FilterMoves event
         * This allows for programmers to have more control over the moves that pieces are allowed to make
         * 
         * Currently adds the Castling to a kings moveset
         * As well as disallowing pieces to move in a way the puts the king into check
         */
        public MoveList FilteringMoves(AbstractPiece piece, MoveList moves)
        {
            //Need to filter certain moves out of this and return a new list
            //Need to filter out moves that put the king in check (By moving out of the way), having the king move into check, and that don't stop the king from being in check (Not taking out the current king attacker or blocking the attack path)

            if (piece.getPieceType().Contains("King"))
            {
                MoveList kingMoves = new MoveList();

                //Readding moves with the added specification that they can't be attacked
                foreach (Move move in moves)
                {
                    if (!gameBoard.checkAttacked(move.endLocation, piece.color))
                    {
                        kingMoves.Add(move);
                    }
                }

                //Add castling as a special move
                if (!piece.getHasMoved())
                {
                    //check for rooks in position that have not moved
                    int baseDistance = 2;
                    for (int i = -1; i <= 1; i += 2)
                    {
                        //baseDistance +1 or +0
                        for (int j = 1; j <= 2; j++)
                        {
                            Location rook = piece.location.Shift(i * (baseDistance + j), 0);
                            if (gameBoard.locationOnBoard(rook) && gameBoard.pieceAtLocation(rook))
                            {
                                AbstractPiece checkedPiece = gameBoard.getPiece(rook);
                                if (checkedPiece.getPieceType() == "Rook" && checkedPiece.color == piece.color && !checkedPiece.getHasMoved())
                                {
                                    //The piece itself is fine now check for spaces inbetween
                                    bool allEmpty = true;
                                    for (int x = rook.x - i; x != piece.location.x; x -= i)
                                    {
                                        Location emptyLocation = new Location(x, piece.location.y);
                                        if (!gameBoard.locationOnBoard(emptyLocation) || gameBoard.pieceAtLocation(emptyLocation))
                                        {
                                            allEmpty = false;
                                            break;
                                        }
                                    }

                                    //And check to see if the spaces the king will pass are under attack
                                    bool allNotUnderAttack = true;
                                    for (int x = piece.location.x; x != piece.location.x + (baseDistance + 1) * i; x += i)
                                    {
                                        Location attackLocation = new Location(x, piece.location.y);
                                        if (gameBoard.checkAttacked(attackLocation, piece.color))
                                        {
                                            allNotUnderAttack = false;
                                            break;
                                        }
                                    }

                                    if (allEmpty && allNotUnderAttack)
                                    {
                                        //Need to switch over to a move based system
                                        Move kingMove = new Move(piece.location, piece.location.Shift(i * baseDistance, 0));
                                        Move rookMove = new Move(rook, piece.location.Shift(i * (baseDistance - 1), 0));
                                        rookMove.SetIgnorePossibleMoves(true);
                                        kingMove.AddSubsequentMove(rookMove);
                                        kingMoves.Add(kingMove);
                                    }
                                }
                            }
                        }
                    }
                }

                //A special case that needs to be handled externally
                return kingMoves;
            }
            //Filter out moves that put the king in check
            else
            {
                MoveList returnMoves = moves.Copy();

                //Filter out moves that don't stop a check
                IList<AbstractPiece> kings = gameBoard.getPieces(typeof(King), piece.color);
                IList<AbstractPiece> attackers = gameBoard.getAttackers(kings);
                if (attackers.Count > 0)
                {
                    for (int i = 0; i < returnMoves.Count; i++)
                    {
                        Move move = returnMoves[i];

                        bool unhandled = true;
                        foreach (AbstractPiece attacker in attackers)
                        {
                            //Take out the attacking piece or Block the attack
                            if (move.GetAttackedLocations().Contains(attacker.location) || gameBoard.doesMoveBlockMove(attacker.getMoves(gameBoard).GetAttackedMove(kings[0].location), move))
                            {
                                continue;
                            }
                            else
                            {
                                //If any attacker is still attacking the king after the move, move is invalid
                                unhandled = false;
                                break;
                            }
                        }

                        if (!unhandled)
                        {
                            returnMoves.Remove(move);
                            i--;//Needs back down one index to account for the removed object
                        }
                    }
                }

                //Filter out moves that put the king into check
                returnMoves = gameBoard.filterMovesThatBlockAttack(returnMoves, kings[0]);

                return returnMoves;
            }

            return moves;
        }

        /*
         * Handles turn shifting, and end game condition (Check, Checkmate, Stalemate)
         */
        public void PieceMoved(object sender, EventArgs e)
        {
            //Switching the player
            ColorEnum previousPlayer = PlayerColor;
            PlayerColor = (ColorEnum)(((int)PlayerColor + 1) % Enum.GetNames(typeof(ColorEnum)).Length);
            lblPlayerTurn.Text = PlayerColor.ToString() + "'s Turn";

            //Check for check, checkmate, stalemate
            //TODO
            IList<AbstractPiece> kings = gameBoard.getPieces(typeof(King), PlayerColor);
            IList<AbstractPiece> attackers = gameBoard.getAttackers(kings);
            if (attackers.Count > 0)
            {//Check
                if(ColorHasMoves(gameBoard, PlayerColor))
                {//Just checked
                    lblWinText.Text = PlayerColor + " Checked";
                    return;
                }
                else
                {
                    aPlayerHasWon = true;
                    lblWinText.Text = previousPlayer + " Checkmate!";
                    return;
                }
            }
            else if(!ColorHasMoves(gameBoard, PlayerColor))
            {
                aPlayerHasWon = true;
                lblWinText.Text = "Stalemate";
                return;
            }

            lblWinText.Text = "";
        }

        //Determines if a side has any moves left to play
        public static bool ColorHasMoves(GameBoard board, ColorEnum color)
        {
            foreach(AbstractPiece piece in board.getPieces(color))
            {
                if (board.getMoves(piece).Count > 0)
                    return true;
            }

            return false;
        }

        //Closes the loop on some unfinished features by showing a text box
        public void Unfinished(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is currently unfinished. Please come back later.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

    }
}
