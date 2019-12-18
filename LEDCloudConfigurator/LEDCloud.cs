using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
            if (!this.Serial.Port.IsOpen) this.Serial.connectionChange(null, null);

            if (this.Serial.Port.IsOpen) this.Serial.Port.Write(serializeMessage(message));
            else throw new Exception("Unable to connect to device.");
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
