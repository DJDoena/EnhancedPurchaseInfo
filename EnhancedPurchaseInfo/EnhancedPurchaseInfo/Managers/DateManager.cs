using System;
using Invelos.DVDProfilerPlugin;

namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    internal sealed class DateManager
    {
        private readonly IDVDInfo _profile;

        internal readonly static DateTime _dateNotSet;

        static DateManager()
        {
            _dateNotSet = new DateTime(1899, 12, 30); //it's what Invelos uses
        }

        internal DateManager(IDVDInfo profile)
        {
            _profile = profile;
        }

        internal delegate bool GetDateDelegate(out DateTime date);

        #region Invelos Data

        #region PurchaseDate

        internal bool GetPurchaseDate(out DateTime pd)
        {
            if (_profile.PurchaseDateIsEmpty())
            {
                pd = _dateNotSet;
            }
            else
            {
                pd = _profile.GetPurchaseDate();
            }

            return pd != _dateNotSet;
        }

        internal void SetPurchaseDate(DateTime pd)
        {
            if (pd == _dateNotSet)
            {
                _profile.ClearPurchaseDate();
            }
            else
            {
                _profile.SetPurchaseDate(pd);
            }
        }

        #endregion

        #endregion

        #region Plugin Data

        #region OrderDate

        internal bool GetOrderDate(out DateTime od) => GetDate(Constants.OrderDate, out od);

        internal void SetOrderDate(DateTime od) => SetDate(Constants.OrderDate, od);

        #endregion

        #region ShippingDate

        internal bool GetShippingDate(out DateTime sd) => GetDate(Constants.ShippingDate, out sd);

        internal void SetShippingDate(DateTime sd) => SetDate(Constants.ShippingDate, sd);

        #endregion

        #region DeliveryDate

        internal bool GetDeliveryDate(out DateTime dd) => GetDate(Constants.DeliveryDate, out dd);

        internal void SetDeliveryDate(DateTime dd) => SetDate(Constants.DeliveryDate, dd);

        #endregion

        #region AdditionalDate1

        internal bool GetAdditionalDate1(out DateTime ad1) => GetDate(Constants.AdditionalDate1, out ad1);

        internal void SetAdditionalDate1(DateTime ad1) => SetDate(Constants.AdditionalDate1, ad1);

        #endregion

        #region AdditionalDate2

        internal bool GetAdditionalDate2(out DateTime ad2) => GetDate(Constants.AdditionalDate2, out ad2);

        internal void SetAdditionalDate2(DateTime ad2) => SetDate(Constants.AdditionalDate2, ad2);

        #endregion

        #endregion

        private bool GetDate(string fieldName, out DateTime date)
        {
            date = _profile.GetCustomDateTime(Constants.FieldDomain, fieldName, Constants.ReadKey, _dateNotSet);

            return date != _dateNotSet;
        }

        private void SetDate(string fieldName, DateTime date)
        {
            if (date == _dateNotSet)
            {
                _profile.ClearCustomField(Constants.FieldDomain, fieldName, InternalConstants.WriteKey);
            }
            else
            {
                date = new DateTime(date.Year, date.Month, date.Day);

                _profile.SetCustomDateTime(Constants.FieldDomain, fieldName, InternalConstants.WriteKey, date);
            }
        }
    }
}