using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DrumSimulator.Model
{
    class DrumSet
    {
        IDictionary<String, Drum> drums;
        IDictionary<String, Drum> pedals;
        
        public DrumSet(int screenX, int screenY)
        {
            this.drums = new Dictionary<String, Drum>();
            this.pedals = new Dictionary<String, Drum>();

            Drum crash = new Drum(screenY / 8, screenX / 8, "Sounds/crash.wav", "/DrumSimulator;component/Data/Images/crash.png", new Point(-screenX / 4, -screenY / 4));
            this.drums.Add("crash", crash);

            Drum snare = new Drum(screenY / 8, screenX / 8, "Sounds/snare.wav", "/DrumSimulator;component/Data/Images/snare.png", new Point(screenX / 10, screenY / 15));
            this.drums.Add("snare", snare);

            Drum hihat = new Drum(screenY / 8, screenX / 8, "Sounds/hihat.wav", "/DrumSimulator;component/Data/Images/hihat.png", new Point(screenX / 8, -screenY / 4));
            this.drums.Add("hihat", hihat);

            Drum tom1 = new Drum(screenY / 8, screenX / 3, "Sounds/tom.wav", "/DrumSimulator;component/Data/Images/tom.png", new Point(-screenX / 15, screenY / 50));
            this.drums.Add("tom1", tom1);

            Drum tom2 = new Drum(screenY / 8, screenX / 3, "Sounds/tom.wav", "/DrumSimulator;component/Data/Images/tom.png", new Point(-screenX / 8, screenY / 50));
            this.drums.Add("tom2", tom2);

            Drum low = new Drum(screenY / 6, screenX / 4, "Sounds/low.wav", "/DrumSimulator;component/Data/Images/low.png", new Point(-screenX / 5, screenY / 25));
            this.drums.Add("low", low);

            Drum bass = new Drum(screenY / 4, screenX / 4, "Sounds/bassPedal.wav", "/DrumSimulator;component/Data/Images/bass.png", new Point(screenX / 75, screenY / 4));
            this.drums.Add("bass", bass);
        }

        public IEnumerable<Drum> GetDrums()
        {
            foreach (KeyValuePair<String, Drum> pair in this.drums)
            {
                Drum current = pair.Value;
                yield return current;
            }
        }

        private Drum GetDrum(String key)
        {
            Drum result;
            if (this.drums.ContainsKey(key))
            {
                // it's a drum
                result = this.drums[key];
            }
            else if (this.pedals.ContainsKey(key))
            {
                // it's a pedal
                result = this.pedals[key];
            }
            else
            {
                Console.WriteLine("Error getting drum: Unknown key " + key);
                result = null;
            }
            return result;
        }

        public Point GetPosition(String key)
        {
            return GetDrum(key).Position;
        }

        public Double GetWidth(String key)
        {
            return GetDrum(key).Width;
        }

        public Double GetHeight(String key)
        {
            return GetDrum(key).Height;
        }

        // pre: assuming that we can only hit one drum at once with one hand
        public DrumHit hit(Extremity hand)
        {
            foreach (KeyValuePair<String, Drum> pair in this.drums)
            {
                Drum current = pair.Value;
                if (current.Hit(hand) && !pair.Key.Equals("bass"))
                {
                    return new DrumHit(current.SoundPath, current.Position, pair.Key);
                }
            }
            return null;
        }

        public DrumHit bassHit(Double ratio)
        {
            Drum bass = this.drums["bass"];
            if (ratio > 0.93 && ratio < 1.2)
            {
                return new DrumHit(bass.SoundPath, bass.Position, "bass");
            }
            return null;
        }

       /* public DrumHit hit(Extremity hand)
        {
            foreach (KeyValuePair<String, Drum> pair in this.drums)
            {
                Drum current = pair.Value;
                if (current.Hit(hand) && !pair.Key.Equals("bass"))
                {
                    return new DrumHit(current.SoundPath, current.Position, pair.Key);
                }
            }
            return null;
        }*/


        public void update(Point body) {
            foreach (KeyValuePair<String, Drum> pair in this.drums)
            {
                Drum current = pair.Value;
                this.drums[pair.Key].Position = new Point(body.X + current.Offset.X, body.Y + current.Offset.Y);
            }        
        }
    }
}
