using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEDCloudConfigurator
{
    public class LEDCloud
    {
        private SerialConnector SerialPort;
        private CloudMessage message;

        public MyColor CurrentColor = new MyColor();
        public ObservableCollection<Thunder> Thunders { get; set; }


        public void sendColor()
        {

        }

        private void sendCommand()
        {

        }
    }
}
