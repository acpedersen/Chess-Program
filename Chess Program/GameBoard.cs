/*
 * A complex user control which contains the data of all pieces on a board
 * Also defines methods that allow for unique logic of a game based on complex rulesets
 * 
 * Handles a GUI for showing pieces on a board of any length greater than 0
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace Chess_Program
{
    public partial class GameBoard : UserControl
    {
        #region Variables
        private AbstractPiece[,] board;
        private IList<AbstractPiece> pieces;
        private Tile[,] _tiles;
        private Tile _selectedTile;

        private int xLength;
        private int yLength;

        private const int X_DEFAULT_LENGTH = 8;
        private const int Y_DEFAULT_LENGTH = 8;

        private const int X_TILE_SIZE = 60;
        private const int Y_TILE_SIZE = 60;

        private readonly Color whiteTileColor = Color.White;
        private readonly Color blackTileColor = Color.DimGray;

        //Delegates
        public delegate void TileSelectedHandler(GameBoard board, TileSelectedEventArgs e);
        public delegate void TileSelectingHandler(GameBoard board, TileSelectingEventArgs e);
        public delegate MoveList MoveFilteringHandler(AbstractPiece piece, MoveList moves);
        //Event list for handling tile clicks
        public event TileSelectingHandler TileSelecting;
        public event TileSelectedHandler TileSelected;
        public event EventHandler PieceMoved;
        public event MoveFilteringHandler FilterMoves;
        #endregion

        ///Constructors
        public GameBoard()
        {
            InitializeComponent();

            
            //Used to reset the board to be 0X0 but when loading it looks weird
            //Might have to do some fancy stuff to prevent drawing until it is complete
            ///this.Disposed += this.OnClose;

            //Intilize different neccasary variables
            createNewBoard();
        }
        public GameBoard(int xLength, int yLength)
        {
            InitializeComponent();


            //Used to reset the board to be 0X0 but when loading it looks weird
            //Might have to do some fancy stuff to prevent drawing until it is complete
            //this.Disposed += this.OnClose;

            //Intilize different necassary variables
            createNewBoard(xLength, yLength);
        }

        #region Properties and Accessors
        //Piece setters/getters
        public AbstractPiece[,] getBoard()
        {
            return board;
        }
        public AbstractPiece getPiece(int x, int y)
        {
            return board[x, y];
        }
        public AbstractPiece getPiece(Location position)
        {
            return board[position.x, position.y];
        }
        public IList<AbstractPiece> getPieces()
        {
            return pieces;
        }
        public IList<AbstractPiece> getPieces(ColorEnum color)
        {
            IList<AbstractPiece> returnList = new List<AbstractPiece>();

            foreach (AbstractPiece piece in pieces)
            {
                if (piece.color == color)
                    returnList.Add(piece);
            }

            return returnList;
        }
        public IList<AbstractPiece> getPieces(Type type, ColorEnum color)
        {
            IList<AbstractPiece> returnList = new List<AbstractPiece>();

            foreach (AbstractPiece piece in pieces)
            {
                if (piece.GetType() == type && piece.color == color)
                    returnList.Add(piece);
            }

            return returnList;
        }
        public IList<AbstractPiece> getPieces(IList<Location> locals)
        {
            IList<AbstractPiece> returnList = new List<AbstractPiece>();

            foreach (Location local in locals)
            {
                AbstractPiece piece = getPiece(local);
                if(piece != null)
                    returnList.Add(piece);
            }

            return returnList;
        }

        public void setPiece(AbstractPiece piece)
        {
            board[piece.location.x, piece.location.y] = piece;
        }
        public void setPiece(AbstractPiece piece, Location position)
        {
            board[position.x, position.y] = piece;
        }
        public void setPiece(AbstractPiece piece, int x, int y)
        {
            board[x, y] = piece;
        }

        //Clears a piece from the 2d array and the master list of pieces
        public AbstractPiece clearPiece(AbstractPiece piece)
        {
            AbstractPiece clearedPiece = board[piece.location.x, piece.location.y];
            board[piece.location.x, piece.location.y] = null;
            return clearedPiece;
        }
        public AbstractPiece clearPiece(Location position)
        {
            AbstractPiece clearedPiece = board[position.x, position.y];
            board[position.x, position.y] = null;
            return clearedPiece;
        }
        public AbstractPiece clearPiece(int x, int y)
        {
            AbstractPiece clearedPiece = board[x, y];
            board[x, y] = null;
            return clearedPiece;
        }

        //Tile getter/setter
        public Tile[,] getTiles()
        {
            return _tiles;
        }
        public Tile getTile(int x, int y)
        {
            return _tiles[x, y];
        }
        public Tile getTile(Location location)
        {
            return _tiles[location.x, location.y];
        }

        //Selected tile/piece/location methods to easily access what is selected
        public Tile getSelectedTile()
        {
            return _selectedTile;
        }
        public AbstractPiece getSelectedPiece()
        {
            if (_selectedTile == null) return null;
            return getPiece(_selectedTile.location);
        }
        public Location getSelectedLocation()
        {
            if (_selectedTile == null) return null;
            return _selectedTile.location;
        }
        public MoveList getSelectedMoves()
        {
            if (_selectedTile == null) return null;
            return getMoves(_selectedTile.location);
        }
        //Resets selection
        public void DeselectTile()
        {
            _selectedTile = null;
        }

        //Getting the board constraints
        public int getXLength()
        {
            return xLength;
        }
        public int getYLength()
        {
            return yLength;
        }
        #endregion

        #region Methods
        //Starter methods that initilize all variables
        public void createNewBoard()
        {
            createNewBoard(X_DEFAULT_LENGTH, Y_DEFAULT_LENGTH);
        }
        public void createNewBoard(int xLength, int yLength)
        {
            this.xLength = xLength;
            this.yLength = yLength;

            board = new AbstractPiece[xLength, yLength];
            pieces = new List<AbstractPiece>();

            _tiles = createTileArray();

            this.Controls.Clear();

            this.SuspendLayout();

            //Add each tile to the form
            foreach (Tile tile in _tiles)
            {
                this.Controls.Add(tile);
                tile.Click += new EventHandler(TileClicked);
            }

            //RefreshPieceImage();

            this.ResumeLayout(true);
        }
        
        //Worker method for creating many tiles with the same values
        private Tile createBaseTile()
        {
            Tile panel = new Tile();
            panel.Location = new Point(0, 0);
            panel.Size = new Size(X_TILE_SIZE, Y_TILE_SIZE);
            panel.BackColor = Color.White;
            panel.BackgroundImageLayout = ImageLayout.Stretch;
            return panel;
        }
        //Creates the tiles for the visual aspect of the GameBoard
        private Tile[,] createTileArray()
        {
            Tile[,] panels = new Tile[xLength, yLength];
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    Tile panel = createBaseTile();
                    panel.location = new Location(x, y);

                    //Set location
                    panel.Location = new Point(x * X_TILE_SIZE, y * Y_TILE_SIZE);

                    //Determine color based on location
                    if ((x + y) % 2 == 0)
                    {
                        panel.BackColor = whiteTileColor;
                    }
                    else
                    {
                        panel.BackColor = blackTileColor;
                    }

                    panels[x, y] = panel;
                }
            }

            return panels;
        }

        //Deep copy of a GameBoard
        public GameBoard Copy()
        {
            //Needs to be a deep copy
            GameBoard newBoard = new GameBoard(this.xLength, this.yLength);

            newBoard.addPieces(AbstractPiece.Copy(this.pieces));

            return newBoard;
        }

        //Methods that determine if locations are valid for certain actions elsewhere in the class
        public bool pieceAtLocation(Location location)
        {
            return locationOnBoard(location) && getPiece(location) != null;
        }
        public bool pieceOnBoard(AbstractPiece piece)
        {
            return pieces.Contains(piece);
        }
        public bool locationOnBoard(Location location)
        {
            if (location.x >= 0 && location.y >= 0
                && location.x < xLength && location.y < yLength)
                return true;

            return false;
        }

        //Adds pieces to both the 2d Array and the master list 
        public bool addPiece(AbstractPiece piece)
        {//Might have it update the tile it is added to
            if (locationOnBoard(piece.location) && !pieceAtLocation(piece.location))
            {
                setPiece(piece);
                pieces.Add(piece);

                return true;
            }

            return false;
        }
        public bool addPieces(AbstractPiece[] pieces)
        {
            bool addedAll = true;
            foreach(AbstractPiece piece in pieces)
            {
                if(!addPiece(piece))
                {
                    addedAll = false;
                }
            }

            return addedAll;
        }
        public bool addPieces(IList<AbstractPiece> pieces)//Same as above but for multiple
        {
            return addPieces(pieces.ToArray());
        }

        //Correctly delinks the piece from the array and the master list
        public AbstractPiece removePiece(Location position)
        {
            if (pieceAtLocation(position))
            {
                return removePiece(getPiece(position));
            }

            return null;
        }
        public AbstractPiece removePiece(AbstractPiece piece)
        {
            if (locationOnBoard(piece.location))
            {
                clearPiece(piece);
                pieces.Remove(piece);

                return piece;
            }

            return null;
        }
        public AbstractPiece[] removePieces(AbstractPiece[] pieces)
        {
            IList<AbstractPiece> removedPieces = new List<AbstractPiece>();
            foreach (AbstractPiece piece in pieces)
            {
                AbstractPiece removed = removePiece(piece);
                if(removed != null)
                {
                    removedPieces.Add(removed);
                }
            }

            return removedPieces.ToArray();
        }
        public IList<AbstractPiece> removePieces(IList<AbstractPiece> pieces)
        {
            IList<AbstractPiece> removedPieces = new List<AbstractPiece>();
            foreach (AbstractPiece piece in pieces)
            {
                AbstractPiece removed = removePiece(piece);
                if (removed != null)
                {
                    removedPieces.Add(removed);
                }
            }

            return removedPieces;
        }


        //Requires a rework with the Move class instead of end locations
        /*
         * Takes in a Move and executes that move to this GameBoard
         * Will also call the subsequent moves recursively until all actions have been completed
         */
        public bool movePiece(Location startLocation, Location endLocation)
        {
            return movePiece(getMoves(startLocation).GetMove(startLocation, endLocation));
        }
        public bool movePiece(Move move)
        {
            if(moveIndividualPiece(move))
            {
                //Trigger the event
                OnPieceMoved(null, null);

                RefreshPieceImage();

                return true;
            }

            return false;
        }
        private bool moveIndividualPiece(Move move)
        {
            if (pieceAtLocation(move.startLocation) && locationOnBoard(move.endLocation))
            {
                if (move.IgnorePossibleMoves() || checkMove(move))
                {///Once in here execute the steps of the move
                    //Remove anypiece that is sitting at the end location
                    foreach (Location attacked in move.GetAttackedLocations())
                    {
                        removePiece(getPiece(attacked));
                    }

                    //Remove from old position
                    AbstractPiece piece = clearPiece(move.startLocation);

                    //replace onto the board in the new position
                    piece.location = move.endLocation;
                    //Need to let the piece know it has moved
                    piece.setHasMoved(true);
                    setPiece(piece);

                    //Execute subsequent moves
                    foreach(Move subsequentMove in move.GetSubsequentMoves())
                    {
                        //Allows for complex moves involving multiple pieces
                        //Might have bad ramifications that need to be handled
                        moveIndividualPiece(subsequentMove);
                    }

                    //Certain pieces can have added functionality after the move
                    piece.afterMoveAction(this);

                    return true;
                }
            }

            return false;
        }

        /*
         * Gets the original pieces moves then filters them through the filter event which can be handled by any client
         */
        public MoveList getMoves(Location startLocation)
        {
            if (!pieceAtLocation(startLocation))
                return null;

            AbstractPiece piece = getPiece(startLocation);
            return OnFilterMoves(piece, piece.getMoves(this));
        }
        public MoveList getMoves(AbstractPiece piece)
        {
            return OnFilterMoves(piece, piece.getMoves(this));
        }
        public MoveList getMoves(ColorEnum color)
        {
            MoveList moves = new MoveList();
            foreach(AbstractPiece piece in getPieces(color))
            {
                moves.Add(getMoves(piece));
            }
            return moves;
        }

        //Sees if a move is a valid option
        public bool checkMove(Move move)
        {
            if(pieceAtLocation(move.startLocation))
                return checkMove(getPiece(move.startLocation), move);
            return false;
        }
        public bool checkMove(AbstractPiece piece, Move move)
        {
            return getMoves(piece).Contains(move);
        }

        //Methods to allow the client to create complex gameplay
        //Determines if a location or piece is "Attacked" (Has move with a target location of the piece or location)
        public bool checkAttacked(AbstractPiece piece)
        {
            return checkAttacked(piece.location, piece.color);
        }
        public bool checkAttacked(Location startLocation, ColorEnum color)
        {
            if (!locationOnBoard(startLocation))
                return false;
            
            //TODO Will require a rework as now not all moves are reversible
            //Loop through each derived type
            foreach (Type type in AbstractPiece.getDerivedTypes())
            {
                Type[] parameterTypes = { typeof(GameBoard), typeof(Location), typeof(ColorEnum) };
                MethodInfo method = type.GetMethod("getPieceMoves", parameterTypes);
                if (method == null)
                    continue; // Should have exceptions here

                object[] parameters = {this, startLocation, color}, constructor = {color, startLocation};

                MoveList moves = (MoveList)method.Invoke(Activator.CreateInstance(type, constructor), parameters);
                foreach (Move move in moves)
                {
                    IList<AbstractPiece> pieces = getPieces(move.GetAttackedLocations());
                    foreach (AbstractPiece piece in pieces)//Kinda wonky
                    {
                        if (piece != null && piece.GetType().Equals(type))
                        {
                            //Means the reverse attack met with a piece of the correct type
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        //Returns a list of all attackers of a piece or location
        public IList<AbstractPiece> getAttackers(AbstractPiece piece)
        {
            return getAttackers(piece.location, piece.color);
        }
        public IList<AbstractPiece> getAttackers(Location startLocation, ColorEnum color)
        {
            if (!locationOnBoard(startLocation))
                return null;
            
            //Loop through each derived type
            IList<AbstractPiece> attackers = new List<AbstractPiece>();
            foreach (Type type in AbstractPiece.getDerivedTypes())
            {
                Type[] parameterTypes = { typeof(GameBoard), typeof(Location), typeof(ColorEnum) };
                MethodInfo method = type.GetMethod("getPieceMoves", parameterTypes);
                if (method == null)
                    continue; // Should have exceptions here
                object[] parameters = {this, startLocation, color}, constructor = {color, startLocation};

                MoveList moves = (MoveList)method.Invoke(Activator.CreateInstance(type, constructor), parameters);
                foreach (Move move in moves)
                {
                    IList<AbstractPiece> pieces = getPieces(move.GetAttackedLocations());
                    foreach (AbstractPiece piece in pieces)//Kinda wonky
                    {
                        if (piece != null && piece.GetType().Equals(type))
                        {
                            //Means the reverse attack met with a piece of the correct type
                            attackers.Add(piece);
                        }
                    }
                }
            }

            return attackers;
        }
        public IList<AbstractPiece> getAttackers(IList<AbstractPiece> pieces)
        {
            IList<AbstractPiece> attackers = new List<AbstractPiece>();
            foreach(AbstractPiece piece in pieces)
            {
                foreach(AbstractPiece attacker in getAttackers(piece))
                {
                    attackers.Add(attacker);
                }
            }

            return attackers;
        }

        //Checks if a pieces current position is blocking an attack to a specific location or piece
        public bool isPieceBlockingAttack(AbstractPiece piece, AbstractPiece blockedPiece)
        {
            return isPieceBlockingAttack(piece, blockedPiece.location);
        }
        public bool isPieceBlockingAttack(AbstractPiece piece, Location blockedAttack)
        {
            GameBoard copy = this.Copy();
            copy.removePiece(piece);//Allows for simulation

            IList<AbstractPiece> attackers = getAttackers(piece);

            foreach(AbstractPiece attacker in attackers)
            {
                MoveList moves = copy.getMoves(attacker);
                if (moves.ContainsAttacked(blockedAttack))
                    return true;
            }

            return false;
        }
        //Checks if a move is blocking a specific attack
        //May have different results from isPieceBlockingAttack as the move may make a piece end up still blocking the attack
        public bool isMoveBlockingAttack(Move move, AbstractPiece blockedPiece)
        {
            return isMoveBlockingAttack(move, blockedPiece.location);
        }
        public bool isMoveBlockingAttack(Move move, Location blockedAttack)
        {
            GameBoard copy = this.Copy();
            copy.movePiece(move);//Allows for simulation

            IList<AbstractPiece> attackers = getAttackers(getPiece(move.startLocation));

            foreach (AbstractPiece attacker in attackers)
            {
                MoveList moves = copy.getMoves(attacker);
                if (moves.ContainsAttacked(blockedAttack))
                    return true;
            }

            return false;
        }
        //Takes a list of moves and removes any that allow an attack to pass
        public MoveList filterMovesThatBlockAttack(MoveList moves, AbstractPiece blockedPiece)
        {
            return filterMovesThatBlockAttack(moves, blockedPiece.location);
        }
        public MoveList filterMovesThatBlockAttack(MoveList moves, Location blockedAttack)
        {
            if (moves.Count == 0)
                return moves;


            MoveList filtered = new MoveList();
            foreach (Move move in moves)
            {
                GameBoard copy = this.Copy();
                copy.movePiece(move);

                IList<AbstractPiece> attackers = copy.getAttackers(moves[0].startLocation, getPiece(moves[0].startLocation).color);

                bool allowsAttack = false;
                foreach (AbstractPiece attacker in attackers)
                {
                    MoveList attackerMoves = copy.getMoves(attacker);
                    if (attackerMoves.ContainsAttacked(blockedAttack))
                    {
                        allowsAttack = true;
                        break;
                    }
                }
                if(!allowsAttack)
                    filtered.Add(move);
            }

            return filtered;
        }


        //Determines a list of pieces or moves that can stop a move from occuring by cheching the moves requires emptySpaces list
        public IList<AbstractPiece> getPiecesThatCanBlockMove(Move attackingMove)
        {
            IList<AbstractPiece> list = new List<AbstractPiece>();

            IList<Location> blockedLocations = attackingMove.GetNeededEmtpyLocations();
            foreach (AbstractPiece piece in pieces)
            {
                foreach (Move move in getMoves(piece))
                {
                    if (doesMoveContainEndLocation(move, blockedLocations))
                    {
                        list.Add(piece);
                        break;
                    }
                }
            }

            return list;
        }
        public MoveList getMovesToBlockMove(Move attackingMove)
        {
            MoveList list = new MoveList();

            IList<Location> blockedLocations = attackingMove.GetNeededEmtpyLocations();
            foreach(AbstractPiece piece in pieces)
            {
                foreach(Move move in piece.getMoves(this))
                {
                    if (doesMoveContainEndLocation(move, blockedLocations))
                        list.Add(move);
                }
            }
            
            return list;
        }
        public IList<AbstractPiece> getPiecesThatCanBlockMove(Move attackingMove, ColorEnum color)
        {
            IList<AbstractPiece> list = new List<AbstractPiece>();

            IList<Location> blockedLocations = attackingMove.GetNeededEmtpyLocations();
            foreach (AbstractPiece piece in getPieces(color))
            {
                foreach (Move move in getMoves(piece))
                {
                    if (doesMoveContainEndLocation(move, blockedLocations))
                    {
                        list.Add(piece);
                        break;
                    }
                }
            }

            return list;
        }
        public MoveList getMovesToBlockMove(Move attackingMove, ColorEnum color)
        {
            MoveList list = new MoveList();

            IList<Location> blockedLocations = attackingMove.GetNeededEmtpyLocations();
            foreach (AbstractPiece piece in getPieces(color))
            {
                foreach (Move move in getMoves(piece))
                {
                    if (doesMoveContainEndLocation(move, blockedLocations))
                        list.Add(move);
                }
            }

            return list;
        }
        //Worker methods for the getMovesToBlockMove and getPiecesThatCanBlockMove
        public bool doesMoveContainEndLocation(Move move, Location location)
        {
            if (move.endLocation == location)
                return true;

            foreach(Move subsequent in move.GetSubsequentMoves())
            {
                if (doesMoveContainEndLocation(subsequent, location))
                    return true;
            }

            return false;
        }
        public bool doesMoveContainEndLocation(Move move, IList<Location> locations)
        {
            if (locations.Contains(move.endLocation))
                return true;

            foreach (Move subsequent in move.GetSubsequentMoves())
            {
                if (doesMoveContainEndLocation(subsequent, locations))
                    return true;
            }

            return false;
        }
        //Checks a singular move if it can block a specific attack
        public bool doesMoveBlockMove(Move attackingMove, Move blockingMove)
        {
            return doesMoveContainEndLocation(blockingMove, attackingMove.GetNeededEmtpyLocations());
        }
        public void TileClicked(object obj, EventArgs e)
        {
            Tile tile = (Tile)obj;

            //Invoke the pre tile selected event
            TileSelectingEventArgs eventArgs = new TileSelectingEventArgs(tile);
            OnTileSelecting(this, eventArgs);

            //Allow the event to be cancelled from within selecting
            if (eventArgs.IsEventCancelled())
                return;

            //Set as the new seleceted tile
            this._selectedTile = tile;

            //Invoke the post tile selected event
            OnTileSelected(this, new TileSelectedEventArgs(tile));
        }

        //Refreshes the images of the Tiles to be based on the Pieces associated with that location from the AbstractPiece method GetPieceImage
        public void RefreshPieceImage()
        {
            foreach (Panel tile in _tiles)
            {
                tile.BackgroundImage = null;
            }
            foreach (AbstractPiece piece in board)
            {
                if (piece == null) continue;

                Location local = piece.location;
                _tiles[local.x, local.y].BackgroundImage = piece.GetPieceImage();
                _tiles[local.x, local.y].Refresh();
                _tiles[local.x, local.y].Update();
            }

            this.Refresh();
            this.Update();
        }

        //Event callers
        protected void OnTileSelecting(GameBoard t, TileSelectingEventArgs e)
        {
            TileSelectingHandler handler = TileSelecting;
            if (handler != null)
            {
                handler(t, e);
            }
        }
        protected void OnTileSelected(GameBoard t, TileSelectedEventArgs e)
        {
            TileSelectedHandler handler = TileSelected;
            if (handler != null)
            {
                handler(t, e);
            }
        }
        protected void OnPieceMoved(object sender, EventArgs e)
        {
            EventHandler handler = PieceMoved;
            if (handler != null)
            {
                handler(sender, e);
            }
        }
        protected MoveList OnFilterMoves(AbstractPiece piece, MoveList moves)
        {
            MoveFilteringHandler handler = FilterMoves;
            if (handler != null)
            {
                return handler(piece, moves);
                
            }

            //Nothing was handling this event pass on the original
            return moves;
        }
        #endregion


        ///Events
        public void OnClose(object sender, EventArgs e)
        {//Resets the user control to a neutral state, might not be used
            this.Controls.Clear();
        }

    }
}
