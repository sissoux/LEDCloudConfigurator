using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEDCloudConfigurator
{

    public enum FX
    {
        SingleFlash,
        BigFlash,
        GroupFlash,
        MegaFlash
    };
    public class ThunderFX
    {
        public 
        UInt32 timestamp = 0;
        FX fX = FX.SingleFlash;
        public ThunderFX(UInt32 _timestamp, FX _fX)
        {
            this.fX = _fX;
            this.timestamp = _timestamp;
        }
    }
}
