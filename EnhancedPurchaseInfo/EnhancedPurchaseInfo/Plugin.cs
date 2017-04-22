using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;
using DoenaSoft.DVDProfiler.EnhancedPurchaseInfo.Resources;
using DoenaSoft.ToolBox.Extensions;
using Invelos.DVDProfilerPlugin;

namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    [Guid(ClassGuid.ClassID), ComVisible(true)]
    public class Plugin : IDVDProfilerPlugin, IDVDProfilerPluginInfo, IDVDProfilerDataAwarePlugin
    {
        private readonly String SettingsFile;

        private readonly String ErrorFile;

        private readonly String ApplicationPath;

        internal IDVDProfilerAPI Api { get; private set; }

        internal Settings Settings { get; private set; }

        internal Dictionary<String, CurrencyInfo> Currencies { get; private set; }

        internal CurrencyInfo DefaultCurrency { get; set; }

        private String CurrentProfileId;

        private Boolean DatabaseRestoreRunning = false;

        internal readonly Dictionary<String, UInt16> CouponTypes;
        internal Boolean IsRemoteAccess { get; private set; }

        #region MenuTokens

        private String DvdMenuToken = "";

        private const Int32 DvdMenuId = 1;

        private String PersonalizeScreenToken = "";

        private const Int32 PersonalizeScreenId = 11;

        private String CollectionExportToXmlMenuToken = "";

        private const Int32 CollectionExportToXmlMenuId = 21;

        private String CollectionImportMenuToken = "";

        private const Int32 CollectionImportMenuId = 22;

        private String CollectionExportToCsvMenuToken = "";

        private const Int32 CollectionExportToCsvMenuId = 23;

        private String CollectionFlaggedExportToXmlMenuToken = "";

        private const Int32 CollectionFlaggedExportToXmlMenuId = 31;

        private String CollectionFlaggedImportMenuToken = "";

        private const Int32 CollectionFlaggedImportMenuId = 32;

        private String CollectionFlaggedExportToCsvMenuToken = "";

        private const Int32 CollectionFlaggedExportToCsvMenuId = 33;

        private String ToolsOptionsMenuToken = "";

        private const Int32 ToolsOptionsMenuId = 41;

        private String ToolsExportOptionsMenuToken = "";

        private const Int32 ToolsExportOptionsMenuId = 42;

        private String ToolsImportOptionsMenuToken = "";

        private const Int32 ToolsImportOptionsMenuId = 43;

        private String ToolsOpenCalculatorMenuToken = "";

        private const Int32 ToolsOpenCalculatorMenuId = 44;

        #endregion

        public Plugin()
        {
            CouponTypes = new Dictionary<String, UInt16>();
            ApplicationPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Doena Soft\EnhancedPurchaseInfo\";
            SettingsFile = ApplicationPath + "EnhancedPurchaseInfo.xml";
            ErrorFile = Environment.GetEnvironmentVariable("TEMP") + @"\EnhancedPurchaseInfoCrash.xml";
        }

        #region I.. Members

        #region IDVDProfilerPlugin Members

        public void Load(IDVDProfilerAPI api)
        {
            //System.Diagnostics.Debugger.Launch();

            Api = api;

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

            EnsureSettingsAndSetUILanguage();

            LoadCurrencies();

            SetIsRemoteAccess();

            Api.RegisterForEvent(PluginConstants.EVENTID_FormCreated);
            Api.RegisterForEvent(PluginConstants.EVENTID_FormDestroyed);

            Api.RegisterForEvent(PluginConstants.EVENTID_DatabaseOpened);

            Api.RegisterForEvent(PluginConstants.EVENTID_DVDPersonalizeShown);

            Api.RegisterForEvent(PluginConstants.EVENTID_RestoreStarting);
            Api.RegisterForEvent(PluginConstants.EVENTID_RestoreFinished);
            Api.RegisterForEvent(PluginConstants.EVENTID_RestoreCancelled);

            DvdMenuToken = Api.RegisterMenuItemA(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                , "DVD", Texts.EPI, DvdMenuId, "", PluginConstants.SHORTCUT_KEY_A + 15, PluginConstants.SHORTCUT_MOD_Ctrl + PluginConstants.SHORTCUT_MOD_Shift, false);

            CollectionExportToXmlMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                , @"Collection\" + Texts.EPI, Texts.ExportToXml, CollectionExportToXmlMenuId);
            if (IsRemoteAccess == false)
            {
                CollectionImportMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                    , @"Collection\" + Texts.EPI, Texts.ImportFromXml, CollectionImportMenuId);
            }
            CollectionExportToCsvMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                , @"Collection\" + Texts.EPI, Texts.ExportToExcel, CollectionExportToCsvMenuId);

            CollectionFlaggedExportToXmlMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                , @"Collection\Flagged\" + Texts.EPI, Texts.ExportToXml, CollectionFlaggedExportToXmlMenuId);
            if (IsRemoteAccess == false)
            {
                CollectionFlaggedImportMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                   , @"Collection\Flagged\" + Texts.EPI, Texts.ImportFromXml, CollectionFlaggedImportMenuId);
            }
            CollectionFlaggedExportToCsvMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                , @"Collection\Flagged\" + Texts.EPI, Texts.ExportToExcel, CollectionFlaggedExportToCsvMenuId);

            ToolsOptionsMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                , @"Tools\" + Texts.EPI, Texts.Options, ToolsOptionsMenuId);
            ToolsExportOptionsMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                , @"Tools\" + Texts.EPI, Texts.ExportOptions, ToolsExportOptionsMenuId);
            ToolsImportOptionsMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                , @"Tools\" + Texts.EPI, Texts.ImportOptions, ToolsImportOptionsMenuId);
            ToolsOpenCalculatorMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                , @"Tools\" + Texts.EPI, Texts.ShippingCostCalculator, ToolsOpenCalculatorMenuId);

            RegisterCustomFields();
        }

        public void Unload()
        {
            Api.UnregisterMenuItem(DvdMenuToken);

            Api.UnregisterMenuItem(CollectionExportToXmlMenuToken);
            Api.UnregisterMenuItem(CollectionImportMenuToken);
            Api.UnregisterMenuItem(CollectionExportToCsvMenuToken);

            Api.UnregisterMenuItem(CollectionFlaggedExportToXmlMenuToken);
            Api.UnregisterMenuItem(CollectionFlaggedImportMenuToken);
            Api.UnregisterMenuItem(CollectionFlaggedExportToCsvMenuToken);

            Api.UnregisterMenuItem(ToolsOptionsMenuToken);
            Api.UnregisterMenuItem(ToolsExportOptionsMenuToken);
            Api.UnregisterMenuItem(ToolsImportOptionsMenuToken);
            Api.UnregisterMenuItem(ToolsOpenCalculatorMenuToken);

            try
            {
                Settings.Serialize(SettingsFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(MessageBoxTexts.FileCantBeWritten, SettingsFile, ex.Message)
                    , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Api = null;
        }

        public void HandleEvent(Int32 EventType, Object EventData)
        {
            try
            {
                switch (EventType)
                {
                    case (PluginConstants.EVENTID_CustomMenuClick):
                        {
                            HandleMenuClick((Int32)EventData);
                            break;
                        }
                    case (PluginConstants.EVENTID_FormCreated):
                        {
                            if ((Int32)EventData == PluginConstants.FORMID_Personalize)
                            {
                                PersonalizeScreenToken = Api.RegisterMenuItemA(PluginConstants.FORMID_Personalize, PluginConstants.MENUID_Form
                                    , Texts.EPI, Texts.EPI, PersonalizeScreenId, "", PluginConstants.SHORTCUT_KEY_A + 15, PluginConstants.SHORTCUT_MOD_Ctrl + PluginConstants.SHORTCUT_MOD_Shift, false);
                            }
                            break;
                        }
                    case (PluginConstants.EVENTID_FormDestroyed):
                        {
                            if ((Int32)EventData == PluginConstants.FORMID_Personalize)
                            {
                                if (String.IsNullOrEmpty(PersonalizeScreenToken) == false)
                                {
                                    Api.UnregisterMenuItem(PersonalizeScreenToken);
                                }
                                CurrentProfileId = null;
                            }
                            break;
                        }
                    case (PluginConstants.EVENTID_RestoreStarting):
                        {
                            DatabaseRestoreRunning = true;
                            RegisterCustomFields();
                            break;
                        }
                    case (PluginConstants.EVENTID_DatabaseOpened):
                    case (PluginConstants.EVENTID_RestoreFinished):
                    case (PluginConstants.EVENTID_RestoreCancelled):
                        {
                            DatabaseRestoreRunning = false;
                            RegisterCustomFields();
                            break;
                        }
                    case (PluginConstants.EVENTID_DVDPersonalizeShown):
                        {
                            //System.Diagnostics.Debugger.Launch();
                            CurrentProfileId = (String)EventData;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    MessageBox.Show(String.Format(MessageBoxTexts.CriticalError, ex.Message, ErrorFile)
                        , MessageBoxTexts.CriticalErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    if (File.Exists(ErrorFile))
                    {
                        File.Delete(ErrorFile);
                    }
                    LogException(ex);
                }
                catch (Exception inEx)
                {
                    MessageBox.Show(String.Format(MessageBoxTexts.FileCantBeWritten, ErrorFile, inEx.Message)
                        , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region IDVDProfilerPluginInfo Members

        public String GetName()
        {
            return (Texts.PluginName);
        }

        public String GetDescription()
        {
            return (Texts.PluginDescription);
        }

        public String GetAuthorName()
        {
            return ("Doena Soft.");
        }

        public String GetAuthorWebsite()
        {
            return (Texts.PluginUrl);
        }

        public Int32 GetPluginAPIVersion()
        {
            return (PluginConstants.API_VERSION);
        }

        public Int32 GetVersionMajor()
        {
            Version version;

            version = System.Reflection.Assembly.GetAssembly(GetType()).GetName().Version;
            return (version.Major);
        }

        public Int32 GetVersionMinor()
        {
            Version version;

            version = System.Reflection.Assembly.GetAssembly(GetType()).GetName().Version;
            return (version.Minor * 100 + version.Build * 10 + version.Revision);
        }

        #endregion

        #region IDVDProfilerDataAwarePlugin

        public Boolean ExportsCustomDataXML()
        {
            Boolean exportsXml;

            exportsXml = ((Settings.DefaultValues.ExportToCollectionXml)
                && (DatabaseRestoreRunning == false));
            return (exportsXml);
        }

        public String GetCustomDataXMLForDVD(IDVDInfo SourceDVD)
        {
            #region Prices

            Decimal op;
            Boolean hasOP;
            Decimal sc;
            Boolean hasSC;
            Decimal ccc;
            Boolean hasCCC;
            Decimal ccf;
            Boolean hasCCF;
            Decimal discount;
            Boolean hasDiscount;
            Decimal cf;
            Boolean hasCF;
            String ct;
            Boolean hasCT;
            String cc;
            Boolean hasCC;
            Decimal ap1;
            Boolean hasAP1;
            Decimal ap2;
            Boolean hasAP2;

            #endregion

            #region Dates

            Boolean hasOD;
            DateTime od;
            Boolean hasSD;
            DateTime sd;
            Boolean hasDD;
            DateTime dd;
            Boolean hasAD1;
            DateTime ad1;
            Boolean hasAD2;
            DateTime ad2;

            #endregion

            String xml;
            PriceManager priceManager;
            TextManager textManager;
            DateManager dateManager;

            if (Settings.DefaultValues.ExportToCollectionXml == false)
            {
                return (String.Empty);
            }
            else if (DatabaseRestoreRunning)
            {
                return (String.Empty);
            }

            priceManager = new PriceManager(SourceDVD);
            textManager = new TextManager(SourceDVD);
            dateManager = new DateManager(SourceDVD);

            #region Prices

            hasOP = priceManager.GetOriginalPrice(out op);
            hasSC = priceManager.GetShippingCost(out sc);
            hasCCC = priceManager.GetCreditCardCharge(out ccc);
            hasCCF = priceManager.GetCreditCardFees(out ccf);
            hasDiscount = priceManager.GetDiscount(out discount);
            hasCF = priceManager.GetCustomsFees(out cf);
            hasCT = textManager.GetCouponType(out ct);
            hasCC = textManager.GetCouponCode(out cc);
            hasAP1 = priceManager.GetAdditionalPrice1(out ap1);
            hasAP2 = priceManager.GetAdditionalPrice2(out ap2);

            #endregion

            #region Dates

            hasOD = dateManager.GetOrderDate(out od);
            hasSD = dateManager.GetShippingDate(out sd);
            hasDD = dateManager.GetDeliveryDate(out dd);
            hasAD1 = dateManager.GetAdditionalDate1(out ad1);
            hasAD2 = dateManager.GetAdditionalDate2(out ad2);

            #endregion

            IEnumerable<Boolean> hasPrices = ObjectExtensions.Enumerate(hasOP, hasSC, hasCCC, hasCCF, hasDiscount, hasCF, hasCT, hasCC, hasAP1, hasAP2);

            IEnumerable<Boolean> hasDates = ObjectExtensions.Enumerate(hasOD, hasSD, hasDD, hasAD1, hasAD2);

            if (hasPrices.HasItemsWhere(price => price) || hasDates.HasItemsWhere(date => date))
            {
                StringBuilder sb;

                sb = new StringBuilder("<EnhancedPurchaseInfo>");
                AppendPrices(sb, hasOP, op, hasSC, sc, hasCCC, ccc, hasCCF, ccf, hasDiscount, discount, hasCF, cf, hasCT, ct
                    , hasCC, cc, hasAP1, ap1, hasAP2, ap2, priceManager);
                AppendDates(sb, hasOD, od, hasSD, sd, hasDD, dd, hasAD1, ad1, hasAD2, ad2);
                sb.Append("</EnhancedPurchaseInfo>");
                xml = sb.ToString();
            }
            else
            {
                xml = String.Empty;
            }

            return (xml);
        }

        public String GetHTMLForDPVarsFunctionSection()
        {
            return (String.Empty);
        }

        public String GetHTMLForDPVarsDataSection(IDVDInfo SourceDVD, IDVDInfo CompareDVD)
        {
            return (String.Empty);
        }

        public String GetHTMLForTag(String TagName
            , String FullTag
            , IDVDInfo SourceDVD
            , IDVDInfo CompareDVD
            , out Boolean Handled)
        {
            PriceManager priceManager;
            TextManager textManager;
            DateManager dateManager;
            String text;
            DefaultValues dv;

            if (String.IsNullOrEmpty(TagName))
            {
                Handled = false;

                return (null);
            }
            else if (TagName.StartsWith(Constants.HtmlPrefix + ".") == false)
            {
                Handled = false;

                return (null);
            }

            priceManager = new PriceManager(SourceDVD);
            textManager = new TextManager(SourceDVD);
            dateManager = new DateManager(SourceDVD);
            Handled = true;
            dv = Settings.DefaultValues;
            switch (TagName)
            {
                #region Prices
                case (Constants.HtmlPrefix + "." + Constants.OriginalPrice):
                    {
                        text = GetFormattedValue(priceManager.GetOriginalPrice, priceManager.GetOriginalPriceCurrency);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.ShippingCost):
                    {
                        text = GetFormattedValue(priceManager.GetShippingCost, priceManager.GetShippingCostCurrency);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.CreditCardCharge):
                    {
                        text = GetFormattedValue(priceManager.GetCreditCardCharge, priceManager.GetCreditCardChargeCurrency);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.CreditCardFees):
                    {
                        text = GetFormattedValue(priceManager.GetCreditCardFees, priceManager.GetCreditCardFeesCurrency);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.Discount):
                    {
                        text = GetFormattedValue(priceManager.GetDiscount, priceManager.GetDiscountCurrency);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.CustomsFees):
                    {
                        text = GetFormattedValue(priceManager.GetCustomsFees, priceManager.GetCustomsFeesCurrency);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.CouponType):
                    {
                        text = HtmlEncode(textManager.GetCouponTypeWithFallback());
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.CouponCode):
                    {
                        text = HtmlEncode(textManager.GetCouponCodeWithFallback());
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.AdditionalPrice1):
                    {
                        text = GetFormattedValue(priceManager.GetAdditionalPrice1, priceManager.GetAdditionalPrice1Currency);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.AdditionalPrice2):
                    {
                        text = GetFormattedValue(priceManager.GetAdditionalPrice2, priceManager.GetAdditionalPrice2Currency);
                        break;
                    }
                #endregion
                #region Dates
                case (Constants.HtmlPrefix + "." + Constants.OrderDate):
                    {
                        text = GetFormattedValue(dateManager.GetOrderDate);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.ShippingDate):
                    {
                        text = GetFormattedValue(dateManager.GetShippingDate);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.DeliveryDate):
                    {
                        text = GetFormattedValue(dateManager.GetDeliveryDate);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.AdditionalDate1):
                    {
                        text = GetFormattedValue(dateManager.GetAdditionalDate1);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.AdditionalDate2):
                    {
                        text = GetFormattedValue(dateManager.GetAdditionalDate1);
                        break;
                    }
                #endregion
                #region Labels
                #region Prices
                case (Constants.HtmlPrefix + "." + Constants.OriginalPrice + Constants.LabelSuffix):
                    {
                        text = HtmlEncode(dv.OriginalPriceLabel);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.ShippingCost + Constants.LabelSuffix):
                    {
                        text = HtmlEncode(dv.ShippingCostLabel);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.CreditCardCharge + Constants.LabelSuffix):
                    {
                        text = HtmlEncode(dv.CreditCardChargeLabel);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.CreditCardFees + Constants.LabelSuffix):
                    {
                        text = HtmlEncode(dv.CreditCardFeesLabel);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.Discount + Constants.LabelSuffix):
                    {
                        text = HtmlEncode(dv.DiscountLabel);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.CustomsFees + Constants.LabelSuffix):
                    {
                        text = HtmlEncode(dv.CustomsFeesLabel);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.CouponType + Constants.LabelSuffix):
                    {
                        text = HtmlEncode(dv.CouponTypeLabel);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.CouponCode + Constants.LabelSuffix):
                    {
                        text = HtmlEncode(dv.CouponCodeLabel);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.AdditionalPrice1 + Constants.LabelSuffix):
                    {
                        text = HtmlEncode(dv.AdditionalPrice1Label);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.AdditionalPrice2 + Constants.LabelSuffix):
                    {
                        text = HtmlEncode(dv.AdditionalPrice2Label);
                        break;
                    }
                #endregion
                #region Dates
                case (Constants.HtmlPrefix + "." + Constants.OrderDate + Constants.LabelSuffix):
                    {
                        text = HtmlEncode(dv.OrderDateLabel);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.ShippingDate + Constants.LabelSuffix):
                    {
                        text = HtmlEncode(dv.ShippingDateLabel);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.DeliveryDate + Constants.LabelSuffix):
                    {
                        text = HtmlEncode(dv.DeliveryDateLabel);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.AdditionalDate1 + Constants.LabelSuffix):
                    {
                        text = HtmlEncode(dv.AdditionalDate1Label);
                        break;
                    }
                case (Constants.HtmlPrefix + "." + Constants.AdditionalDate2 + Constants.LabelSuffix):
                    {
                        text = HtmlEncode(dv.AdditionalDate2Label);
                        break;
                    }
                #endregion
                #endregion
                default:
                    {
                        Handled = false;
                        text = null;
                        break;
                    }
            }
            return (text);
        }

        public Object GetCustomHTMLTagNames()
        {
            String[] tags;

            tags = new String[] { Constants.HtmlPrefix + "." + Constants.OriginalPrice
                , Constants.HtmlPrefix + "." + Constants.ShippingCost
                , Constants.HtmlPrefix + "." + Constants.CreditCardCharge
                , Constants.HtmlPrefix + "." + Constants.CreditCardFees
                , Constants.HtmlPrefix + "." + Constants.Discount
                , Constants.HtmlPrefix + "." + Constants.CustomsFees
                , Constants.HtmlPrefix + "." + Constants.CouponType
                , Constants.HtmlPrefix + "." + Constants.CouponCode
                , Constants.HtmlPrefix + "." + Constants.AdditionalPrice1
                , Constants.HtmlPrefix + "." + Constants.AdditionalPrice2
                , Constants.HtmlPrefix + "." + Constants.OriginalPrice + Constants.LabelSuffix
                , Constants.HtmlPrefix + "." + Constants.ShippingCost + Constants.LabelSuffix
                , Constants.HtmlPrefix + "." + Constants.CreditCardCharge + Constants.LabelSuffix
                , Constants.HtmlPrefix + "." + Constants.CreditCardFees + Constants.LabelSuffix
                , Constants.HtmlPrefix + "." + Constants.Discount + Constants.LabelSuffix
                , Constants.HtmlPrefix + "." + Constants.CustomsFees + Constants.LabelSuffix
                , Constants.HtmlPrefix + "." + Constants.CouponType + Constants.LabelSuffix
                , Constants.HtmlPrefix + "." + Constants.CouponCode + Constants.LabelSuffix
                , Constants.HtmlPrefix + "." + Constants.AdditionalPrice1 + Constants.LabelSuffix
                , Constants.HtmlPrefix + "." + Constants.AdditionalPrice2 + Constants.LabelSuffix};
            return (tags);
        }

        public Object GetCustomHTMLParamsForTag(String TagName)
        {
            return (null);
        }

        public Boolean FilterFieldMatch(String FieldFilterToken, Int32 ComparisonTypeIndex, Object ComparisonValue, IDVDInfo TestDVD)
        {
            return (false);
        }

        #endregion

        #endregion

        private void LoadCurrencies()
        {
            List<CurrencyInfo> list;

            DefaultCurrency = new CurrencyInfo(Settings.DefaultValues.DefaultCurrency);
            list = new List<CurrencyInfo>(CurrencyInfo.MaxID + 1);
            for (UInt16 i = CurrencyInfo.MinID; i <= CurrencyInfo.MaxID; i++)
            {
                list.Add(new CurrencyInfo(i));
            }
            list.Sort();
            Currencies = new Dictionary<String, CurrencyInfo>();
            foreach (CurrencyInfo ci in list)
            {
                Currencies.Add(ci.Name, ci);
            }
        }

        #region RegisterCustomFields

        private void RegisterCustomFields()
        {
            //System.Diagnostics.Debugger.Launch();

            try
            {

                #region Schema

                using (Stream stream
                    = typeof(EnhancedPurchaseInfo).Assembly.GetManifestResourceStream("DoenaSoft.DVDProfiler.EnhancedPurchaseInfo.EnhancedPurchaseInfo.xsd"))
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        String xsd;

                        xsd = sr.ReadToEnd();
                        Api.SetGlobalSetting(Constants.FieldDomain, "EnhancedPurchaseInfoSchema", xsd, Constants.ReadKey, InternalConstants.WriteKey);
                    }
                }

                #endregion

                #region Prices

                RegisterCustomPriceField(Constants.OriginalPrice);
                RegisterCustomPriceField(Constants.ShippingCost);
                RegisterCustomPriceField(Constants.CreditCardCharge);
                RegisterCustomPriceField(Constants.CreditCardFees);
                RegisterCustomPriceField(Constants.Discount);
                RegisterCustomPriceField(Constants.CustomsFees);
                RegisterCustomTextField(Constants.CouponType);
                RegisterCustomTextField(Constants.CouponCode);
                RegisterCustomPriceField(Constants.AdditionalPrice1);
                RegisterCustomPriceField(Constants.AdditionalPrice2);

                #endregion

                #region Dates

                RegisterCustomDateField(Constants.OrderDate);
                RegisterCustomDateField(Constants.ShippingDate);
                RegisterCustomDateField(Constants.DeliveryDate);
                RegisterCustomDateField(Constants.AdditionalDate1);
                RegisterCustomDateField(Constants.AdditionalDate2);

                #endregion

                PrepareCouponTypes();

            }
            catch (Exception ex)
            {
                try
                {
                    MessageBox.Show(String.Format(MessageBoxTexts.CriticalError, ex.Message, ErrorFile)
                        , MessageBoxTexts.CriticalErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    if (File.Exists(ErrorFile))
                    {
                        File.Delete(ErrorFile);
                    }
                    LogException(ex);
                }
                catch (Exception inEx)
                {
                    MessageBox.Show(String.Format(MessageBoxTexts.FileCantBeWritten, ErrorFile, inEx.Message)
                        , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PrepareCouponTypes()
        {
            Object[] ids;

            //Debugger.Launch();

            CouponTypes.Clear();

            ids = (Object[])(Api.GetAllProfileIDs());
            if ((ids != null) && (ids.Length > 0))
            {
                for (Int32 i = 0; i < ids.Length; i++)
                {
                    String id;
                    IDVDInfo profile;
                    TextManager textManager;
                    String couponType;

                    id = ids[i].ToString();

                    profile = Api.CreateDVD();
                    profile.SetProfileID(id);

                    textManager = new TextManager(profile);

                    if (textManager.GetCouponType(out couponType))
                    {
                        UInt16 count;

                        if (CouponTypes.TryGetValue(couponType, out count))
                        {
                            count++;
                            CouponTypes[couponType] = count;
                        }
                        else
                        {
                            CouponTypes.Add(couponType, 1);
                        }
                    }
                }
            }
        }

        private void RegisterCustomPriceField(String fieldName)
        {
            RegisterCustomField(fieldName, PluginConstants.FIELD_TYPE_CURRENCY);
            RegisterCustomField(fieldName + Constants.CurrencySuffix, PluginConstants.FIELD_TYPE_INT);
        }

        private void RegisterCustomField(String fieldName
            , Int32 fieldType)
        {
            Api.CreateCustomDVDField(Constants.FieldDomain, fieldName, fieldType, Constants.ReadKey, InternalConstants.WriteKey);
            Api.SetCustomDVDFieldStorage(Constants.FieldDomain, fieldName, InternalConstants.WriteKey, true, false);
        }

        private void RegisterCustomTextField(String fieldName)
        {
            RegisterCustomField(fieldName, PluginConstants.FIELD_TYPE_STRING);
        }

        private void RegisterCustomDateField(String fieldName)
        {
            RegisterCustomField(fieldName, PluginConstants.FIELD_TYPE_DATETIME);
        }

        #endregion

        private void SetIsRemoteAccess()
        {
            String name;
            Boolean isRemote;
            String localPath;

            Api.GetCurrentDatabaseInformation(out name, out isRemote, out localPath);

            //System.Diagnostics.Debugger.Launch();

            IsRemoteAccess = isRemote;
        }

        #region Export to XML

        private void AppendDates(StringBuilder sb
            , Boolean hasOD
            , DateTime od
            , Boolean hasSD
            , DateTime sd
            , Boolean hasDD
            , DateTime dd
            , Boolean hasAD1
            , DateTime ad1
            , Boolean hasAD2
            , DateTime ad2)
        {
            DefaultValues dv;

            dv = Settings.DefaultValues;
            if (hasOD)
            {
                AddDateTag(sb, Constants.OrderDate, dv.OrderDateLabel, od);
            }
            if (hasSD)
            {
                AddDateTag(sb, Constants.ShippingDate, dv.ShippingDateLabel, sd);
            }
            if (hasDD)
            {
                AddDateTag(sb, Constants.DeliveryDate, dv.DeliveryDateLabel, dd);
            }
            if (hasAD1)
            {
                AddDateTag(sb, Constants.AdditionalDate1, dv.AdditionalDate1Label, ad1);
            }
            if (hasAD2)
            {
                AddDateTag(sb, Constants.AdditionalDate1, dv.AdditionalDate2Label, ad2);
            }
        }

        private void AddDateTag(StringBuilder sb
            , String tagName
            , String displayName
            , DateTime od)
        {
            sb.Append("<");
            sb.Append(tagName);
            AddDisplayNameAttribute(sb, displayName);
            sb.Append(">");
            sb.Append(od.Year.ToString("D4"));
            sb.Append("-");
            sb.Append(od.Month.ToString("D2"));
            sb.Append("-");
            sb.Append(od.Day.ToString("D2"));
            sb.Append("</");
            sb.Append(tagName);
            sb.Append(">");
        }

        private static void AddDisplayNameAttribute(StringBuilder sb
            , String displayName)
        {
            String base64;

            displayName = XmlConvertHelper.GetWindows1252Text(displayName, out base64);
            sb.Append(" DisplayName=\"");
            sb.Append(displayName);
            sb.Append("\"");
            if (base64 != null)
            {
                sb.Append(" Base64DisplayName=\"");
                sb.Append(base64);
                sb.Append("\"");
            }
        }

        private static bool HasPrice(Boolean hasOP
            , Boolean hasSC
            , Boolean hasCF
            , Boolean hasCCF
            , Boolean hasDiscount
            , Boolean hasCT
            , Boolean hasCC
            , Boolean hasAP1
            , Boolean hasAP2)
        {
            return hasOP || hasSC || hasCF || hasCCF || hasDiscount || hasCT || hasCC || hasAP1 || hasAP2;
        }

        private void AppendPrices(StringBuilder sb
            , Boolean hasOP
            , Decimal op
            , Boolean hasSC
            , Decimal sc
            , Boolean hasCCC
            , Decimal ccc
            , Boolean hasCCF
            , Decimal ccf
            , Boolean hasDiscount
            , Decimal discount
            , Boolean hasCF
            , Decimal cf
            , Boolean hasCT
            , String ct
            , Boolean hasCC
            , String cc
            , Boolean hasAP1
            , Decimal ap1
            , Boolean hasAP2
            , Decimal ap2
            , PriceManager priceManager)
        {
            CurrencyInfo ci;
            DefaultValues dv;

            dv = Settings.DefaultValues;
            if (hasOP)
            {
                priceManager.GetOriginalPriceCurrency(out ci);
                AddPriceTag(sb, Constants.OriginalPrice, dv.OriginalPriceLabel, op, ci);
            }
            if (hasSC)
            {
                priceManager.GetShippingCostCurrency(out ci);
                AddPriceTag(sb, Constants.ShippingCost, dv.ShippingCostLabel, sc, ci);
            }
            if (hasCCC)
            {
                priceManager.GetCreditCardChargeCurrency(out ci);
                AddPriceTag(sb, Constants.CreditCardCharge, dv.CreditCardChargeLabel, ccc, ci);
            }
            if (hasCCF)
            {
                priceManager.GetCreditCardFeesCurrency(out ci);
                AddPriceTag(sb, Constants.CreditCardFees, dv.CreditCardFeesLabel, ccf, ci);
            }
            if (hasDiscount)
            {
                priceManager.GetDiscountCurrency(out ci);
                AddPriceTag(sb, Constants.Discount, dv.DiscountLabel, discount, ci);
            }
            if (hasCF)
            {
                priceManager.GetCustomsFeesCurrency(out ci);
                AddPriceTag(sb, Constants.CustomsFees, dv.CustomsFeesLabel, cf, ci);
            }
            if (hasCT)
            {
                AddTextTag(sb, Constants.CouponType, dv.CouponTypeLabel, ct);
            }
            if (hasCC)
            {
                AddTextTag(sb, Constants.CouponCode, dv.CouponCodeLabel, cc);
            }
            if (hasAP1)
            {
                priceManager.GetAdditionalPrice1Currency(out ci);
                AddPriceTag(sb, Constants.AdditionalPrice1, dv.AdditionalPrice1Label, ap1, ci);
            }
            if (hasAP2)
            {
                priceManager.GetAdditionalPrice2Currency(out ci);
                AddPriceTag(sb, Constants.AdditionalPrice2, dv.AdditionalPrice2Label, ap2, ci);
            }
        }

        private static void AddTextTag(StringBuilder sb
            , String tagName
            , String displayName
            , String text)
        {
            sb.Append("<");
            sb.Append(tagName);
            AddDisplayNameAttribute(sb, displayName);
            sb.Append(">");
            sb.Append(text);
            sb.Append("</");
            sb.Append(tagName);
            sb.Append(">");
        }

        private static void AddPriceTag(StringBuilder sb
            , String tagName
            , String displayName
            , Decimal price
            , CurrencyInfo ci)
        {
            sb.Append("<");
            sb.Append(tagName);
            sb.Append(" DenominationType=\"");
            sb.Append(ci.Type);
            sb.Append("\" DenominationDesc=\"");
            sb.Append(ci.Name);
            sb.Append("\" FormattedValue=\"");
            sb.Append(ci.GetFormattedValue(price));
            sb.Append("\"");
            AddDisplayNameAttribute(sb, displayName);
            sb.Append(">");
            sb.Append(price.ToString("F2", CultureInfo.GetCultureInfo("en-US")));
            sb.Append("</");
            sb.Append(tagName);
            sb.Append(">");
        }

        #endregion

        #region Export to HTML

        private String GetFormattedValue(DateManager.GetDateDelegate getDate)
        {
            String dateString;
            DateTime date;

            dateString = String.Empty;
            if (getDate(out date))
            {
                String dateFormat;

                dateFormat = Application.CurrentCulture.DateTimeFormat.ShortDatePattern;
                dateString = date.ToString(dateFormat);
            }
            return (dateString);
        }

        private String GetFormattedValue(PriceManager.GetPriceDelegate getPrice
            , PriceManager.GetCurrencyDelegate getCurrency)
        {
            Decimal price;
            String priceString;

            priceString = String.Empty;
            if (getPrice(out price))
            {
                CurrencyInfo ci;

                getCurrency(out ci);
                priceString = ci.GetFormattedValue(price);
                priceString = HtmlEncode(priceString);
            }
            return (priceString);
        }

        #endregion

        private void EnsureSettingsAndSetUILanguage()
        {
            Texts.Culture = DefaultValues.GetUILanguage();

            CultureInfo uiLanguage = EnsureSettings();

            Texts.Culture = uiLanguage;

            MessageBoxTexts.Culture = uiLanguage;
        }

        private CultureInfo EnsureSettings()
        {
            if (Settings == null)
            {
                Settings = new Settings();
            }

            if (Settings.DefaultValues == null)
            {
                Settings.DefaultValues = new DefaultValues();
            }

            return (Settings.DefaultValues.UiLanguage);
        }

        public static String HtmlEncode(String decoded)
        {
            String encoded;

            encoded = String.Join("", decoded.ToCharArray().Select(c =>
                {
                    Int32 number;
                    String newChar;

                    number = (Int32)c;
                    if (number > 127)
                    {
                        newChar = "&#" + number.ToString() + ";";
                    }
                    else
                    {
                        newChar = HttpUtility.HtmlEncode(c.ToString());
                    }
                    return (newChar);
                }).ToArray());
            return (encoded);
        }

        private void HandleMenuClick(Int32 MenuEventID)
        {
            try
            {
                switch (MenuEventID)
                {
                    case (DvdMenuId):
                        {
                            OpenEditor(true);
                            break;
                        }
                    case (PersonalizeScreenId):
                        {
                            OpenEditor(false);
                            break;
                        }
                    case (CollectionExportToXmlMenuId):
                        {
                            XmlManager xmlManager;

                            xmlManager = new XmlManager(this);
                            xmlManager.Export(true);
                            break;
                        }
                    case (CollectionImportMenuId):
                        {
                            XmlManager xmlManager;

                            xmlManager = new XmlManager(this);
                            xmlManager.Import(true);
                            break;
                        }
                    case (CollectionExportToCsvMenuId):
                        {
                            CsvManager csvManager;

                            csvManager = new CsvManager(this);
                            csvManager.Export(true);
                            break;
                        }
                    case (CollectionFlaggedExportToXmlMenuId):
                        {
                            XmlManager xmlManager;

                            xmlManager = new XmlManager(this);
                            xmlManager.Export(false);
                            break;
                        }
                    case (CollectionFlaggedImportMenuId):
                        {
                            XmlManager xmlManager;

                            xmlManager = new XmlManager(this);
                            xmlManager.Import(false);
                            break;
                        }
                    case (CollectionFlaggedExportToCsvMenuId):
                        {
                            CsvManager csvManager;

                            csvManager = new CsvManager(this);
                            csvManager.Export(false);
                            break;
                        }
                    case (ToolsOptionsMenuId):
                        {
                            OpenSettings();
                            break;
                        }
                    case (ToolsExportOptionsMenuId):
                        {
                            ExportOptions();
                            break;
                        }
                    case (ToolsImportOptionsMenuId):
                        {
                            ImportOptions();
                            break;
                        }
                    case (ToolsOpenCalculatorMenuId):
                        {
                            OnOpenCalculator(this, EventArgs.Empty);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    MessageBox.Show(String.Format(MessageBoxTexts.CriticalError, ex.Message, ErrorFile)
                        , MessageBoxTexts.CriticalErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    if (File.Exists(ErrorFile))
                    {
                        File.Delete(ErrorFile);
                    }
                    LogException(ex);
                }
                catch (Exception inEx)
                {
                    MessageBox.Show(String.Format(MessageBoxTexts.FileCantBeWritten, ErrorFile, inEx.Message), MessageBoxTexts.ErrorHeader
                        , MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        internal void ImportOptions()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.CheckFileExists = true;
                ofd.Filter = "XML files|*.xml";
                ofd.Multiselect = false;
                ofd.RestoreDirectory = true;
                ofd.Title = Texts.LoadXmlFile;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    DefaultValues dv = null;

                    try
                    {
                        dv = Serializer<DefaultValues>.Deserialize(ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format(MessageBoxTexts.FileCantBeRead, ofd.FileName, ex.Message)
                           , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (dv != null)
                    {
                        Settings.DefaultValues = dv;
                        Texts.Culture = dv.UiLanguage;
                        MessageBoxTexts.Culture = dv.UiLanguage;
                        MessageBox.Show(MessageBoxTexts.Done, MessageBoxTexts.InformationHeader, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        internal void ExportOptions()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.AddExtension = true;
                sfd.DefaultExt = ".xml";
                sfd.Filter = "XML files|*.xml";
                sfd.OverwritePrompt = true;
                sfd.RestoreDirectory = true;
                sfd.Title = Texts.SaveXmlFile;
                sfd.FileName = "EnhancedPurchaseInfoOptions.xml";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    DefaultValues dv = Settings.DefaultValues;

                    try
                    {
                        Serializer<DefaultValues>.Serialize(sfd.FileName, dv);

                        MessageBox.Show(MessageBoxTexts.Done, MessageBoxTexts.InformationHeader, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format(MessageBoxTexts.FileCantBeWritten, sfd.FileName, ex.Message)
                            , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void OpenSettings()
        {
            using (SettingsForm form = new SettingsForm(this))
            {
                form.ShowDialog();
            }
        }

        private void OpenEditor(Boolean fullEdit)
        {
            IDVDInfo profile;
            String profileId;

            profileId = CurrentProfileId;
            if (String.IsNullOrEmpty(profileId))
            {
                profile = Api.GetDisplayedDVD();
                profileId = profile.GetProfileID();
            }
            if (String.IsNullOrEmpty(profileId) == false)
            {
                Api.DVDByProfileID(out profile, profileId, PluginConstants.DATASEC_AllSections, -1);
                if (profile.GetProfileID() == null)
                {
                    profile = Api.CreateDVD();
                    profile.SetProfileID(profileId);
                }
                using (MainForm form = new MainForm(this, profile, fullEdit))
                {
                    form.OpenCalculator += OnOpenCalculator;
                    form.ShowDialog();
                    form.OpenCalculator -= OnOpenCalculator;
                }
            }
        }

        void OnOpenCalculator(Object sender, EventArgs e)
        {
            FileInfo fi;
            String file;
            String dir;
            Process p;
            String args;

            file = GetType().Assembly.Location;
            fi = new FileInfo(file);
            dir = fi.DirectoryName;
            file = "PurchasePriceSplitter.exe";
            file = Path.Combine(dir, file);
            args = "/lang=" + Settings.DefaultValues.UiLanguage.Name;
            p = new Process();
            p.StartInfo = new ProcessStartInfo(file, args);
            p.Start();
        }

        private void LogException(Exception ex)
        {
            ex = WrapCOMException(ex);

            ExceptionXml exceptionXml = new ExceptionXml(ex);

            Serializer<ExceptionXml>.Serialize(ErrorFile, exceptionXml);
        }

        private Exception WrapCOMException(Exception ex)
        {
            Exception returnEx = ex;

            COMException comEx = ex as COMException;

            if (comEx != null)
            {
                String lastApiError = Api.GetLastError();

                EnhancedCOMException newEx = new EnhancedCOMException(comEx, lastApiError);

                returnEx = newEx;
            }

            return (returnEx);
        }

        #region Plugin Registering

        [DllImport("user32.dll")]
        public extern static int SetParent(int child, int parent);

        [ComImport(), Guid("0002E005-0000-0000-C000-000000000046")]
        internal class StdComponentCategoriesMgr { }

        [ComRegisterFunction()]
        public static void RegisterServer(Type t)
        {
            CategoryRegistrar.ICatRegister cr = (CategoryRegistrar.ICatRegister)new StdComponentCategoriesMgr();
            Guid clsidThis = new Guid(ClassGuid.ClassID);
            Guid catid = new Guid("833F4274-5632-41DB-8FC5-BF3041CEA3F1");

            cr.RegisterClassImplCategories(ref clsidThis, 1,
                new Guid[] { catid });
        }

        [ComUnregisterFunction()]
        public static void UnregisterServer(Type t)
        {
            CategoryRegistrar.ICatRegister cr = (CategoryRegistrar.ICatRegister)new StdComponentCategoriesMgr();
            Guid clsidThis = new Guid(ClassGuid.ClassID);
            Guid catid = new Guid("833F4274-5632-41DB-8FC5-BF3041CEA3F1");

            cr.UnRegisterClassImplCategories(ref clsidThis, 1,
                new Guid[] { catid });
        }

        #endregion
    }
}