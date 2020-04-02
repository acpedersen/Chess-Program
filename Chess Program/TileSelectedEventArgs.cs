/*
 * Event Arg for a specific circumstances
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_Program
{
    public class TileSelectedEventArgs : TileEventArgs
    {
        //Not much here
        public TileSelectedEventArgs(Tile tile)
            : base(tile)
        {

        }
    }
}
