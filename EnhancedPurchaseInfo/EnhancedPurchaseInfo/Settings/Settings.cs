using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    [ComVisible(false)]
    [Serializable]
    public sealed class Settings
    {
        public string CurrentVersion;

        public DefaultValues DefaultValues;

        #region Serialization

        private static XmlSerializer s_XmlSerializer;

        [XmlIgnore]
        public static XmlSerializer XmlSerializer
        {
            get
            {
                if (s_XmlSerializer == null)
                {
                    s_XmlSerializer = new XmlSerializer(typeof(Settings));
                }

                return s_XmlSerializer;
            }
        }

        public static void Serialize(string fileName, Settings instance)
        {
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (var xtw = new XmlTextWriter(fs, Encoding.UTF8))
                {
                    xtw.Formatting = Formatting.Indented;

                    XmlSerializer.Serialize(xtw, instance);
                }
            }
        }

        public void Serialize(string fileName)
        {
            Serialize(fileName, this);
        }

        public static Settings Deserialize(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (XmlTextReader xtr = new XmlTextReader(fs))
                {
                    var instance = (Settings)(XmlSerializer.Deserialize(xtr));

                    return instance;
                }
            }
        }

        #endregion
    }
}