using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DrumSimulator.Model;
using Microsoft.Kinect;
using System.Linq;

namespace DrumSimulator
{
    public partial class MainWindow : Window 
    {
        //Instantiate the Kinect runtime. Required to initialize the device.
        //IMPORTANT NOTE: You can pass the device ID here, in case more than one Kinect device is connected.

        SceneController scene;

        public MainWindow()
        {
            this.scene = new SceneController((int)System.Windows.SystemParameters.PrimaryScreenWidth, (int)System.Windows.SystemParameters.PrimaryScreenHeight);
            
            this.DataContext = scene;

            InitializeComponent();
            
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.Unloaded += new RoutedEventHandler(MainWindow_Unloaded);      
        }

        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            this.scene.Stop();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.scene.InitializeKinect();
        }

    }
}
