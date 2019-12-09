using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LEDCloudConfigurator
{

    public enum ThunderType
    {
        Distant,
        Medium,
        Heavy,
        VeryHeavy
    };

    [DataContract(Namespace = "")]
    public class Thunder
    {
        public const int THUNDER_MAX_NUMBER_OF_EVENTS = 30;
        [DataMember]
        public string filename;

        [DataMember]
        public ThunderType type = ThunderType.Distant;

        [DataMember]
        public List<ThunderFX> Script;

        public Thunder(string _filename)
        {
            this.filename = _filename;
        }
        public Thunder(string _filename, ThunderType _type)
        {
            this.filename = _filename;
            this.type = _type;
        }
    }

}
