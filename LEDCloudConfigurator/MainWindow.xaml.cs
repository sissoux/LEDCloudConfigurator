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

namespace LEDCloudConfigurator
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thunder firsthunder = new Thunder("MEDIUM1.wav", ThunderType.Medium);
        Thunder secondthunder = new Thunder("MEDIUM2.wav", ThunderType.Medium);
        List<Thunder> ThunderList = new List<Thunder>();
        public MainWindow()
        {
            InitializeComponent();
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
            ThunderList.Add(firsthunder);
            ThunderList.Add(secondthunder);
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
                buffer += sr.ReadToEnd() + '\n' + '\n';
            }
            outputbox.Text = buffer;
        }
    }
}
