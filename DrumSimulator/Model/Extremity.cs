using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DrumSimulator.Model
{
    class Extremity
    {
        public enum State {OUT,IN} // where in means that is hitting one drum and out that is not
        public State ExtremityState;

        // If we hit a drum, we store the value of the drum key
        private String hitDrum;
        public string HitDrum
        {
            get
            {
                return this.hitDrum;
            }
            set
            {
                this.hitDrum = value;
            }
        }

        private Point position;
        public Point Position
        {
            get
            {
                return this.position;
            }

            set
            {
                this.position = value;
            }

        }

        private BitmapImage image;
        public BitmapImage Image
        {
            get
            {
                return this.image;
            }
            set
            {
                this.image = value;
            }


        }

        public Double XCoord
        {
            get
            {
                return position.X;
            }
            set
            {
                this.position.X = value;
            }
        }

        public Double YCoord
        {
            get
            {
                return position.Y;
            }
            set
            {
                this.position.Y = value;
            }
        }

        public Extremity(String imagePath)
        {
            this.Image = new BitmapImage(new Uri(imagePath, UriKind.Relative));
        }
    }
}
