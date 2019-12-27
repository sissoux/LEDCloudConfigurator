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
using System.Threading;
using System.IO.Ports;

namespace LEDCloudConfigurator
{
    public partial class MainWindow : Window
    {
        public MyColor CurrentColor = new MyColor();
        public ObservableCollection<Thunder> Thunders { get; set; }
        private int ThunderEventIndex = 0;
        private SoundPlayer soundPlayer = new SoundPlayer();
        LEDCloud myCloud;
        volatile static string SerialBuffer = "";

        bool AutoScroll = true;

        public MainWindow()
        {
            InitializeComponent();
            initProperties();
        }

        private void initProperties()
        {
            var dueTime = TimeSpan.FromMilliseconds(10);
            var interval = TimeSpan.FromMilliseconds(10);
            RunPeriodicAsync(OnTick, dueTime, interval, CancellationToken.None);

            myCloud = new LEDCloud(SerialPort);
            this.DataContext = this;
            ColorManagement.DataContext = CurrentColor;
            datagrid.DataContext = Thunders;
            ThunderComboBox.DataContext = this;

            Thunders = new ObservableCollection<Thunder>();
            soundPlayer.StreamChanged += new EventHandler(player_streamChanged);
            parseThunderFile(AppDomain.CurrentDomain.BaseDirectory + "SD\\Thunders.txt");
            SerialPort.Port.DataReceived += new SerialDataReceivedEventHandler(SerialDataReceivedHandler);

        }
        
        private static void SerialDataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = sender as SerialPort;
            while (sp.BytesToRead > 0)
            {
                SerialBuffer += sp.ReadLine();
            }
        }
        
        private void OnTick()
        {
            SerialViewer.Content = SerialBuffer;
            if (liveUpdateEnable.IsChecked == true)
            {
                try
                {
                    myCloud.sendCommand(new CloudMessage(CurrentColor));
                }
                catch (Exception ex)
                {
                    StatusViewer.Text = ex.Message;
                }
            }
        }
        private void ScrollViewer_ScrollChanged(Object sender, ScrollChangedEventArgs e)
        {
            // User scroll event : set or unset auto-scroll mode
            if (e.ExtentHeightChange == 0)
            {   // Content unchanged : user scroll event
                if (SerialViewer.VerticalOffset == SerialViewer.ScrollableHeight)
                {   // Scroll bar is in bottom
                    // Set auto-scroll mode
                    AutoScroll = true;
                }
                else
                {   // Scroll bar isn't in bottom
                    // Unset auto-scroll mode
                    AutoScroll = false;
                }
            }

            // Content scroll event : auto-scroll eventually
            if (AutoScroll && e.ExtentHeightChange != 0)
            {   // Content changed and auto-scroll mode set
                // Autoscroll
                SerialViewer.ScrollToVerticalOffset(SerialViewer.ExtentHeight);
            }
        }

        private void ScriptEvent()
        {
            if ((ThunderComboBox.SelectedItem as Thunder).Script.Count > ThunderEventIndex)
            {
                var evnt = (ThunderComboBox.SelectedItem as Thunder).Script[ThunderEventIndex];
                if (SerialPort.Port.IsOpen)
                {
                    switch (evnt.fX)
                    {
                        case FX.SingleFlash:
                            myCloud.sendCommand(new CloudMessage(Command.SingleFlash));
                            break;
                        case FX.BigFlash:
                            myCloud.sendCommand(new CloudMessage(Command.BigFlash));
                            break;
                        case FX.GroupFlash:
                            myCloud.sendCommand(new CloudMessage(Command.GroupFlash));
                            break;
                        case FX.MegaFlash:
                            myCloud.sendCommand(new CloudMessage(Command.MegaFlash));
                            break;
                        default:
                            break;
                    }
                }
                StatusViewer.Text = evnt.timestamp.ToString() + " " + evnt.fX.ToString();
                ThunderEventIndex++;
                if ((ThunderComboBox.SelectedItem as Thunder).Script.Count <= ThunderEventIndex)
                {
                    StatusViewer.Text = "Done";
                    return;
                }
                var nextEvnt = (ThunderComboBox.SelectedItem as Thunder).Script[ThunderEventIndex];
                TimeSpan nextEventIn = TimeSpan.FromMilliseconds(nextEvnt.timestamp - evnt.timestamp);
                ScriptTaskStep(ScriptEvent, nextEventIn, CancellationToken.None);
            }
        }

        // The `onTick` method will be called periodically unless cancelled.
        private static async Task ScriptTaskStep(Action onTick, TimeSpan dueTime, CancellationToken token)
        {
            // Initial wait time before we begin the periodic loop.
            if (dueTime > TimeSpan.Zero)
                await Task.Delay(dueTime, token);
            if (!token.IsCancellationRequested) onTick?.Invoke();
        }

        // The `onTick` method will be called periodically unless cancelled.
        private static async Task RunPeriodicAsync(Action onTick, TimeSpan dueTime, TimeSpan interval, CancellationToken token)
        {
            // Initial wait time before we begin the periodic loop.
            if (dueTime > TimeSpan.Zero)
                await Task.Delay(dueTime, token);

            // Repeat this loop until cancelled.
            while (!token.IsCancellationRequested)
            {
                // Call our onTick function.
                onTick?.Invoke();

                // Wait to repeat again.
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval, token);
            }
        }

        private void player_streamChanged(object sender, EventArgs e)
        {
            soundPlayer.LoadAsync();
        }

        private void remoteBtn_Click(object sender, RoutedEventArgs e)
        {
            Button Sndr = sender as Button;
            if (Sndr.CommandParameter == null) return;
            string parameter = Sndr.CommandParameter.ToString();
            try
            {
                if (parameter.Contains("IR"))
                {
                    if (AttributeColor.IsChecked == true) myCloud.sendCommand(new CloudMessage(CurrentColor, int.Parse(parameter.Substring(2))));
                    else myCloud.sendCommand(new CloudMessage(int.Parse(parameter.Substring(2))));
                }
                else if (parameter.Contains("Save"))
                {
                    myCloud.sendCommand(new CloudMessage(Command.saveColors));
                }
            }
            catch (Exception ex)
            {
                StatusViewer.Text = ex.Message;
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
                (ThunderComboBox.SelectedItem as Thunder).addEvent(new ThunderFX());
            }
            catch (Exception ex)
            {
                StatusViewer.Text = ex.Message;
            }
        }

        private void SaveFile_Clic(object sender, RoutedEventArgs e)
        {
            string buffer = "";
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Thunder>));
            serializer.WriteObject(stream, Thunders);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            buffer += sr.ReadToEnd() + '\n';
            try
            {
                string filename;
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Text files (*.txt;*.json)|*.txt;*.json";
                saveDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "SD";
                saveDialog.FileName = "Thunders.txt";
                System.IO.Directory.CreateDirectory(saveDialog.InitialDirectory);
                if (saveDialog.ShowDialog() == true)
                {
                    filename = saveDialog.FileName;
                    File.WriteAllText(filename, buffer);
                }
            }
            catch (Exception ex)
            {
                StatusViewer.Text = ex.Message;
            }
        }

        private void OpenFile_Clic(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result;
            if (Thunders.Count != 0)
                result = MessageBox.Show("This action will overwrite current configuration. Are you sure ?", "Erase notice", MessageBoxButton.OKCancel);
            else result = MessageBoxResult.OK;

            if (result == MessageBoxResult.OK)
            {
                try
                {
                    string filename;
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "Text files (*.txt;*.json)|*.txt;*.json | All files (*.*) | *.*";
                    openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "SD";
                    System.IO.Directory.CreateDirectory(openFileDialog.InitialDirectory);
                    if (openFileDialog.ShowDialog() == true)
                    {
                        parseThunderFile(openFileDialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    StatusViewer.Text = ex.Message;
                }
            }
        }

        private void parseThunderFile(string filename)
        {
            FileStream stream = new FileStream(filename, FileMode.Open);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Thunder>));
            Thunders.Clear();
            (serializer.ReadObject(stream) as List<Thunder>).ToList().ForEach(Thunders.Add);
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
            }
        }

        private void PlayWav_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (soundPlayer == null)
                {
                    StatusViewer.Text = "Load WavFile first";
                    return;
                }
                if (soundPlayer.IsLoadCompleted) soundPlayer.Play();
                ThunderEventIndex = 0;
                if ((ThunderComboBox.SelectedItem as Thunder).Script.Count != 0)
                {
                    ScriptTaskStep(ScriptEvent, TimeSpan.FromMilliseconds((ThunderComboBox.SelectedItem as Thunder).Script[0].timestamp), CancellationToken.None);
                }
            }
            catch (Exception)
            {
                StatusViewer.Text = "Select a Thunder first";
            }
        }

        private void StopWav_Click(object sender, RoutedEventArgs e)
        {
            if (soundPlayer == null)
                return;
            if (soundPlayer.IsLoadCompleted) soundPlayer.Stop();
        }

        private void ColorSendBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                myCloud.sendCommand(new CloudMessage(CurrentColor, (UInt16)900));
            }
            catch (Exception ex)
            {
                StatusViewer.Text = ex.Message;
            }
        }

        private void sendFlash_Click(object sender, RoutedEventArgs e)
        {
            Button Sndr = sender as Button;
            if (Sndr.CommandParameter == null) return;
            string parameter = Sndr.CommandParameter.ToString();
            try
            {
                switch (parameter)
                {
                    case "single":
                        myCloud.sendCommand(new CloudMessage(Command.SingleFlash));
                        break;
                    case "group":
                        myCloud.sendCommand(new CloudMessage(Command.GroupFlash));
                        break;
                    case "mega":
                        myCloud.sendCommand(new CloudMessage(Command.MegaFlash));
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                StatusViewer.Text = ex.Message;
            }
        }

        private void SerialSendBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                myCloud.send(SerialSender.Text);
            }
            catch (Exception)
            {

                throw;
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
}
