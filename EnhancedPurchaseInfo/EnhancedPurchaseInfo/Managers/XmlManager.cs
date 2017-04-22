using DoenaSoft.DVDProfiler.DVDProfilerHelper;
using DoenaSoft.DVDProfiler.EnhancedPurchaseInfo.Resources;
using Invelos.DVDProfilerPlugin;
using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    internal sealed class XmlManager
    {
        private readonly Plugin Plugin;

        public XmlManager(Plugin plugin)
        {
            Plugin = plugin;
        }

        #region Export

        internal void Export(Boolean exportAll)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.AddExtension = true;
                sfd.DefaultExt = ".xml";
                sfd.Filter = "XML files|*.xml";
                sfd.OverwritePrompt = true;
                sfd.RestoreDirectory = true;
                sfd.Title = Texts.SaveXmlFile;
                sfd.FileName = "EnhancedPurchaseInfos.xml";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Object[] ids;
                    EnhancedPurchaseInfoList epis;

                    Cursor.Current = Cursors.WaitCursor;
                    using (ProgressWindow progressWindow = new ProgressWindow())
                    {
                        #region Progress

                        Int32 onePercent;

                        progressWindow.ProgressBar.Minimum = 0;
                        progressWindow.ProgressBar.Step = 1;
                        progressWindow.CanClose = false;

                        #endregion

                        ids = GetProfileIds(exportAll);

                        epis = new EnhancedPurchaseInfoList();
                        epis.Profiles = new Profile[ids.Length];

                        #region Progress

                        progressWindow.ProgressBar.Maximum = ids.Length;
                        progressWindow.Show();
                        if (TaskbarManager.IsPlatformSupported)
                        {
                            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                            TaskbarManager.Instance.SetProgressValue(0, progressWindow.ProgressBar.Maximum);
                        }
                        onePercent = progressWindow.ProgressBar.Maximum / 100;
                        if ((progressWindow.ProgressBar.Maximum % 100) != 0)
                        {
                            onePercent++;
                        }

                        #endregion

                        for (Int32 i = 0; i < ids.Length; i++)
                        {
                            String id;
                            IDVDInfo profile;

                            id = ids[i].ToString();
                            Plugin.Api.DVDByProfileID(out profile, id, PluginConstants.DATASEC_AllSections, 0);
                            epis.Profiles[i] = GetXmlProfile(profile);

                            #region Progress

                            progressWindow.ProgressBar.PerformStep();
                            if (TaskbarManager.IsPlatformSupported)
                            {
                                TaskbarManager.Instance.SetProgressValue(progressWindow.ProgressBar.Value, progressWindow.ProgressBar.Maximum);
                            }
                            if ((progressWindow.ProgressBar.Value % onePercent) == 0)
                            {
                                Application.DoEvents();
                            }

                            #endregion
                        }

                        try
                        {
                            epis.Serialize(sfd.FileName);

                            #region Progress

                            Application.DoEvents();
                            if (TaskbarManager.IsPlatformSupported)
                            {
                                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                            }
                            progressWindow.CanClose = true;
                            progressWindow.Close();

                            #endregion

                            MessageBox.Show(String.Format(MessageBoxTexts.DoneWithNumber, ids.Length, MessageBoxTexts.Exported)
                                , MessageBoxTexts.InformationHeader, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(String.Format(MessageBoxTexts.FileCantBeWritten, sfd.FileName, ex.Message)
                                , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            #region Progress

                            if (progressWindow.Visible)
                            {
                                if (TaskbarManager.IsPlatformSupported)
                                {
                                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                                }
                                progressWindow.CanClose = true;
                                progressWindow.Close();
                            }

                            #endregion
                        }
                    }

                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private Profile GetXmlProfile(IDVDInfo profile)
        {
            PriceManager priceManager;
            TextManager textManager;
            DateManager dateManager;
            Profile xmlProfile;
            Decimal price;
            CurrencyInfo ci;
            String text;
            EnhancedPurchaseInfo epi;
            DefaultValues dv;

            dv = Plugin.Settings.DefaultValues;

            priceManager = new PriceManager(profile);
            textManager = new TextManager(profile);
            dateManager = new DateManager(profile);

            xmlProfile = new Profile();
            xmlProfile.Id = profile.GetProfileID();
            xmlProfile.Title = profile.GetTitle();
            epi = new EnhancedPurchaseInfo();
            xmlProfile.EnhancedPurchaseInfo = epi;

            #region Prices

            if (priceManager.GetOriginalPrice(out price))
            {
                priceManager.GetPurchasePriceCurrency(out ci);
                epi.OriginalPrice = GetXmlPrice(price, ci, dv.OriginalPriceLabel);
            }
            if (priceManager.GetShippingCost(out price))
            {
                priceManager.GetShippingCostCurrency(out ci);
                epi.ShippingCost = GetXmlPrice(price, ci, dv.ShippingCostLabel);
            }
            if (priceManager.GetCreditCardCharge(out price))
            {
                priceManager.GetCreditCardChargeCurrency(out ci);
                epi.CreditCardCharge = GetXmlPrice(price, ci, dv.CreditCardChargeLabel);
            }
            if (priceManager.GetCreditCardFees(out price))
            {
                priceManager.GetCreditCardFeesCurrency(out ci);
                epi.CreditCardFees = GetXmlPrice(price, ci, dv.CreditCardFeesLabel);
            }
            if (priceManager.GetDiscount(out price))
            {
                priceManager.GetDiscountCurrency(out ci);
                epi.Discount = GetXmlPrice(price, ci, dv.DiscountLabel);
            }
            if (priceManager.GetCustomsFees(out price))
            {
                priceManager.GetCustomsFeesCurrency(out ci);
                epi.CustomsFees = GetXmlPrice(price, ci, dv.CustomsFeesLabel);
            }
            if (textManager.GetCouponType(out text))
            {
                epi.CouponType = GetXmlText(text, dv.CouponTypeLabel);
            }
            if (textManager.GetCouponCode(out text))
            {
                epi.CouponCode = GetXmlText(text, dv.CouponCodeLabel);
            }
            if (priceManager.GetAdditionalPrice1(out price))
            {
                priceManager.GetAdditionalPrice1Currency(out ci);
                epi.AdditionalPrice1 = GetXmlPrice(price, ci, dv.AdditionalPrice1Label);
            }
            if (priceManager.GetAdditionalPrice2(out price))
            {
                priceManager.GetAdditionalPrice2Currency(out ci);
                epi.AdditionalPrice2 = GetXmlPrice(price, ci, dv.AdditionalPrice2Label);
            }

            #endregion

            #region Dates

            epi.OrderDate = GetDate(dateManager.GetOrderDate, dv.OrderDateLabel);
            epi.ShippingDate = GetDate(dateManager.GetShippingDate, dv.ShippingDateLabel);
            epi.DeliveryDate = GetDate(dateManager.GetDeliveryDate, dv.DeliveryDateLabel);
            epi.AdditionalDate1 = GetDate(dateManager.GetAdditionalDate1, dv.AdditionalDate1Label);
            epi.AdditionalDate2 = GetDate(dateManager.GetAdditionalDate2, dv.AdditionalDate2Label);

            #endregion

            return (xmlProfile);
        }

        private Text GetXmlText(String textValue
            , String displayName)
        {
            Text text;

            text = new Text();
            text.Value = textValue;
            text.DisplayName = displayName;
            return (text);
        }

        private Date GetDate(DateManager.GetDateDelegate getDate
            , String displayName)
        {
            DateTime dateValue;
            Date date;

            if (getDate(out dateValue))
            {
                date = new Date();
                date.Value = dateValue;
                date.DisplayName = displayName;
            }
            else
            {
                date = null;
            }
            return (date);
        }

        private static Price GetXmlPrice(Decimal price
            , CurrencyInfo ci
            , String displayName)
        {
            Price xmlPrice;

            xmlPrice = new Price();
            xmlPrice.Value = Convert.ToSingle(price);
            xmlPrice.DenominationType = ci.Type;
            xmlPrice.DenominationDesc = ci.Name;
            xmlPrice.FormattedValue = ci.GetFormattedValue(price);
            xmlPrice.DisplayName = displayName;
            return (xmlPrice);
        }

        #endregion

        #region Import

        internal void Import(Boolean importAll)
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
                    EnhancedPurchaseInfoList epis;

                    Cursor.Current = Cursors.WaitCursor;

                    epis = null;

                    try
                    {
                        epis = EnhancedPurchaseInfoList.Deserialize(ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format(MessageBoxTexts.FileCantBeRead, ofd.FileName, ex.Message)
                           , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (epis != null)
                    {
                        Int32 count;

                        count = 0;
                        if ((epis.Profiles != null) && (epis.Profiles.Length > 0))
                        {
                            using (ProgressWindow progressWindow = new ProgressWindow())
                            {
                                Dictionary<String, Boolean> profileIds;
                                Object[] ids;
                                Dictionary<String, CurrencyInfo> inverse;

                                #region Progress

                                Int32 onePercent;

                                progressWindow.ProgressBar.Minimum = 0;
                                progressWindow.ProgressBar.Step = 1;
                                progressWindow.CanClose = false;

                                #endregion

                                ids = GetProfileIds(importAll);

                                profileIds = new Dictionary<String, Boolean>(ids.Length);

                                #region Progress

                                progressWindow.ProgressBar.Maximum = ids.Length;
                                progressWindow.Show();
                                if (TaskbarManager.IsPlatformSupported)
                                {
                                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                                    TaskbarManager.Instance.SetProgressValue(0, progressWindow.ProgressBar.Maximum);
                                }
                                onePercent = progressWindow.ProgressBar.Maximum / 100;
                                if ((progressWindow.ProgressBar.Maximum % 100) != 0)
                                {
                                    onePercent++;
                                }

                                #endregion

                                for (Int32 i = 0; i < ids.Length; i++)
                                {
                                    profileIds.Add(ids[i].ToString(), true);
                                }

                                inverse = new Dictionary<String, CurrencyInfo>(Plugin.Currencies.Count);

                                foreach (CurrencyInfo ci in Plugin.Currencies.Values)
                                {
                                    inverse.Add(ci.Type, ci);
                                }

                                foreach (Profile xmlProfile in epis.Profiles)
                                {
                                    if ((xmlProfile != null) && (xmlProfile.EnhancedPurchaseInfo != null) && (profileIds.ContainsKey(xmlProfile.Id)))
                                    {
                                        IDVDInfo profile;
                                        EnhancedPurchaseInfo epi;
                                        PriceManager priceManager;
                                        TextManager textManager;
                                        DateManager dateManager;

                                        profile = Plugin.Api.CreateDVD();
                                        profile.SetProfileID(xmlProfile.Id);

                                        priceManager = new PriceManager(profile);
                                        textManager = new TextManager(profile);
                                        dateManager = new DateManager(profile);

                                        epi = xmlProfile.EnhancedPurchaseInfo;

                                        #region Prices

                                        SetPrice(inverse, priceManager.SetOriginalPrice, priceManager.SetOriginalPriceCurrency, epi.OriginalPrice, priceManager);
                                        SetPrice(inverse, priceManager.SetShippingCost, priceManager.SetShippingCostCurrency, epi.ShippingCost, priceManager);
                                        SetPrice(inverse, priceManager.SetCreditCardCharge, priceManager.SetCreditCardChargeCurrency, epi.CreditCardCharge, priceManager);
                                        SetPrice(inverse, priceManager.SetCreditCardFees, priceManager.SetCreditCardFeesCurrency, epi.CreditCardFees, priceManager);
                                        SetPrice(inverse, priceManager.SetDiscount, priceManager.SetDiscountCurrency, epi.Discount, priceManager);
                                        SetText(textManager.SetCouponType, epi.CouponType);
                                        SetText(textManager.SetCouponCode, epi.CouponCode);
                                        SetPrice(inverse, priceManager.SetCustomsFees, priceManager.SetCustomsFeesCurrency, epi.CustomsFees, priceManager);
                                        SetPrice(inverse, priceManager.SetAdditionalPrice1, priceManager.SetAdditionalPrice1Currency, epi.AdditionalPrice1, priceManager);
                                        SetPrice(inverse, priceManager.SetAdditionalPrice2, priceManager.SetAdditionalPrice2Currency, epi.AdditionalPrice2, priceManager);

                                        #endregion

                                        #region Dates

                                        SetDate(epi.OrderDate, dateManager.SetOrderDate);
                                        SetDate(epi.ShippingDate, dateManager.SetShippingDate);
                                        SetDate(epi.DeliveryDate, dateManager.SetDeliveryDate);
                                        SetDate(epi.AdditionalDate1, dateManager.SetAdditionalDate1);
                                        SetDate(epi.AdditionalDate2, dateManager.SetAdditionalDate2);

                                        #endregion

                                        count++;
                                    }

                                    #region Progress

                                    progressWindow.ProgressBar.PerformStep();
                                    if (TaskbarManager.IsPlatformSupported)
                                    {
                                        TaskbarManager.Instance.SetProgressValue(progressWindow.ProgressBar.Value, progressWindow.ProgressBar.Maximum);
                                    }
                                    if ((progressWindow.ProgressBar.Value % onePercent) == 0)
                                    {
                                        Application.DoEvents();
                                    }

                                    #endregion
                                }

                                #region Progress

                                Application.DoEvents();
                                if (TaskbarManager.IsPlatformSupported)
                                {
                                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                                }
                                progressWindow.CanClose = true;
                                progressWindow.Close();

                                #endregion
                            }
                        }

                        MessageBox.Show(String.Format(MessageBoxTexts.DoneWithNumber, count, MessageBoxTexts.Imported)
                                , MessageBoxTexts.InformationHeader, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void SetDate(Date date
            , Action<DateTime> setDate)
        {
            if (date != null)
            {
                setDate(date.Value);
            }
            else
            {
                setDate(DateManager.DateNotSet);
            }
        }

        private void SetText(Action<String> setText
            , Text text)
        {
            if (text != null)
            {
                setText(text.Value);
            }
            else
            {
                setText(null);
            }
        }

        private void SetPrice(Dictionary<String, CurrencyInfo> inverse
            , Action<String> setPrice
            , Action<CurrencyInfo> setCurrency
            , Price xmlPrice
            , PriceManager priceManager)
        {
            if (xmlPrice != null)
            {
                Decimal price;
                String priceString;
                CurrencyInfo ci;

                price = Convert.ToDecimal(xmlPrice.Value);
                priceString = priceManager.GetFormattedPriceString(price);
                setPrice(priceString);
                ci = inverse[xmlPrice.DenominationType];
                setCurrency(ci);
            }
            else
            {
                setPrice(null);
                setCurrency(CurrencyInfo.Empty);
            }
        }

        #endregion

        private Object[] GetProfileIds(Boolean allIds)
        {
            Object[] ids;

            if (allIds)
            {
                ids = (Object[])(Plugin.Api.GetAllProfileIDs());
            }
            else
            {
                ids = (Object[])(Plugin.Api.GetFlaggedProfileIDs());
            }

            return (ids);
        }
    }
}