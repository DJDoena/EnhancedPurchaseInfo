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

        public string OriginalPriceLabel = Texts.AdditionalPrice1;

        public string ShippingCostLabel = Texts.ShippingCost;

        public string CreditCardChargeLabel = Texts.CreditCardCharge;

        public string CreditCardFeesLabel = Texts.CreditCardFees;

        public string DiscountLabel = Texts.Discount;

        public string CustomsFeesLabel = Texts.CustomsFees;

        public string CouponTypeLabel = Texts.CouponType;

        public string CouponCodeLabel = Texts.CouponCode;

        public string AdditionalPrice1Label = Texts.AdditionalPrice1;

        public string AdditionalPrice2Label = Texts.AdditionalPrice2;

        #endregion

        #region Dates

        public string OrderDateLabel = Texts.OrderDate;

        public string ShippingDateLabel = Texts.ShippingDate;

        public string DeliveryDateLabel = Texts.DeliveryDate;

        public string AdditionalDate1Label = Texts.AdditionalDate1;

        public string AdditionalDate2Label = Texts.AdditionalDate2;

        #endregion

        #endregion

        #region Basics

        public bool Id = true;

        public bool Title = true;

        public bool Edition = true;

        public bool SortTitle = true;

        public bool PurchasePlace = true;

        #endregion

        #region Prices

        #region Invelos Data

        public bool PurchasePrice = true;

        public bool SRP = true;

        #endregion

        #region Plugin Data

        public bool OriginalPrice = true;

        public bool ShippingCost = true;

        public bool CreditCardCharge = true;

        public bool CreditCardFees = true;

        public bool Discount = true;

        public bool CustomsFees = false;

        public bool CouponType = false;

        public bool CouponCode = false;

        public bool AdditionalPrice1 = false;

        public bool AdditionalPrice2 = false;

        #endregion

        #endregion

        #region Dates

        #region Invelos Data

        public bool PurchaseDate = true;

        public bool OrderDate = true;

        public bool ShippingDate = true;

        public bool DeliveryDate = true;

        public bool AdditionalDate1 = false;

        public bool AdditionalDate2 = false;

        #endregion

        #endregion

        #region Misc

        private int m_DefaultCurrency = PluginConstants.CURRENCY_USD;

        public int DefaultCurrency
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

        public int UiLcid
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

        public bool ExportToCollectionXml = false;

        public bool ShippingCostSplitAbsolute = true;

        #endregion

        public DefaultValues()
        {
            UiLanguage = GetUILanguage();
        }

        internal static CultureInfo GetUILanguage()
            => ((Thread.CurrentThread.CurrentUICulture.Name.StartsWith("de")) ? (CultureInfo.GetCultureInfo("de")) : (CultureInfo.GetCultureInfo("en")));
    }
}