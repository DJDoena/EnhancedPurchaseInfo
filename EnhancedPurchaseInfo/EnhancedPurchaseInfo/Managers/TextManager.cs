using System;
using System.Text;
using Invelos.DVDProfilerPlugin;

namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    internal sealed class TextManager
    {
        private const String TextNotSet = null;

        private const Int64 PriceNotSet = -1;

        private const Int32 CurrencyNotSet = -1;

        public const Decimal DigitDivider = 10000m;

        private readonly IDVDInfo Profile;

        public TextManager(IDVDInfo profile)
        {
            Profile = profile;
        }

        #region Plugin Data

        #region CouponType

        internal String GetCouponTypeWithFallback()
        {
            return (GetTextWithFallback(GetCouponType));
        }

        internal Boolean GetCouponType(out String ct)
        {
            return (GetText(Constants.CouponType, out ct));
        }

        internal void SetCouponType(String ct)
        {
            SetText(Constants.CouponType, ct);
        }

        #endregion

        #region CouponCode

        internal String GetCouponCodeWithFallback()
        {
            return (GetTextWithFallback(GetCouponCode));
        }

        internal Boolean GetCouponCode(out String cc)
        {
            return (GetText(Constants.CouponCode, out cc));
        }

        internal void SetCouponCode(String cc)
        {
            SetText(Constants.CouponCode, cc);
        }

        #endregion

        #endregion

        #region Text

        private delegate Boolean GetTextDelegate(out String text);

        internal Boolean GetText(String fieldName, out String text)
        {
            String encoded;
            String decoded;

            decoded = TextNotSet;
            encoded = Profile.GetCustomString(Constants.FieldDomain, fieldName, Constants.ReadKey, TextNotSet);
            if (encoded != TextNotSet)
            {
                decoded = Encoding.Unicode.GetString(Convert.FromBase64String(encoded));
            }
            text = decoded;
            return (text != TextNotSet);
        }

        private String GetTextWithFallback(GetTextDelegate getText)
        {
            String text;

            if (getText(out text) == false)
            {
                text = String.Empty;
            }
            return (text);
        }

        private void SetText(String fieldName, String decoded)
        {
            String encoded;

            if (String.IsNullOrEmpty(decoded))
            {
                Profile.ClearCustomField(Constants.FieldDomain, fieldName, InternalConstants.WriteKey);
            }
            else
            {
                encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(decoded));
                Profile.SetCustomString(Constants.FieldDomain, fieldName, InternalConstants.WriteKey, encoded);
            }
        }

        #endregion
    }
}