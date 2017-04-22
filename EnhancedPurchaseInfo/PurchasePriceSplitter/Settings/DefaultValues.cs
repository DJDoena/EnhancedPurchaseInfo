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
    public sealed class DefaultValues
    {
        public Boolean ShippingCostSplitAbsolute = true;

        #region Serialization
        private static XmlSerializer s_XmlSerializer;

        [XmlIgnore]
        public static XmlSerializer XmlSerializer
        {
            get
            {
                if (s_XmlSerializer == null)
                {
                    s_XmlSerializer = new XmlSerializer(typeof(DefaultValues));
                }
                return (s_XmlSerializer);
            }
        }

        public static void Serialize(String fileName, DefaultValues instance)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (XmlTextWriter xtw = new XmlTextWriter(fs, Encoding.UTF8))
                {
                    xtw.Formatting = Formatting.Indented;
                    XmlSerializer.Serialize(xtw, instance);
                }
            }
        }

        public void Serialize(String fileName)
        {
            Serialize(fileName, this);
        }

        public static DefaultValues Deserialize(String fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (XmlTextReader xtr = new XmlTextReader(fs))
                {
                    DefaultValues instance;

                    instance = (DefaultValues)(XmlSerializer.Deserialize(xtr));
                    return (instance);
                }
            }
        }
        #endregion
    }
}