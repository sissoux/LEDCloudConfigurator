using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace LEDCloudConfigurator
{
    [DataContract]
    public class CloudMessage
    {
        [DataMember]
        public string filename;

        [DataMember]
        public ThunderType type = ThunderType.Distant;

        [DataMember]
        internal  List<EventList> Script = new List<EventList>();

        public CloudMessage()
        {

        }
        public CloudMessage(Thunder ThunderToSend)
        {
            this.filename = ThunderToSend.Filename;
            this.type = ThunderToSend.Type;
            foreach (ThunderFX thunderFX in ThunderToSend.Script)
            {
                this.Script.Add(new EventList(thunderFX));
            }
        }

    }

    [DataContract]
    internal class EventList
    {
        [DataMember]
        public UInt32 timestamp = 0;
        [DataMember]
        public FX fX = FX.SingleFlash;

        public EventList(ThunderFX thunderFX)
        {

            this.fX = thunderFX.fX;
            this.timestamp = thunderFX.timestamp;
        }
    }
}
