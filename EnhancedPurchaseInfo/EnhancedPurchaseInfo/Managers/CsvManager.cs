using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;
using DoenaSoft.DVDProfiler.EnhancedPurchaseInfo.Resources;
using Invelos.DVDProfilerPlugin;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    internal sealed class CsvManager
    {
        private readonly Plugin Plugin;

        public CsvManager(Plugin plugin)
        {
            Plugin = plugin;
        }

        #region Export

        internal void Export(bool exportAll)
        {
            using (var sfd = new SaveFileDialog()
            {
                AddExtension = true,
                DefaultExt = ".csv",
                Filter = "CSV (comma-separated values) files|*.csv",
                OverwritePrompt = true,
                RestoreDirectory = true,
                Title = Texts.SaveXmlFile,
                FileName = "EnhancedPurchaseInfos.csv",
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

                        var currentCulture = Application.CurrentCulture;

                        var listSeparator = currentCulture.TextInfo.ListSeparator;

                        var dateFormat = currentCulture.DateTimeFormat.ShortDatePattern;

                        var dv = Plugin.Settings.DefaultValues;

                        var ids = exportAll
                            ? (object[])(Plugin.Api.GetAllProfileIDs())
                            : (object[])(Plugin.Api.GetFlaggedProfileIDs());

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

                        try
                        {
                            using (var fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                            {
                                using (var sw = new StreamWriter(fs, Encoding.GetEncoding(1252)))
                                {
                                    #region Header

                                    #region Basics

                                    if (dv.Id)
                                    {
                                        WriteTextHeader(Texts.Id, sw, listSeparator);
                                    }

                                    if (dv.Title)
                                    {
                                        WriteTextHeader(Texts.Title, sw, listSeparator);
                                    }

                                    if (dv.Edition)
                                    {
                                        WriteTextHeader(Texts.Edition, sw, listSeparator);
                                    }

                                    if (dv.SortTitle)
                                    {
                                        WriteTextHeader(Texts.SortTitle, sw, listSeparator);
                                    }

                                    if (dv.PurchasePlace)
                                    {
                                        WriteTextHeader(Texts.PurchasePlace, sw, listSeparator);
                                    }

                                    #endregion

                                    #region Prices

                                    #region Invelos Data

                                    if (dv.PurchasePrice)
                                    {
                                        WritePriceHeader(Texts.PurchasePrice, sw, listSeparator);
                                    }
                                    if (dv.SRP)
                                    {
                                        WritePriceHeader(Texts.SRP, sw, listSeparator);
                                    }

                                    #endregion

                                    #region Plugin Data

                                    if (dv.OriginalPrice)
                                    {
                                        WritePriceHeader(dv.OriginalPriceLabel, sw, listSeparator);
                                    }
                                    if (dv.ShippingCost)
                                    {
                                        WritePriceHeader(dv.ShippingCostLabel, sw, listSeparator);
                                    }
                                    if (dv.CreditCardCharge)
                                    {
                                        WritePriceHeader(dv.CreditCardChargeLabel, sw, listSeparator);
                                    }
                                    if (dv.CreditCardFees)
                                    {
                                        WritePriceHeader(dv.CreditCardFeesLabel, sw, listSeparator);
                                    }
                                    if (dv.Discount)
                                    {
                                        WritePriceHeader(dv.DiscountLabel, sw, listSeparator);
                                    }
                                    if (dv.CustomsFees)
                                    {
                                        WritePriceHeader(dv.CustomsFeesLabel, sw, listSeparator);
                                    }
                                    if (dv.CouponType)
                                    {
                                        WriteTextHeader(dv.CouponTypeLabel, sw, listSeparator);
                                    }
                                    if (dv.CouponCode)
                                    {
                                        WriteTextHeader(dv.CouponCodeLabel, sw, listSeparator);
                                    }
                                    if (dv.AdditionalPrice1)
                                    {
                                        WritePriceHeader(dv.AdditionalPrice1Label, sw, listSeparator);
                                    }
                                    if (dv.AdditionalPrice2)
                                    {
                                        WritePriceHeader(dv.AdditionalPrice2Label, sw, listSeparator);
                                    }
                                    #endregion

                                    #endregion

                                    #region Dates

                                    #region Invelos Data

                                    if (dv.PurchaseDate)
                                    {
                                        WriteTextHeader(Texts.PurchaseDate, sw, listSeparator);
                                    }

                                    #endregion

                                    #region Plugin Data

                                    if (dv.OrderDate)
                                    {
                                        WriteTextHeader(dv.OrderDateLabel, sw, listSeparator);
                                    }
                                    if (dv.ShippingDate)
                                    {
                                        WriteTextHeader(dv.ShippingDateLabel, sw, listSeparator);
                                    }
                                    if (dv.DeliveryDate)
                                    {
                                        WriteTextHeader(dv.DeliveryDateLabel, sw, listSeparator);
                                    }
                                    if (dv.AdditionalDate1)
                                    {
                                        WriteTextHeader(dv.AdditionalDate1Label, sw, listSeparator);
                                    }
                                    if (dv.AdditionalDate2)
                                    {
                                        WriteTextHeader(dv.AdditionalDate2Label, sw, listSeparator);
                                    }

                                    #endregion

                                    #endregion

                                    sw.WriteLine();

                                    #endregion

                                    for (int idIndex = 0; idIndex < ids.Length; idIndex++)
                                    {
                                        #region Row

                                        var id = ids[idIndex].ToString();

                                        IDVDInfo profile;
                                        if (dv.Title || dv.Edition || dv.SortTitle || dv.PurchasePrice || dv.SRP || dv.PurchaseDate)
                                        {
                                            Plugin.Api.DVDByProfileID(out profile, id, PluginConstants.DATASEC_AllSections, 0);
                                        }
                                        else
                                        {
                                            profile = Plugin.Api.CreateDVD();

                                            profile.SetProfileID(id);
                                        }

                                        #region Basics

                                        if (dv.Id)
                                        {
                                            WriteText(profile.GetFormattedProfileID(), sw, listSeparator);
                                        }

                                        if (dv.Title)
                                        {
                                            sw.Write("\"");

                                            WriteText(sw, profile.GetTitle());

                                            sw.Write("\"");
                                            sw.Write(listSeparator);
                                        }

                                        if (dv.Edition)
                                        {
                                            sw.Write("\"");

                                            WriteText(sw, profile.GetEdition());

                                            sw.Write("\"");
                                            sw.Write(listSeparator);
                                        }

                                        if (dv.SortTitle)
                                        {
                                            sw.Write("\"");

                                            WriteText(sw, profile.GetSortTitle());

                                            sw.Write("\"");
                                            sw.Write(listSeparator);
                                        }

                                        if (dv.PurchasePlace)
                                        {
                                            sw.Write("\"");

                                            WriteText(sw, profile.GetPurchasePlace());

                                            sw.Write("\"");
                                            sw.Write(listSeparator);
                                        }

                                        #endregion

                                        #region Prices

                                        #region Invelos Data

                                        if (dv.PurchasePrice)
                                        {
                                            if (profile.GetPurchasePriceIsEmpty() == false)
                                            {
                                                var price = profile.GetPurchasePriceValue();

                                                var ci = new CurrencyInfo(profile.GetPurchasePriceCurrency());

                                                WritePrice(price, ci, sw, listSeparator);
                                            }
                                            else
                                            {
                                                sw.Write(listSeparator);
                                                sw.Write(listSeparator);
                                            }
                                        }
                                        if (dv.SRP)
                                        {
                                            var price = profile.GetSRPValue();

                                            var ci = new CurrencyInfo(profile.GetSRPCurrency());

                                            WritePrice(price, ci, sw, listSeparator);
                                        }

                                        #endregion

                                        #region Plugin Data

                                        var priceManager = new PriceManager(profile);

                                        var textManager = new TextManager(profile);

                                        if (dv.OriginalPrice)
                                        {
                                            WritePrice(priceManager.GetOriginalPrice, priceManager.GetOriginalPriceCurrency, sw, listSeparator);
                                        }

                                        if (dv.ShippingCost)
                                        {
                                            WritePrice(priceManager.GetShippingCost, priceManager.GetShippingCostCurrency, sw, listSeparator);
                                        }

                                        if (dv.CreditCardCharge)
                                        {
                                            WritePrice(priceManager.GetCreditCardCharge, priceManager.GetCreditCardChargeCurrency, sw, listSeparator);
                                        }

                                        if (dv.CreditCardFees)
                                        {
                                            WritePrice(priceManager.GetCreditCardFees, priceManager.GetCreditCardFeesCurrency, sw, listSeparator);
                                        }

                                        if (dv.Discount)
                                        {
                                            WritePrice(priceManager.GetDiscount, priceManager.GetDiscountCurrency, sw, listSeparator);
                                        }

                                        if (dv.CustomsFees)
                                        {
                                            WritePrice(priceManager.GetCustomsFees, priceManager.GetCustomsFeesCurrency, sw, listSeparator);
                                        }

                                        if (dv.CouponType)
                                        {
                                            WriteText(textManager.GetCouponTypeWithFallback(), sw, listSeparator);
                                        }

                                        if (dv.CouponCode)
                                        {
                                            WriteText(textManager.GetCouponCodeWithFallback(), sw, listSeparator);
                                        }

                                        if (dv.AdditionalPrice1)
                                        {
                                            WritePrice(priceManager.GetAdditionalPrice1, priceManager.GetAdditionalPrice1Currency, sw, listSeparator);
                                        }

                                        if (dv.AdditionalPrice2)
                                        {
                                            WritePrice(priceManager.GetAdditionalPrice2, priceManager.GetAdditionalPrice2Currency, sw, listSeparator);
                                        }

                                        #endregion

                                        #endregion

                                        #region Dates

                                        #region Invelos Data

                                        if (dv.PurchaseDate)
                                        {
                                            var date = profile.GetPurchaseDate();

                                            if (date != DateManager._dateNotSet)
                                            {
                                                var stringDate = date.ToString(dateFormat);

                                                sw.Write(stringDate);
                                                sw.Write(listSeparator);
                                            }
                                            else
                                            {
                                                sw.Write(listSeparator);
                                            }
                                        }

                                        #endregion

                                        #region Plugin Data

                                        var dateManager = new DateManager(profile);

                                        if (dv.OrderDate)
                                        {
                                            WriteDate(dateManager.GetOrderDate, sw, dateFormat, listSeparator);
                                        }

                                        if (dv.ShippingDate)
                                        {
                                            WriteDate(dateManager.GetShippingDate, sw, dateFormat, listSeparator);
                                        }

                                        if (dv.DeliveryDate)
                                        {
                                            WriteDate(dateManager.GetDeliveryDate, sw, dateFormat, listSeparator);
                                        }

                                        if (dv.AdditionalDate1)
                                        {
                                            WriteDate(dateManager.GetAdditionalDate1, sw, dateFormat, listSeparator);
                                        }

                                        if (dv.AdditionalDate2)
                                        {
                                            WriteDate(dateManager.GetAdditionalDate2, sw, dateFormat, listSeparator);
                                        }

                                        #endregion

                                        #endregion

                                        sw.WriteLine();

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

                                        #endregion
                                    }
                                }
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

        private static void WriteDate(DateManager.GetDateDelegate getDate, StreamWriter sw, string dateFormat, string listSeparator)
        {
            if (getDate(out var date))
            {
                var stringDate = date.ToString(dateFormat);

                sw.Write(stringDate);
                sw.Write(listSeparator);
            }
            else
            {
                sw.Write(listSeparator);
            }
        }

        private static void WritePrice(PriceManager.GetPriceDelegate getPrice, PriceManager.GetCurrencyDelegate getCurrency, StreamWriter sw, string listSeparator)
        {
            if (getPrice(out var price))
            {
                getCurrency(out var ci);

                WritePrice(price, ci, sw, listSeparator);
            }
            else
            {
                sw.Write(listSeparator);
                sw.Write(listSeparator);
            }
        }

        private static void WritePrice(decimal price, CurrencyInfo ci, StreamWriter sw, string listSeparator)
        {
            sw.Write(ci.GetFormattedPlainValue(price));
            sw.Write(listSeparator);
            sw.Write(ci.Name);
            sw.Write(listSeparator);
        }

        private static void WritePrice(long price, CurrencyInfo ci, StreamWriter sw, string listSeparator)
        {
            sw.Write(ci.GetFormattedPlainValue(price));
            sw.Write(listSeparator);
            sw.Write(ci.Name);
            sw.Write(listSeparator);
        }

        private static void WriteText(string text, StreamWriter sw, string listSeparator)
        {
            sw.Write("\"");

            WriteText(sw, text);

            sw.Write("\"");
            sw.Write(listSeparator);
        }

        private static void WritePriceHeader(string header, StreamWriter sw, string listSeparator)
        {
            sw.Write("\"");

            WriteText(sw, header);

            sw.Write("\"");
            sw.Write(listSeparator);
            sw.Write("\"");

            WriteText(sw, header + " (" + Texts.Currency + ")");

            sw.Write("\"");
            sw.Write(listSeparator);
        }

        private static void WriteTextHeader(string header, StreamWriter sw, string listSeparator)
        {
            sw.Write("\"");

            WriteText(sw, header);

            sw.Write("\"");
            sw.Write(listSeparator);
        }

        private static void WriteText(StreamWriter sw, string text)
        {
            if (string.IsNullOrEmpty(text) == false)
            {
                text = text.Replace("\"", "\"\"");
            }

            sw.Write(text);
        }

        #endregion
    }
}