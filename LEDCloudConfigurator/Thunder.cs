using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public class Thunder : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public const int THUNDER_MAX_NUMBER_OF_EVENTS = 30;
        [DataMember]
        public string filename;

        [DataMember]
        public ThunderType type = ThunderType.Distant;

        [DataMember]
        public List<ThunderFX> Script;

        [DataMember]
        public int NumberOfEvents
        {
            get { return Script.Count(); }
            set { }
        }


        public Thunder(string _filename)
        {
            this.filename = _filename; 
            NotifyPropertyChanged();
        }
        public Thunder(string _filename, ThunderType _type)
        {
            this.filename = _filename;
            this.type = _type;
            NotifyPropertyChanged();
        }

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
