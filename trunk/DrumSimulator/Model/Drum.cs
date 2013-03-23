using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DrumSimulator.Model;
using System.Windows.Media.Imaging;

namespace DrumSimulator.Model
{
    class Drum
    {
        private Point offset;
        public Point Offset
        {
            get
            {
                return this.offset;
            }

            set
            {
                this.offset = value;
            }

        }

        private Point position;
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

        private Double height;
        public Double Height
        {
            get
            {
                return this.height;
            }

            set
            {
                this.height = value;
            }

        }

        private Double width;
        public Double Width
        {
            get
            {
                return this.width;
            }

            set
            {
                this.width = value;
            }

        }

        private String soundPath;
        public String SoundPath
        {
            get
            {
                return this.soundPath;
            }

            set
            {
                this.soundPath = value;
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

        public Drum(Double height, Double width, String soundPath, String imagePath, Point off)
        {
            this.height = height;
            this.width = width;
            this.soundPath = soundPath;
            this.image = new BitmapImage(new Uri(imagePath, UriKind.Relative));
            this.offset = off;
        }

        public Boolean Hit(Extremity hand)
        {
            bool inX = hand.Position.X > this.Position.X && hand.Position.X < this.Position.X + this.Width;
            bool inY = hand.Position.Y > this.Position.Y && hand.Position.Y < this.Position.Y + this.Height;
            return inX && inY;
        }
    }

    class PedalDrum : Drum
    {

        public PedalDrum(Double height, Double width, String soundPath, String imagePath, Point off)
            : base(height, width, soundPath, imagePath, off) { }

        public Boolean Hit(Double footRatio)
        {
            return (footRatio > 0.93 && footRatio < 1.2);
        }
    }

    class DrumHit
    {
        private String key;
        public String DrumKey
        {
            get
            {
                return this.key;
            }
            set
            {
                this.key = value;
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

        private String soundPath;
        public String SoundPath
        {
            get
            {
                return this.soundPath;
            }

            set
            {
                this.soundPath = value;
            }
        }

        public DrumHit(string sound, Point pos, String key)
        {
            this.Position = pos;
            this.SoundPath = sound;
            this.DrumKey = key;
        }
    }
}
