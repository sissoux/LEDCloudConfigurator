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
using Microsoft.Win32;
using System.Media;
using System.ComponentModel;
using System.Windows.Media.Animation;

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
        private SoundPlayer soundPlayer = new SoundPlayer();
        private Storyboard storyboard;
        private DoubleAnimation myAnim = new DoubleAnimation();

        public MainWindow()
        {
            InitializeComponent(); 
            FLASHER.Loaded += new RoutedEventHandler(flasherLoaded);
            this.DataContext = this;
            ColorManagement.DataContext = CurrentColor;

            datagrid.DataContext = Thunders;
            ThunderComboBox.DataContext = this;

            Thunders = new ObservableCollection<Thunder>();
            soundPlayer.StreamChanged += new EventHandler(player_streamChanged);

            StatusViewer.Text = "";
            myAnim.From = 1.0;
            myAnim.To = 0.0;
            myAnim.Duration = new Duration(TimeSpan.FromSeconds(1));
            storyboard = new Storyboard();
            storyboard.Children.Add(myAnim);
            Storyboard.SetTargetName(myAnim, FLASHER.Name);
            Storyboard.SetTargetProperty(myAnim, new PropertyPath(Rectangle.OpacityProperty));



        }
        private void flasherLoaded(object sender, RoutedEventArgs e)
        {
            //storyboard.Begin(this);
        }

        private void player_streamChanged(object sender, EventArgs e)
        {
            soundPlayer.LoadAsync();
        }

        private void actionbtn_Click(object sender, RoutedEventArgs e)
        {
            string buffer = "";
            //CloudMessage tempCM = new CloudMessage(thunder);
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Thunder>));
            serializer.WriteObject(stream, Thunders);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            buffer += sr.ReadToEnd() + '\n';
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
                Thunder SelectedThunder = Item as Thunder;
                datagrid.ItemsSource = SelectedThunder.Script;
                SelectedThunder.LoadWAV(soundPlayer);
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
                StatusViewer.Text = ex.Message;
            }
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            try
            {
                string filename;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Text files (*.txt;*.json)|*.txt;*.json | All files (*.*) | *.*";
                openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (openFileDialog.ShowDialog() == true)
                {
                    filename = openFileDialog.FileName;
                    FileStream stream = new FileStream(filename, FileMode.Open);
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Thunder>));
                    List<Thunder> ImportedList = serializer.ReadObject(stream) as List<Thunder>;
                }

            }
            catch (Exception ex)
            {
                StatusViewer.Text = ex.Message;
            }
        }

        private void AddThunderFromFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Wavfilename;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Audio wave files (*.wav)|*.wav";
                openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (openFileDialog.ShowDialog() == true)
                {
                    Wavfilename = openFileDialog.FileName;
                    FileStream stream = new FileStream(Wavfilename, FileMode.Open);
                    WAVParser WAV = new WAVParser(stream);
                    if (!WAV.isValidWavefile(AudioFormat.PCM, 44100, 16))
                        throw new Exception("Invalid WAVE audio format. Must be 16 bits PCM at 44100 kHz.");

                    Thunders.Add(new Thunder(System.IO.Path.GetFileName(Wavfilename), Wavfilename));
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                StatusViewer.Text = ex.Message;
                myAnim.Completed += MyAnim_Completed;
            }
        }

        private void MyAnim_Completed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PlayWav_Click(object sender, RoutedEventArgs e)
        {
            if (soundPlayer == null)
            {
                StatusViewer.Text = "Load WavFile first";
                return;
            }
            if (soundPlayer.IsLoadCompleted) soundPlayer.Play();
            storyboard.Begin(this);
        }

        private void StopWav_Click(object sender, RoutedEventArgs e)
        {
            if (soundPlayer == null)
                return;
            if (soundPlayer.IsLoadCompleted) soundPlayer.Stop();
        }
    }

    public class SliderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (parameter.ToString())
            {
                case "H":
                    return Math.Round((float)value, 0).ToString() + "°";
                case "SV":
                    return Math.Round((float)value * 100, 1).ToString() + "%";

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
