using System.Globalization;
using Invelos.DVDProfilerPlugin;

namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    internal sealed class PriceManager
    {
        private const long PriceNotSet = -1;

        private const int CurrencyNotSet = -1;

        private readonly IDVDInfo _profile;

        public PriceManager(IDVDInfo profile)
        {
            _profile = profile;
        }

        #region Invelos Data

        #region PurchasePrice

        internal bool GetPurchasePriceCurrency(out CurrencyInfo ppc)
        {
            var id = _profile.GetPurchasePriceCurrency();

            ppc = new CurrencyInfo(id);

            return true;
        }

        internal string GetPurchasePriceWithFallback()
        {
            if (_profile.GetPurchasePriceIsEmpty())
            {
                return string.Empty;
            }

            var priceValue = _profile.GetPurchasePriceValue();

            var price = priceValue / Constants.DigitDivider;

            var priceString = GetFormattedPriceString(price);

            return priceString;
        }

        internal void SetPurchasePrice(string pp)
        {
            if (string.IsNullOrEmpty(pp))
            {
                SetPurchasePrice(0, true);
            }
            else
            {
                if (decimal.TryParse(pp, NumberStyles.Float, CultureInfo.CurrentCulture, out var price))
                {
                    var priceValue = (long)(price * Constants.DigitDivider);

                    SetPurchasePrice(priceValue, false);
                }
            }
        }

        private void SetPurchasePrice(long priceValue, bool priceIsEmpty)
        {
            if (priceIsEmpty)
            {
                _profile.ClearPurchasePrice();
            }
            else
            {
                _profile.SetPurchasePriceValue(priceValue);
            }
        }

        internal void SetPurchasePriceCurrency(CurrencyInfo ppc)
        {
            if (ppc.IsEmpty)
            {
                ppc = new CurrencyInfo(PluginConstants.CURRENCY_USD);
            }

            _profile.SetPurchasePriceCurrency(ppc.Id);
        }

        #endregion

        #region SRP

        internal bool GetSRPCurrency(out CurrencyInfo opc)
        {
            var id = _profile.GetSRPCurrency();

            opc = new CurrencyInfo(id);

            return true;
        }

        internal string GetSRPWithFallback()
        {
            var priceValue = _profile.GetSRPValue();

            var price = priceValue / Constants.DigitDivider;

            var priceString = GetFormattedPriceString(price);

            return priceString;
        }

        internal void SetSRP(string srp)
        {
            if (string.IsNullOrEmpty(srp))
            {
                SetSRP(0);
            }
            else
            {
                if (decimal.TryParse(srp, NumberStyles.Float, CultureInfo.CurrentCulture, out var price))
                {
                    var priceValue = (long)(price * Constants.DigitDivider);

                    SetSRP(priceValue);
                }
            }
        }

        private void SetSRP(long priceValue)
        {
            if (_profile.GetSRPValue() != priceValue)
            {
                _profile.SetSRPValue(priceValue);
            }
        }

        internal void SetSRPCurrency(CurrencyInfo srpc)
        {
            if (srpc.IsEmpty)
            {
                srpc = new CurrencyInfo(PluginConstants.CURRENCY_USD);
            }

            _profile.SetSRPCurrency(srpc.Id);
        }

        #endregion

        #endregion

        #region Plugin Data

        #region OriginalPrice

        internal string GetOriginalPriceWithFallback() => GetPriceWithFallback(GetOriginalPrice);

        internal bool GetOriginalPrice(out decimal op) => GetPriceFromField(Constants.OriginalPrice, out op);

        internal void SetOriginalPrice(string op) => SetPrice(Constants.OriginalPrice, op);

        internal bool GetOriginalPriceCurrency(out CurrencyInfo opc) => GetCurrency(Constants.OriginalPrice, out opc);

        internal void SetOriginalPriceCurrency(CurrencyInfo opc) => SetCurrency(Constants.OriginalPrice, opc);

        #endregion

        #region ShippingCost

        internal string GetShippingCostWithFallback() => GetPriceWithFallback(GetShippingCost);

        internal bool GetShippingCost(out decimal sc) => GetPriceFromField(Constants.ShippingCost, out sc);

        internal void SetShippingCost(string sc) => SetPrice(Constants.ShippingCost, sc);

        internal bool GetShippingCostCurrency(out CurrencyInfo scc) => GetCurrency(Constants.ShippingCost, out scc);

        internal void SetShippingCostCurrency(CurrencyInfo scc) => SetCurrency(Constants.ShippingCost, scc);

        #endregion

        #region CreditCardCharge

        internal string GetCreditCardChargeWithFallback() => GetPriceWithFallback(GetCreditCardCharge);

        internal bool GetCreditCardCharge(out decimal ccc) => GetPriceFromField(Constants.CreditCardCharge, out ccc);

        internal void SetCreditCardCharge(string ccc) => SetPrice(Constants.CreditCardCharge, ccc);

        internal bool GetCreditCardChargeCurrency(out CurrencyInfo cccc) => GetCurrency(Constants.CreditCardCharge, out cccc);

        internal void SetCreditCardChargeCurrency(CurrencyInfo cccc) => SetCurrency(Constants.CreditCardCharge, cccc);

        #endregion

        #region CreditCardFees

        internal string GetCreditCardFeesWithFallback() => GetPriceWithFallback(GetCreditCardFees);

        internal bool GetCreditCardFees(out decimal ccf) => GetPriceFromField(Constants.CreditCardFees, out ccf);

        internal void SetCreditCardFees(string ccf) => SetPrice(Constants.CreditCardFees, ccf);

        internal bool GetCreditCardFeesCurrency(out CurrencyInfo ccfc) => GetCurrency(Constants.CreditCardFees, out ccfc);

        internal void SetCreditCardFeesCurrency(CurrencyInfo ccfc) => SetCurrency(Constants.CreditCardFees, ccfc);

        #endregion

        #region Discount

        internal string GetDiscountWithFallback() => GetPriceWithFallback(GetDiscount);

        internal bool GetDiscount(out decimal discount) => GetPriceFromField(Constants.Discount, out discount);

        internal void SetDiscount(string discount) => SetPrice(Constants.Discount, discount);

        internal bool GetDiscountCurrency(out CurrencyInfo discountC) => GetCurrency(Constants.Discount, out discountC);

        internal void SetDiscountCurrency(CurrencyInfo discountC) => SetCurrency(Constants.Discount, discountC);

        #endregion

        #region CustomsFees

        internal string GetCustomsFeesWithFallback() => GetPriceWithFallback(GetCustomsFees);

        internal bool GetCustomsFees(out decimal cf) => GetPriceFromField(Constants.CustomsFees, out cf);

        internal void SetCustomsFees(string cf) => SetPrice(Constants.CustomsFees, cf);

        internal bool GetCustomsFeesCurrency(out CurrencyInfo cfc) => GetCurrency(Constants.CustomsFees, out cfc);

        internal void SetCustomsFeesCurrency(CurrencyInfo cfc) => SetCurrency(Constants.CustomsFees, cfc);

        #endregion

        #region AdditionalPrice1

        internal string GetAdditionalPrice1WithFallback() => GetPriceWithFallback(GetAdditionalPrice1);

        internal bool GetAdditionalPrice1(out decimal ap1) => GetPriceFromField(Constants.AdditionalPrice1, out ap1);

        internal void SetAdditionalPrice1(string ap1) => SetPrice(Constants.AdditionalPrice1, ap1);

        internal bool GetAdditionalPrice1Currency(out CurrencyInfo ap1c) => GetCurrency(Constants.AdditionalPrice1, out ap1c);

        internal void SetAdditionalPrice1Currency(CurrencyInfo ap1c) => SetCurrency(Constants.AdditionalPrice1, ap1c);

        #endregion

        #region AdditionalPrice2

        internal string GetAdditionalPrice2WithFallback() => GetPriceWithFallback(GetAdditionalPrice2);

        internal bool GetAdditionalPrice2(out decimal ap2) => GetPriceFromField(Constants.AdditionalPrice2, out ap2);

        internal void SetAdditionalPrice2(string ap2) => SetPrice(Constants.AdditionalPrice2, ap2);

        internal bool GetAdditionalPrice2Currency(out CurrencyInfo ap2c) => GetCurrency(Constants.AdditionalPrice2, out ap2c);

        internal void SetAdditionalPrice2Currency(CurrencyInfo ap2c) => SetCurrency(Constants.AdditionalPrice2, ap2c);

        #endregion

        #endregion

        #region Integer

        internal delegate bool GetCurrencyDelegate(out CurrencyInfo ci);

        internal bool GetCurrency(string fieldName, out CurrencyInfo ci)
        {
            var currencyId = _profile.GetCustomInt(Constants.FieldDomain, fieldName + Constants.CurrencySuffix, Constants.ReadKey, CurrencyNotSet);

            if (currencyId != CurrencyNotSet)
            {
                ci = new CurrencyInfo(currencyId);
            }
            else
            {
                ci = CurrencyInfo.Empty;
            }

            return currencyId != CurrencyNotSet;
        }

        private void SetCurrency(string fieldName, CurrencyInfo ci)
        {
            _profile.SetCustomInt(Constants.FieldDomain, fieldName + Constants.CurrencySuffix, InternalConstants.WriteKey, ci.Id);
        }

        #endregion

        #region decimal

        internal delegate bool GetPriceDelegate(out decimal price);

        internal static bool GetPriceFromText(string decoded, out decimal price)
        {
            if (string.IsNullOrEmpty(decoded) == false)
            {
                if (decimal.TryParse(decoded, NumberStyles.Float, CultureInfo.CurrentCulture, out price))
                {
                    return true;
                }
            }

            price = 0;

            return false;
        }

        internal bool GetPriceFromField(string fieldName, out decimal price)
        {
            price = PriceNotSet;

            var encoded = _profile.GetCustomCurrency(Constants.FieldDomain, fieldName, Constants.ReadKey, PriceNotSet);

            if (encoded != PriceNotSet)
            {
                price = encoded / Constants.DigitDivider;
            }

            return price != PriceNotSet;
        }

        private string GetPriceWithFallback(GetPriceDelegate getPrice)
        {
            if (getPrice(out var price) == false)
            {
                return string.Empty;
            }
            else
            {
                var priceString = GetFormattedPriceString(price);

                return priceString;
            }
        }

        internal string GetFormattedPriceString(decimal price)
        {
            var currentCulture = CultureInfo.CurrentCulture;

            //var rounded = Math.Round(price, 2, MidpointRounding.AwayFromZero);

            var priceString = price.ToString("F2", currentCulture);

            return priceString;
        }

        private void SetPrice(string fieldName, string decoded)
        {
            if (GetPriceFromText(decoded, out var price) == false)
            {
                _profile.ClearCustomField(Constants.FieldDomain, fieldName, InternalConstants.WriteKey);
            }
            else
            {
                var encoded = (long)(price * Constants.DigitDivider);

                _profile.SetCustomCurrency(Constants.FieldDomain, fieldName, InternalConstants.WriteKey, encoded);
            }
        }

        #endregion
    }
}