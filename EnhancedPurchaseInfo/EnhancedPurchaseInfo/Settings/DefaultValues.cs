using DoenaSoft.DVDProfiler.EnhancedPurchaseInfo.Resources;
using Invelos.DVDProfilerPlugin;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    [ComVisible(false)]
    [Serializable]
    public sealed class DefaultValues
    {
        #region Labels

        #region Prices

        public String OriginalPriceLabel = Texts.AdditionalPrice1;

        public String ShippingCostLabel = Texts.ShippingCost;

        public String CreditCardChargeLabel = Texts.CreditCardCharge;

        public String CreditCardFeesLabel = Texts.CreditCardFees;

        public String DiscountLabel = Texts.Discount;

        public String CustomsFeesLabel = Texts.CustomsFees;

        public String CouponTypeLabel = Texts.CouponType;

        public String CouponCodeLabel = Texts.CouponCode;

        public String AdditionalPrice1Label = Texts.AdditionalPrice1;

        public String AdditionalPrice2Label = Texts.AdditionalPrice2;

        #endregion

        #region Dates

        public String OrderDateLabel = Texts.OrderDate;

        public String ShippingDateLabel = Texts.ShippingDate;

        public String DeliveryDateLabel = Texts.DeliveryDate;

        public String AdditionalDate1Label = Texts.AdditionalDate1;

        public String AdditionalDate2Label = Texts.AdditionalDate2;

        #endregion

        #endregion

        #region Basics

        public Boolean Id = true;

        public Boolean Title = true;

        public Boolean Edition = true;

        public Boolean SortTitle = true;

        public Boolean PurchasePlace = true;

        #endregion

        #region Prices

        #region Invelos Data

        public Boolean PurchasePrice = true;

        public Boolean SRP = true;

        #endregion

        #region Plugin Data

        public Boolean OriginalPrice = true;

        public Boolean ShippingCost = true;

        public Boolean CreditCardCharge = true;

        public Boolean CreditCardFees = true;

        public Boolean Discount = true;

        public Boolean CustomsFees = false;

        public Boolean CouponType = false;

        public Boolean CouponCode = false;

        public Boolean AdditionalPrice1 = false;

        public Boolean AdditionalPrice2 = false;

        #endregion

        #endregion

        #region Dates

        #region Invelos Data

        public Boolean PurchaseDate = true;

        public Boolean OrderDate = true;

        public Boolean ShippingDate = true;

        public Boolean DeliveryDate = true;

        public Boolean AdditionalDate1 = false;

        public Boolean AdditionalDate2 = false;

        #endregion

        #endregion

        #region Misc

        private Int32 m_DefaultCurrency = PluginConstants.CURRENCY_USD;

        public Int32 DefaultCurrency
        {
            get
            {
                return (m_DefaultCurrency);
            }
            set
            {
                if ((value < 0) || (value > 30))
                {
                    throw (new ArgumentException("Value must be between 0 and 30.", "value"));
                }
                m_DefaultCurrency = value;
            }
        }

        public Int32 UiLcid
        {
            get
            {
                return (UiLanguage.LCID);
            }
            set
            {
                UiLanguage = CultureInfo.GetCultureInfo(value);
            }
        }

        [XmlIgnore]
        internal CultureInfo UiLanguage;

        public Boolean ExportToCollectionXml = false;

        public Boolean ShippingCostSplitAbsolute = true;

        #endregion

        public DefaultValues()
        {
            UiLanguage = GetUILanguage();
        }

        internal static CultureInfo GetUILanguage()
            => ((Thread.CurrentThread.CurrentUICulture.Name.StartsWith("de")) ? (CultureInfo.GetCultureInfo("de")) : (CultureInfo.GetCultureInfo("en")));
    }
}