using DoenaSoft.DVDProfiler.DVDProfilerHelper;
using DoenaSoft.DVDProfiler.EnhancedPurchaseInfo.Resources;
using Invelos.DVDProfilerPlugin;
using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

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

        internal void Export(Boolean exportAll)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.AddExtension = true;
                sfd.DefaultExt = ".csv";
                sfd.Filter = "CSV (comma-separated values) files|*.csv";
                sfd.OverwritePrompt = true;
                sfd.RestoreDirectory = true;
                sfd.Title = Texts.SaveXmlFile;
                sfd.FileName = "EnhancedPurchaseInfos.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    CultureInfo currentCulture;
                    Object[] ids;
                    String listSeparator;
                    String dateFormat;
                    DefaultValues dv;

                    Cursor.Current = Cursors.WaitCursor;
                    using (ProgressWindow progressWindow = new ProgressWindow())
                    {
                        #region Progress

                        Int32 onePercent;

                        progressWindow.ProgressBar.Minimum = 0;
                        progressWindow.ProgressBar.Step = 1;
                        progressWindow.CanClose = false;

                        #endregion

                        currentCulture = Application.CurrentCulture;
                        listSeparator = currentCulture.TextInfo.ListSeparator;
                        dateFormat = currentCulture.DateTimeFormat.ShortDatePattern;

                        dv = Plugin.Settings.DefaultValues;

                        if (exportAll)
                        {
                            ids = (Object[])(Plugin.Api.GetAllProfileIDs());
                        }
                        else
                        {
                            ids = (Object[])(Plugin.Api.GetFlaggedProfileIDs());
                        }

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

                        try
                        {
                            using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                            {
                                using (StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding(1252)))
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

                                    for (Int32 i = 0; i < ids.Length; i++)
                                    {
                                        #region Row

                                        String id;
                                        IDVDInfo profile;
                                        PriceManager priceManager;
                                        TextManager textManager;
                                        DateManager dateManager;

                                        id = ids[i].ToString();

                                        if ((dv.Title) || (dv.Edition) || (dv.SortTitle) || (dv.PurchasePrice) || (dv.SRP) || (dv.PurchaseDate))
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
                                                Int64 price;
                                                CurrencyInfo ci;

                                                price = profile.GetPurchasePriceValue();
                                                ci = new CurrencyInfo(profile.GetPurchasePriceCurrency());
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
                                            Int64 price;
                                            CurrencyInfo ci;

                                            price = profile.GetSRPValue();
                                            ci = new CurrencyInfo(profile.GetSRPCurrency());
                                            WritePrice(price, ci, sw, listSeparator);
                                        }

                                        #endregion

                                        #region Plugin Data

                                        priceManager = new PriceManager(profile);
                                        textManager = new TextManager(profile);

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
                                            DateTime date;

                                            date = profile.GetPurchaseDate();
                                            if (date != DateManager.DateNotSet)
                                            {
                                                String stringDate;

                                                stringDate = date.ToString(dateFormat);
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

                                        dateManager = new DateManager(profile);

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

        private static void WriteDate(DateManager.GetDateDelegate getDate
            , StreamWriter sw
            , String dateFormat
            , String listSeparator)
        {
            DateTime date;

            if (getDate(out date))
            {
                String stringDate;

                stringDate = date.ToString(dateFormat);
                sw.Write(stringDate);
                sw.Write(listSeparator);
            }
            else
            {
                sw.Write(listSeparator);
            }
        }

        private static void WritePrice(PriceManager.GetPriceDelegate getPrice
            , PriceManager.GetCurrencyDelegate getCurrency
            , StreamWriter sw
            , String listSeparator)
        {
            Decimal price;

            if (getPrice(out price))
            {
                CurrencyInfo ci;

                getCurrency(out ci);
                WritePrice(price, ci, sw, listSeparator);
            }
            else
            {
                sw.Write(listSeparator);
                sw.Write(listSeparator);
            }
        }

        private static void WritePrice(Decimal price
            , CurrencyInfo ci
            , StreamWriter sw
            , String listSeparator)
        {
            sw.Write(ci.GetFormattedPlainValue(price));
            sw.Write(listSeparator);
            sw.Write(ci.Name);
            sw.Write(listSeparator);
        }

        private static void WritePrice(Int64 price
            , CurrencyInfo ci
            , StreamWriter sw
            , String listSeparator)
        {
            sw.Write(ci.GetFormattedPlainValue(price));
            sw.Write(listSeparator);
            sw.Write(ci.Name);
            sw.Write(listSeparator);
        }

        private static void WriteText(String text
            , StreamWriter sw
            , String listSeparator)
        {
            sw.Write("\"");
            WriteText(sw, text);
            sw.Write("\"");
            sw.Write(listSeparator);
        }

        private static void WritePriceHeader(String header
            , StreamWriter sw
            , String listSeparator)
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

        private static void WriteTextHeader(String header
            , StreamWriter sw
            , String listSeparator)
        {
            sw.Write("\"");
            WriteText(sw, header);
            sw.Write("\"");
            sw.Write(listSeparator);
        }

        private static void WriteText(StreamWriter sw
            , String text)
        {
            if (String.IsNullOrEmpty(text) == false)
            {
                text = text.Replace("\"", "\"\"");
            }
            sw.Write(text);
        }

        #endregion
    }
}