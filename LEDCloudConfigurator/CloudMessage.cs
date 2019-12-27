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
        attributeColor,
        callColor,
        saveColors
    };

    [DataContract]
    public class CloudMessage
    {
        [DataMember(IsRequired = true)]
        private String command = null;

        [DataMember(IsRequired = false)]
        byte H;

        [DataMember(IsRequired = false)]
        byte S;

        [DataMember(IsRequired = false)]
        byte V;

        [DataMember(IsRequired = false)]
        UInt16 Delay;

        [DataMember(IsRequired = false)]
        int ButtonID;


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
                case Command.callColor:
                    break;
                default:
                    this.command = null;
                    return;
            }
            this.setCommand(cmd);
        }
        public CloudMessage(MyColor ColorToSend)
        {
            this.H = (byte)ColorToSend.h_b;
            this.S = (byte)ColorToSend.s_b;
            this.V = (byte)ColorToSend.v_b;
            this.setCommand(Command.setToHSV);
        }
        public CloudMessage(MyColor ColorToSend, int ButtonID)
        {
            this.H = (byte)ColorToSend.h_b;
            this.S = (byte)ColorToSend.s_b;
            this.V = (byte)ColorToSend.v_b;
            this.ButtonID = ButtonID;
            this.setCommand(Command.attributeColor);
        }
        public CloudMessage(MyColor ColorToSend, UInt16 fadingTime)
        {
            this.H = (byte)ColorToSend.h_b;
            this.S = (byte)ColorToSend.s_b;
            this.V = (byte)ColorToSend.v_b;
            this.Delay = fadingTime;
            this.setCommand(Command.fadeToHSV);
        }
        public CloudMessage(int ButtonID)
        {
            this.ButtonID = ButtonID;
            this.setCommand(Command.callColor);
        }

        private void setCommand(Command command)
        {
            this.command = command.ToString();
        }


    }

}
