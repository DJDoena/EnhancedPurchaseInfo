using System;
using System.Globalization;
using Invelos.DVDProfilerPlugin;

namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    internal sealed class PriceManager
    {
        private const String TextNotSet = null;

        private const Int64 PriceNotSet = -1;

        private const Int32 CurrencyNotSet = -1;

        private readonly IDVDInfo Profile;

        public PriceManager(IDVDInfo profile)
        {
            Profile = profile;
        }

        #region Invelos Data

        #region PurchasePrice

        internal Boolean GetPurchasePriceCurrency(out CurrencyInfo ppc)
        {
            Int32 id;

            id = Profile.GetPurchasePriceCurrency();
            ppc = new CurrencyInfo(id);
            return (true);
        }

        internal String GetPurchasePriceWithFallback()
        {
            Int64 priceValue;
            Decimal price;
            String priceString;

            priceString = String.Empty;
            if (Profile.GetPurchasePriceIsEmpty())
            {
                return (String.Empty);
            }
            priceValue = Profile.GetPurchasePriceValue();
            price = ((Decimal)priceValue) / Constants.DigitDivider;
            priceString = GetFormattedPriceString(price);
            return (priceString);
        }

        internal void SetPurchasePrice(String pp)
        {
            if (String.IsNullOrEmpty(pp))
            {
                SetPurchasePrice(0, true);
            }
            else
            {
                Decimal price;

                if (Decimal.TryParse(pp, NumberStyles.Float, CultureInfo.CurrentCulture, out price))
                {
                    Int64 priceValue;

                    priceValue = (Int64)(price * Constants.DigitDivider);
                    SetPurchasePrice(priceValue, false);
                }
            }
        }

        private void SetPurchasePrice(Int64 priceValue
            , Boolean priceIsEmpty)
        {
            if (priceIsEmpty)
            {
                Profile.ClearPurchasePrice();
            }
            else
            {
                Profile.SetPurchasePriceValue(priceValue);
            }
        }

        internal void SetPurchasePriceCurrency(CurrencyInfo ppc)
        {
            if (ppc.IsEmpty)
            {
                ppc = new CurrencyInfo(PluginConstants.CURRENCY_USD);
            }
            Profile.SetPurchasePriceCurrency(ppc.Id);
        }

        #endregion

        #region SRP

        internal Boolean GetSRPCurrency(out CurrencyInfo opc)
        {
            Int32 id;

            id = Profile.GetSRPCurrency();
            opc = new CurrencyInfo(id);
            return (true);
        }

        internal String GetSRPWithFallback()
        {
            Int64 priceValue;
            Decimal price;
            String priceString;

            priceString = String.Empty;
            priceValue = Profile.GetSRPValue();
            price = ((Decimal)priceValue) / Constants.DigitDivider;
            priceString = GetFormattedPriceString(price);
            return (priceString);
        }

        internal void SetSRP(String srp)
        {
            Int64 priceValue;

            if (String.IsNullOrEmpty(srp))
            {
                priceValue = 0;
                SetSRP(priceValue);
            }
            else
            {
                Decimal price;

                if (Decimal.TryParse(srp, NumberStyles.Float, CultureInfo.CurrentCulture, out price))
                {
                    priceValue = (Int64)(price * Constants.DigitDivider);
                    SetSRP(priceValue);
                }
            }
        }

        private void SetSRP(Int64 priceValue)
        {
            if (Profile.GetSRPValue() != priceValue)
            {
                Profile.SetSRPValue(priceValue);
            }
        }

        internal void SetSRPCurrency(CurrencyInfo srpc)
        {
            if (srpc.IsEmpty)
            {
                srpc = new CurrencyInfo(PluginConstants.CURRENCY_USD);
            }
            Profile.SetSRPCurrency(srpc.Id);
        }

        #endregion

        #endregion

        #region Plugin Data

        #region OriginalPrice

        internal String GetOriginalPriceWithFallback()
        {
            return (GetPriceWithFallback(GetOriginalPrice));
        }

        internal Boolean GetOriginalPrice(out Decimal op)
        {
            return (GetPriceFromField(Constants.OriginalPrice, out op));
        }

        internal void SetOriginalPrice(String op)
        {
            SetPrice(Constants.OriginalPrice, op);
        }

        internal Boolean GetOriginalPriceCurrency(out CurrencyInfo opc)
        {
            return (GetCurrency(Constants.OriginalPrice, out opc));
        }

        internal void SetOriginalPriceCurrency(CurrencyInfo opc)
        {
            SetCurrency(Constants.OriginalPrice, opc);
        }

        #endregion

        #region ShippingCost

        internal String GetShippingCostWithFallback()
        {
            return (GetPriceWithFallback(GetShippingCost));
        }

        internal Boolean GetShippingCost(out Decimal sc)
        {
            return (GetPriceFromField(Constants.ShippingCost, out sc));
        }

        internal void SetShippingCost(String sc)
        {
            SetPrice(Constants.ShippingCost, sc);
        }

        internal Boolean GetShippingCostCurrency(out CurrencyInfo scc)
        {
            return (GetCurrency(Constants.ShippingCost, out scc));
        }

        internal void SetShippingCostCurrency(CurrencyInfo scc)
        {
            SetCurrency(Constants.ShippingCost, scc);
        }

        #endregion

        #region CreditCardCharge

        internal String GetCreditCardChargeWithFallback()
        {
            return (GetPriceWithFallback(GetCreditCardCharge));
        }

        internal Boolean GetCreditCardCharge(out Decimal ccc)
        {
            return (GetPriceFromField(Constants.CreditCardCharge, out ccc));
        }

        internal void SetCreditCardCharge(String ccc)
        {
            SetPrice(Constants.CreditCardCharge, ccc);
        }

        internal Boolean GetCreditCardChargeCurrency(out CurrencyInfo cccc)
        {
            return (GetCurrency(Constants.CreditCardCharge, out cccc));
        }

        internal void SetCreditCardChargeCurrency(CurrencyInfo cccc)
        {
            SetCurrency(Constants.CreditCardCharge, cccc);
        }

        #endregion

        #region CreditCardFees

        internal String GetCreditCardFeesWithFallback()
        {
            return (GetPriceWithFallback(GetCreditCardFees));
        }

        internal Boolean GetCreditCardFees(out Decimal ccf)
        {
            return (GetPriceFromField(Constants.CreditCardFees, out ccf));
        }

        internal void SetCreditCardFees(String ccf)
        {
            SetPrice(Constants.CreditCardFees, ccf);
        }

        internal Boolean GetCreditCardFeesCurrency(out CurrencyInfo ccfc)
        {
            return (GetCurrency(Constants.CreditCardFees, out ccfc));
        }

        internal void SetCreditCardFeesCurrency(CurrencyInfo ccfc)
        {
            SetCurrency(Constants.CreditCardFees, ccfc);
        }

        #endregion

        #region Discount

        internal String GetDiscountWithFallback()
        {
            return (GetPriceWithFallback(GetDiscount));
        }

        internal Boolean GetDiscount(out Decimal discount)
        {
            return (GetPriceFromField(Constants.Discount, out discount));
        }

        internal void SetDiscount(String discount)
        {
            SetPrice(Constants.Discount, discount);
        }

        internal Boolean GetDiscountCurrency(out CurrencyInfo discountC)
        {
            return (GetCurrency(Constants.Discount, out discountC));
        }

        internal void SetDiscountCurrency(CurrencyInfo discountC)
        {
            SetCurrency(Constants.Discount, discountC);
        }

        #endregion

        #region CustomsFees

        internal String GetCustomsFeesWithFallback()
        {
            return (GetPriceWithFallback(GetCustomsFees));
        }

        internal Boolean GetCustomsFees(out Decimal cf)
        {
            return (GetPriceFromField(Constants.CustomsFees, out cf));
        }

        internal void SetCustomsFees(String cf)
        {
            SetPrice(Constants.CustomsFees, cf);
        }

        internal Boolean GetCustomsFeesCurrency(out CurrencyInfo cfc)
        {
            return (GetCurrency(Constants.CustomsFees, out cfc));
        }

        internal void SetCustomsFeesCurrency(CurrencyInfo cfc)
        {
            SetCurrency(Constants.CustomsFees, cfc);
        }

        #endregion

        #region AdditionalPrice1

        internal String GetAdditionalPrice1WithFallback()
        {
            return (GetPriceWithFallback(GetAdditionalPrice1));
        }

        internal Boolean GetAdditionalPrice1(out Decimal ap1)
        {
            return (GetPriceFromField(Constants.AdditionalPrice1, out ap1));
        }

        internal void SetAdditionalPrice1(String ap1)
        {
            SetPrice(Constants.AdditionalPrice1, ap1);
        }

        internal Boolean GetAdditionalPrice1Currency(out CurrencyInfo ap1c)
        {
            return (GetCurrency(Constants.AdditionalPrice1, out ap1c));
        }

        internal void SetAdditionalPrice1Currency(CurrencyInfo ap1c)
        {
            SetCurrency(Constants.AdditionalPrice1, ap1c);
        }

        #endregion

        #region AdditionalPrice2

        internal String GetAdditionalPrice2WithFallback()
        {
            return (GetPriceWithFallback(GetAdditionalPrice2));
        }

        internal Boolean GetAdditionalPrice2(out Decimal ap2)
        {
            return (GetPriceFromField(Constants.AdditionalPrice2, out ap2));
        }

        internal void SetAdditionalPrice2(String ap2)
        {
            SetPrice(Constants.AdditionalPrice2, ap2);
        }

        internal Boolean GetAdditionalPrice2Currency(out CurrencyInfo ap2c)
        {
            return (GetCurrency(Constants.AdditionalPrice2, out ap2c));
        }

        internal void SetAdditionalPrice2Currency(CurrencyInfo ap2c)
        {
            SetCurrency(Constants.AdditionalPrice2, ap2c);
        }

        #endregion

        #endregion

        #region Integer

        internal delegate Boolean GetCurrencyDelegate(out CurrencyInfo ci);

        internal Boolean GetCurrency(String fieldName, out CurrencyInfo ci)
        {
            Int32 currencyId;

            currencyId = Profile.GetCustomInt(Constants.FieldDomain, fieldName + Constants.CurrencySuffix, Constants.ReadKey, CurrencyNotSet);
            if (currencyId != CurrencyNotSet)
            {
                ci = new CurrencyInfo(currencyId);
            }
            else
            {
                ci = CurrencyInfo.Empty;
            }
            return (currencyId != CurrencyNotSet);
        }

        private void SetCurrency(String fieldName, CurrencyInfo ci)
        {
            Profile.SetCustomInt(Constants.FieldDomain, fieldName + Constants.CurrencySuffix, InternalConstants.WriteKey, ci.Id);
        }

        #endregion

        #region Decimal

        internal delegate Boolean GetPriceDelegate(out Decimal price);

        internal static Boolean GetPriceFromText(String decoded
            , out Decimal price)
        {
            price = 0m;
            if (String.IsNullOrEmpty(decoded) == false)
            {
                if (Decimal.TryParse(decoded, NumberStyles.Float, CultureInfo.CurrentCulture, out price))
                {
                    return (true);
                }
            }
            return (false);
        }

        internal Boolean GetPriceFromField(String fieldName, out Decimal price)
        {
            Int64 encoded;

            price = PriceNotSet;
            encoded = Profile.GetCustomCurrency(Constants.FieldDomain, fieldName, Constants.ReadKey, PriceNotSet);
            if (encoded != PriceNotSet)
            {
                price = ((Decimal)encoded) / Constants.DigitDivider;
            }
            return (price != PriceNotSet);
        }

        private String GetPriceWithFallback(GetPriceDelegate getPrice)
        {
            Decimal price;
            String priceString;

            if (getPrice(out price) == false)
            {
                priceString = String.Empty;
            }
            else
            {
                priceString = GetFormattedPriceString(price);
            }
            return (priceString);
        }

        internal String GetFormattedPriceString(Decimal price)
        {
            String priceString;
            CultureInfo currentCulture;
            Decimal rounded;

            currentCulture = CultureInfo.CurrentCulture;
            rounded = Math.Round(price, 2, MidpointRounding.AwayFromZero);
            priceString = price.ToString("F2", currentCulture);
            return (priceString);
        }

        private void SetPrice(String fieldName, String decoded)
        {
            Decimal price;

            if (GetPriceFromText(decoded, out price) == false)
            {
                Profile.ClearCustomField(Constants.FieldDomain, fieldName, InternalConstants.WriteKey);
            }
            else
            {
                Int64 encoded;

                encoded = (Int64)(price * Constants.DigitDivider);
                Profile.SetCustomCurrency(Constants.FieldDomain, fieldName, InternalConstants.WriteKey, encoded);
            }
        }

        #endregion
    }
}