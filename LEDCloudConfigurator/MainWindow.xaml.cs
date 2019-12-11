using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using System.IO;
using System.Collections.ObjectModel;

namespace LEDCloudConfigurator
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Thunder firsthunder = new Thunder("MEDIUM1.wav", ThunderType.Medium);
        public Thunder secondthunder = new Thunder("MEDIUM2.wav", ThunderType.Medium);
        public List<Thunder> ThunderList = new List<Thunder>();
        public MyColor CurrentColor = new MyColor();
        public ObservableCollection<Thunder> Thunders;
        public List<MyColor> Colorlist = new List<MyColor>();
        public ObservableCollection<MyColor> Colors;


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            ColorManagement.DataContext = CurrentColor;
            datagrid.DataContext = Colorlist;
            Colorlist.Add(new MyColor());
            Colorlist.Add(new MyColor());
            Colorlist.Add(new MyColor());
            Colors = new ObservableCollection<MyColor>(Colorlist);


            List<ThunderFX> script1 = new List<ThunderFX>();
            script1.Add(new ThunderFX(600, FX.BigFlash));
            script1.Add(new ThunderFX(800, FX.SingleFlash));
            script1.Add(new ThunderFX(900, FX.GroupFlash));
            firsthunder.Script = new List<ThunderFX>(script1);
            script1.Add(new ThunderFX(12000, FX.SingleFlash));
            script1.Add(new ThunderFX(1500, FX.GroupFlash));
            script1.Add(new ThunderFX(1500, FX.GroupFlash));
            script1.Add(new ThunderFX(1500, FX.GroupFlash));
            script1.Add(new ThunderFX(1500, FX.GroupFlash));
            script1.Add(new ThunderFX(1500, FX.GroupFlash));
            script1.Add(new ThunderFX(1500, FX.GroupFlash));
            script1.Add(new ThunderFX(1500, FX.GroupFlash));
            script1.Add(new ThunderFX(1500, FX.GroupFlash));
            script1.Add(new ThunderFX(1500, FX.GroupFlash));
            script1.Add(new ThunderFX(1500, FX.GroupFlash));
            script1.Add(new ThunderFX(1500, FX.GroupFlash));
            secondthunder.Script = new List<ThunderFX>(script1);
            Thunders = new ObservableCollection<Thunder>(ThunderList);
        }

        private void actionbtn_Click(object sender, RoutedEventArgs e)
        {
            string buffer = "";
            foreach (Thunder thunder in ThunderList)
            {
                //CloudMessage tempCM = new CloudMessage(thunder);
                MemoryStream stream = new MemoryStream();
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Thunder));
                serializer.WriteObject(stream, thunder);
                stream.Position = 0;
                StreamReader sr = new StreamReader(stream);
                buffer += sr.ReadToEnd() + '\n';
            }
            File.WriteAllText(@"./output.txt", buffer);
            //outputbox.Text = File.ReadAllText(@"./output.txt");
        }

        private void remoteBtn_Click(object sender, RoutedEventArgs e)
        {
            Button Sndr = sender as Button;
            if (Sndr.CommandParameter == null) return;
            string parameter = Sndr.CommandParameter.ToString();
            if (parameter.Contains("IR"))
            {
                int ButtonID = int.Parse(parameter.Substring(2));
            }
        }

        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            switch (((Slider)sender).Name)
            {
                case "HSlider":
                    CurrentColor.H = (float)HSlider.Value;
                    break;
                case "SSlider":
                    CurrentColor.S = (float)SSlider.Value;
                    break;
                case "VSlider":
                    CurrentColor.V = (float)VSlider.Value;
                    break;

                default:
                    break;
            }
            ColorViewer.Background = new SolidColorBrush(Color.FromRgb(CurrentColor.r, CurrentColor.g, CurrentColor.b));
        }
    }
}
