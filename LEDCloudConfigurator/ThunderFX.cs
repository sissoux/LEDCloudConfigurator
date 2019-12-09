using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace LEDCloudConfigurator
{

    public enum FX
    {
        SingleFlash,
        BigFlash,
        GroupFlash,
        MegaFlash
    };

    [DataContract(Namespace = "")]
    public class ThunderFX
    {
        [DataMember]
        public UInt32 timestamp = 0;

        [DataMember]
        public FX fX = FX.SingleFlash;

        public ThunderFX(UInt32 _timestamp, FX _fX)
        {
            this.fX = _fX;
            this.timestamp = _timestamp;
        }
    }
}
