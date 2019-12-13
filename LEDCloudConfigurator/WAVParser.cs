using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEDCloudConfigurator
{
    ///     [Bloc de déclaration d'un fichier au format WAVE]
    ///0     FileTypeBlocID(4 octets) : Constante «RIFF»  (0x52,0x49,0x46,0x46)
    ///4     FileSize(4 octets) : Taille du fichier moins 8 octets
    ///8     FileFormatID(4 octets) : Format = «WAVE»  (0x57,0x41,0x56,0x45)
    ///     [Bloc décrivant le format audio]
    ///12     FormatBlocID(4 octets) : Identifiant «fmt »  (0x66,0x6D, 0x74,0x20)
    ///16     BlocSize(4 octets) : Nombre d'octets du bloc - 16  (0x10)
    ///20     AudioFormat(2 octets) : Format du stockage dans le fichier(1: PCM, ...)
    ///22     NbrCanaux(2 octets) : Nombre de canaux(de 1 à 6, cf.ci-dessous)
    ///24     Frequence(4 octets) : Fréquence d'échantillonnage (en hertz) [Valeurs standardisées : 11 025, 22 050, 44 100 et éventuellement 48 000 et 96 000]
    ///28     BytePerSec(4 octets) : Nombre d'octets à lire par seconde (c.-à-d., Frequence * BytePerBloc).
    ///32     BytePerBloc(2 octets) : Nombre d'octets par bloc d'échantillonnage(c.-à-d., tous canaux confondus : NbrCanaux* BitsPerSample/8).
    ///34     BitsPerSample(2 octets) : Nombre de bits utilisés pour le codage de chaque échantillon(8, 16, 24)
    ///      [Bloc des données]
    ///36      DataBlocID(4 octets) : Constante «data»  (0x64,0x61,0x74,0x61)
    ///40      DataSize(4 octets) : Nombre d'octets des données (c.-à-d. "Data[]", c.-à-d. taille_du_fichier - taille_de_l'entête(qui fait 44 octets normalement).
    ///


    public enum AudioFormat
    {
        PCM = 1,
        IEEE_FLOAT = 3,
        ALAW = 6,
        MULAW = 7
    }

    public class WAVParser
    {
        public byte[] header;
        

        public AudioFormat Format { get; private set; }
        public UInt16 ChannelCount { get; private set; }
        public UInt16 BitsPerSample { get; private set; }
        public UInt16 BytesPerBlock { get; private set; }
        public UInt32 Frequency { get; private set; }
        public UInt32 BytePerSec { get; private set; }
        public UInt32 DataSize { get; private set; }
        public double FileDuration { get; private set; }
        public string FileTypeBlocID { get; private set; }
        public string FileFormatID { get; private set; }

        public WAVParser(FileStream inputFile)
        {
            header = new byte[44];
            inputFile.Read(header, 0, 44);
            try
            {
                FileTypeBlocID = System.Text.Encoding.UTF8.GetString(header, 0, 4);
                FileFormatID = System.Text.Encoding.UTF8.GetString(header, 8, 4);
                AudioFormat temp;
                Enum.TryParse<AudioFormat>(BitConverter.ToUInt16(header, 20).ToString(), out temp);
                Format = temp;

                ChannelCount = BitConverter.ToUInt16(header, 22);
                BitsPerSample = BitConverter.ToUInt16(header, 34);
                Frequency = BitConverter.ToUInt32(header, 24);
                DataSize = BitConverter.ToUInt32(header, 40);
                BytePerSec = BitConverter.ToUInt32(header, 28);
                BytesPerBlock = BitConverter.ToUInt16(header, 32);

                FileDuration = (double)DataSize / (double)BytePerSec;


            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool isValidWavefile(AudioFormat audioFormat, uint samplingFrequency, uint bytePerSample)
        {
            return samplingFrequency == this.Frequency && bytePerSample == this.BitsPerSample && audioFormat == this.Format;
        }



    }
}
