using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.DVDProfilerHelper;
using DoenaSoft.DVDProfiler.EnhancedPurchaseInfo.Resources;
using Invelos.DVDProfilerPlugin;

namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    internal partial class MainForm : Form
    {
        private readonly Plugin _plugin;

        private readonly IDVDInfo _profile;

        private readonly PriceManager _priceManager;

        private readonly TextManager _textManager;

        private readonly DateManager _dateManager;

        private readonly bool _fullEdit;

        private bool _dataChanged;

        internal event EventHandler OpenCalculator;

        internal MainForm(Plugin plugin, IDVDInfo profile, bool fullEdit)
        {
            _plugin = plugin;
            _profile = profile;
            _fullEdit = fullEdit;

            _priceManager = new PriceManager(profile);
            _textManager = new TextManager(profile);
            _dateManager = new DateManager(profile);

            InitializeComponent();

            BindComboBoxes();
            SetTextBoxes();
            SetComboBoxes();
            SetDatePicker();
            SetLabels();
            SetReadOnlies();

            _dataChanged = false;
        }

        private void SetReadOnlies()
        {
            PurchasePriceTextBox.ReadOnly = _fullEdit == false;
            PurchasePriceComboBox.Enabled = _fullEdit;

            SRPTextBox.ReadOnly = (_fullEdit == false);
            SRPComboBox.Enabled = _fullEdit;

            PurchaseDatePicker.Enabled = _fullEdit;

            if (_plugin.IsRemoteAccess)
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
                    if (control is TextBox textBox)
                    {
                        textBox.ReadOnly = true;
                    }
                    else if (control is ComboBox comboBox)
                    {
                        comboBox.Enabled = false;
                    }
                    else if (control is DateTimePicker datePicker)
                    {
                        datePicker.Enabled = false;
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
            SetDatePicker(PurchaseDatePicker, _dateManager.GetPurchaseDate);

            SetDatePicker(OrderDatePicker, _dateManager.GetOrderDate);
            SetDatePicker(ShippingDatePicker, _dateManager.GetShippingDate);
            SetDatePicker(DeliveryDatePicker, _dateManager.GetDeliveryDate);
            SetDatePicker(AdditionalDate1Picker, _dateManager.GetAdditionalDate1);
            SetDatePicker(AdditionalDate2Picker, _dateManager.GetAdditionalDate2);
        }

        private void SetDatePicker(DateTimePicker picker, DateManager.GetDateDelegate getDate)
        {
            if (getDate(out var date))
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

            var couponTypes = new List<string>(_plugin.CouponTypes.Keys);

            couponTypes.Sort();

            CouponTypeComboBox.Items.AddRange(couponTypes.ToArray());

            #endregion
        }

        private void BindDatabox(ComboBox comboBox)
        {
            comboBox.DataSource = new BindingSource(_plugin.Currencies, null);
            comboBox.DisplayMember = "Key";
            comboBox.ValueMember = "Value";
        }

        private void SetComboBoxes()
        {
            #region Invelos Prices

            SetComboBox(PurchasePriceComboBox, _priceManager.GetPurchasePriceCurrency);
            SetComboBox(SRPComboBox, _priceManager.GetSRPCurrency);

            #endregion

            #region Plugin Prices

            SetComboBox(OriginalPriceComboBox, _priceManager.GetOriginalPriceCurrency);
            SetComboBox(ShippingCostComboBox, _priceManager.GetShippingCostCurrency);
            SetComboBox(CreditCardChargeComboBox, _priceManager.GetCreditCardChargeCurrency);
            SetComboBox(CreditCardFeesComboBox, _priceManager.GetCreditCardFeesCurrency);
            SetComboBox(DiscountComboBox, _priceManager.GetDiscountCurrency);
            SetComboBox(CustomsFeesComboBox, _priceManager.GetCustomsFeesCurrency);
            SetComboBox(AdditionalPrice1ComboBox, _priceManager.GetAdditionalPrice1Currency);
            SetComboBox(AdditionalPrice2ComboBox, _priceManager.GetAdditionalPrice2Currency);

            #endregion
        }

        private void SetComboBox(ComboBox comboBox, PriceManager.GetCurrencyDelegate getCurrency)
        {
            if (getCurrency(out var ci))
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
            var dv = _plugin.Settings.DefaultValues;

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

            PurchasePriceTextBox.Text = _priceManager.GetPurchasePriceWithFallback();
            SRPTextBox.Text = _priceManager.GetSRPWithFallback();

            #endregion

            #region Plugin Prices

            OriginalPriceTextBox.Text = _priceManager.GetOriginalPriceWithFallback();
            ShippingCostTextBox.Text = _priceManager.GetShippingCostWithFallback();
            CreditCardChargeTextBox.Text = _priceManager.GetCreditCardChargeWithFallback();
            CreditCardFeesTextBox.Text = _priceManager.GetCreditCardFeesWithFallback();
            DiscountTextBox.Text = _priceManager.GetDiscountWithFallback();
            CustomsFeesTextBox.Text = _priceManager.GetCustomsFeesWithFallback();
            CouponTypeComboBox.Text = _textManager.GetCouponTypeWithFallback();
            CouponCodeTextBox.Text = _textManager.GetCouponCodeWithFallback();
            AdditionalPrice1TextBox.Text = _priceManager.GetAdditionalPrice1WithFallback();
            AdditionalPrice2TextBox.Text = _priceManager.GetAdditionalPrice2WithFallback();

            #endregion

            #region Invelos Dates
            #endregion
        }

        private void OnSaveButtonClick(object sender, EventArgs e)
        {
            #region Invelos Prices

            if (_fullEdit)
            {
                _priceManager.SetPurchasePriceCurrency(GetCurrencyInfo(PurchasePriceComboBox));
                _priceManager.SetPurchasePrice(PurchasePriceTextBox.Text);

                _priceManager.SetSRPCurrency(GetCurrencyInfo(SRPComboBox));
                _priceManager.SetSRP(SRPTextBox.Text);
            }

            #endregion

            #region Plugin Prices

            _priceManager.SetOriginalPrice(OriginalPriceTextBox.Text);
            _priceManager.SetOriginalPriceCurrency(GetCurrencyInfo(OriginalPriceComboBox));

            _priceManager.SetShippingCost(ShippingCostTextBox.Text);
            _priceManager.SetShippingCostCurrency(GetCurrencyInfo(ShippingCostComboBox));

            _priceManager.SetCreditCardCharge(CreditCardChargeTextBox.Text);
            _priceManager.SetCreditCardChargeCurrency(GetCurrencyInfo(CreditCardChargeComboBox));

            _priceManager.SetCreditCardFees(CreditCardFeesTextBox.Text);
            _priceManager.SetCreditCardFeesCurrency(GetCurrencyInfo(CreditCardFeesComboBox));

            _priceManager.SetDiscount(DiscountTextBox.Text);
            _priceManager.SetDiscountCurrency(GetCurrencyInfo(DiscountComboBox));

            _priceManager.SetCustomsFees(CustomsFeesTextBox.Text);
            _priceManager.SetCustomsFeesCurrency(GetCurrencyInfo(CustomsFeesComboBox));

            if (_textManager.GetCouponType(out var couponType))
            {
                if (_plugin.CouponTypes.TryGetValue(couponType, out var count))
                {
                    count--;

                    if (count == 0)
                    {
                        _plugin.CouponTypes.Remove(couponType);
                    }
                    else
                    {
                        _plugin.CouponTypes[couponType] = count;
                    }
                }
            }

            _textManager.SetCouponType(CouponTypeComboBox.Text);

            if (_textManager.GetCouponType(out couponType))
            {
                if (_plugin.CouponTypes.TryGetValue(couponType, out var count))
                {
                    count++;

                    _plugin.CouponTypes[couponType] = count;
                }
                else
                {
                    _plugin.CouponTypes.Add(couponType, 1);
                }
            }

            _textManager.SetCouponCode(CouponCodeTextBox.Text);

            _priceManager.SetAdditionalPrice1(AdditionalPrice1TextBox.Text);
            _priceManager.SetAdditionalPrice1Currency(GetCurrencyInfo(AdditionalPrice1ComboBox));

            _priceManager.SetAdditionalPrice2(AdditionalPrice2TextBox.Text);
            _priceManager.SetAdditionalPrice2Currency(GetCurrencyInfo(AdditionalPrice2ComboBox));

            #endregion

            #region Invelos Dates

            if (_fullEdit)
            {
                _dateManager.SetPurchaseDate(GetDate(PurchaseDatePicker));
            }

            #endregion

            #region Plugin Dates

            _dateManager.SetOrderDate(GetDate(OrderDatePicker));
            _dateManager.SetShippingDate(GetDate(ShippingDatePicker));
            _dateManager.SetDeliveryDate(GetDate(DeliveryDatePicker));
            _dateManager.SetAdditionalDate1(GetDate(AdditionalDate1Picker));
            _dateManager.SetAdditionalDate2(GetDate(AdditionalDate2Picker));

            #endregion

            if (_fullEdit)
            {
                _plugin.Api.SaveDVDToCollection(_profile);
            }

            _plugin.Api.ReloadCurrentDVD();

            DialogResult = DialogResult.OK;

            _dataChanged = false;

            Close();
        }

        private DateTime GetDate(DateTimePicker picker) => picker.Checked == false ? DateManager._dateNotSet : picker.Value;

        private void OnDiscardButtonClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            Close();
        }

        private CurrencyInfo GetCurrencyInfo(ComboBox comboBox)
        {
            if (comboBox.SelectedIndex == -1)
            {
                return CurrencyInfo.Empty;
            }
            else
            {
                var kvp = (KeyValuePair<string, CurrencyInfo>)(comboBox.SelectedItem);

                return kvp.Value;
            }
        }

        private void ValidatePrice(TextBox textBox, ComboBox comboBox)
        {
            if (string.IsNullOrEmpty(textBox.Text) == false)
            {
                if (decimal.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out var price) == false)
                {
                    var text = string.Format(MessageBoxTexts.InvalidPrice, (12.34m).ToString("F2", CultureInfo.CurrentCulture));

                    MessageBox.Show(text, MessageBoxTexts.InvalidPriceHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    textBox.Focus();
                }

                textBox.Text = _priceManager.GetFormattedPriceString(price);

                if (comboBox.SelectedIndex == -1)
                {
                    comboBox.Text = _plugin.DefaultCurrency.Name;
                }
            }
            else
            {
                comboBox.SelectedIndex = -1;
            }
        }

        #region Invelos Prices

        private void OnPurchasePriceTextBoxLeave(object sender, EventArgs e) => ValidatePrice(PurchasePriceTextBox, PurchasePriceComboBox);

        private void OnSRPTextBoxLeave(object sender, EventArgs e) => ValidatePrice(SRPTextBox, SRPComboBox);

        #endregion

        #region Plugin Prices

        private void OnOriginalPriceTextBoxLeave(object sender, EventArgs e) => ValidatePrice(OriginalPriceTextBox, OriginalPriceComboBox);

        private void OnShippingCostTextBoxLeave(object sender, EventArgs e) => ValidatePrice(ShippingCostTextBox, ShippingCostComboBox);

        private void OnCreditCardChargeTextBoxLeave(object sender, EventArgs e) => ValidatePrice(CreditCardChargeTextBox, CreditCardChargeComboBox);

        private void OnCreditCardFeesTextBoxLeave(object sender, EventArgs e) => ValidatePrice(CreditCardFeesTextBox, CreditCardFeesComboBox);

        private void OnDiscountTextBoxLeave(object sender, EventArgs e) => ValidatePrice(DiscountTextBox, DiscountComboBox);

        private void OnCustomsFeesTextBoxLeave(object sender, EventArgs e) => ValidatePrice(CustomsFeesTextBox, CustomsFeesComboBox);

        private void OnAdditionalPrice1TextBoxLeave(object sender, EventArgs e) => ValidatePrice(AdditionalPrice1TextBox, AdditionalPrice1ComboBox);

        private void OnAdditionalPrice2TextBoxLeave(object sender, EventArgs e) => ValidatePrice(AdditionalPrice2TextBox, AdditionalPrice2ComboBox);

        #endregion

        private void OnOptionsToolStripMenuItemClick(object sender, EventArgs e)
        {
            using (var form = new SettingsForm(_plugin))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    SetLabels();
                }
            }
        }

        private void OnAboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            using (var aboutBox = new AboutBox(GetType().Assembly))
            {
                aboutBox.ShowDialog();
            }
        }

        private void OnCheckForUpdatesToolStripMenuItemClick(object sender, EventArgs e)
        {
            OnlineAccess.Init("Doena Soft.", "EnhancedPurchaseInfo");
            OnlineAccess.CheckForNewVersion("http://doena-soft.de/dvdprofiler/3.9.5/versions.xml", this, "EnhancedPurchaseInfo", GetType().Assembly);
        }

        private void OnDataChanged(object sender, EventArgs e)
        {
            _dataChanged = true;
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (_dataChanged)
            {
                if (MessageBox.Show(MessageBoxTexts.AbandonChangesText, MessageBoxTexts.AbandonChangesHeader, MessageBoxButtons.YesNo
                    , MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void OnExportToXMLToolStripMenuItemClick(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog()
            {
                AddExtension = true,
                DefaultExt = ".xml",
                Filter = "XML files|*.xml",
                OverwritePrompt = true,
                RestoreDirectory = true,
                Title = Texts.SaveXmlFile,
                FileName = "EnhancedPurchaseInfo." + _profile.GetProfileID() + ".xml",
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var epi = GetEnhancedPurchaseInfoForXmlStructure();

                    try
                    {
                        epi.Serialize(sfd.FileName);

                        MessageBox.Show(MessageBoxTexts.Done, MessageBoxTexts.InformationHeader, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format(MessageBoxTexts.FileCantBeWritten, sfd.FileName, ex.Message)
                            , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private EnhancedPurchaseInfo GetEnhancedPurchaseInfoForXmlStructure()
        {
            var dv = _plugin.Settings.DefaultValues;

            var epi = new EnhancedPurchaseInfo()
            {
                #region Prices

                OriginalPrice = GetXmlPrice(OriginalPriceTextBox, OriginalPriceComboBox, dv.OriginalPriceLabel),
                ShippingCost = GetXmlPrice(ShippingCostTextBox, ShippingCostComboBox, dv.ShippingCostLabel),
                CreditCardCharge = GetXmlPrice(CreditCardChargeTextBox, CreditCardChargeComboBox, dv.CreditCardChargeLabel),
                CreditCardFees = GetXmlPrice(CreditCardFeesTextBox, CreditCardFeesComboBox, dv.CreditCardFeesLabel),
                Discount = GetXmlPrice(DiscountTextBox, DiscountComboBox, dv.DiscountLabel),
                CustomsFees = GetXmlPrice(CustomsFeesTextBox, CustomsFeesComboBox, dv.CustomsFeesLabel),
                CouponType = GetXmlText(CouponTypeComboBox, dv.CouponTypeLabel),
                CouponCode = GetXmlText(CouponCodeTextBox, dv.CouponCodeLabel),
                AdditionalPrice1 = GetXmlPrice(AdditionalPrice1TextBox, AdditionalPrice1ComboBox, dv.AdditionalPrice1Label),
                AdditionalPrice2 = GetXmlPrice(AdditionalPrice2TextBox, AdditionalPrice2ComboBox, dv.AdditionalPrice2Label),

                #endregion

                #region Dates

                OrderDate = GetXmlDate(OrderDatePicker, dv.OrderDateLabel),
                ShippingDate = GetXmlDate(ShippingDatePicker, dv.ShippingDateLabel),
                DeliveryDate = GetXmlDate(DeliveryDatePicker, dv.DeliveryDateLabel),
                AdditionalDate1 = GetXmlDate(AdditionalDate1Picker, dv.AdditionalDate1Label),
                AdditionalDate2 = GetXmlDate(AdditionalDate2Picker, dv.AdditionalDate2Label),

                #endregion

                #region Invelos Data

                InvelosData = new InvelosData()
                {
                    PurchasePrice = GetXmlPrice(PurchasePriceTextBox, PurchasePriceComboBox, null),
                    SRP = GetXmlPrice(SRPTextBox, SRPComboBox, null),
                }

                #endregion
            };

            #region Invelos Data

            if (PurchaseDatePicker.Checked)
            {
                epi.InvelosData.PurchaseDate = PurchaseDatePicker.Value;
                epi.InvelosData.PurchaseDateSpecified = true;
            }

            #endregion

            return epi;
        }

        private Text GetXmlText(Control textControl, string displayName)
        {
            if (string.IsNullOrEmpty(textControl.Text))
            {
                return null;
            }
            else
            {
                var text = new Text()
                {
                    Value = textControl.Text,
                    DisplayName = displayName,
                };

                return text;
            }
        }

        private Date GetXmlDate(DateTimePicker datePicker, string displayName)
        {
            if (datePicker.Checked)
            {
                var date = new Date()
                {
                    Value = datePicker.Value,
                    DisplayName = displayName,
                };

                return date;
            }
            else
            {
                return null;
            }
        }

        private Price GetXmlPrice(TextBox textBox, ComboBox comboBox, string displayName)
        {
            if (PriceManager.GetPriceFromText(textBox.Text, out var price))
            {
                var ci = GetCurrencyInfo(comboBox);

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
            else
            {
                return null;
            }
        }

        private void OnImportFromXMLToolStripMenuItemClick(object sender, EventArgs e)
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
                    EnhancedPurchaseInfo epi;
                    try
                    {
                        epi = EnhancedPurchaseInfo.Deserialize(ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        epi = null;

                        MessageBox.Show(string.Format(MessageBoxTexts.FileCantBeRead, ofd.FileName, ex.Message)
                           , MessageBoxTexts.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (epi != null)
                    {
                        var inverse = GetInverseCurrencyInfo();

                        SetEnhancedPurchaseInfoFromXmlStructure(epi, inverse);
                    }
                }
            }
        }

        private void SetEnhancedPurchaseInfoFromXmlStructure(EnhancedPurchaseInfo epi, Dictionary<string, CurrencyInfo> inverse)
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

        private Dictionary<string, CurrencyInfo> GetInverseCurrencyInfo()
        {
            var inverse = new Dictionary<string, CurrencyInfo>(_plugin.Currencies.Count);

            foreach (var ci in _plugin.Currencies.Values)
            {
                inverse.Add(ci.Type, ci);
            }

            return inverse;
        }

        private static void SetDate(Date date, DateTimePicker datePicker)
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

        private void SetText(Control textControl, Text text)
        {
            textControl.Text = text != null && text.Value != null
                ? text.Value
                : string.Empty;
        }

        private void SetPrice(Dictionary<string, CurrencyInfo> inverse, TextBox textBox, ComboBox comboBox, Price xmlPrice)
        {
            if (xmlPrice != null)
            {
                var price = Convert.ToDecimal(xmlPrice.Value);

                textBox.Text = _priceManager.GetFormattedPriceString(price);

                comboBox.Text = inverse[xmlPrice.DenominationType].Name;
            }
            else
            {
                textBox.Text = string.Empty;

                comboBox.SelectedIndex = -1;
            }
        }

        private void OnImportOptionsToolStripMenuItemClick(object sender, EventArgs e)
        {
            _plugin.ImportOptions();

            SetLabels();
        }

        private void OnExportOptionsToolStripMenuItemClick(object sender, EventArgs e) => _plugin.ExportOptions();

        private void OnShippingCostCalculatorToolStripMenuItemClick(object sender, EventArgs e)
        {
            var eventHandler = OpenCalculator;

            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        private void OnCopyAllToolStripMenuItemClick(object sender, EventArgs e)
        {
            var epi = GetEnhancedPurchaseInfoForXmlStructure();

            string xml;
            using (var sw = new Utf8StringWriter())
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

        private void OnPasteAllToolStripMenuItemClick(object sender, EventArgs e)
        {
            EnhancedPurchaseInfo epi;
            try
            {
                var xml = Clipboard.GetText();

                using (var sr = new StringReader(xml))
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
                var inverse = GetInverseCurrencyInfo();

                SetEnhancedPurchaseInfoFromXmlStructure(epi, inverse);

                SetStandardPurchaseInfo(epi, inverse);
            }
        }

        private void SetStandardPurchaseInfo(EnhancedPurchaseInfo epi, Dictionary<string, CurrencyInfo> inverse)
        {
            if (_fullEdit == false)
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