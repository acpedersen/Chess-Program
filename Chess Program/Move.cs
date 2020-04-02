/*
 * Represents a pieces move as well as the consequences of that move
 * Includes Start and End location
 * What locations are attacked
 * Subsequent moves intendend to follow up this move
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_Program
{
    public class Move
    {
        private Location _startLocal;
        private Location _endLocal;
        private MoveList subsequentMoves;
        private IList<Location> attackedLocations;
        private IList<Location> neededEmptyLocations;
        //Used for moves that were added, or not originally from the piece
        private bool ignorePossibleMoves;

        ///Constructors
        public Move()
        {
            subsequentMoves = new MoveList();
            attackedLocations = new List<Location>();
            neededEmptyLocations = new List<Location>();
            ignorePossibleMoves = false;
        }
        public Move(Location startLocal, Location endLocal)
            : this()
        {
            this._startLocal = startLocal;
            this._endLocal = endLocal;
        }
        public Move(Location startLocal, Location endLocal, MoveList subsequentMoves, IList<Location> attackedLocations, IList<Location> neededEmtpyLocations)
            : this(startLocal, endLocal)
        {
            this.subsequentMoves = subsequentMoves;
            this.attackedLocations = attackedLocations;
            this.neededEmptyLocations = neededEmtpyLocations;
        }

        ///Properties and Accessors
        public Location startLocation
        {
            get { return _startLocal; }
            set { _startLocal = value; }
        }
        public Location endLocation
        {
            get { return _endLocal; }
            set { _endLocal = value; }
        }

        public MoveList GetSubsequentMoves()
        {
            return subsequentMoves;
        }
        public void AddSubsequentMove(Move move)
        {
            subsequentMoves.Add(move);
        }

        public IList<Location> GetAttackedLocations()
        {
            return attackedLocations;
        }
        public void AddAttackedLocation(Location local)
        {
            attackedLocations.Add(local);
        }

        public IList<Location> GetNeededEmtpyLocations()
        {
            return neededEmptyLocations;
        }
        public void AddNeededEmtpyLocations(Location local)
        {
            neededEmptyLocations.Add(local);
        }

        public bool IgnorePossibleMoves()
        {
            return ignorePossibleMoves;
        }
        public void SetIgnorePossibleMoves(bool value)
        {
            ignorePossibleMoves = value;
        }

        ///Methods
        //Add in equals and overide the == and != operator
        public static bool operator== (Move move1, Move move2)
        {
            if (object.ReferenceEquals(move1, null) || object.ReferenceEquals(move2, null))
            {
                if (object.ReferenceEquals(move1, move2))//Both null
                    return true;
                else
                    return false;
            }

            return move1.Equals(move2);
        }
        public static bool operator!= (Move move1, Move move2)
        {
            if (object.ReferenceEquals(move1, null) || object.ReferenceEquals(move2, null))
            {
                if (object.ReferenceEquals(move1, move2))//Both null
                    return false;
                else
                    return true;
            }

            return !move1.Equals(move2);
        }
        public override bool Equals(object comparison)//Is a deep comparison on objects contained in the Move
        {
            if (comparison == null)
                return false;

            if (comparison.GetType() == typeof(Move))
            {//Allowed because we confirmed the type
                Move move = (Move)comparison;

                if(this.startLocation == move.startLocation && this.endLocation == move.endLocation)
                {//First comparison

                    //Check attacked locations
                    foreach(Location attacked in this.GetAttackedLocations())
                    {
                        if(!move.GetAttackedLocations().Contains(attacked))
                        {
                            //Doesn't contain one of the attacked
                            return false;
                        }
                    }

                    //Check for subsequent moves
                    foreach (Move subsequentMove in this.GetSubsequentMoves())
                    {
                        //Should chain until it reaches the bottom of the list
                        if (!move.GetSubsequentMoves().Contains(subsequentMove))
                        {
                            //Doesn't contain one of the attacked
                            return false;
                        }
                    }

                    //Reached the end of comparisons
                    return true;
                }
            }

            return false;
        }
    }

    //A Specified collection for Moves, allows added methods
    public class MoveList : List<Move>
    {
        //Adds a collection of moves for easy manipulation
        public void Add(MoveList moves)
        {
            foreach(Move move in moves)
            {
                this.Add(move);
            }
        }
        //Creates a shallow copy of the List
        public MoveList Copy()
        {
            MoveList newList = new MoveList();
            foreach(Move move in this)
            {
                newList.Add(move);
            }
            return newList;
        }

        //Used to easily get the correct move from the board
        public Move GetMove(Location endLocation)
        {
            foreach (Move move in this)
            {
                if (move.endLocation == endLocation)
                    return move;
            }
            return null;
        }
        public Move GetMove(Location startLocation, Location endLocation)
        {
            foreach (Move move in this)
            {
                if (move.startLocation == startLocation && move.endLocation == endLocation)
                    return move;
            }
            return null;
        }

        //Finds if a move exists from one location to another
        public bool Contains(Location endLocation)
        {
            foreach (Move move in this)
            {
                if (move.endLocation == endLocation)
                    return true;
            }
            return false;
        }
        public bool Contains(Location startLocation, Location endLocation)
        {
            foreach(Move move in this)
            {
                if (move.startLocation == startLocation && move.endLocation == endLocation)
                    return true;
            }
            return false;
        }

        //Finds if an attacked location is included in the list of moves
        public bool ContainsAttacked(Location attacked)
        {
            foreach (Move move in this)
            {
                if (move.GetAttackedLocations().Contains(attacked))
                    return true;
            }
            return false;
        }
        public Move GetAttackedMove(Location attacked)
        {
            foreach (Move move in this)
            {
                if (move.GetAttackedLocations().Contains(attacked))
                    return move;
            }
            return null;
        }
    }
}
