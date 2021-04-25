using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;
using DoenaSoft.DVDProfiler.EnhancedPurchaseInfo.Resources;
using Invelos.DVDProfilerPlugin;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    internal sealed class XmlManager
    {
        private readonly Plugin _plugin;

        public XmlManager(Plugin plugin)
        {
            _plugin = plugin;
        }

        #region Export

        internal void Export(bool exportAll)
        {
            using (var sfd = new SaveFileDialog()
            {
                AddExtension = true,
                DefaultExt = ".xml",
                Filter = "XML files|*.xml",
                OverwritePrompt = true,
                RestoreDirectory = true,
                Title = Texts.SaveXmlFile,
                FileName = "EnhancedPurchaseInfos.xml",
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;

                    using (var progressWindow = new ProgressWindow())
                    {
                        #region Progress

                        progressWindow.ProgressBar.Minimum = 0;
                        progressWindow.ProgressBar.Step = 1;
                        progressWindow.CanClose = false;

                        #endregion

                        var ids = GetProfileIds(exportAll);

                        var epis = new EnhancedPurchaseInfoList()
                        {
                            Profiles = new Profile[ids.Length],
                        };

                        #region Progress

                        progressWindow.ProgressBar.Maximum = ids.Length;
                        progressWindow.Show();

                        if (TaskbarManager.IsPlatformSupported)
                        {
                            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                            TaskbarManager.Instance.SetProgressValue(0, progressWindow.ProgressBar.Maximum);
                        }

                        var onePercent = progressWindow.ProgressBar.Maximum / 100;

                        if ((progressWindow.ProgressBar.Maximum % 100) != 0)
                        {
                            onePercent++;
                        }

                        #endregion

                        for (var idIndex = 0; idIndex < ids.Length; idIndex++)
                        {
                            var id = ids[idIndex].ToString();

                            _plugin.Api.DVDByProfileID(out var profile, id, PluginConstants.DATASEC_AllSections, 0);

                            epis.Profiles[idIndex] = GetXmlProfile(profile);

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

                            MessageBox.Show(string.Format(MessageBoxTexts.DoneWithNumber, ids.Length, MessageBoxTexts.Exported)
                                , MessageBoxTexts.InformationHeader, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format(MessageBoxTexts.FileCantBeWritten, sfd.FileName, ex.Message)
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
            var dv = _plugin.Settings.DefaultValues;

            var priceManager = new PriceManager(profile);

            var textManager = new TextManager(profile);

            var dateManager = new DateManager(profile);

            var xmlProfile = new Profile()
            {
                Id = profile.GetProfileID(),
                Title = profile.GetTitle(),
            };

            var epi = new EnhancedPurchaseInfo();

            xmlProfile.EnhancedPurchaseInfo = epi;

            #region Prices

            if (priceManager.GetOriginalPrice(out var price))
            {
                priceManager.GetPurchasePriceCurrency(out var ci);

                epi.OriginalPrice = GetXmlPrice(price, ci, dv.OriginalPriceLabel);
            }

            if (priceManager.GetShippingCost(out price))
            {
                priceManager.GetShippingCostCurrency(out var ci);

                epi.ShippingCost = GetXmlPrice(price, ci, dv.ShippingCostLabel);
            }

            if (priceManager.GetCreditCardCharge(out price))
            {
                priceManager.GetCreditCardChargeCurrency(out var ci);

                epi.CreditCardCharge = GetXmlPrice(price, ci, dv.CreditCardChargeLabel);
            }

            if (priceManager.GetCreditCardFees(out price))
            {
                priceManager.GetCreditCardFeesCurrency(out var ci);

                epi.CreditCardFees = GetXmlPrice(price, ci, dv.CreditCardFeesLabel);
            }

            if (priceManager.GetDiscount(out price))
            {
                priceManager.GetDiscountCurrency(out var ci);

                epi.Discount = GetXmlPrice(price, ci, dv.DiscountLabel);
            }

            if (priceManager.GetCustomsFees(out price))
            {
                priceManager.GetCustomsFeesCurrency(out var ci);

                epi.CustomsFees = GetXmlPrice(price, ci, dv.CustomsFeesLabel);
            }

            if (textManager.GetCouponType(out var text))
            {
                epi.CouponType = GetXmlText(text, dv.CouponTypeLabel);
            }

            if (textManager.GetCouponCode(out text))
            {
                epi.CouponCode = GetXmlText(text, dv.CouponCodeLabel);
            }

            if (priceManager.GetAdditionalPrice1(out price))
            {
                priceManager.GetAdditionalPrice1Currency(out var ci);

                epi.AdditionalPrice1 = GetXmlPrice(price, ci, dv.AdditionalPrice1Label);
            }

            if (priceManager.GetAdditionalPrice2(out price))
            {
                priceManager.GetAdditionalPrice2Currency(out var ci);

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

            return xmlProfile;
        }

        private Text GetXmlText(string textValue, string displayName)
        {
            var text = new Text
            {
                Value = textValue,
                DisplayName = displayName,
            };

            return text;
        }

        private Date GetDate(DateManager.GetDateDelegate getDate, string displayName)
        {
            if (getDate(out var dateValue))
            {
                var date = new Date()
                {
                    Value = dateValue,
                    DisplayName = displayName,
                };

                return date;
            }
            else
            {
                return null;
            }
        }

        private static Price GetXmlPrice(decimal price, CurrencyInfo ci, string displayName)
        {
            var xmlPrice = new Price()
            {
                Value = Convert.ToSingle(price),
                DenominationType = ci.Type,
                DenominationDesc = ci.Name,
                FormattedValue = ci.GetFormattedValue(price),
                DisplayName = displayName,
            };

            return xmlPrice;
        }

        #endregion

        #region Import

        internal void Import(bool importAll)
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
                    Cursor.Current = Cursors.WaitCursor;

                    EnhancedPurchaseInfoList epis;
                    try
                    {
                        epis = EnhancedPurchaseInfoList.Deserialize(ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        epis = null;

                        MessageBox.Show(string.Format(MessageBoxTexts.FileCantBeRead, ofd.FileName, ex.Message)
                           , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (epis != null)
                    {
                        var count = 0;

                        if (epis.Profiles != null && epis.Profiles.Length > 0)
                        {
                            using (var progressWindow = new ProgressWindow())
                            {
                                #region Progress

                                progressWindow.ProgressBar.Minimum = 0;
                                progressWindow.ProgressBar.Step = 1;
                                progressWindow.CanClose = false;

                                #endregion

                                var ids = GetProfileIds(importAll);

                                var profileIds = new Dictionary<string, bool>(ids.Length);

                                #region Progress

                                progressWindow.ProgressBar.Maximum = ids.Length;
                                progressWindow.Show();

                                if (TaskbarManager.IsPlatformSupported)
                                {
                                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                                    TaskbarManager.Instance.SetProgressValue(0, progressWindow.ProgressBar.Maximum);
                                }

                                var onePercent = progressWindow.ProgressBar.Maximum / 100;

                                if ((progressWindow.ProgressBar.Maximum % 100) != 0)
                                {
                                    onePercent++;
                                }

                                #endregion

                                for (var idIndex = 0; idIndex < ids.Length; idIndex++)
                                {
                                    profileIds.Add(ids[idIndex].ToString(), true);
                                }

                                var inverse = new Dictionary<string, CurrencyInfo>(_plugin.Currencies.Count);

                                foreach (CurrencyInfo ci in _plugin.Currencies.Values)
                                {
                                    inverse.Add(ci.Type, ci);
                                }

                                foreach (var xmlProfile in epis.Profiles)
                                {
                                    if (xmlProfile != null && xmlProfile.EnhancedPurchaseInfo != null && profileIds.ContainsKey(xmlProfile.Id))
                                    {
                                        var profile = _plugin.Api.CreateDVD();

                                        profile.SetProfileID(xmlProfile.Id);

                                        var priceManager = new PriceManager(profile);

                                        var textManager = new TextManager(profile);

                                        var dateManager = new DateManager(profile);

                                        var epi = xmlProfile.EnhancedPurchaseInfo;

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

                        MessageBox.Show(string.Format(MessageBoxTexts.DoneWithNumber, count, MessageBoxTexts.Imported)
                                , MessageBoxTexts.InformationHeader, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void SetDate(Date date, Action<DateTime> setDate)
        {
            if (date != null)
            {
                setDate(date.Value);
            }
            else
            {
                setDate(DateManager._dateNotSet);
            }
        }

        private void SetText(Action<string> setText, Text text)
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

        private void SetPrice(Dictionary<string, CurrencyInfo> inverse, Action<string> setPrice, Action<CurrencyInfo> setCurrency, Price xmlPrice, PriceManager priceManager)
        {
            if (xmlPrice != null)
            {
                var price = Convert.ToDecimal(xmlPrice.Value);

                var priceString = priceManager.GetFormattedPriceString(price);

                setPrice(priceString);

                var ci = inverse[xmlPrice.DenominationType];

                setCurrency(ci);
            }
            else
            {
                setPrice(null);

                setCurrency(CurrencyInfo.Empty);
            }
        }

        #endregion

        private object[] GetProfileIds(bool allIds)
        {
            var ids = allIds
                ? (object[])(_plugin.Api.GetAllProfileIDs())
                : (object[])(_plugin.Api.GetFlaggedProfileIDs());

            return ids;
        }
    }
}