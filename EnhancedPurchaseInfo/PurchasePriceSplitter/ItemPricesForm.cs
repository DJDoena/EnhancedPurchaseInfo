using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DoenaSoft.DVDProfiler.EnhancedPurchaseInfo.Resources;

namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    public partial class ItemPricesForm : Form
    {
        private List<ItemPriceRow> PriceRows = null;

        private Button CalculateButton = null;

        private ItemPriceRow FinalPriceRow = null;

        public ItemPricesForm()
        {
            InitializeComponent();
            SetComboBox();
            ResetForm();
        }

        private void SetComboBox()
        {
            ShippinCostSplitComboBox.Items.Add(Texts.SplitTypeAbsolute);
            ShippinCostSplitComboBox.Items.Add(Texts.SplitTypeRelative);

            if (Program.Settings.DefaultValues.ShippingCostSplitAbsolute)
            {
                ShippinCostSplitComboBox.SelectedIndex = 0;
            }
            else
            {
                ShippinCostSplitComboBox.SelectedIndex = 1;
            }
        }

        private void SetLabels()
        {
            Text = Texts.ItemPrices;

            #region TabPages

            BasicsTabPage.Text = Texts.Basics;
            ItemsTabPage.Text = Texts.Items;

            #endregion

            #region Basics

            NumberOfItemsLabel.Text = Texts.NumberOfItems;
            ShippinCostSplitLabel.Text = Texts.SplitType;
            ShippingCostLabel.Text = Texts.ShippingCostInForeignCurrency;
            CreditCardChargeLabel.Text = Texts.CreditCardChargeInDomesticCurrency;
            CreditCardFeesLabel.Text = Texts.CreditCardFeesInDomesticCurrency;
            CustomsFeesLabel.Text = Texts.CustomsFeesInDomesticCurrency;
            OtherCostsLabel.Text = Texts.OtherCostsInDomesticCurrency;

            ResetButton.Text = Texts.ResetAll;

            #endregion

            #region Items

            ForeignItemPriceLabel.Text = Texts.ItemPriceInForeignCurrency;
            ShippingCostShareLabel.Text = Texts.ShippingCostInForeignCurrency;
            DomesticItemPriceLabel.Text = Texts.ItemPriceInDomesticCurrency;
            CreditCardChargeShareLabel.Text = Texts.CreditCardChargeInDomesticCurrency;
            CreditCardFeeShareLabel.Text = Texts.CreditCardFeesInDomesticCurrency;
            CustomsFeeShareLabel.Text = Texts.CustomsFeesInDomesticCurrency;
            OtherCostShareLabel.Text = Texts.OtherCostsInDomesticCurrency;

            #endregion
        }

        private void OnCalculateButtonClick(object sender, EventArgs e)
        {
            var itemSumForeign = 0m;
            for (var i = 0; i < PriceRows.Count; i++)
            {
                itemSumForeign += PriceRows[i].ForeignPriceUpDown.Value;
            }

            var accumulatedItemSum = 0m;
            var accumulatedShippingCost = 0m;
            var accumulatedCreditCardCharge = 0m;
            var accumulatedCreditCardFee = 0m;
            var accumulatedCustoms = 0m;
            var accumulatedOtherCost = 0m;

            var itemSumDomestic = CreditCardChargeUpDown.Value + CreditCardFeesUpDown.Value + CustomsFeesUpDown.Value + OtherCostsUpDown.Value;

            for (var i = 0; i < PriceRows.Count; i++)
            {
                CalculateRow(i, itemSumForeign, itemSumDomestic, ref accumulatedItemSum, ref accumulatedShippingCost, ref accumulatedCreditCardCharge, ref accumulatedCreditCardFee, ref accumulatedCustoms, ref accumulatedOtherCost);
            }

            FinalPriceRow.ForeignPriceUpDown.Value = itemSumForeign;

            SetFormattedValues(FinalPriceRow, itemSumDomestic, ShippingCostUpDown.Value, CreditCardChargeUpDown.Value, CreditCardFeesUpDown.Value, CustomsFeesUpDown.Value, OtherCostsUpDown.Value);
        }

        private void CalculateRow(int rowIndex, decimal itemSumForeign, decimal itemSumDomestic, ref decimal accumulatedItemSum, ref decimal accumulatedShippingCost, ref decimal accumulatedCreditCardCharge, ref decimal accumulatedCreditCardFee, ref decimal accumulatedCustoms, ref decimal accumulatedOtherCost)
        {
            var row = PriceRows[rowIndex];

            if (rowIndex != PriceRows.Count - 1)
            {
                CalulateRow(row, itemSumForeign, ref accumulatedItemSum, ref accumulatedShippingCost, ref accumulatedCreditCardCharge, ref accumulatedCreditCardFee, ref accumulatedCustoms, ref accumulatedOtherCost);
            }
            else
            {
                CalculateFinalRow(row, itemSumDomestic, accumulatedItemSum, accumulatedShippingCost, accumulatedCreditCardCharge, accumulatedCreditCardFee, accumulatedCustoms, accumulatedOtherCost);
            }
        }

        private void CalulateRow(ItemPriceRow row, decimal itemSumForeign, ref decimal accumulatedItemSum, ref decimal accumulatedShippingCost, ref decimal accumulatedCreditCardCharge, ref decimal accumulatedCreditCardFee, ref decimal accumulatedCustoms, ref decimal accumulatedOtherCost)
        {
            CalculateShippingCost(itemSumForeign, row, ref accumulatedShippingCost, out var shippingCost);

            var domesticPriceShare = 0m;

            if (itemSumForeign + ShippingCostUpDown.Value > 0)
            {
                domesticPriceShare = (row.ForeignPriceUpDown.Value + shippingCost) / (itemSumForeign + ShippingCostUpDown.Value);
            }

            CalculateDomesticPrice(CreditCardChargeUpDown.Value, domesticPriceShare, ref accumulatedCreditCardCharge, out var creditCardCharge);

            CalculateDomesticPrice(CreditCardFeesUpDown.Value, domesticPriceShare, ref accumulatedCreditCardFee, out var creditCardFee);

            CalculateDomesticPrice(CustomsFeesUpDown.Value, domesticPriceShare, ref accumulatedCustoms, out var customs);

            CalculateDomesticPrice(OtherCostsUpDown.Value, domesticPriceShare, ref accumulatedOtherCost, out var otherCost);

            var domesticPrice = creditCardCharge + creditCardFee + customs + otherCost;

            accumulatedItemSum += domesticPrice;

            SetFormattedValues(row, domesticPrice, shippingCost, creditCardCharge, creditCardFee, customs, otherCost);
        }

        private void CalculateFinalRow(ItemPriceRow row, decimal itemSumDomestic, decimal accumulatedItemSum, decimal accumulatedShippingCost, decimal accumulatedCreditCardCharge, decimal accumulatedCreditCardFee, decimal accumulatedCustoms, decimal accumulatedOtherCost)
        {
            var domesticPrice = itemSumDomestic - accumulatedItemSum;
            var shippingCost = ShippingCostUpDown.Value - accumulatedShippingCost;
            var creditCardCharge = CreditCardChargeUpDown.Value - accumulatedCreditCardCharge;
            var creditCardFee = CreditCardFeesUpDown.Value - accumulatedCreditCardFee;
            var customs = CustomsFeesUpDown.Value - accumulatedCustoms;
            var otherCost = OtherCostsUpDown.Value - accumulatedOtherCost;

            SetFormattedValues(row, domesticPrice, shippingCost, creditCardCharge, creditCardFee, customs, otherCost);
        }

        private static void CalculateDomesticPrice(decimal itemSumDomestic, decimal domesticPriceShare, ref decimal accumulatedItemSum, out decimal domesticPrice)
        {
            domesticPrice = itemSumDomestic * domesticPriceShare;

            domesticPrice = Math.Round(domesticPrice, 2, MidpointRounding.AwayFromZero);

            accumulatedItemSum += domesticPrice;
        }

        private void CalculateShippingCost(decimal itemSumForeign, ItemPriceRow row, ref decimal accumulatedShippingCost, out decimal shippingCost)
        {
            shippingCost = ShippinCostSplitComboBox.SelectedIndex == 0
                ? ShippingCostUpDown.Value / PriceRows.Count
                : (row.ForeignPriceUpDown.Value / itemSumForeign) * ShippingCostUpDown.Value;

            shippingCost = Math.Round(shippingCost, 2, MidpointRounding.AwayFromZero);

            accumulatedShippingCost += shippingCost;
        }

        private void SetFormattedValues(ItemPriceRow row, decimal domesticPrice, decimal shippingCost, decimal creditCardCharge, decimal creditCardFee, decimal customs, decimal otherCost)
        {
            SetFormattedValue(row.DomesticPriceTextBox, domesticPrice);
            SetFormattedValue(row.ShippingCostShareTextBox, shippingCost);
            SetFormattedValue(row.CreditCardChargeShareTextBox, creditCardCharge);
            SetFormattedValue(row.CreditCardFeeShareTextBox, creditCardFee);
            SetFormattedValue(row.CustomsFeeShareTextBox, customs);
            SetFormattedValue(row.OtherCostShareTextBox, otherCost);
        }

        private void SetFormattedValue(TextBox textBox, decimal value)
        {
            var ci = Application.CurrentCulture;

            var formattedValue = value.ToString("F2", ci);

            textBox.Text = formattedValue;

            textBox.Tag = value;
        }

        private void AddRows()
        {
            var itemCount = (int)(NumberOfItemsUpDown.Value);

            ResetRows(itemCount);

            int top;
            if ((PriceRows == null) || (PriceRows.Count == 0))
            {
                PriceRows = new List<ItemPriceRow>();

                top = 5;
            }
            else
            {
                top = PriceRows[PriceRows.Count - 1].Top + 30;
            }

            for (var i = PriceRows.Count; i < itemCount; i++)
            {
                var row = AddRow(top, i);

                PriceRows.Add(row);

                top += 30;
            }
        }

        private void ResetRows(int itemCount)
        {
            if ((PriceRows != null) && (PriceRows.Count > 0))
            {
                for (var i = PriceRows.Count - 1; i >= itemCount; i--)
                {
                    var row = PriceRows[i];

                    ItemPanel.Controls.Remove(row);

                    UnregisterRowEvents(row);

                    row.Dispose();

                    PriceRows.RemoveAt(i);
                }
            }
        }

        private void UnregisterRowEvents(ItemPriceRow row)
        {
            row.ForeignPriceUpDown.Enter -= OnUpDownEnter;

            row.ShippingCostShareTextBox.Enter -= OnUpDownEnter;

            row.DomesticPriceTextBox.Enter -= OnUpDownEnter;
        }

        private ItemPriceRow AddRow(int top, int index)
        {
            var row = new ItemPriceRow()
            {
                Top = top,
                Left = 6,
                Name = "ItemPriceRow" + index.ToString(),
                TabIndex = index,
            };

            RegisterRowEvents(row);

            ItemPanel.Controls.Add(row);

            return row;
        }

        private void RegisterRowEvents(ItemPriceRow row)
        {
            row.ForeignPriceUpDown.Enter += OnUpDownEnter;
            row.ShippingCostShareTextBox.Enter += OnUpDownEnter;
            row.DomesticPriceTextBox.Enter += OnUpDownEnter;
        }

        private void OnUpDownEnter(object sender, EventArgs e)
        {
            if (sender is NumericUpDown upDown)
            {
                upDown.Select(0, upDown.Text.Length);
            }
            else if (sender is TextBox textBox)
            {
                textBox.Select(0, textBox.Text.Length);
            }
        }

        private void OnResetButtonClick(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            SuspendLayout();

            ResetRows(0);

            NumberOfItemsUpDown.Value = 2;
            ShippingCostUpDown.Value = 0;
            CreditCardChargeUpDown.Value = 0;
            CreditCardFeesUpDown.Value = 0;
            CustomsFeesUpDown.Value = 0;
            OtherCostsUpDown.Value = 0;

            if (FinalPriceRow != null)
            {
                FinalPriceRow.ForeignPriceUpDown.Value = 0;

                SetFormattedValue(FinalPriceRow.ShippingCostShareTextBox, 0);
                SetFormattedValue(FinalPriceRow.DomesticPriceTextBox, 0);
            }

            ResumeLayout(true);

            OnNumberOfItemsUpDownValueChanged(this, EventArgs.Empty);
        }

        private void OnNumberOfItemsUpDownValueChanged(object sender, EventArgs e)
        {
            SuspendLayout();

            AddRows();

            if (CalculateButton == null)
            {
                CalculateButton = new Button();
                CalculateButton.UseVisualStyleBackColor = true;
                CalculateButton.Name = "CalculateButton";
                CalculateButton.Text = Texts.Calculate;
                CalculateButton.Left = 7;
                CalculateButton.Size = new System.Drawing.Size(150, 23);
                CalculateButton.Click += OnCalculateButtonClick;

                ItemPanel.Controls.Add(CalculateButton);
            }

            var top = PriceRows[PriceRows.Count - 1].Top + 35;

            CalculateButton.Top = top;
            CalculateButton.TabIndex = PriceRows.Count;

            top = CalculateButton.Top + CalculateButton.Height + 10;

            if (FinalPriceRow == null)
            {
                FinalPriceRow = AddRow(top, PriceRows.Count);
                FinalPriceRow.ForeignPriceUpDown.ReadOnly = true;
                FinalPriceRow.ForeignPriceUpDown.TabStop = false;
            }

            FinalPriceRow.Top = top;
            FinalPriceRow.TabIndex = PriceRows.Count + 1;

            ItemPanel.AutoScrollMinSize = new System.Drawing.Size(0, top);

            ResumeLayout(true);
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            Program.Settings.DefaultValues.ShippingCostSplitAbsolute = (ShippinCostSplitComboBox.SelectedIndex == 0);
        }

        private void OnFormShown(object sender, EventArgs e)
        {
            SetLabels();
        }

        private void OnUpDownLeave(object sender, EventArgs e)
        {
            var upDown = (NumericUpDown)sender;

            if (string.IsNullOrEmpty(upDown.Text))
            {
                upDown.Value = upDown.Minimum;
                upDown.Text = upDown.Value.ToString();
            }
        }
    }
}