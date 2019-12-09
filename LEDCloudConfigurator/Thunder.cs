using System;
using System.Collections.Generic;
using System.Linq;
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

    public class Thunder
    {
        public
        const int THUNDER_MAX_NUMBER_OF_EVENTS = 30;
        string filename;
        ThunderType type = ThunderType.Distant;
        List<ThunderFX> Script;

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
