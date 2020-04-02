/*
 * Stores a cartesian coordinate system location
 * Has methods to manipulate locations with other locations
 * Easily comparable
 */

namespace Chess_Program
{
    public class Location
    {
        public int x;
        public int y;
        
        //Constructors
        public Location()
        {
            x = 0;
            y = 0;
        }
        public Location(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Location(Location location)
        {
            this.x = location.x;
            this.y = location.y;
        }
        
        //Used for easy comparison of this object
        public static bool operator== (Location loc1, Location loc2)
        {
            if (object.ReferenceEquals(loc1, null) || object.ReferenceEquals(loc2, null))
            {
                if (object.ReferenceEquals(loc1, loc2))//Both null
                    return true;
                else
                    return false;
            }

            return loc1.Equals(loc2);
        }
        public static bool operator!= (Location loc1, Location loc2)
        {
            if (object.ReferenceEquals(loc1, null) || object.ReferenceEquals(loc2, null))
            {
                if (object.ReferenceEquals(loc1, loc2))//Both null
                    return false;
                else
                    return true;
            }

            return !loc1.Equals(loc2);
        }
        public override bool Equals(object comparison)
        {
            if (comparison == null)
                return false;

            if(comparison.GetType() == typeof(Location))
                if(((Location)comparison).x == this.x && ((Location)comparison).y == this.y)
                    return true;
            
            return false;
        }

        //Creates a copy
        public Location Copy()
        {
            Location local = new Location(this.x, this.y);
            return local;
        }

        //Combining Locations together to form new Location (Similar to vector math)
        public Location Shift(Location shift)
        {
            return this.Shift(shift.x, shift.y);
        }
        public Location Shift(int xShift, int yShift)
        {
            return new Location(this.x + xShift, this.y + yShift);
        }

        //Index of Locations for the 8 directions available on the GameBoard
        //Useful for Pieces deriving moves based on direction
        public static Location directionIndex(int i)
        {
            Location shift;
            switch (i)
            {
                case 0:
                    shift = new Location(1, 0);
                    break;

                case 1:
                    shift = new Location(0, 1);
                    break;

                case 2:
                    shift = new Location(-1, 0);
                    break;

                case 3:
                    shift = new Location(0, -1);
                    break;

                case 4:
                    shift = new Location(1, 1);
                    break;

                case 5:
                    shift = new Location(-1, 1);
                    break;

                case 6:
                    shift = new Location(-1, -1);
                    break;

                case 7:
                    shift = new Location(1, -1);
                    break;

                default:
                    shift = new Location(0, 0);
                    break;
            }

            return shift;
        }

        public override string ToString()
        {
            return x + ", " + y;
        }
    }
}
