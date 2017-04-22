using DoenaSoft.DVDProfiler.EnhancedPurchaseInfo.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    public partial class SettingsForm : Form
    {
        private readonly Plugin Plugin;

        public SettingsForm(Plugin plugin)
        {
            Plugin = plugin;

            InitializeComponent();

            SetSettings();

            SetLabels();

            SetToolTips();

            SetComboBoxes();
        }

        private void SetComboBoxes()
        {
            Dictionary<Int32, CultureInfo> uiLanguages;
            CultureInfo ci;

            DefaultCurrencyComboBox.DataSource = new BindingSource(Plugin.Currencies, null);
            DefaultCurrencyComboBox.DisplayMember = "Key";
            DefaultCurrencyComboBox.ValueMember = "Value";
            DefaultCurrencyComboBox.Text = Plugin.DefaultCurrency.Name;

            uiLanguages = new Dictionary<Int32, CultureInfo>(2);
            ci = CultureInfo.GetCultureInfo("en");
            uiLanguages.Add(ci.LCID, ci);
            ci = CultureInfo.GetCultureInfo("de");
            uiLanguages.Add(ci.LCID, ci);
            UiLanguageComboBox.DataSource = new BindingSource(uiLanguages, null);
            UiLanguageComboBox.DisplayMember = "Value";
            UiLanguageComboBox.ValueMember = "Key";
            UiLanguageComboBox.Text = Plugin.Settings.DefaultValues.UiLanguage.DisplayName;
        }

        private void SetToolTips()
        {
            ToolTip tt;

            tt = new ToolTip();
            tt.SetToolTip(ResetAdditionalPrice1Button, Texts.Reset);
            tt.SetToolTip(ResetAdditionalPrice2Button, Texts.Reset);
            tt.SetToolTip(ResetAdditionalDate1Button, Texts.Reset);
            tt.SetToolTip(ResetAdditionalDate2Button, Texts.Reset);
        }

        private void SetSettings()
        {
            DefaultValues dv = Plugin.Settings.DefaultValues;

            #region Labels

            #region Prices

            OriginalPriceTextBox.Text = dv.OriginalPriceLabel;
            ShippingCostTextBox.Text = dv.ShippingCostLabel;
            CreditCardChargeTextBox.Text = dv.CreditCardChargeLabel;
            CreditCardFeesTextBox.Text = dv.CreditCardFeesLabel;
            DiscountTextBox.Text = dv.DiscountLabel;
            CustomsFeesTextBox.Text = dv.CustomsFeesLabel;
            CouponTypeTextBox.Text = dv.CouponTypeLabel;
            CouponCodeTextBox.Text = dv.CouponCodeLabel;
            AdditionalPrice1TextBox.Text = dv.AdditionalPrice1Label;
            AdditionalPrice2TextBox.Text = dv.AdditionalPrice2Label;

            #endregion

            #region Dates

            OrderDateTextBox.Text = dv.OrderDateLabel;
            ShippingDateTextBox.Text = dv.ShippingDateLabel;
            DeliveryDateTextBox.Text = dv.DeliveryDateLabel;
            AdditionalDate1TextBox.Text = dv.AdditionalDate1Label;
            AdditionalDate2TextBox.Text = dv.AdditionalDate2Label;

            #endregion

            #endregion

            #region Basics

            IdCheckBox.Checked = dv.Id;

            TitleCheckBox.Checked = dv.Title;

            EditionCheckBox.Checked = dv.Edition;

            SortTitleCheckBox.Checked = dv.SortTitle;

            PurchasePlaceCheckBox.Checked = dv.PurchasePlace;

            #endregion

            #region Prices

            #region Invelos Prices

            PurchasePriceCheckBox.Checked = dv.PurchasePrice;
            SRPCheckBox.Checked = dv.SRP;

            #endregion

            #region Plugin Prices

            OriginalPriceCheckBox.Checked = dv.OriginalPrice;
            ShippingCostCheckBox.Checked = dv.ShippingCost;
            CreditCardChargeCheckBox.Checked = dv.CreditCardCharge;
            CreditCardFeesCheckBox.Checked = dv.CreditCardFees;
            DiscountCheckBox.Checked = dv.Discount;
            CustomsFeesCheckBox.Checked = dv.CustomsFees;
            CouponTypeCheckBox.Checked = dv.CouponType;
            CouponCodeCheckBox.Checked = dv.CouponCode;
            AdditionalPrice1CheckBox.Checked = dv.AdditionalPrice1;
            AdditionalPrice2CheckBox.Checked = dv.AdditionalPrice2;

            #endregion

            #region Dates

            #region Invelos Dates

            PurchaseDateCheckBox.Checked = dv.PurchaseDate;

            #endregion

            #region Plugin Dates

            OrderDateCheckBox.Checked = dv.OrderDate;
            ShippingDateCheckBox.Checked = dv.ShippingDate;
            DeliveryDateCheckBox.Checked = dv.DeliveryDate;
            AdditionalDate1CheckBox.Checked = dv.AdditionalDate1;
            AdditionalDate2CheckBox.Checked = dv.AdditionalDate2;

            #endregion

            #endregion

            #endregion

            ExportToCollectionXmlCheckBox.Checked = dv.ExportToCollectionXml;
        }

        private void SetLabels()
        {
            #region Labels

            #region Prices

            OriginalPriceLabel.Text = Texts.OriginalPrice;
            ShippingCostLabel.Text = Texts.ShippingCost;
            CreditCardChargeLabel.Text = Texts.CreditCardCharge;
            CreditCardFeesLabel.Text = Texts.CreditCardFees;
            DiscountLabel.Text = Texts.Discount;
            CustomsFeesLabel.Text = Texts.CustomsFees;
            CouponTypeLabel.Text = Texts.CouponType;
            CouponCodeLabel.Text = Texts.CouponCode;
            AdditionalPrice1Label.Text = Texts.AdditionalPrice1;
            AdditionalPrice2Label.Text = Texts.AdditionalPrice2;

            #endregion

            #region Dates

            OrderDateLabel.Text = Texts.OrderDate;
            ShippingDateLabel.Text = Texts.ShippingDate;
            DeliveryDateLabel.Text = Texts.DeliveryDate;
            AdditionalDate1Label.Text = Texts.AdditionalDate1;
            AdditionalDate2Label.Text = Texts.AdditionalDate2;

            #endregion

            #endregion

            #region Basics

            IdCheckBox.Text = Texts.Id;

            TitleCheckBox.Text = Texts.Title;

            EditionCheckBox.Text = Texts.Edition;

            SortTitleCheckBox.Text = Texts.SortTitle;

            PurchasePlaceCheckBox.Text = Texts.PurchasePlace;

            #endregion

            #region Invelos Prices

            PurchasePriceCheckBox.Text = Texts.PurchasePrice;
            SRPCheckBox.Text = Texts.SRP;

            #endregion

            #region Plugin Prices

            OriginalPriceCheckBox.Text = Texts.OriginalPrice;
            ShippingCostCheckBox.Text = Texts.ShippingCost;
            CreditCardChargeCheckBox.Text = Texts.CreditCardCharge;
            CreditCardFeesCheckBox.Text = Texts.CreditCardFees;
            DiscountCheckBox.Text = Texts.Discount;
            CustomsFeesCheckBox.Text = Texts.CustomsFees;
            CouponTypeCheckBox.Text = Texts.CouponType;
            CouponCodeCheckBox.Text = Texts.CouponCode;

            #endregion

            #region Invelos Dates

            PurchaseDateCheckBox.Text = Texts.PurchaseDate;

            #endregion

            #region Plugin Dates

            OrderDateCheckBox.Text = Texts.OrderDate;
            ShippingDateCheckBox.Text = Texts.ShippingDate;
            DeliveryDateCheckBox.Text = Texts.DeliveryDate;

            #endregion

            #region Misc

            Text = Texts.Options;

            #region Labels

            DefaultCurrencyLabel.Text = Texts.DefaultCurrency;
            UiLanguageLabel.Text = Texts.UiLanguage;
            ExportToCollectionXmlCheckBox.Text = Texts.ExportToCollectionXml;

            #endregion

            #region TabPages

            LabelTabPage.Text = Texts.Labels;
            ExcelTabPage.Text = Texts.ExcelColumns;
            MiscTabPage.Text = Texts.Misc;
            LabelsPricesTabPage.Text = Texts.Prices;
            LabelsDatesTabPage.Text = Texts.Dates;
            ExcelBasicsTabPage.Text = Texts.Basics;
            ExcelDatesTabPage.Text = Texts.Dates;
            ExcelPricesTabPage.Text = Texts.Prices;

            #endregion

            #region GroupBoxes

            PricesInvelosDataGroupBox.Text = Texts.InvelosData;
            PricesPluginDataGroupBox.Text = Texts.PluginData;
            DatesInvelosDataGroupBox.Text = Texts.InvelosData;
            DatesPluginDataGroupBox.Text = Texts.PluginData;

            #endregion

            #region Buttons

            #region Prices

            ResetOriginalPriceButton.Text = Texts.ResetLabel;
            ResetShippingCostButton.Text = Texts.ResetLabel;
            ResetCreditCardChargeButton.Text = Texts.ResetLabel;
            ResetCreditCardFeesButton.Text = Texts.ResetLabel;
            ResetDiscountButton.Text = Texts.ResetLabel;
            ResetCustomsFeesButton.Text = Texts.ResetLabel;
            ResetCouponTypeButton.Text = Texts.ResetLabel;
            ResetCouponCodeButton.Text = Texts.ResetLabel;
            ResetAdditionalPrice1Button.Text = Texts.ResetLabel;
            ResetAdditionalPrice2Button.Text = Texts.ResetLabel;

            #endregion

            #region Dates

            ResetOrderDateButton.Text = Texts.ResetLabel;
            ResetShippingDateButton.Text = Texts.ResetLabel;
            ResetDeliveryDateButton.Text = Texts.ResetLabel;
            ResetAdditionalDate1Button.Text = Texts.ResetLabel;
            ResetAdditionalDate2Button.Text = Texts.ResetLabel;

            #endregion

            SaveButton.Text = Texts.Save;
            DiscardButton.Text = Texts.Cancel;

            #endregion

            #endregion

            SetVariableLabels();
        }

        private void SetVariableLabels()
        {
            #region Prices

            OriginalPriceCheckBox.Text = OriginalPriceTextBox.Text;
            ShippingCostCheckBox.Text = ShippingCostTextBox.Text;
            CreditCardChargeCheckBox.Text = CreditCardChargeTextBox.Text;
            CreditCardFeesCheckBox.Text = CreditCardFeesTextBox.Text;
            DiscountCheckBox.Text = DiscountTextBox.Text;
            CustomsFeesCheckBox.Text = CustomsFeesTextBox.Text;
            CouponTypeCheckBox.Text = CouponTypeTextBox.Text;
            CouponCodeCheckBox.Text = CouponCodeTextBox.Text;
            AdditionalPrice1CheckBox.Text = AdditionalPrice1TextBox.Text;
            AdditionalPrice2CheckBox.Text = AdditionalPrice2TextBox.Text;

            #endregion

            #region Dates

            OrderDateCheckBox.Text = OrderDateTextBox.Text;
            ShippingDateCheckBox.Text = ShippingDateTextBox.Text;
            DeliveryDateCheckBox.Text = DeliveryDateTextBox.Text;
            AdditionalDate1CheckBox.Text = AdditionalDate1TextBox.Text;
            AdditionalDate2CheckBox.Text = AdditionalDate2TextBox.Text;

            #endregion
        }

        private void OnLabelTextChanged(Object sender, EventArgs e)
        {
            SetVariableLabels();
        }

        private void OnDiscardButtonClick(Object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void OnSaveButtonClick(Object sender, EventArgs e)
        {
            DefaultValues dv = Plugin.Settings.DefaultValues;

            #region Labels

            #region Prices

            dv.OriginalPriceLabel = OriginalPriceTextBox.Text;
            dv.ShippingCostLabel = ShippingCostTextBox.Text;
            dv.CreditCardChargeLabel = CreditCardChargeTextBox.Text;
            dv.CreditCardFeesLabel = CreditCardFeesTextBox.Text;
            dv.DiscountLabel = DiscountTextBox.Text;
            dv.CustomsFeesLabel = CustomsFeesTextBox.Text;
            dv.CouponTypeLabel = CouponTypeTextBox.Text;
            dv.CouponCodeLabel = CouponCodeTextBox.Text;
            dv.AdditionalPrice1Label = AdditionalPrice1TextBox.Text;
            dv.AdditionalPrice2Label = AdditionalPrice2TextBox.Text;

            #endregion

            #region Dates

            dv.OrderDateLabel = OrderDateTextBox.Text;
            dv.ShippingDateLabel = ShippingDateTextBox.Text;
            dv.DeliveryDateLabel = DeliveryDateTextBox.Text;
            dv.AdditionalDate1Label = AdditionalDate1TextBox.Text;
            dv.AdditionalDate2Label = AdditionalDate2TextBox.Text;

            #endregion

            #endregion

            #region Basics

            dv.Id = IdCheckBox.Checked;

            dv.Title = TitleCheckBox.Checked;

            dv.Edition = EditionCheckBox.Checked;

            dv.SortTitle = SortTitleCheckBox.Checked;

            dv.PurchasePlace = PurchasePlaceCheckBox.Checked;

            #endregion

            #region Prices

            #region Invelos Prices

            dv.PurchasePrice = PurchasePriceCheckBox.Checked;
            dv.SRP = SRPCheckBox.Checked;

            #endregion

            #region Plugin Prices

            dv.OriginalPrice = OriginalPriceCheckBox.Checked;
            dv.ShippingCost = ShippingCostCheckBox.Checked;
            dv.CreditCardCharge = CreditCardChargeCheckBox.Checked;
            dv.CreditCardFees = CreditCardFeesCheckBox.Checked;
            dv.Discount = DiscountCheckBox.Checked;
            dv.CustomsFees = CustomsFeesCheckBox.Checked;
            dv.CouponType = CouponTypeCheckBox.Checked;
            dv.CouponCode = CouponCodeCheckBox.Checked;
            dv.AdditionalPrice1 = AdditionalPrice1CheckBox.Checked;
            dv.AdditionalPrice2 = AdditionalPrice2CheckBox.Checked;

            #endregion

            #region Dates

            #region Invelos Dates

            dv.PurchaseDate = PurchaseDateCheckBox.Checked;

            #endregion

            #region Plugin Dates

            dv.OrderDate = OrderDateCheckBox.Checked;
            dv.ShippingDate = ShippingDateCheckBox.Checked;
            dv.DeliveryDate = DeliveryDateCheckBox.Checked;
            dv.AdditionalDate1 = AdditionalDate1CheckBox.Checked;
            dv.AdditionalDate2 = AdditionalDate2CheckBox.Checked;

            #endregion

            #endregion

            #endregion

            dv.ExportToCollectionXml = ExportToCollectionXmlCheckBox.Checked;

            CurrencyInfo defaultCurrency = GetCurrencyInfo(DefaultCurrencyComboBox);

            dv.DefaultCurrency = defaultCurrency.Id;

            Plugin.DefaultCurrency = defaultCurrency;

            CultureInfo uiLanguage = GetUiLanguage();

            dv.UiLanguage = uiLanguage;

            Texts.Culture = uiLanguage;

            MessageBoxTexts.Culture = uiLanguage;

            DialogResult = DialogResult.OK;

            Close();
        }

        private CultureInfo GetUiLanguage()
        {
            CultureInfo ci;
            KeyValuePair<Int32, CultureInfo> kvp;

            kvp = (KeyValuePair<Int32, CultureInfo>)(UiLanguageComboBox.SelectedItem);
            ci = kvp.Value;
            return (ci);
        }

        private CurrencyInfo GetCurrencyInfo(ComboBox comboBox)
        {
            CurrencyInfo ci;
            KeyValuePair<String, CurrencyInfo> kvp;

            kvp = (KeyValuePair<String, CurrencyInfo>)(comboBox.SelectedItem);
            ci = kvp.Value;
            return (ci);
        }

        #region OnResetButtonClick

        #region Prices

        private void OnResetOriginalPriceButtonClick(Object sender, EventArgs e)
        {
            CultureInfo ci;

            ci = SetTempLanguage();
            OriginalPriceTextBox.Text = Texts.OriginalPrice;
            UnsetTempLanguage(ci);
        }

        private void OnResetShippingCostButtonClick(Object sender, EventArgs e)
        {
            CultureInfo ci;

            ci = SetTempLanguage();
            ShippingCostTextBox.Text = Texts.ShippingCost;
            UnsetTempLanguage(ci);
        }

        private void OnResetCreditCardChargeButtonClick(Object sender, EventArgs e)
        {
            CultureInfo ci;

            ci = SetTempLanguage();
            CreditCardChargeTextBox.Text = Texts.CreditCardCharge;
            UnsetTempLanguage(ci);
        }

        private void OnResetCreditCardFeesButtonClick(Object sender, EventArgs e)
        {
            CultureInfo ci;

            ci = SetTempLanguage();
            CreditCardFeesTextBox.Text = Texts.CreditCardFees;
            UnsetTempLanguage(ci);
        }

        private void OnResetDiscountButtonClick(Object sender, EventArgs e)
        {
            CultureInfo ci;

            ci = SetTempLanguage();
            DiscountTextBox.Text = Texts.Discount;
            UnsetTempLanguage(ci);
        }

        private void OnResetCustomsFeesButtonClick(Object sender, EventArgs e)
        {
            CultureInfo ci;

            ci = SetTempLanguage();
            CustomsFeesTextBox.Text = Texts.CustomsFees;
            UnsetTempLanguage(ci);
        }

        private void OnResetCouponTypeButtonClick(Object sender, EventArgs e)
        {
            CultureInfo ci;

            ci = SetTempLanguage();
            CouponTypeTextBox.Text = Texts.CouponType;
            UnsetTempLanguage(ci);
        }

        private void OnResetCouponCodeButtonClick(Object sender, EventArgs e)
        {
            CultureInfo ci;

            ci = SetTempLanguage();
            CouponCodeTextBox.Text = Texts.CouponCode;
            UnsetTempLanguage(ci);
        }

        private void OnResetAdditionalPrice1ButtonClick(Object sender, EventArgs e)
        {
            CultureInfo ci;

            ci = SetTempLanguage();
            AdditionalPrice1TextBox.Text = Texts.AdditionalPrice1;
            UnsetTempLanguage(ci);
        }

        private void OnResetAdditionalPrice2ButtonClick(Object sender, EventArgs e)
        {
            CultureInfo ci;

            ci = SetTempLanguage();
            AdditionalPrice2TextBox.Text = Texts.AdditionalPrice2;
            UnsetTempLanguage(ci);
        }

        #endregion

        #region Dates

        private void OnResetOrderDateButtonClick(Object sender, EventArgs e)
        {
            CultureInfo ci;

            ci = SetTempLanguage();
            OrderDateTextBox.Text = Texts.OrderDate;
            UnsetTempLanguage(ci);
        }

        private void OnResetShippingDateButtonClick(Object sender, EventArgs e)
        {
            CultureInfo ci;

            ci = SetTempLanguage();
            ShippingDateTextBox.Text = Texts.ShippingDate;
            UnsetTempLanguage(ci);
        }

        private void OnResetDeliveryDateButtonClick(Object sender, EventArgs e)
        {
            CultureInfo ci;

            ci = SetTempLanguage();
            DeliveryDateTextBox.Text = Texts.DeliveryDate;
            UnsetTempLanguage(ci);
        }

        private void OnResetAdditionalDate1ButtonClick(Object sender, EventArgs e)
        {
            CultureInfo ci;

            ci = SetTempLanguage();
            AdditionalDate1TextBox.Text = Texts.AdditionalDate1;
            UnsetTempLanguage(ci);
        }

        private void OnResetAdditionalDate2ButtonClick(Object sender, EventArgs e)
        {
            CultureInfo ci;

            ci = SetTempLanguage();
            AdditionalDate2TextBox.Text = Texts.AdditionalDate2;
            UnsetTempLanguage(ci);
        }

        #endregion

        private CultureInfo SetTempLanguage()
        {
            CultureInfo previousCI;
            CultureInfo currentCI;

            previousCI = Texts.Culture;
            currentCI = GetUiLanguage();
            Texts.Culture = currentCI;
            return (previousCI);
        }

        private void UnsetTempLanguage(CultureInfo previousCI)
        {
            Texts.Culture = previousCI;
        }

        #endregion
    }
}