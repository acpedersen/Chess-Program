/*
 * A user control with two panels one on top of the other
 * Allows for a programmer to easily create visuals that overlap eachother without the need to create multiple images
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

namespace Chess_Program
{
    public partial class Tile : Panel
    {
        ///Variables
        private Panel _overlay;
        private Location _location;

        ///Properties
        public Panel overlay
        {
            get
            {
                return _overlay;
            }
        }
        public Location location
        {
            get { return _location; }
            set { _location = value; }
        }

        ///Constructor
        public Tile()
        {
            InitializeComponent();

            //Creating the overlay tile
            ResetOverlay();
        }

        public Tile(Location local)
            :this()
        {
            //Setting the location
            location = local;
        }

        ///Methods
        public void ResetOverlay()
        {
            this.Controls.Clear();

            _overlay = new Panel();
            _overlay.Location = new Point(0, 0);
            _overlay.Size = this.Size;
            _overlay.BackColor = Color.Transparent;
            _overlay.BackgroundImageLayout = ImageLayout.Stretch;
            _overlay.Click += this.OverlayClick;
            this.Controls.Add(overlay);
        }

        ///Events
        private void OverlayClick(object sender, EventArgs e)
        {
            //Passes through the click event to the control underneath
            //Need to make the control perform correctly
            this.OnClick(null);
        }

        private void setOverlaySize(object sender, EventArgs args)
        {
            _overlay.Size = this.Size;
        }

        private void setOverlayLocation(object sender, EventArgs args)
        {//Overlay remains at (0, 0) of the parent, base Panel
            //overlay.Location = this.Location;
        }
    }
}
