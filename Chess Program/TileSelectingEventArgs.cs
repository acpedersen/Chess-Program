/*
 * For when a Tile is clicked, is used before the selecting happens
 * Has a way to cancel the event from proceeding
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_Program
{
    public class TileSelectingEventArgs : TileEventArgs
    {
        private bool cancel = false;

        public TileSelectingEventArgs(Tile tile)
            : base(tile)
        {

        }

        //When the event returns to the whatever called it, allows the caller to cancel any further actions
        public void CancelEvent()
        {
            cancel = true;
        }

        public bool IsEventCancelled()
        {
            return cancel;
        }
    }
}
