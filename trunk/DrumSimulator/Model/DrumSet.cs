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
        IDictionary<String, bool> pedalPressed;
        
        public DrumSet(int screenX, int screenY)
        {
            this.drums = new Dictionary<String, Drum>();
            this.pedals = new Dictionary<String, Drum>();
            this.pedalPressed = new Dictionary<String, bool>();

            Drum crash = new Drum(screenY / 10, screenX / 10, "Sounds/crash.wav", "/DrumSimulator;component/Data/Images/crash.png", new Point(screenX / 10, -screenY / 7));
            this.drums.Add("crash", crash);

            Drum snare = new Drum(screenY / 10, screenX / 10, "Sounds/snare.wav", "/DrumSimulator;component/Data/Images/snare.png", new Point(-screenX / 6, 0));
            this.drums.Add("snare", snare);

            Drum hihat = new Drum(screenY / 10, screenX / 10, "Sounds/hihat.wav", "/DrumSimulator;component/Data/Images/hihat.png", new Point(-screenX / 5, -screenY / 5));
            this.drums.Add("hihat", hihat);

            Drum tom1 = new Drum(screenY / 10, screenX / 10, "Sounds/tom.wav", "/DrumSimulator;component/Data/Images/tom.png", new Point(0, screenY / 20));
            this.drums.Add("tom1", tom1);

            Drum tom2 = new Drum(screenY / 10, screenX / 10, "Sounds/tom.wav", "/DrumSimulator;component/Data/Images/tom.png", new Point(-screenX / 15, screenY / 20));
            this.drums.Add("tom2", tom2);

            Drum low = new Drum(screenY / 10, screenX / 10, "Sounds/low.wav", "/DrumSimulator;component/Data/Images/low.png", new Point(screenX / 12, 0));
            this.drums.Add("low", low);

            PedalDrum bass = new PedalDrum(screenY / 5, screenX / 5, "Sounds/bassPedal.wav", "/DrumSimulator;component/Data/Images/bass.png", new Point(screenX / 75, screenY / 4));
            this.pedals.Add("bass", bass);
        }

        private IDictionary<String, Drum> AllDrums()
        {
            IDictionary<String, Drum> all = new Dictionary<String, Drum>();
            foreach (KeyValuePair<String, Drum> pair in this.drums)
            {
                all.Add(pair.Key, pair.Value);
            }
            foreach (KeyValuePair<String, Drum> pair in this.pedals)
            {
                all.Add(pair.Key, pair.Value);
            }
            return all;
        }

        public IEnumerable<Drum> GetDrums()
        {
            IDictionary<String, Drum> all = this.AllDrums();
            foreach (KeyValuePair<String, Drum> pair in all)
            {
                yield return pair.Value;
            }
        }

        private Drum GetDrum(String key)
        {
            Drum result;
            IDictionary<String, Drum> all = this.AllDrums();
            if (all.ContainsKey(key))
            {
                result = all[key];
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
                if (current.Hit(hand))
                {
                    return new DrumHit(current.SoundPath, current.Position, pair.Key);
                }
            }
            return null;
        }

        // so far we only have one pedal
        public DrumHit bassHit(Double ratio)
        {
            PedalDrum bass = (PedalDrum) this.pedals["bass"];
            if (bass.Hit(ratio))
            {
                if (this.pedalPressed["bass"].Equals(false))
                {
                    this.pedalPressed["bass"] = true;
                    return new DrumHit(bass.SoundPath, bass.Position, "bass");
                }
                return null;
            }
            else
            {
                this.pedalPressed["bass"] = false;
                return null;
            }
        }

        private void updateDrumDictionary(ref IDictionary<String, Drum> drumDict, Point body)
        {
            foreach (KeyValuePair<String, Drum> pair in drumDict)
            {
                Drum current = pair.Value;
                drumDict[pair.Key].Position = new Point(body.X + current.Offset.X, body.Y + current.Offset.Y);
            } 
        }

        public void update(Point body) {
            this.updateDrumDictionary(ref this.drums, body);
            this.updateDrumDictionary(ref this.pedals, body);    
        }
    }
}
