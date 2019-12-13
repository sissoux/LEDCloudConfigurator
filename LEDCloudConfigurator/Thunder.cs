using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
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
        private string filename;

        public string Filename
        {
            get { return filename; }
            set { filename = value; NotifyPropertyChanged(); }
        }
        public string filePath { get; set; }


        private ThunderType type;
        [DataMember]
        public ThunderType Type
        {
            get { return type; }
            set { type = value; NotifyPropertyChanged(); }
        }


        [DataMember]
        public ObservableCollection<ThunderFX> Script { get; set; }


        [DataMember]
        public int NumberOfEvents
        {
            get { return Script.Count(); }
            set { }
        }


        public Thunder(string _filename)
        {
            this.Filename = _filename;
            Script = new ObservableCollection<ThunderFX>();
        }
        public Thunder(string _filename, string _filePath)
        {
            this.Filename = _filename;
            this.filePath = _filePath;
            Script = new ObservableCollection<ThunderFX>();
        }
        public Thunder(string _filename, ThunderType _type)
        {
            this.filename = _filename;
            this.Type = _type;
            Script = new ObservableCollection<ThunderFX>();
        }


        public void LoadWAV(SoundPlayer sp)
        {
            if (this.filePath!=null)
            {
                sp.Stop();
                FileStream file = new FileStream(this.filePath, FileMode.Open);
                sp.Stream = file;
            }
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
