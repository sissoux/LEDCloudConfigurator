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
using System.Globalization;
using System.Diagnostics;

namespace LEDCloudConfigurator
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Thunder firsthunder = new Thunder("MEDIUM1.wav", ThunderType.Medium);
        public Thunder secondthunder = new Thunder("MEDIUM2.wav", ThunderType.Medium);
        //public List<Thunder> ThunderList = new List<Thunder>();
        public MyColor CurrentColor = new MyColor();
        public ObservableCollection<Thunder> Thunders { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            ColorManagement.DataContext = CurrentColor;
            /*Binding colorBind = new Binding("Brush");
            colorBind.Source = CurrentColor;
            ColorViewer.SetBinding(Label.BackgroundProperty, colorBind);
            */
            datagrid.DataContext = Thunders;
            ThunderComboBox.DataContext = this;

            Thunders = new ObservableCollection<Thunder>();
            Thunders.Add(firsthunder);
            Thunders.Add(secondthunder);

            firsthunder.Script.Add(new ThunderFX(600, FX.BigFlash));
            firsthunder.Script.Add(new ThunderFX(800, FX.SingleFlash));
            firsthunder.Script.Add(new ThunderFX(900, FX.GroupFlash));
            secondthunder.Script.Add(new ThunderFX(12000, FX.SingleFlash));
            secondthunder.Script.Add(new ThunderFX(1500, FX.GroupFlash));
            secondthunder.Script.Add(new ThunderFX(1500, FX.GroupFlash));
            secondthunder.Script.Add(new ThunderFX(1500, FX.GroupFlash));
            secondthunder.Script.Add(new ThunderFX(1500, FX.GroupFlash));
            secondthunder.Script.Add(new ThunderFX(1500, FX.GroupFlash));
            secondthunder.Script.Add(new ThunderFX(1500, FX.GroupFlash));
            secondthunder.Script.Add(new ThunderFX(1500, FX.GroupFlash));
            secondthunder.Script.Add(new ThunderFX(1500, FX.GroupFlash));
            secondthunder.Script.Add(new ThunderFX(1500, FX.GroupFlash));
            secondthunder.Script.Add(new ThunderFX(1500, FX.GroupFlash));
            secondthunder.Script.Add(new ThunderFX(1500, FX.GroupFlash));

            StatusViewer.Text = "Connect to target and select an available Thunder file to begin, or import a new Thunder file.";

            //datagrid.ItemsSource = Thunders;



        }

        private void actionbtn_Click(object sender, RoutedEventArgs e)
        {
            string buffer = "";
            foreach (Thunder thunder in Thunders)
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
        }

        private void NewThunderSelected(object sender, SelectionChangedEventArgs e)
        {
            object Item = ((ComboBox)sender).SelectedItem;
            if (Item == null)
            {
                return;
            }

            if (Item is Thunder)
            {
                datagrid.ItemsSource = (Item as Thunder).Script;
            }
            return;
        }

        private void AddEvent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (ThunderComboBox.SelectedItem as Thunder).Script.Add(new ThunderFX());
            }
            catch (Exception ex)
            {
                StatusViewer.Text= ex.Message;
            }
        }
    }

    public class SliderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (parameter.ToString())
            {
                case "H":
                    return Math.Round((float)value, 0).ToString()+"°";
                case "SV":
                    return Math.Round((float)value*100, 1).ToString() + "%";

                default:
                    return value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BindingDebugger : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debugger.Break();
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debugger.Break();
            throw new NotImplementedException();
        }
    }
}
