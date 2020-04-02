/*
 * Base Tile event
 * Contains the tile that the event is linked to
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_Program
{
    public class TileEventArgs : EventArgs
    {
        //Store the calling Tile
        private Tile _tile;

        public Tile tile
        {
            set { _tile = value; }
            get { return _tile; }
        }

        //Some accesors to get information
        public Location location
        {
            get { return _tile.location; }
        }

        public TileEventArgs(Tile tile)
        {
            this._tile = tile;
        }
    }
}
