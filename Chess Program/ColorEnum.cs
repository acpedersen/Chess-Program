/*
 * Represents the side
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_Program
{
    //This is used in place of a string to store a "Side" or color
    //Used in many aspects of the pieces and GameBoard to help determine locations of moves based on Side
    public enum ColorEnum
    {
        White = 0,
        Black = 1
    }
}
