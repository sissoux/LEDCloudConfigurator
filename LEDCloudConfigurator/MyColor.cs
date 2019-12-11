using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media;
using System.Runtime.CompilerServices;

namespace LEDCloudConfigurator
{
    public class MyColor : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private float h = 0;
        private float s = 1;
        private float v = 1;
        public byte r = 0, g = 0, b = 0;



        public float H
        {
            get { return h; }
            set
            {
                h = value;
                toRGB();
                NotifyPropertyChanged();
            }
        }
        public float S
        {
            get { return s; }
            set
            {
                s = value;
                toRGB();
                NotifyPropertyChanged();
            }
        }
        public float V
        {
            get { return v; }
            set
            {
                v = value;
                toRGB();
                NotifyPropertyChanged();
            }
        }


        public void toRGB()
        {
            if (h < 0f || h > 360f)
                throw new ArgumentOutOfRangeException(nameof(h), h, "Hue must be in the range [0,360]");
            if (s < 0f || s > 1f)
                throw new ArgumentOutOfRangeException(nameof(s), s, "Saturation must be in the range [0,1]");
            if (v < 0f || v > 1f)
                throw new ArgumentOutOfRangeException(nameof(v), v, "Value must be in the range [0,1]");

            byte Component(int n)
            {
                var k = (n + h / 60f) % 6;
                var c = v - v * s * Math.Max(Math.Min(Math.Min(k, 4 - k), 1), 0);
                var b = (int)Math.Round(c * 255);
                return (byte)(b < 0 ? 0 : b > 255 ? 255 : b);
            }
            r = Component(5);
            g = Component(3);
            b = Component(1);
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
