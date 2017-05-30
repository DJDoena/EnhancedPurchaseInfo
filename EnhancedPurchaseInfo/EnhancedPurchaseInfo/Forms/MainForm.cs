using DoenaSoft.DVDProfiler.DVDProfilerHelper;
using DoenaSoft.DVDProfiler.EnhancedPurchaseInfo.Resources;
using Invelos.DVDProfilerPlugin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    internal partial class MainForm : Form
    {
        private readonly Plugin Plugin;

        private readonly IDVDInfo Profile;

        private readonly PriceManager PriceManager;

        private readonly TextManager TextManager;

        private readonly DateManager DateManager;

        private readonly Boolean FullEdit;

        private Boolean DataChanged;

        internal event EventHandler OpenCalculator;

        internal MainForm(Plugin plugin
            , IDVDInfo profile
            , Boolean fullEdit)
        {
            Plugin = plugin;
            Profile = profile;
            FullEdit = fullEdit;

            PriceManager = new PriceManager(profile);
            TextManager = new TextManager(profile);
            DateManager = new DateManager(profile);

            InitializeComponent();

            BindComboBoxes();
            SetTextBoxes();
            SetComboBoxes();
            SetDatePicker();
            SetLabels();
            SetReadOnlies();

            DataChanged = false;
        }

        private void SetReadOnlies()
        {
            this.PurchasePriceTextBox.ReadOnly = (FullEdit == false);
            this.PurchasePriceComboBox.Enabled = FullEdit;

            this.SRPTextBox.ReadOnly = (FullEdit == false);
            this.SRPComboBox.Enabled = FullEdit;

            this.PurchaseDatePicker.Enabled = FullEdit;

            if (Plugin.IsRemoteAccess)
            {
                ImportFromXMLToolStripMenuItem.Enabled = false;
                PasteAllToolStripMenuItem.Enabled = false;
                SaveButton.Enabled = false;

                SetControlsReadonly(Controls);
            }
        }

        private void SetControlsReadonly(Control.ControlCollection controls)
        {
            if (controls != null)
            {
                foreach (Control control in controls)
                {
                    if (control is TextBox)
                    {
                        ((TextBox)control).ReadOnly = true;
                    }
                    else if (control is ComboBox)
                    {
                        ((ComboBox)control).Enabled = false;
                    }
                    else if (control is DateTimePicker)
                    {
                        ((DateTimePicker)control).Enabled = false;
                    }
                    else
                    {
                        SetControlsReadonly(control.Controls);
                    }
                }
            }
        }

        private void SetDatePicker()
        {
            SetDatePicker(PurchaseDatePicker, DateManager.GetPurchaseDate);

            SetDatePicker(OrderDatePicker, DateManager.GetOrderDate);
            SetDatePicker(ShippingDatePicker, DateManager.GetShippingDate);
            SetDatePicker(DeliveryDatePicker, DateManager.GetDeliveryDate);
            SetDatePicker(AdditionalDate1Picker, DateManager.GetAdditionalDate1);
            SetDatePicker(AdditionalDate2Picker, DateManager.GetAdditionalDate2);
        }

        private void SetDatePicker(DateTimePicker picker, DateManager.GetDateDelegate getDate)
        {
            DateTime date;

            if (getDate(out date))
            {
                picker.Checked = true;
                picker.Value = date;
            }
            else
            {
                picker.Checked = false;
            }
        }

        private void BindComboBoxes()
        {
            List<String> couponTypes;

            #region Invelos Prices

            BindDatabox(PurchasePriceComboBox);
            BindDatabox(SRPComboBox);

            #endregion

            #region Plugin Prices

            BindDatabox(OriginalPriceComboBox);
            BindDatabox(ShippingCostComboBox);
            BindDatabox(CreditCardChargeComboBox);
            BindDatabox(CreditCardFeesComboBox);
            BindDatabox(DiscountComboBox);
            BindDatabox(CustomsFeesComboBox);
            BindDatabox(AdditionalPrice1ComboBox);
            BindDatabox(AdditionalPrice2ComboBox);

            #endregion

            #region CouponType

            couponTypes = new List<String>(Plugin.CouponTypes.Keys);
            couponTypes.Sort();
            CouponTypeComboBox.Items.AddRange(couponTypes.ToArray());

            #endregion
        }

        private void BindDatabox(ComboBox comboBox)
        {
            comboBox.DataSource = new BindingSource(Plugin.Currencies, null);
            comboBox.DisplayMember = "Key";
            comboBox.ValueMember = "Value";
        }

        private void SetComboBoxes()
        {
            #region Invelos Prices

            SetComboBox(PurchasePriceComboBox, PriceManager.GetPurchasePriceCurrency);
            SetComboBox(SRPComboBox, PriceManager.GetSRPCurrency);

            #endregion

            #region Plugin Prices

            SetComboBox(OriginalPriceComboBox, PriceManager.GetOriginalPriceCurrency);
            SetComboBox(ShippingCostComboBox, PriceManager.GetShippingCostCurrency);
            SetComboBox(CreditCardChargeComboBox, PriceManager.GetCreditCardChargeCurrency);
            SetComboBox(CreditCardFeesComboBox, PriceManager.GetCreditCardFeesCurrency);
            SetComboBox(DiscountComboBox, PriceManager.GetDiscountCurrency);
            SetComboBox(CustomsFeesComboBox, PriceManager.GetCustomsFeesCurrency);
            SetComboBox(AdditionalPrice1ComboBox, PriceManager.GetAdditionalPrice1Currency);
            SetComboBox(AdditionalPrice2ComboBox, PriceManager.GetAdditionalPrice2Currency);

            #endregion
        }

        private void SetComboBox(ComboBox comboBox, PriceManager.GetCurrencyDelegate getCurrency)
        {
            CurrencyInfo ci;

            if (getCurrency(out ci))
            {
                comboBox.Text = ci.Name;
            }
            else
            {
                comboBox.SelectedIndex = -1;
            }
        }

        private void SetLabels()
        {
            DefaultValues dv;

            dv = Plugin.Settings.DefaultValues;

            #region Invelos Prices

            PurchasePriceLabel.Text = Texts.PurchasePrice;
            SRPLabel.Text = Texts.SRP;

            #endregion

            #region Plugin Prices

            OriginalPriceLabel.Text = dv.OriginalPriceLabel;
            ShippingCostLabel.Text = dv.ShippingCostLabel;
            CreditCardChargeLabel.Text = dv.CreditCardChargeLabel;
            CreditCardFeesLabel.Text = dv.CreditCardFeesLabel;
            DiscountLabel.Text = dv.DiscountLabel;
            CustomsFeesLabel.Text = dv.CustomsFeesLabel;
            CouponTypeLabel.Text = dv.CouponTypeLabel;
            CouponCodeLabel.Text = dv.CouponCodeLabel;
            AdditionalPrice1Label.Text = dv.AdditionalPrice1Label;
            AdditionalPrice2Label.Text = dv.AdditionalPrice2Label;

            #endregion

            #region Invelos Dates

            PurchaseDateLabel.Text = Texts.PurchaseDate;

            #endregion

            #region Plugin Dates

            OrderDateLabel.Text = dv.OrderDateLabel;
            ShippingDateLabel.Text = dv.ShippingDateLabel;
            DeliveryDateLabel.Text = dv.DeliveryDateLabel;
            AdditionalDate1Label.Text = dv.AdditionalDate1Label;
            AdditionalDate2Label.Text = dv.AdditionalDate2Label;

            #endregion

            #region Misc

            #region Menu

            EditToolStripMenuItem.Text = Texts.Edit;
            CopyAllToolStripMenuItem.Text = Texts.CopyAllToClipboard;
            PasteAllToolStripMenuItem.Text = Texts.PasteAllFromClipboard;

            ToolsToolStripMenuItem.Text = Texts.Tools;
            ShippingCostCalculatorToolStripMenuItem.Text = Texts.ShippingCostCalculator;
            OptionsToolStripMenuItem.Text = Texts.Options;
            ExportToXMLToolStripMenuItem.Text = Texts.ExportToXml;
            ImportFromXMLToolStripMenuItem.Text = Texts.ImportFromXml;
            ExportOptionsToolStripMenuItem.Text = Texts.ExportOptions;
            ImportOptionsToolStripMenuItem.Text = Texts.ImportOptions;

            HelpToolStripMenuItem.Text = Texts.Help;
            CheckForUpdatesToolStripMenuItem.Text = Texts.CheckForUpdates;
            AboutToolStripMenuItem.Text = Texts.About;

            #endregion

            #region GroupBoxes

            PricesInvelosDataGroupBox.Text = Texts.InvelosData;
            PricesPluginDataGroupBox.Text = Texts.PluginData;
            DatesInvelosDataGroupBox.Text = Texts.InvelosData;
            DatesPluginDataGroupBox.Text = Texts.PluginData;

            #endregion

            #region Buttons

            SaveButton.Text = Texts.Save;
            DiscardButton.Text = Texts.Cancel;

            #endregion

            #region TabPages

            PricesTabPage.Text = Texts.Prices;
            DatesTabPage.Text = Texts.Dates;

            #endregion

            #endregion
        }

        private void SetTextBoxes()
        {
            #region Invelos Prices

            PurchasePriceTextBox.Text = PriceManager.GetPurchasePriceWithFallback();
            SRPTextBox.Text = PriceManager.GetSRPWithFallback();

            #endregion

            #region Plugin Prices

            OriginalPriceTextBox.Text = PriceManager.GetOriginalPriceWithFallback();
            ShippingCostTextBox.Text = PriceManager.GetShippingCostWithFallback();
            CreditCardChargeTextBox.Text = PriceManager.GetCreditCardChargeWithFallback();
            CreditCardFeesTextBox.Text = PriceManager.GetCreditCardFeesWithFallback();
            DiscountTextBox.Text = PriceManager.GetDiscountWithFallback();
            CustomsFeesTextBox.Text = PriceManager.GetCustomsFeesWithFallback();
            CouponTypeComboBox.Text = TextManager.GetCouponTypeWithFallback();
            CouponCodeTextBox.Text = TextManager.GetCouponCodeWithFallback();
            AdditionalPrice1TextBox.Text = PriceManager.GetAdditionalPrice1WithFallback();
            AdditionalPrice2TextBox.Text = PriceManager.GetAdditionalPrice2WithFallback();

            #endregion

            #region Invelos Dates
            #endregion
        }

        private void OnSaveButtonClick(Object sender, EventArgs e)
        {
            String couponType;

            #region Invelos Prices

            if (FullEdit)
            {
                PriceManager.SetPurchasePriceCurrency(GetCurrencyInfo(PurchasePriceComboBox));
                PriceManager.SetPurchasePrice(PurchasePriceTextBox.Text);

                PriceManager.SetSRPCurrency(GetCurrencyInfo(SRPComboBox));
                PriceManager.SetSRP(SRPTextBox.Text);
            }

            #endregion

            #region Plugin Prices

            PriceManager.SetOriginalPrice(OriginalPriceTextBox.Text);
            PriceManager.SetOriginalPriceCurrency(GetCurrencyInfo(OriginalPriceComboBox));

            PriceManager.SetShippingCost(ShippingCostTextBox.Text);
            PriceManager.SetShippingCostCurrency(GetCurrencyInfo(ShippingCostComboBox));

            PriceManager.SetCreditCardCharge(CreditCardChargeTextBox.Text);
            PriceManager.SetCreditCardChargeCurrency(GetCurrencyInfo(CreditCardChargeComboBox));

            PriceManager.SetCreditCardFees(CreditCardFeesTextBox.Text);
            PriceManager.SetCreditCardFeesCurrency(GetCurrencyInfo(CreditCardFeesComboBox));

            PriceManager.SetDiscount(DiscountTextBox.Text);
            PriceManager.SetDiscountCurrency(GetCurrencyInfo(DiscountComboBox));

            PriceManager.SetCustomsFees(CustomsFeesTextBox.Text);
            PriceManager.SetCustomsFeesCurrency(GetCurrencyInfo(CustomsFeesComboBox));

            if (TextManager.GetCouponType(out couponType))
            {
                UInt16 count;

                if (Plugin.CouponTypes.TryGetValue(couponType, out count))
                {
                    count--;
                    if (count == 0)
                    {
                        Plugin.CouponTypes.Remove(couponType);
                    }
                    else
                    {
                        Plugin.CouponTypes[couponType] = count;
                    }
                }
            }
            TextManager.SetCouponType(CouponTypeComboBox.Text);
            if (TextManager.GetCouponType(out couponType))
            {
                UInt16 count;

                if (Plugin.CouponTypes.TryGetValue(couponType, out count))
                {
                    count++;
                    Plugin.CouponTypes[couponType] = count;
                }
                else
                {
                    Plugin.CouponTypes.Add(couponType, 1);
                }
            }
            TextManager.SetCouponCode(CouponCodeTextBox.Text);

            PriceManager.SetAdditionalPrice1(AdditionalPrice1TextBox.Text);
            PriceManager.SetAdditionalPrice1Currency(GetCurrencyInfo(AdditionalPrice1ComboBox));

            PriceManager.SetAdditionalPrice2(AdditionalPrice2TextBox.Text);
            PriceManager.SetAdditionalPrice2Currency(GetCurrencyInfo(AdditionalPrice2ComboBox));

            #endregion

            #region Invelos Dates


            if (FullEdit)
            {
                DateManager.SetPurchaseDate(GetDate(PurchaseDatePicker));
            }

            #endregion

            #region Plugin Dates

            DateManager.SetOrderDate(GetDate(OrderDatePicker));
            DateManager.SetShippingDate(GetDate(ShippingDatePicker));
            DateManager.SetDeliveryDate(GetDate(DeliveryDatePicker));
            DateManager.SetAdditionalDate1(GetDate(AdditionalDate1Picker));
            DateManager.SetAdditionalDate2(GetDate(AdditionalDate2Picker));

            #endregion

            if (FullEdit)
            {
                Plugin.Api.SaveDVDToCollection(Profile);
            }

            Plugin.Api.ReloadCurrentDVD();

            DialogResult = DialogResult.OK;

            DataChanged = false;

            Close();
        }

        private DateTime GetDate(DateTimePicker picker)
        {
            DateTime date;

            if (picker.Checked == false)
            {
                date = DateManager.DateNotSet;
            }
            else
            {
                date = picker.Value;
            }
            return (date);
        }

        private void OnDiscardButtonClick(Object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private CurrencyInfo GetCurrencyInfo(ComboBox comboBox)
        {
            CurrencyInfo ci;

            if (comboBox.SelectedIndex == -1)
            {
                ci = CurrencyInfo.Empty;
            }
            else
            {
                KeyValuePair<String, CurrencyInfo> kvp;

                kvp = (KeyValuePair<String, CurrencyInfo>)(comboBox.SelectedItem);
                ci = kvp.Value;
            }
            return (ci);
        }

        private void ValidatePrice(TextBox textBox, ComboBox comboBox)
        {
            if (String.IsNullOrEmpty(textBox.Text) == false)
            {
                Decimal price;

                if (Decimal.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out price) == false)
                {
                    String text;

                    text = String.Format(MessageBoxTexts.InvalidPrice, (12.34m).ToString("F2", CultureInfo.CurrentCulture));
                    MessageBox.Show(text, MessageBoxTexts.InvalidPriceHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox.Focus();
                }
                textBox.Text = PriceManager.GetFormattedPriceString(price);
                if (comboBox.SelectedIndex == -1)
                {
                    comboBox.Text = Plugin.DefaultCurrency.Name;
                }
            }
            else
            {
                comboBox.SelectedIndex = -1;
            }
        }

        #region Invelos Prices

        private void OnPurchasePriceTextBoxLeave(Object sender, EventArgs e)
        {
            ValidatePrice(PurchasePriceTextBox, PurchasePriceComboBox);
        }

        private void OnSRPTextBoxLeave(Object sender, EventArgs e)
        {
            ValidatePrice(SRPTextBox, SRPComboBox);
        }

        #endregion

        #region Plugin Prices

        private void OnOriginalPriceTextBoxLeave(Object sender, EventArgs e)
        {
            ValidatePrice(OriginalPriceTextBox, OriginalPriceComboBox);
        }

        private void OnShippingCostTextBoxLeave(Object sender, EventArgs e)
        {
            ValidatePrice(ShippingCostTextBox, ShippingCostComboBox);
        }

        private void OnCreditCardChargeTextBoxLeave(Object sender, EventArgs e)
        {
            ValidatePrice(CreditCardChargeTextBox, CreditCardChargeComboBox);
        }

        private void OnCreditCardFeesTextBoxLeave(Object sender, EventArgs e)
        {
            ValidatePrice(CreditCardFeesTextBox, CreditCardFeesComboBox);
        }

        private void OnDiscountTextBoxLeave(Object sender, EventArgs e)
        {
            ValidatePrice(DiscountTextBox, DiscountComboBox);
        }

        private void OnCustomsFeesTextBoxLeave(Object sender, EventArgs e)
        {
            ValidatePrice(CustomsFeesTextBox, CustomsFeesComboBox);
        }

        private void OnAdditionalPrice1TextBoxLeave(Object sender, EventArgs e)
        {
            ValidatePrice(AdditionalPrice1TextBox, AdditionalPrice1ComboBox);
        }

        private void OnAdditionalPrice2TextBoxLeave(Object sender, EventArgs e)
        {
            ValidatePrice(AdditionalPrice2TextBox, AdditionalPrice2ComboBox);
        }

        #endregion

        private void OnOptionsToolStripMenuItemClick(Object sender, EventArgs e)
        {
            using (SettingsForm form = new SettingsForm(Plugin))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    SetLabels();
                }
            }
        }

        private void OnAboutToolStripMenuItemClick(Object sender, EventArgs e)
        {
            using (AboutBox aboutBox = new AboutBox(GetType().Assembly))
            {
                aboutBox.ShowDialog();
            }
        }

        private void OnCheckForUpdatesToolStripMenuItemClick(Object sender, EventArgs e)
        {
            OnlineAccess.Init("Doena Soft.", "EnhancedPurchaseInfo");
            OnlineAccess.CheckForNewVersion("http://doena-soft.de/dvdprofiler/3.9.5/versions.xml", this, "EnhancedPurchaseInfo", this.GetType().Assembly);
        }

        private void OnDataChanged(Object sender, EventArgs e)
        {
            DataChanged = true;
        }

        private void OnFormClosing(Object sender, FormClosingEventArgs e)
        {
            if (DataChanged)
            {
                if (MessageBox.Show(MessageBoxTexts.AbandonChangesText, MessageBoxTexts.AbandonChangesHeader, MessageBoxButtons.YesNo
                    , MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void OnExportToXMLToolStripMenuItemClick(Object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.AddExtension = true;
                sfd.DefaultExt = ".xml";
                sfd.Filter = "XML files|*.xml";
                sfd.OverwritePrompt = true;
                sfd.RestoreDirectory = true;
                sfd.Title = Texts.SaveXmlFile;
                sfd.FileName = "EnhancedPurchaseInfo." + Profile.GetProfileID() + ".xml";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    EnhancedPurchaseInfo epi;

                    epi = GetEnhancedPurchaseInfoForXmlStructure();

                    try
                    {
                        epi.Serialize(sfd.FileName);
                        MessageBox.Show(MessageBoxTexts.Done, MessageBoxTexts.InformationHeader, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format(MessageBoxTexts.FileCantBeWritten, sfd.FileName, ex.Message)
                            , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private EnhancedPurchaseInfo GetEnhancedPurchaseInfoForXmlStructure()
        {
            EnhancedPurchaseInfo epi;
            DefaultValues dv;

            dv = Plugin.Settings.DefaultValues;
            epi = new EnhancedPurchaseInfo();

            #region Prices

            epi.OriginalPrice = GetXmlPrice(OriginalPriceTextBox, OriginalPriceComboBox, dv.OriginalPriceLabel);
            epi.ShippingCost = GetXmlPrice(ShippingCostTextBox, ShippingCostComboBox, dv.ShippingCostLabel);
            epi.CreditCardCharge = GetXmlPrice(CreditCardChargeTextBox, CreditCardChargeComboBox, dv.CreditCardChargeLabel);
            epi.CreditCardFees = GetXmlPrice(CreditCardFeesTextBox, CreditCardFeesComboBox, dv.CreditCardFeesLabel);
            epi.Discount = GetXmlPrice(DiscountTextBox, DiscountComboBox, dv.DiscountLabel);
            epi.CustomsFees = GetXmlPrice(CustomsFeesTextBox, CustomsFeesComboBox, dv.CustomsFeesLabel);
            epi.CouponType = GetXmlText(CouponTypeComboBox, dv.CouponTypeLabel);
            epi.CouponCode = GetXmlText(CouponCodeTextBox, dv.CouponCodeLabel);
            epi.AdditionalPrice1 = GetXmlPrice(AdditionalPrice1TextBox, AdditionalPrice1ComboBox, dv.AdditionalPrice1Label);
            epi.AdditionalPrice2 = GetXmlPrice(AdditionalPrice2TextBox, AdditionalPrice2ComboBox, dv.AdditionalPrice2Label);

            #endregion

            #region Dates

            epi.OrderDate = GetXmlDate(OrderDatePicker, dv.OrderDateLabel);
            epi.ShippingDate = GetXmlDate(ShippingDatePicker, dv.ShippingDateLabel);
            epi.DeliveryDate = GetXmlDate(DeliveryDatePicker, dv.DeliveryDateLabel);
            epi.AdditionalDate1 = GetXmlDate(AdditionalDate1Picker, dv.AdditionalDate1Label);
            epi.AdditionalDate2 = GetXmlDate(AdditionalDate2Picker, dv.AdditionalDate2Label);

            #endregion

            #region Invelos Data

            epi.InvelosData = new InvelosData();

            epi.InvelosData.PurchasePrice = GetXmlPrice(PurchasePriceTextBox, PurchasePriceComboBox, null);
            epi.InvelosData.SRP = GetXmlPrice(SRPTextBox, SRPComboBox, null);

            if (PurchaseDatePicker.Checked)
            {
                epi.InvelosData.PurchaseDate = PurchaseDatePicker.Value;
                epi.InvelosData.PurchaseDateSpecified = true;
            }

            #endregion

            return (epi);
        }

        private Text GetXmlText(Control textControl
            , String displayName)
        {
            if (String.IsNullOrEmpty(textControl.Text))
            {
                return (null);
            }
            else
            {
                Text text;

                text = new Text();
                text.Value = textControl.Text;
                text.DisplayName = displayName;
                return (text);
            }
        }

        private Date GetXmlDate(DateTimePicker datePicker
            , String displayName)
        {
            Date date;

            if (datePicker.Checked)
            {
                date = new Date();
                date.Value = datePicker.Value;
                date.DisplayName = displayName;
            }
            else
            {
                date = null;
            }
            return (date);
        }

        private Price GetXmlPrice(TextBox textBox
            , ComboBox comboBox
            , String displayName)
        {
            Decimal price;
            Price xmlPrice;

            xmlPrice = null;
            if (PriceManager.GetPriceFromText(textBox.Text, out price))
            {
                CurrencyInfo ci;

                xmlPrice = new Price();
                xmlPrice.Value = Convert.ToSingle(price);
                ci = GetCurrencyInfo(comboBox);
                xmlPrice.DenominationType = ci.Type;
                xmlPrice.DenominationDesc = ci.Name;
                xmlPrice.FormattedValue = ci.GetFormattedValue(price);
                xmlPrice.DisplayName = displayName;
            }
            return (xmlPrice);
        }

        private void OnImportFromXMLToolStripMenuItemClick(Object sender, EventArgs e)
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
                    EnhancedPurchaseInfo epi;

                    try
                    {
                        epi = EnhancedPurchaseInfo.Deserialize(ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        epi = null;
                        MessageBox.Show(String.Format(MessageBoxTexts.FileCantBeRead, ofd.FileName, ex.Message)
                           , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (epi != null)
                    {
                        Dictionary<String, CurrencyInfo> inverse;

                        inverse = GetInverseCurrencyInfo();

                        SetEnhancedPurchaseInfoFromXmlStructure(epi, inverse);
                    }
                }
            }
        }

        private void SetEnhancedPurchaseInfoFromXmlStructure(EnhancedPurchaseInfo epi
            , Dictionary<String, CurrencyInfo> inverse)
        {
            #region Prices

            SetPrice(inverse, OriginalPriceTextBox, OriginalPriceComboBox, epi.OriginalPrice);
            SetPrice(inverse, ShippingCostTextBox, ShippingCostComboBox, epi.ShippingCost);
            SetPrice(inverse, CreditCardChargeTextBox, CreditCardChargeComboBox, epi.CreditCardCharge);
            SetPrice(inverse, CreditCardFeesTextBox, CreditCardFeesComboBox, epi.CreditCardFees);
            SetPrice(inverse, DiscountTextBox, DiscountComboBox, epi.Discount);
            SetPrice(inverse, CustomsFeesTextBox, CustomsFeesComboBox, epi.CustomsFees);
            SetText(CouponTypeComboBox, epi.CouponType);
            SetText(CouponCodeTextBox, epi.CouponCode);
            SetPrice(inverse, AdditionalPrice1TextBox, AdditionalPrice1ComboBox, epi.AdditionalPrice1);
            SetPrice(inverse, AdditionalPrice2TextBox, AdditionalPrice2ComboBox, epi.AdditionalPrice2);

            #endregion

            #region Dates

            SetDate(epi.OrderDate, OrderDatePicker);
            SetDate(epi.ShippingDate, ShippingDatePicker);
            SetDate(epi.DeliveryDate, DeliveryDatePicker);
            SetDate(epi.AdditionalDate1, AdditionalDate1Picker);
            SetDate(epi.AdditionalDate2, AdditionalDate2Picker);

            #endregion
        }

        private Dictionary<String, CurrencyInfo> GetInverseCurrencyInfo()
        {
            Dictionary<String, CurrencyInfo> inverse;

            inverse = new Dictionary<String, CurrencyInfo>(Plugin.Currencies.Count);
            foreach (CurrencyInfo ci in Plugin.Currencies.Values)
            {
                inverse.Add(ci.Type, ci);
            }
            return inverse;
        }

        private static void SetDate(Date date
            , DateTimePicker datePicker)
        {
            if (date != null)
            {
                datePicker.Checked = true;
                datePicker.Value = date.Value;
            }
            else
            {
                datePicker.Checked = false;
            }
        }

        private void SetText(Control textControl
        , Text text)
        {
            if ((text != null) && (text.Value != null))
            {
                textControl.Text = text.Value;
            }
            else
            {
                textControl.Text = String.Empty;
            }
        }

        private void SetPrice(Dictionary<String, CurrencyInfo> inverse
            , TextBox textBox
            , ComboBox comboBox
            , Price xmlPrice)
        {
            if (xmlPrice != null)
            {
                Decimal price;

                price = Convert.ToDecimal(xmlPrice.Value);
                textBox.Text = PriceManager.GetFormattedPriceString(price);
                comboBox.Text = inverse[xmlPrice.DenominationType].Name;
            }
            else
            {
                textBox.Text = String.Empty;
                comboBox.SelectedIndex = -1;
            }
        }

        private void OnImportOptionsToolStripMenuItemClick(Object sender, EventArgs e)
        {
            Plugin.ImportOptions();
            SetLabels();
        }

        private void OnExportOptionsToolStripMenuItemClick(Object sender, EventArgs e)
        {
            Plugin.ExportOptions();
        }

        private void OnShippingCostCalculatorToolStripMenuItemClick(Object sender, EventArgs e)
        {
            EventHandler eventHandler;

            eventHandler = OpenCalculator;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        private void OnCopyAllToolStripMenuItemClick(Object sender, EventArgs e)
        {
            EnhancedPurchaseInfo epi;
            String xml;

            epi = GetEnhancedPurchaseInfoForXmlStructure();

            using (Utf8StringWriter sw = new Utf8StringWriter())
            {
                EnhancedPurchaseInfo.XmlSerializer.Serialize(sw, epi);

                xml = sw.ToString();
            }

            try
            {
                Clipboard.SetDataObject(xml, true, 4, 250);
            }
            catch
            {
                MessageBox.Show(MessageBoxTexts.CopyToClipboardFailed, MessageBoxTexts.ErrorHeader
                 , MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void OnPasteAllToolStripMenuItemClick(Object sender, EventArgs e)
        {
            EnhancedPurchaseInfo epi;

            try
            {
                String xml;

                xml = Clipboard.GetText();

                using (StringReader sr = new StringReader(xml))
                {
                    epi = (EnhancedPurchaseInfo)(EnhancedPurchaseInfo.XmlSerializer.Deserialize(sr));
                }
            }
            catch
            {
                epi = null;
                MessageBox.Show(MessageBoxTexts.PasteFromClipboardFailed, MessageBoxTexts.ErrorHeader
                    , MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            if (epi != null)
            {
                Dictionary<String, CurrencyInfo> inverse;

                inverse = GetInverseCurrencyInfo();

                SetEnhancedPurchaseInfoFromXmlStructure(epi, inverse);

                SetStandardPurchaseInfo(epi, inverse);
            }
        }

        private void SetStandardPurchaseInfo(EnhancedPurchaseInfo epi
            , Dictionary<String, CurrencyInfo> inverse)
        {
            if (FullEdit == false)
            {
                return;
            }

            if (epi.InvelosData != null)
            {
                SetPrice(inverse, PurchasePriceTextBox, PurchasePriceComboBox, epi.InvelosData.PurchasePrice);
                SetPrice(inverse, SRPTextBox, SRPComboBox, epi.InvelosData.SRP);

                if (epi.InvelosData.PurchaseDateSpecified)
                {
                    PurchaseDatePicker.Value = epi.InvelosData.PurchaseDate;
                }
                else
                {
                    PurchaseDatePicker.Checked = false;
                }
            }
            else
            {
                SetPrice(inverse, PurchasePriceTextBox, PurchasePriceComboBox, null);
                SetPrice(inverse, SRPTextBox, SRPComboBox, null);

                SetDate(null, PurchaseDatePicker);
            }
        }
    }
}