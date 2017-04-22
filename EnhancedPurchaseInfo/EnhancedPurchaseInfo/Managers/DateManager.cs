using System;
using Invelos.DVDProfilerPlugin;

namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    internal sealed class DateManager
    {
        private readonly IDVDInfo Profile;

        internal readonly static DateTime DateNotSet;

        static DateManager()
        {
            DateNotSet = new DateTime(1899, 12, 30); //it's what Invelos uses
        }

        internal DateManager(IDVDInfo profile)
        {
            Profile = profile;
        }

        internal delegate Boolean GetDateDelegate(out DateTime date);

        #region Invelos Data

        #region PurchaseDate

        internal Boolean GetPurchaseDate(out DateTime pd)
        {
            if (Profile.PurchaseDateIsEmpty())
            {
                pd = DateNotSet;
            }
            else
            {
                pd = Profile.GetPurchaseDate();
            }
            return (pd != DateNotSet);
        }

        internal void SetPurchaseDate(DateTime pd)
        {
            if (pd == DateNotSet)
            {
                Profile.ClearPurchaseDate();
            }
            else
            {
                Profile.SetPurchaseDate(pd);
            }
        }

        #endregion

        #endregion

        #region Plugin Data

        #region OrderDate

        internal Boolean GetOrderDate(out DateTime od)
        {
            return (GetDate(Constants.OrderDate, out od));
        }

        internal void SetOrderDate(DateTime od)
        {
            SetDate(Constants.OrderDate, od);
        }

        #endregion

        #region ShippingDate

        internal Boolean GetShippingDate(out DateTime sd)
        {
            return (GetDate(Constants.ShippingDate, out sd));
        }

        internal void SetShippingDate(DateTime sd)
        {
            SetDate(Constants.ShippingDate, sd);
        }

        #endregion

        #region DeliveryDate

        internal Boolean GetDeliveryDate(out DateTime dd)
        {
            return (GetDate(Constants.DeliveryDate, out dd));
        }

        internal void SetDeliveryDate(DateTime dd)
        {
            SetDate(Constants.DeliveryDate, dd);
        }

        #endregion

        #region AdditionalDate1

        internal Boolean GetAdditionalDate1(out DateTime ad1)
        {
            return (GetDate(Constants.AdditionalDate1, out ad1));
        }

        internal void SetAdditionalDate1(DateTime ad1)
        {
            SetDate(Constants.AdditionalDate1, ad1);
        }

        #endregion

        #region AdditionalDate2

        internal Boolean GetAdditionalDate2(out DateTime ad2)
        {
            return (GetDate(Constants.AdditionalDate2, out ad2));
        }

        internal void SetAdditionalDate2(DateTime ad2)
        {
            SetDate(Constants.AdditionalDate2, ad2);
        }

        #endregion

        #endregion

        private Boolean GetDate(String fieldName, out DateTime date)
        {
            date = Profile.GetCustomDateTime(Constants.FieldDomain, fieldName, Constants.ReadKey, DateNotSet);
            return (date != DateNotSet);
        }

        private void SetDate(String fieldName, DateTime date)
        {
            if (date == DateNotSet)
            {
                Profile.ClearCustomField(Constants.FieldDomain, fieldName, InternalConstants.WriteKey);
            }
            else
            {
                date = new DateTime(date.Year, date.Month, date.Day);
                Profile.SetCustomDateTime(Constants.FieldDomain, fieldName, InternalConstants.WriteKey, date);
            }
        }
    }
}