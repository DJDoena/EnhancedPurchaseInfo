using System;
using System.Text;
using Invelos.DVDProfilerPlugin;

namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    internal sealed class TextManager
    {
        private const string TextNotSet = null;

        public const decimal DigitDivider = 10000m;

        private readonly IDVDInfo _profile;

        public TextManager(IDVDInfo profile)
        {
            _profile = profile;
        }

        #region Plugin Data

        #region CouponType

        internal string GetCouponTypeWithFallback() => GetTextWithFallback(GetCouponType);

        internal bool GetCouponType(out string ct) => GetText(Constants.CouponType, out ct);

        internal void SetCouponType(string ct) => SetText(Constants.CouponType, ct);

        #endregion

        #region CouponCode

        internal string GetCouponCodeWithFallback() => GetTextWithFallback(GetCouponCode);

        internal bool GetCouponCode(out string cc) => GetText(Constants.CouponCode, out cc);

        internal void SetCouponCode(string cc) => SetText(Constants.CouponCode, cc);

        #endregion

        #endregion

        #region Text

        private delegate bool GetTextDelegate(out string text);

        internal bool GetText(string fieldName, out string text)
        {
            var decoded = TextNotSet;

            var encoded = _profile.GetCustomString(Constants.FieldDomain, fieldName, Constants.ReadKey, TextNotSet);

            if (encoded != TextNotSet)
            {
                decoded = Encoding.Unicode.GetString(Convert.FromBase64String(encoded));
            }

            text = decoded;

            return text != TextNotSet;
        }

        private string GetTextWithFallback(GetTextDelegate getText)
        {
            if (getText(out var text) == false)
            {
                text = string.Empty;
            }

            return text;
        }

        private void SetText(string fieldName, string decoded)
        {
            if (string.IsNullOrEmpty(decoded))
            {
                _profile.ClearCustomField(Constants.FieldDomain, fieldName, InternalConstants.WriteKey);
            }
            else
            {
                var encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(decoded));

                _profile.SetCustomString(Constants.FieldDomain, fieldName, InternalConstants.WriteKey, encoded);
            }
        }

        #endregion
    }
}