using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
    public class ThunderFX : INotifyPropertyChanged
    {
        private UInt32 _timestamp = 0;
        [DataMember]
        public UInt32 timestamp
        {
            get { return _timestamp; }
            set
            {
                _timestamp = value;
                NotifyPropertyChanged("timestamp");
            }
        }

        private FX _fx = FX.SingleFlash;

        [DataMember]
        public FX fX
        {
            get { return _fx; }
            set
            {
                _fx = value;
                NotifyPropertyChanged("timestamp");
            }
        }



        public ThunderFX(UInt32 _timestamp, FX _fX)
        {
            this.fX = _fX;
            this.timestamp = _timestamp;
        }

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

}
