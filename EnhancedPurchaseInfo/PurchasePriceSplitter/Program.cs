using DoenaSoft.DVDProfiler.EnhancedPurchaseInfo.Resources;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    public static class Program
    {
        internal static Settings Settings { get; private set; }

        private static readonly String SettingsFile;

        private static readonly String ApplicationPath;

        static Program()
        {
            ApplicationPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Doena Soft\EnhancedPurchaseInfo\";
            SettingsFile = ApplicationPath + "PurchasePriceSplitter.xml";
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(String[] args)
        {
            //System.Diagnostics.Debugger.Launch();
            if ((args != null) && (args.Length > 0))
            {
                String[] split;

                split = args[0].ToLower().Split('=');
                if ((split.Length == 2) && (split[0] == "/lang"))
                {
                    switch (split[1])
                    {
                        case ("de"):
                        case ("en"):
                            {
                                CultureInfo ci;

                                ci = CultureInfo.GetCultureInfo(split[1]);
                                Texts.Culture = ci;
                                MessageBoxTexts.Culture = ci;
                                break;
                            }
                    }
                }
            }

            if (Directory.Exists(ApplicationPath) == false)
            {
                Directory.CreateDirectory(ApplicationPath);
            }
            if (File.Exists(SettingsFile))
            {
                try
                {
                    Settings = Settings.Deserialize(SettingsFile);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format(MessageBoxTexts.FileCantBeRead, SettingsFile, ex.Message)
                        , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            CreateSettings();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ItemPricesForm());

            try
            {
                Settings.Serialize(SettingsFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(MessageBoxTexts.FileCantBeWritten, SettingsFile, ex.Message)
                    , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void CreateSettings()
        {
            if (Settings == null)
            {
                Settings = new Settings();
            }
            if (Settings.DefaultValues == null)
            {
                Settings.DefaultValues = new DefaultValues();
            }
        }
    }
}