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
using DoenaSoft.ToolBox.Generics;
using Invelos.DVDProfilerPlugin;

namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    [Guid(ClassGuid.ClassID), ComVisible(true)]
    public class Plugin : IDVDProfilerPlugin, IDVDProfilerPluginInfo, IDVDProfilerDataAwarePlugin
    {
        private readonly string _settingsFile;

        private readonly string _errorFile;

        private readonly string _applicationPath;

        private string _currentProfileId;

        private bool _databaseRestoreRunning;

        internal IDVDProfilerAPI Api { get; private set; }

        internal Settings Settings { get; private set; }

        internal Dictionary<string, CurrencyInfo> Currencies { get; private set; }

        internal CurrencyInfo DefaultCurrency { get; set; }

        internal Dictionary<string, ushort> CouponTypes { get; }

        internal bool IsRemoteAccess { get; private set; }

        #region MenuTokens

        private string _dvdMenuToken = "";

        private const int DvdMenuId = 1;

        private string _personalizeScreenToken = "";

        private const int PersonalizeScreenId = 11;

        private string _collectionExportToXmlMenuToken = "";

        private const int CollectionExportToXmlMenuId = 21;

        private string _collectionImportMenuToken = "";

        private const int CollectionImportMenuId = 22;

        private string _collectionExportToCsvMenuToken = "";

        private const int CollectionExportToCsvMenuId = 23;

        private string _collectionFlaggedExportToXmlMenuToken = "";

        private const int CollectionFlaggedExportToXmlMenuId = 31;

        private string _collectionFlaggedImportMenuToken = "";

        private const int CollectionFlaggedImportMenuId = 32;

        private string _collectionFlaggedExportToCsvMenuToken = "";

        private const int CollectionFlaggedExportToCsvMenuId = 33;

        private string _toolsOptionsMenuToken = "";

        private const int ToolsOptionsMenuId = 41;

        private string _toolsExportOptionsMenuToken = "";

        private const int ToolsExportOptionsMenuId = 42;

        private string _toolsImportOptionsMenuToken = "";

        private const int ToolsImportOptionsMenuId = 43;

        private string _toolsOpenCalculatorMenuToken = "";

        private const int ToolsOpenCalculatorMenuId = 44;

        #endregion

        public Plugin()
        {
            _databaseRestoreRunning = false;

            CouponTypes = new Dictionary<string, ushort>();

            _applicationPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Doena Soft\EnhancedPurchaseInfo\";

            _settingsFile = _applicationPath + "EnhancedPurchaseInfo.xml";

            _errorFile = Environment.GetEnvironmentVariable("TEMP") + @"\EnhancedPurchaseInfoCrash.xml";
        }

        #region I.. Members

        #region IDVDProfilerPlugin Members

        public void Load(IDVDProfilerAPI api)
        {
            //System.Diagnostics.Debugger.Launch();

            Api = api;

            if (Directory.Exists(_applicationPath) == false)
            {
                Directory.CreateDirectory(_applicationPath);
            }

            if (File.Exists(_settingsFile))
            {
                try
                {
                    Settings = Settings.Deserialize(_settingsFile);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(MessageBoxTexts.FileCantBeRead, _settingsFile, ex.Message)
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

            _dvdMenuToken = Api.RegisterMenuItemA(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                , "DVD", Texts.EPI, DvdMenuId, "", PluginConstants.SHORTCUT_KEY_A + 15, PluginConstants.SHORTCUT_MOD_Ctrl + PluginConstants.SHORTCUT_MOD_Shift, false);

            _collectionExportToXmlMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                , @"Collection\" + Texts.EPI, Texts.ExportToXml, CollectionExportToXmlMenuId);
            if (IsRemoteAccess == false)
            {
                _collectionImportMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                    , @"Collection\" + Texts.EPI, Texts.ImportFromXml, CollectionImportMenuId);
            }
            _collectionExportToCsvMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                , @"Collection\" + Texts.EPI, Texts.ExportToExcel, CollectionExportToCsvMenuId);

            _collectionFlaggedExportToXmlMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                , @"Collection\Flagged\" + Texts.EPI, Texts.ExportToXml, CollectionFlaggedExportToXmlMenuId);
            if (IsRemoteAccess == false)
            {
                _collectionFlaggedImportMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                   , @"Collection\Flagged\" + Texts.EPI, Texts.ImportFromXml, CollectionFlaggedImportMenuId);
            }
            _collectionFlaggedExportToCsvMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                , @"Collection\Flagged\" + Texts.EPI, Texts.ExportToExcel, CollectionFlaggedExportToCsvMenuId);

            _toolsOptionsMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                , @"Tools\" + Texts.EPI, Texts.Options, ToolsOptionsMenuId);
            _toolsExportOptionsMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                , @"Tools\" + Texts.EPI, Texts.ExportOptions, ToolsExportOptionsMenuId);
            _toolsImportOptionsMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                , @"Tools\" + Texts.EPI, Texts.ImportOptions, ToolsImportOptionsMenuId);
            _toolsOpenCalculatorMenuToken = api.RegisterMenuItem(PluginConstants.FORMID_Main, PluginConstants.MENUID_Form
                , @"Tools\" + Texts.EPI, Texts.ShippingCostCalculator, ToolsOpenCalculatorMenuId);

            RegisterCustomFields();
        }

        public void Unload()
        {
            Api.UnregisterMenuItem(_dvdMenuToken);

            Api.UnregisterMenuItem(_collectionExportToXmlMenuToken);
            Api.UnregisterMenuItem(_collectionImportMenuToken);
            Api.UnregisterMenuItem(_collectionExportToCsvMenuToken);

            Api.UnregisterMenuItem(_collectionFlaggedExportToXmlMenuToken);
            Api.UnregisterMenuItem(_collectionFlaggedImportMenuToken);
            Api.UnregisterMenuItem(_collectionFlaggedExportToCsvMenuToken);

            Api.UnregisterMenuItem(_toolsOptionsMenuToken);
            Api.UnregisterMenuItem(_toolsExportOptionsMenuToken);
            Api.UnregisterMenuItem(_toolsImportOptionsMenuToken);
            Api.UnregisterMenuItem(_toolsOpenCalculatorMenuToken);

            try
            {
                Settings.Serialize(_settingsFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(MessageBoxTexts.FileCantBeWritten, _settingsFile, ex.Message)
                    , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Api = null;
        }

        public void HandleEvent(int EventType, object EventData)
        {
            try
            {
                switch (EventType)
                {
                    case PluginConstants.EVENTID_CustomMenuClick:
                        {
                            HandleMenuClick((int)EventData);

                            break;
                        }
                    case PluginConstants.EVENTID_FormCreated:
                        {
                            if ((int)EventData == PluginConstants.FORMID_Personalize)
                            {
                                _personalizeScreenToken = Api.RegisterMenuItemA(PluginConstants.FORMID_Personalize, PluginConstants.MENUID_Form
                                    , Texts.EPI, Texts.EPI, PersonalizeScreenId, "", PluginConstants.SHORTCUT_KEY_A + 15, PluginConstants.SHORTCUT_MOD_Ctrl + PluginConstants.SHORTCUT_MOD_Shift, false);
                            }

                            break;
                        }
                    case PluginConstants.EVENTID_FormDestroyed:
                        {
                            if ((int)EventData == PluginConstants.FORMID_Personalize)
                            {
                                if (string.IsNullOrEmpty(_personalizeScreenToken) == false)
                                {
                                    Api.UnregisterMenuItem(_personalizeScreenToken);
                                }

                                _currentProfileId = null;
                            }

                            break;
                        }
                    case PluginConstants.EVENTID_RestoreStarting:
                        {
                            _databaseRestoreRunning = true;

                            RegisterCustomFields();

                            break;
                        }
                    case PluginConstants.EVENTID_DatabaseOpened:
                    case PluginConstants.EVENTID_RestoreFinished:
                    case PluginConstants.EVENTID_RestoreCancelled:
                        {
                            _databaseRestoreRunning = false;

                            RegisterCustomFields();

                            break;
                        }
                    case PluginConstants.EVENTID_DVDPersonalizeShown:
                        {
                            //System.Diagnostics.Debugger.Launch();
                            _currentProfileId = (string)EventData;

                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    MessageBox.Show(string.Format(MessageBoxTexts.CriticalError, ex.Message, _errorFile)
                        , MessageBoxTexts.CriticalErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    if (File.Exists(_errorFile))
                    {
                        File.Delete(_errorFile);
                    }

                    LogException(ex);
                }
                catch (Exception inEx)
                {
                    MessageBox.Show(string.Format(MessageBoxTexts.FileCantBeWritten, _errorFile, inEx.Message)
                        , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region IDVDProfilerPluginInfo Members

        public string GetName() => Texts.PluginName;

        public string GetDescription() => Texts.PluginDescription;

        public string GetAuthorName() => "Doena Soft.";

        public string GetAuthorWebsite() => Texts.PluginUrl;

        public int GetPluginAPIVersion() => PluginConstants.API_VERSION;

        public int GetVersionMajor()
        {
            var version = System.Reflection.Assembly.GetAssembly(GetType()).GetName().Version;

            return version.Major;
        }

        public int GetVersionMinor()
        {
            var version = System.Reflection.Assembly.GetAssembly(GetType()).GetName().Version;

            return version.Minor * 100 + version.Build * 10 + version.Revision;
        }

        #endregion

        #region IDVDProfilerDataAwarePlugin

        public bool ExportsCustomDataXML() => Settings.DefaultValues.ExportToCollectionXml && _databaseRestoreRunning == false;

        public string GetCustomDataXMLForDVD(IDVDInfo SourceDVD)
        {
            if (Settings.DefaultValues.ExportToCollectionXml == false)
            {
                return string.Empty;
            }
            else if (_databaseRestoreRunning)
            {
                return string.Empty;
            }

            var priceManager = new PriceManager(SourceDVD);

            var textManager = new TextManager(SourceDVD);

            var dateManager = new DateManager(SourceDVD);

            #region Prices

            var hasOP = priceManager.GetOriginalPrice(out var op);

            var hasSC = priceManager.GetShippingCost(out var sc);

            var hasCCC = priceManager.GetCreditCardCharge(out var ccc);

            var hasCCF = priceManager.GetCreditCardFees(out var ccf);

            var hasDiscount = priceManager.GetDiscount(out var discount);

            var hasCF = priceManager.GetCustomsFees(out var cf);

            var hasCT = textManager.GetCouponType(out var ct);

            var hasCC = textManager.GetCouponCode(out var cc);

            var hasAP1 = priceManager.GetAdditionalPrice1(out var ap1);

            var hasAP2 = priceManager.GetAdditionalPrice2(out var ap2);

            #endregion

            #region Dates

            var hasOD = dateManager.GetOrderDate(out var od);

            var hasSD = dateManager.GetShippingDate(out var sd);

            var hasDD = dateManager.GetDeliveryDate(out var dd);

            var hasAD1 = dateManager.GetAdditionalDate1(out var ad1);

            var hasAD2 = dateManager.GetAdditionalDate2(out var ad2);

            #endregion

            IEnumerable<bool> hasPrices = ObjectExtensions.Enumerate(hasOP, hasSC, hasCCC, hasCCF, hasDiscount, hasCF, hasCT, hasCC, hasAP1, hasAP2);

            IEnumerable<bool> hasDates = ObjectExtensions.Enumerate(hasOD, hasSD, hasDD, hasAD1, hasAD2);

            if (hasPrices.HasItemsWhere(price => price) || hasDates.HasItemsWhere(date => date))
            {
                var sb = new StringBuilder("<EnhancedPurchaseInfo>");

                AppendPrices(sb, hasOP, op, hasSC, sc, hasCCC, ccc, hasCCF, ccf, hasDiscount, discount, hasCF, cf, hasCT, ct
                    , hasCC, cc, hasAP1, ap1, hasAP2, ap2, priceManager);

                AppendDates(sb, hasOD, od, hasSD, sd, hasDD, dd, hasAD1, ad1, hasAD2, ad2);

                sb.Append("</EnhancedPurchaseInfo>");

                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetHTMLForDPVarsFunctionSection() => string.Empty;

        public string GetHTMLForDPVarsDataSection(IDVDInfo SourceDVD, IDVDInfo CompareDVD) => string.Empty;

        public string GetHTMLForTag(string TagName, string FullTag, IDVDInfo SourceDVD, IDVDInfo CompareDVD, out bool Handled)
        {
            if (string.IsNullOrEmpty(TagName))
            {
                Handled = false;

                return null;
            }
            else if (TagName.StartsWith(Constants.HtmlPrefix + ".") == false)
            {
                Handled = false;

                return null;
            }

            var priceManager = new PriceManager(SourceDVD);

            var textManager = new TextManager(SourceDVD);

            var dateManager = new DateManager(SourceDVD);

            Handled = true;

            var dv = Settings.DefaultValues;

            string text;
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

            return text;
        }

        public object GetCustomHTMLTagNames()
        {
            var tags = new[] { Constants.HtmlPrefix + "." + Constants.OriginalPrice
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

            return tags;
        }

        public object GetCustomHTMLParamsForTag(string TagName) => null;

        public bool FilterFieldMatch(string FieldFilterToken, int ComparisonTypeIndex, object ComparisonValue, IDVDInfo TestDVD) => false;

        #endregion

        #endregion

        private void LoadCurrencies()
        {
            DefaultCurrency = new CurrencyInfo(Settings.DefaultValues.DefaultCurrency);

            var currencies = new List<CurrencyInfo>(CurrencyInfo.MaxID + 1);

            for (ushort currencyId = CurrencyInfo.MinID; currencyId <= CurrencyInfo.MaxID; currencyId++)
            {
                currencies.Add(new CurrencyInfo(currencyId));
            }

            currencies.Sort();

            Currencies = new Dictionary<string, CurrencyInfo>();

            foreach (var currency in currencies)
            {
                Currencies.Add(currency.Name, currency);
            }
        }

        #region RegisterCustomFields

        private void RegisterCustomFields()
        {
            //System.Diagnostics.Debugger.Launch();

            try
            {
                #region Schema

                using (var stream = typeof(EnhancedPurchaseInfo).Assembly.GetManifestResourceStream("DoenaSoft.DVDProfiler.EnhancedPurchaseInfo.EnhancedPurchaseInfo.xsd"))
                {
                    using (var sr = new StreamReader(stream))
                    {
                        var xsd = sr.ReadToEnd();

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
                    MessageBox.Show(string.Format(MessageBoxTexts.CriticalError, ex.Message, _errorFile)
                        , MessageBoxTexts.CriticalErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    if (File.Exists(_errorFile))
                    {
                        File.Delete(_errorFile);
                    }

                    LogException(ex);
                }
                catch (Exception inEx)
                {
                    MessageBox.Show(string.Format(MessageBoxTexts.FileCantBeWritten, _errorFile, inEx.Message)
                        , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PrepareCouponTypes()
        {
            //Debugger.Launch();

            CouponTypes.Clear();

            var ids = (object[])Api.GetAllProfileIDs();

            if (ids != null && ids.Length > 0)
            {
                for (var idIndex = 0; idIndex < ids.Length; idIndex++)
                {
                    var id = ids[idIndex].ToString();

                    var profile = Api.CreateDVD();

                    profile.SetProfileID(id);

                    var textManager = new TextManager(profile);

                    if (textManager.GetCouponType(out var couponType))
                    {
                        if (CouponTypes.TryGetValue(couponType, out var count))
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

        private void RegisterCustomPriceField(string fieldName)
        {
            RegisterCustomField(fieldName, PluginConstants.FIELD_TYPE_CURRENCY);
            RegisterCustomField(fieldName + Constants.CurrencySuffix, PluginConstants.FIELD_TYPE_INT);
        }

        private void RegisterCustomField(string fieldName, int fieldType)
        {
            Api.CreateCustomDVDField(Constants.FieldDomain, fieldName, fieldType, Constants.ReadKey, InternalConstants.WriteKey);
            Api.SetCustomDVDFieldStorage(Constants.FieldDomain, fieldName, InternalConstants.WriteKey, true, false);
        }

        private void RegisterCustomTextField(string fieldName) => RegisterCustomField(fieldName, PluginConstants.FIELD_TYPE_STRING);

        private void RegisterCustomDateField(string fieldName) => RegisterCustomField(fieldName, PluginConstants.FIELD_TYPE_DATETIME);

        #endregion

        private void SetIsRemoteAccess()
        {
            Api.GetCurrentDatabaseInformation(out var name, out var isRemote, out var localPath);

            //System.Diagnostics.Debugger.Launch();

            IsRemoteAccess = isRemote;
        }

        #region Export to XML

        private void AppendDates(StringBuilder sb, bool hasOD, DateTime od, bool hasSD, DateTime sd, bool hasDD, DateTime dd, bool hasAD1, DateTime ad1, bool hasAD2, DateTime ad2)
        {
            var dv = Settings.DefaultValues;

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

        private void AddDateTag(StringBuilder sb, string tagName, string displayName, DateTime od)
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

        private static void AddDisplayNameAttribute(StringBuilder sb, string displayName)
        {
            displayName = XmlConvertHelper.GetWindows1252Text(displayName, out var base64);

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

        //private static bool HasPrice(bool hasOP, bool hasSC, bool hasCF, bool hasCCF, bool hasDiscount, bool hasCT, bool hasCC, bool hasAP1, bool hasAP2)
        //{
        //    return hasOP || hasSC || hasCF || hasCCF || hasDiscount || hasCT || hasCC || hasAP1 || hasAP2;
        //}

        private void AppendPrices(StringBuilder sb, bool hasOP, decimal op, bool hasSC, decimal sc, bool hasCCC, decimal ccc, bool hasCCF, decimal ccf, bool hasDiscount, decimal discount, bool hasCF, decimal cf, bool hasCT, string ct, bool hasCC, string cc, bool hasAP1, decimal ap1, bool hasAP2, decimal ap2, PriceManager priceManager)
        {
            var dv = Settings.DefaultValues;

            if (hasOP)
            {
                priceManager.GetOriginalPriceCurrency(out var ci);

                AddPriceTag(sb, Constants.OriginalPrice, dv.OriginalPriceLabel, op, ci);
            }

            if (hasSC)
            {
                priceManager.GetShippingCostCurrency(out var ci);

                AddPriceTag(sb, Constants.ShippingCost, dv.ShippingCostLabel, sc, ci);
            }

            if (hasCCC)
            {
                priceManager.GetCreditCardChargeCurrency(out var ci);

                AddPriceTag(sb, Constants.CreditCardCharge, dv.CreditCardChargeLabel, ccc, ci);
            }

            if (hasCCF)
            {
                priceManager.GetCreditCardFeesCurrency(out var ci);

                AddPriceTag(sb, Constants.CreditCardFees, dv.CreditCardFeesLabel, ccf, ci);
            }

            if (hasDiscount)
            {
                priceManager.GetDiscountCurrency(out var ci);

                AddPriceTag(sb, Constants.Discount, dv.DiscountLabel, discount, ci);
            }

            if (hasCF)
            {
                priceManager.GetCustomsFeesCurrency(out var ci);

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
                priceManager.GetAdditionalPrice1Currency(out var ci);

                AddPriceTag(sb, Constants.AdditionalPrice1, dv.AdditionalPrice1Label, ap1, ci);
            }

            if (hasAP2)
            {
                priceManager.GetAdditionalPrice2Currency(out var ci);

                AddPriceTag(sb, Constants.AdditionalPrice2, dv.AdditionalPrice2Label, ap2, ci);
            }
        }

        private static void AddTextTag(StringBuilder sb, string tagName, string displayName, string text)
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

        private static void AddPriceTag(StringBuilder sb, string tagName, string displayName, decimal price, CurrencyInfo ci)
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

        private string GetFormattedValue(DateManager.GetDateDelegate getDate)
        {
            var dateString = string.Empty;

            if (getDate(out var date))
            {
                var dateFormat = Application.CurrentCulture.DateTimeFormat.ShortDatePattern;

                dateString = date.ToString(dateFormat);
            }

            return dateString;
        }

        private string GetFormattedValue(PriceManager.GetPriceDelegate getPrice, PriceManager.GetCurrencyDelegate getCurrency)
        {
            var priceString = string.Empty;

            if (getPrice(out var price))
            {
                getCurrency(out var ci);

                priceString = ci.GetFormattedValue(price);
                priceString = HtmlEncode(priceString);
            }

            return priceString;
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

            return Settings.DefaultValues.UiLanguage;
        }

        public static string HtmlEncode(string decoded)
        {
            var encoded = string.Join("", decoded.ToCharArray().Select(c =>
            {
                var number = (int)c;

                var newChar = number > 127
                    ? "&#" + number.ToString() + ";"
                    : HttpUtility.HtmlEncode(c.ToString());

                return newChar;
            }).ToArray());

            return encoded;
        }

        private void HandleMenuClick(int MenuEventID)
        {
            try
            {
                switch (MenuEventID)
                {
                    case DvdMenuId:
                        {
                            OpenEditor(true);

                            break;
                        }
                    case PersonalizeScreenId:
                        {
                            OpenEditor(false);

                            break;
                        }
                    case CollectionExportToXmlMenuId:
                        {
                            var xmlManager = new XmlManager(this);

                            xmlManager.Export(true);

                            break;
                        }
                    case CollectionImportMenuId:
                        {
                            var xmlManager = new XmlManager(this);

                            xmlManager.Import(true);

                            break;
                        }
                    case CollectionExportToCsvMenuId:
                        {
                            var csvManager = new CsvManager(this);

                            csvManager.Export(true);

                            break;
                        }
                    case CollectionFlaggedExportToXmlMenuId:
                        {
                            var xmlManager = new XmlManager(this);

                            xmlManager.Export(false);

                            break;
                        }
                    case CollectionFlaggedImportMenuId:
                        {
                            var xmlManager = new XmlManager(this);

                            xmlManager.Import(false);

                            break;
                        }
                    case CollectionFlaggedExportToCsvMenuId:
                        {
                            var csvManager = new CsvManager(this);

                            csvManager.Export(false);

                            break;
                        }
                    case ToolsOptionsMenuId:
                        {
                            OpenSettings();

                            break;
                        }
                    case ToolsExportOptionsMenuId:
                        {
                            ExportOptions();

                            break;
                        }
                    case ToolsImportOptionsMenuId:
                        {
                            ImportOptions();

                            break;
                        }
                    case ToolsOpenCalculatorMenuId:
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
                    MessageBox.Show(string.Format(MessageBoxTexts.CriticalError, ex.Message, _errorFile)
                        , MessageBoxTexts.CriticalErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    if (File.Exists(_errorFile))
                    {
                        File.Delete(_errorFile);
                    }

                    LogException(ex);
                }
                catch (Exception inEx)
                {
                    MessageBox.Show(string.Format(MessageBoxTexts.FileCantBeWritten, _errorFile, inEx.Message), MessageBoxTexts.ErrorHeader
                        , MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        internal void ImportOptions()
        {
            using (var ofd = new OpenFileDialog()
            {
                CheckFileExists = true,
                Filter = "XML files|*.xml",
                Multiselect = false,
                RestoreDirectory = true,
                Title = Texts.LoadXmlFile,
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    DefaultValues dv;
                    try
                    {
                        dv = Serializer<DefaultValues>.Deserialize(ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        dv = null;

                        MessageBox.Show(string.Format(MessageBoxTexts.FileCantBeRead, ofd.FileName, ex.Message)
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
            using (var sfd = new SaveFileDialog()
            {
                AddExtension = true,
                DefaultExt = ".xml",
                Filter = "XML files|*.xml",
                OverwritePrompt = true,
                RestoreDirectory = true,
                Title = Texts.SaveXmlFile,
                FileName = "EnhancedPurchaseInfoOptions.xml",
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var dv = Settings.DefaultValues;

                    try
                    {
                        Serializer<DefaultValues>.Serialize(sfd.FileName, dv);

                        MessageBox.Show(MessageBoxTexts.Done, MessageBoxTexts.InformationHeader, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format(MessageBoxTexts.FileCantBeWritten, sfd.FileName, ex.Message)
                            , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void OpenSettings()
        {
            using (var form = new SettingsForm(this))
            {
                form.ShowDialog();
            }
        }

        private void OpenEditor(bool fullEdit)
        {
            var profileId = _currentProfileId;

            if (string.IsNullOrEmpty(profileId))
            {
                var profile = Api.GetDisplayedDVD();

                profileId = profile.GetProfileID();
            }

            if (string.IsNullOrEmpty(profileId) == false)
            {
                Api.DVDByProfileID(out var profile, profileId, PluginConstants.DATASEC_AllSections, -1);

                if (profile.GetProfileID() == null)
                {
                    profile = Api.CreateDVD();

                    profile.SetProfileID(profileId);
                }

                using (var form = new MainForm(this, profile, fullEdit))
                {
                    form.OpenCalculator += OnOpenCalculator;

                    form.ShowDialog();

                    form.OpenCalculator -= OnOpenCalculator;
                }
            }
        }

        void OnOpenCalculator(object sender, EventArgs e)
        {
            var file = GetType().Assembly.Location;

            var fi = new FileInfo(file);

            var dir = fi.DirectoryName;

            file = "PurchasePriceSplitter.exe";
            file = Path.Combine(dir, file);

            var args = "/lang=" + Settings.DefaultValues.UiLanguage.Name;

            var p = new Process()
            {
                StartInfo = new ProcessStartInfo(file, args),
            };

            p.Start();
        }

        private void LogException(Exception ex)
        {
            ex = WrapCOMException(ex);

            var exceptionXml = new ExceptionXml(ex);

            Serializer<ExceptionXml>.Serialize(_errorFile, exceptionXml);
        }

        private Exception WrapCOMException(Exception ex)
        {
            var returnEx = ex;

            var comEx = ex as COMException;

            if (comEx != null)
            {
                string lastApiError = Api.GetLastError();

                var newEx = new EnhancedCOMException(lastApiError, comEx);

                returnEx = newEx;
            }

            return returnEx;
        }

        #region Plugin Registering

        [DllImport("user32.dll")]
        public extern static int SetParent(int child, int parent);

        [ComImport(), Guid("0002E005-0000-0000-C000-000000000046")]
        internal class StdComponentCategoriesMgr { }

        [ComRegisterFunction()]
        public static void RegisterServer(Type t)
        {
            var cr = (CategoryRegistrar.ICatRegister)new StdComponentCategoriesMgr();

            var clsidThis = new Guid(ClassGuid.ClassID);

            var catid = new Guid("833F4274-5632-41DB-8FC5-BF3041CEA3F1");

            cr.RegisterClassImplCategories(ref clsidThis, 1, new[] { catid });
        }

        [ComUnregisterFunction()]
        public static void UnregisterServer(Type t)
        {
            var cr = (CategoryRegistrar.ICatRegister)new StdComponentCategoriesMgr();

            var clsidThis = new Guid(ClassGuid.ClassID);

            var catid = new Guid("833F4274-5632-41DB-8FC5-BF3041CEA3F1");

            cr.UnRegisterClassImplCategories(ref clsidThis, 1, new[] { catid });
        }

        #endregion
    }
}