using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.ComponentModel;
using DrumSimulator.Model;
using System.Threading;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;

namespace DrumSimulator
{

    class SceneController : INotifyPropertyChanged
    {
        
        private KinectSensor sensor;
        private Skeleton currentSkeleton;

        private System.Windows.Point bodyCenter;
        private DrumSet drumSet;

        private IDictionary<String, Extremity> extremities;
        private IEnumerable<Extremity> getExtremities()
        {
            foreach (KeyValuePair<String, Extremity> pair in this.extremities)
            {
                Extremity current = pair.Value;
                yield return current;
            }
        }

        public ObservableCollection<Drum> Drums
        {
            get
            {
                return new ObservableCollection<Drum>(this.drumSet.GetDrums());
            }
        }

        public ObservableCollection<Extremity> Extremities
        {
            get
            {
                return new ObservableCollection<Extremity>(this.getExtremities());
            }
        }

        private int screenX;
        private int screenY;
        
        private int frameCounter = 0;
        private bool bassPressed = false;

        public event PropertyChangedEventHandler PropertyChanged;

        private BitmapSource videosource;
        public BitmapSource KinectSource
        {
            get
            {
                return videosource;
            }

            set
            {
                this.videosource = value;
                OnPropertyChanged("KinectSource");
            }
        }

        public SceneController(int sizeX, int sizeY) 
        {
            this.screenX = sizeX;
            this.screenY = sizeY;

            // Create drum set
            this.drumSet = new DrumSet(this.screenX, this.screenY);
            // Initialize extremities
            this.extremities = new Dictionary<String, Extremity>();
            extremities.Add("leftHand", new Extremity("/DrumSimulator;component/Data/Images/drumsticksLeft.png"));
            extremities.Add("rightHand", new Extremity("/DrumSimulator;component/Data/Images/drumsticksRight.png"));
            extremities.Add("leftFoot", new Extremity("/DrumSimulator;component/Data/Images/foot.png"));
            extremities.Add("rightFoot", new Extremity("/DrumSimulator;component/Data/Images/foot.png"));
        }

        public void InitializeKinect() 
        {
            this.sensor = KinectSensor.KinectSensors.First();
            if (this.sensor == null) 
            {
                Console.WriteLine("No sensor detected");
            }
            else 
            {
                Console.WriteLine("Sensor Detected");
                
                this.sensor.ColorStream.Enable();
                this.sensor.SkeletonStream.Enable();
                
                sensor.SkeletonFrameReady += frameUpdate;
                sensor.ColorFrameReady += videoUpdate;

                sensor.Start();
            }
        }

        public void Stop()
        {
            if (this.sensor == null)
            {
                Console.WriteLine("No sensor detected - unable to stop");
            }
            else
            {
                this.sensor.Stop();
                Console.WriteLine("Sensor stopped");
            }
        }

        private void updateDrums()
        {
            ++this.frameCounter;
            if (frameCounter == 5)
            {
                this.drumSet.update(this.bodyCenter);
                // We notify that we update the user position and so the drums position
                OnPropertyChanged("Drums");
                this.frameCounter = 0;
            }

        }

        private void updateBody()
        {
            Double leftHandX = ScaleVector(this.screenX, this.currentSkeleton.Joints[JointType.HandLeft].Position.X);
            Double leftHandY = ScaleVector(this.screenY, -this.currentSkeleton.Joints[JointType.HandLeft].Position.Y);
            this.extremities["leftHand"].Position = new System.Windows.Point(leftHandX, leftHandY);
            
            Double rightHandX = ScaleVector(this.screenX, this.currentSkeleton.Joints[JointType.HandRight].Position.X);
            Double rightHandY = ScaleVector(this.screenY, -this.currentSkeleton.Joints[JointType.HandRight].Position.Y);
            this.extremities["rightHand"].Position = new System.Windows.Point(rightHandX, rightHandY);

            Double leftFootX = ScaleVector(this.screenX, this.currentSkeleton.Joints[JointType.FootLeft].Position.X);
            Double leftFootY = ScaleVector(this.screenY, -this.currentSkeleton.Joints[JointType.FootLeft].Position.Y);
            this.extremities["leftFoot"].Position = new System.Windows.Point(leftFootX, leftFootY);
            Double heightL = this.currentSkeleton.Joints[JointType.FootLeft].Position.Y;
            Double heightR = this.currentSkeleton.Joints[JointType.FootRight].Position.Y;
            Double ratio = heightR / heightL;
            Console.WriteLine(ratio);
            /*if (ratio > 0.7 && ratio < 0.95)
            {
                this.playSound("Sounds/bell.wav");
            }*/

            Double rightFootX = ScaleVector(this.screenX, this.currentSkeleton.Joints[JointType.FootRight].Position.X);
            Double rightFootY = ScaleVector(this.screenY, -this.currentSkeleton.Joints[JointType.FootRight].Position.Y);
            this.extremities["rightFoot"].Position = new System.Windows.Point(rightFootX, rightFootY);

            Double bodyX = ScaleVector(this.screenX, this.currentSkeleton.Joints[JointType.HipCenter].Position.X);
            Double bodyY = ScaleVector(this.screenY, -this.currentSkeleton.Joints[JointType.HipCenter].Position.Y);
            this.bodyCenter = new System.Windows.Point(bodyX, bodyY);

            // We notify the update to the window
            OnPropertyChanged("Extremities");
            OnPropertyChanged("BodyTop");
            OnPropertyChanged("BodyLeft");
        }

        private void checkHits()
        {
            foreach (KeyValuePair<String, Extremity> pair in this.extremities)
            {
                Extremity currentEx = pair.Value;
                DrumHit hit = this.drumSet.hit(currentEx);
                if (hit != null)
                {
                    // We hit a drum
                    if (currentEx.ExtremityState.Equals(Extremity.State.OUT) || !currentEx.HitDrum.Equals(hit.DrumKey))
                    {
                        // We only allow to hit if we were out of the drum or inside another drum
                        this.extremities[pair.Key].ExtremityState = Extremity.State.IN;
                        this.extremities[pair.Key].HitDrum = hit.DrumKey;
                        this.playSound(hit.SoundPath);
                    }
                }
                else
                {
                    // We don't hit a drum, we upate the state
                    this.extremities[pair.Key].ExtremityState = Extremity.State.OUT;
                }
            }
            Double heightL = this.currentSkeleton.Joints[JointType.FootLeft].Position.Y;
            Double heightR = this.currentSkeleton.Joints[JointType.FootRight].Position.Y;
            Double feetRatio = heightR / heightL;
            DrumHit bassHit = this.drumSet.bassHit(feetRatio);
            if (bassHit != null) {
                if (!bassPressed)
                {
                    this.playSound(bassHit.SoundPath);
                    bassPressed = true;
                }
                
            }
            else
            {
                bassPressed = false;
            }

            
        }

        private void frameUpdate(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame SFrame = e.OpenSkeletonFrame())
            {
                if (SFrame == null)
                {
                    // The image processing took too long. More than 2 frames behind.
                }
                else
                {
                    Skeleton[] skeletons = new Skeleton[SFrame.SkeletonArrayLength];
                    SFrame.CopySkeletonDataTo(skeletons);
                    this.currentSkeleton = (from s in skeletons
                                            where s.TrackingState == SkeletonTrackingState.Tracked
                                            select s).FirstOrDefault();
                    if (this.currentSkeleton != null)
                    {
                        this.updateBody();
                        this.updateDrums();
                        this.checkHits();
                    }
                }
            }
        }

        private void videoUpdate(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame CFrame = e.OpenColorImageFrame())
            {
                if (CFrame == null)
                {
                    // The image processing took too long. More than 2 frames behind.
                }
                else
                {
                    byte[] currentVideoData = new byte[CFrame.PixelDataLength];
                    CFrame.CopyPixelDataTo(currentVideoData);
                    this.KinectSource = BitmapSource.Create(640, 480, 96, 96, PixelFormats.Bgr32, null, currentVideoData, 640 * 4);
                }
            }
        }

        #region Body

        public Double BodyLeft
        {
            get
            {
                if (this.currentSkeleton != null)
                {
                    return this.bodyCenter.X;
                }
                else
                {
                    return 0;
                }
            }
        }

        public Double BodyTop
        {
            get
            {
                if (this.currentSkeleton != null)
                {
                    return this.bodyCenter.Y;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

        #region Property Changed

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        #region Auxiliar Functions

        private void playSound(String path)
        {
            SoundEffect sound = SoundEffect.FromStream(TitleContainer.OpenStream(path));
            FrameworkDispatcher.Update();
            sound.Play();
        }

        private float ScaleVector(int length, float position)
        {
            float value = (((((float)length) / 1f) / 2f) * position) + (length / 2);
            if (value > length)
            {
                return (float)length;
            }
            if (value < 0f)
            {
                return 0f;
            }
            return value;
        }

        #endregion
    }
}
