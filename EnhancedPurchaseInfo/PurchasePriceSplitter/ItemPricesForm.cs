using DoenaSoft.DVDProfiler.EnhancedPurchaseInfo.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

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
            ShippingCostShareLabel.Text = Texts.ShippingCostShareInForeignCurrency;
            DomesticItemPriceLabel.Text = Texts.ItemPriceInDomesticCurrency;

            #endregion

        }

        private void OnCalculateButtonClick(Object sender, EventArgs e)
        {
            Decimal itemSum;
            Decimal endSum;
            Decimal shippingCost;

            itemSum = 0;
            for (Int32 i = 0; i < PriceRows.Count; i++)
            {
                itemSum += PriceRows[i].ForeignPriceUpDown.Value;
            }

            shippingCost = ShippingCostUpDown.Value;

            endSum = CreditCardChargeUpDown.Value + CreditCardFeesUpDown.Value + CustomsFeesUpDown.Value + OtherCostsUpDown.Value;

            for (Int32 i = 0; i < PriceRows.Count; i++)
            {
                Decimal shippingCostShare;
                Decimal domesticPriceShare;
                Decimal domesticPrice;
                Decimal foreignPrice;
                Decimal divider;

                foreignPrice = PriceRows[i].ForeignPriceUpDown.Value;

                if (ShippinCostSplitComboBox.SelectedIndex == 0)
                {
                    shippingCostShare = shippingCost / PriceRows.Count;
                }
                else
                {
                    shippingCostShare = (foreignPrice / itemSum) * shippingCost;
                }

                divider = itemSum + shippingCost;

                if (divider > 0)
                {
                    domesticPriceShare = (foreignPrice + shippingCostShare) / divider;
                }
                else
                {
                    domesticPriceShare = 0;
                }

                domesticPrice = endSum * domesticPriceShare;

                GetFormattedValue(PriceRows[i].ShippingCostShareTextBox, shippingCostShare);
                GetFormattedValue(PriceRows[i].DomesticPriceTextBox, domesticPrice);
            }

            shippingCost = 0;
            endSum = 0;

            for (Int32 i = 0; i < PriceRows.Count; i++)
            {
                shippingCost += (Decimal)(PriceRows[i].ShippingCostShareTextBox.Tag);
                endSum += (Decimal)(PriceRows[i].DomesticPriceTextBox.Tag);
            }

            FinalPriceRow.ForeignPriceUpDown.Value = itemSum;
            GetFormattedValue(FinalPriceRow.ShippingCostShareTextBox, shippingCost);
            GetFormattedValue(FinalPriceRow.DomesticPriceTextBox, endSum);
        }

        private void GetFormattedValue(TextBox textBox, Decimal value)
        {
            String formattedValue;
            CultureInfo ci;

            ci = Application.CurrentCulture;
            formattedValue = value.ToString("F2", ci);
            textBox.Text = formattedValue;
            textBox.Tag = value;
        }

        private void AddRows()
        {
            Int32 itemCount;
            Int32 top;

            itemCount = (Int32)(NumberOfItemsUpDown.Value);

            ResetRows(itemCount);

            if ((PriceRows == null) || (PriceRows.Count == 0))
            {
                PriceRows = new List<ItemPriceRow>();
                top = 5;
            }
            else
            {
                top = PriceRows[PriceRows.Count - 1].Top + 30;
            }
            for (Int32 i = PriceRows.Count; i < itemCount; i++)
            {
                ItemPriceRow row;

                row = AddRow(top, i);
                PriceRows.Add(row);
                top += 30;
            }
        }

        private void ResetRows(Int32 itemCount)
        {
            if ((PriceRows != null) && (PriceRows.Count > 0))
            {
                for (Int32 i = PriceRows.Count - 1; i >= itemCount; i--)
                {
                    ItemPriceRow row;

                    row = PriceRows[i];
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

        private ItemPriceRow AddRow(Int32 top
            , Int32 index)
        {
            ItemPriceRow row;

            row = new ItemPriceRow();
            row.Top = top;
            row.Left = 6;
            row.Name = "ItemPriceRow" + index.ToString();
            row.TabIndex = index;
            RegisterRowEvents(row);
            ItemPanel.Controls.Add(row);

            return (row);
        }

        private void RegisterRowEvents(ItemPriceRow row)
        {
            row.ForeignPriceUpDown.Enter += OnUpDownEnter;
            row.ShippingCostShareTextBox.Enter += OnUpDownEnter;
            row.DomesticPriceTextBox.Enter += OnUpDownEnter;
        }

        private void OnUpDownEnter(Object sender, EventArgs e)
        {
            NumericUpDown upDown;

            upDown = sender as NumericUpDown;
            if (upDown != null)
            {
                upDown.Select(0, upDown.Text.Length);
            }
            else
            {
                TextBox textBox;

                textBox = sender as TextBox;
                if(textBox  != null)
                {
                    textBox.Select(0, textBox.Text.Length);
                }
            }
        }

        private void OnResetButtonClick(Object sender, EventArgs e)
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

                GetFormattedValue(FinalPriceRow.ShippingCostShareTextBox, 0);
                GetFormattedValue(FinalPriceRow.DomesticPriceTextBox, 0);
            }

            ResumeLayout(true);

            OnNumberOfItemsUpDownValueChanged(this, EventArgs.Empty);
        }

        private void OnNumberOfItemsUpDownValueChanged(Object sender, EventArgs e)
        {
            Int32 top;

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

            top = PriceRows[PriceRows.Count - 1].Top + 35;
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

        private void OnFormClosing(Object sender, FormClosingEventArgs e)
        {
            Program.Settings.DefaultValues.ShippingCostSplitAbsolute = (ShippinCostSplitComboBox.SelectedIndex == 0);
        }

        private void OnFormShown(Object sender, EventArgs e)
        {
            SetLabels();
        }

        private void OnUpDownLeave(Object sender, EventArgs e)
        {
            NumericUpDown upDown;

            upDown = (NumericUpDown)sender;
            if (String.IsNullOrEmpty(upDown.Text))
            {                
                upDown.Value = upDown.Minimum;
                upDown.Text = upDown.Value.ToString();
            }
        }
    }
}