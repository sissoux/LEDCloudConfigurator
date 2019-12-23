using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace LEDCloudConfigurator
{
    public class LEDCloud
    {
        private SerialConnector Serial;

        public MyColor CurrentColor = new MyColor();
        public ObservableCollection<Thunder> Thunders { get; set; }

        public LEDCloud(SerialConnector SPort)
        {
            this.Serial = SPort;
        }

        public void sendCommand(CloudMessage message)
        {
            try
            {

                if (!this.Serial.Port.IsOpen) this.Serial.connectionChange(null, null);
                string msg = serializeMessage(message);
                if (this.Serial.Port.IsOpen) this.Serial.Port.Write(msg);
                else throw new Exception("Unable to connect to device.");
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unable to connect to device.")
                    throw ex;
                else throw new Exception("Undefined serial port, please select a valid COM port first.");
            }
        }
        public void send(string message)
        {
            try
            {

                if (!this.Serial.Port.IsOpen) this.Serial.connectionChange(null, null);

                if (this.Serial.Port.IsOpen) this.Serial.Port.Write(message);
                else throw new Exception("Unable to connect to device.");
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unable to connect to device.")
                    throw ex;
                else throw new Exception("Undefined serial port, please select a valid COM port first.");
            }
        }

        private String serializeMessage(CloudMessage message)
        {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(CloudMessage));
            serializer.WriteObject(stream, message);
            stream.Position = 0;
            return new StreamReader(stream).ReadToEnd();
        }
    }
}
