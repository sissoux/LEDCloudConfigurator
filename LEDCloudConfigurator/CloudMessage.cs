using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace LEDCloudConfigurator
{

    //Exemple of JSON messages: 
    //{method:fadeToHSV,H:160,S:255,V:255,Delay:2000}
    public enum Command
    {
        fadeToHSV,
        setToHSV,
        SingleFlash,
        BigFlash,
        GroupFlash,
        MegaFlash,
        saveColors
    };

    [DataContract]
    public class CloudMessage
    {
        [DataMember]
        private String command;


        [DataMember(IsRequired = false)]
        public string filename;

        [DataMember(IsRequired = false)]
        public ThunderType type = ThunderType.Distant;

        [DataMember(IsRequired = false)]
        internal  List<EventList> Script = new List<EventList>();

        [DataMember(IsRequired = false)]
        UInt16 H;

        [DataMember(IsRequired = false)]
        UInt16 S;

        [DataMember(IsRequired = false)]
        UInt16 V;
        [DataMember(IsRequired = false)]
        UInt16 Delay;

        public CloudMessage(Command cmd)
        {
            switch (cmd)
            {
                case Command.SingleFlash:
                    break;
                case Command.BigFlash:
                    break;
                case Command.GroupFlash:
                    break;
                case Command.MegaFlash:
                    break;
                case Command.saveColors:
                    break;
                default:
                    this.command = null;
                    return;
            }
            this.setCommand(cmd);
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
        public CloudMessage(MyColor ColorToSend)
        {
            this.H = (UInt16)ColorToSend.H;
            this.S = (UInt16)ColorToSend.S;
            this.V = (UInt16)ColorToSend.V;
            this.setCommand(Command.setToHSV);
        }
        public CloudMessage(MyColor ColorToSend, UInt16 fadingTime)
        {
            this.H = (UInt16)ColorToSend.H;
            this.S = (UInt16)ColorToSend.S;
            this.V = (UInt16)ColorToSend.V;
            this.Delay = fadingTime;
            this.setCommand(Command.fadeToHSV);
        }

        public void setCommand(Command command)
        {
            this.command = command.ToString();
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
