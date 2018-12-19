namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    partial class ItemPricesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemPricesForm));
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.BasicsTabPage = new System.Windows.Forms.TabPage();
            this.ResetButton = new System.Windows.Forms.Button();
            this.OtherCostsLabel = new System.Windows.Forms.Label();
            this.CustomsFeesLabel = new System.Windows.Forms.Label();
            this.CreditCardFeesLabel = new System.Windows.Forms.Label();
            this.CreditCardChargeLabel = new System.Windows.Forms.Label();
            this.ShippingCostLabel = new System.Windows.Forms.Label();
            this.ShippinCostSplitLabel = new System.Windows.Forms.Label();
            this.NumberOfItemsLabel = new System.Windows.Forms.Label();
            this.OtherCostsUpDown = new System.Windows.Forms.NumericUpDown();
            this.CustomsFeesUpDown = new System.Windows.Forms.NumericUpDown();
            this.CreditCardFeesUpDown = new System.Windows.Forms.NumericUpDown();
            this.CreditCardChargeUpDown = new System.Windows.Forms.NumericUpDown();
            this.ShippingCostUpDown = new System.Windows.Forms.NumericUpDown();
            this.ShippinCostSplitComboBox = new System.Windows.Forms.ComboBox();
            this.NumberOfItemsUpDown = new System.Windows.Forms.NumericUpDown();
            this.ItemsTabPage = new System.Windows.Forms.TabPage();
            this.CreditCardFeeShareLabel = new System.Windows.Forms.Label();
            this.CreditCardChargeShareLabel = new System.Windows.Forms.Label();
            this.DomesticItemPriceLabel = new System.Windows.Forms.Label();
            this.ShippingCostShareLabel = new System.Windows.Forms.Label();
            this.ForeignItemPriceLabel = new System.Windows.Forms.Label();
            this.ItemPanel = new System.Windows.Forms.Panel();
            this.CustomsFeeShareLabel = new System.Windows.Forms.Label();
            this.OtherCostShareLabel = new System.Windows.Forms.Label();
            this.MainTabControl.SuspendLayout();
            this.BasicsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OtherCostsUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CustomsFeesUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CreditCardFeesUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CreditCardChargeUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ShippingCostUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumberOfItemsUpDown)).BeginInit();
            this.ItemsTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTabControl
            // 
            this.MainTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainTabControl.Controls.Add(this.BasicsTabPage);
            this.MainTabControl.Controls.Add(this.ItemsTabPage);
            this.MainTabControl.Location = new System.Drawing.Point(12, 12);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(1540, 288);
            this.MainTabControl.TabIndex = 0;
            // 
            // BasicsTabPage
            // 
            this.BasicsTabPage.Controls.Add(this.ResetButton);
            this.BasicsTabPage.Controls.Add(this.OtherCostsLabel);
            this.BasicsTabPage.Controls.Add(this.CustomsFeesLabel);
            this.BasicsTabPage.Controls.Add(this.CreditCardFeesLabel);
            this.BasicsTabPage.Controls.Add(this.CreditCardChargeLabel);
            this.BasicsTabPage.Controls.Add(this.ShippingCostLabel);
            this.BasicsTabPage.Controls.Add(this.ShippinCostSplitLabel);
            this.BasicsTabPage.Controls.Add(this.NumberOfItemsLabel);
            this.BasicsTabPage.Controls.Add(this.OtherCostsUpDown);
            this.BasicsTabPage.Controls.Add(this.CustomsFeesUpDown);
            this.BasicsTabPage.Controls.Add(this.CreditCardFeesUpDown);
            this.BasicsTabPage.Controls.Add(this.CreditCardChargeUpDown);
            this.BasicsTabPage.Controls.Add(this.ShippingCostUpDown);
            this.BasicsTabPage.Controls.Add(this.ShippinCostSplitComboBox);
            this.BasicsTabPage.Controls.Add(this.NumberOfItemsUpDown);
            this.BasicsTabPage.Location = new System.Drawing.Point(4, 22);
            this.BasicsTabPage.Name = "BasicsTabPage";
            this.BasicsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.BasicsTabPage.Size = new System.Drawing.Size(1152, 262);
            this.BasicsTabPage.TabIndex = 0;
            this.BasicsTabPage.Text = "Basics";
            this.BasicsTabPage.UseVisualStyleBackColor = true;
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(245, 189);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(171, 23);
            this.ResetButton.TabIndex = 14;
            this.ResetButton.Text = "Reset All";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.OnResetButtonClick);
            // 
            // OtherCostsLabel
            // 
            this.OtherCostsLabel.AutoSize = true;
            this.OtherCostsLabel.Location = new System.Drawing.Point(6, 165);
            this.OtherCostsLabel.Name = "OtherCostsLabel";
            this.OtherCostsLabel.Size = new System.Drawing.Size(168, 13);
            this.OtherCostsLabel.TabIndex = 12;
            this.OtherCostsLabel.Text = "Other Costs in Domestic Currency:";
            // 
            // CustomsFeesLabel
            // 
            this.CustomsFeesLabel.AutoSize = true;
            this.CustomsFeesLabel.Location = new System.Drawing.Point(6, 139);
            this.CustomsFeesLabel.Name = "CustomsFeesLabel";
            this.CustomsFeesLabel.Size = new System.Drawing.Size(179, 13);
            this.CustomsFeesLabel.TabIndex = 10;
            this.CustomsFeesLabel.Text = "Customs Fees in Domestic Currency:";
            // 
            // CreditCardFeesLabel
            // 
            this.CreditCardFeesLabel.AutoSize = true;
            this.CreditCardFeesLabel.Location = new System.Drawing.Point(6, 113);
            this.CreditCardFeesLabel.Name = "CreditCardFeesLabel";
            this.CreditCardFeesLabel.Size = new System.Drawing.Size(191, 13);
            this.CreditCardFeesLabel.TabIndex = 8;
            this.CreditCardFeesLabel.Text = "Credit Card Fees in Domestic Currency:";
            // 
            // CreditCardChargeLabel
            // 
            this.CreditCardChargeLabel.AutoSize = true;
            this.CreditCardChargeLabel.Location = new System.Drawing.Point(6, 87);
            this.CreditCardChargeLabel.Name = "CreditCardChargeLabel";
            this.CreditCardChargeLabel.Size = new System.Drawing.Size(202, 13);
            this.CreditCardChargeLabel.TabIndex = 6;
            this.CreditCardChargeLabel.Text = "Credit Card Charge in Domestic Currency:";
            // 
            // ShippingCostLabel
            // 
            this.ShippingCostLabel.AutoSize = true;
            this.ShippingCostLabel.Location = new System.Drawing.Point(6, 61);
            this.ShippingCostLabel.Name = "ShippingCostLabel";
            this.ShippingCostLabel.Size = new System.Drawing.Size(169, 13);
            this.ShippingCostLabel.TabIndex = 4;
            this.ShippingCostLabel.Text = "Shipping Cost in Foreign Currency:";
            // 
            // ShippinCostSplitLabel
            // 
            this.ShippinCostSplitLabel.AutoSize = true;
            this.ShippinCostSplitLabel.Location = new System.Drawing.Point(6, 35);
            this.ShippinCostSplitLabel.Name = "ShippinCostSplitLabel";
            this.ShippinCostSplitLabel.Size = new System.Drawing.Size(69, 13);
            this.ShippinCostSplitLabel.TabIndex = 2;
            this.ShippinCostSplitLabel.Text = "Type of Split:";
            // 
            // NumberOfItemsLabel
            // 
            this.NumberOfItemsLabel.AutoSize = true;
            this.NumberOfItemsLabel.Location = new System.Drawing.Point(6, 8);
            this.NumberOfItemsLabel.Name = "NumberOfItemsLabel";
            this.NumberOfItemsLabel.Size = new System.Drawing.Size(87, 13);
            this.NumberOfItemsLabel.TabIndex = 0;
            this.NumberOfItemsLabel.Text = "Number of Items:";
            // 
            // OtherCostsUpDown
            // 
            this.OtherCostsUpDown.DecimalPlaces = 2;
            this.OtherCostsUpDown.Location = new System.Drawing.Point(245, 163);
            this.OtherCostsUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.OtherCostsUpDown.Name = "OtherCostsUpDown";
            this.OtherCostsUpDown.Size = new System.Drawing.Size(171, 20);
            this.OtherCostsUpDown.TabIndex = 13;
            this.OtherCostsUpDown.Enter += new System.EventHandler(this.OnUpDownEnter);
            this.OtherCostsUpDown.Leave += new System.EventHandler(this.OnUpDownLeave);
            // 
            // CustomsFeesUpDown
            // 
            this.CustomsFeesUpDown.DecimalPlaces = 2;
            this.CustomsFeesUpDown.Location = new System.Drawing.Point(245, 137);
            this.CustomsFeesUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.CustomsFeesUpDown.Name = "CustomsFeesUpDown";
            this.CustomsFeesUpDown.Size = new System.Drawing.Size(171, 20);
            this.CustomsFeesUpDown.TabIndex = 11;
            this.CustomsFeesUpDown.Enter += new System.EventHandler(this.OnUpDownEnter);
            this.CustomsFeesUpDown.Leave += new System.EventHandler(this.OnUpDownLeave);
            // 
            // CreditCardFeesUpDown
            // 
            this.CreditCardFeesUpDown.DecimalPlaces = 2;
            this.CreditCardFeesUpDown.Location = new System.Drawing.Point(245, 111);
            this.CreditCardFeesUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.CreditCardFeesUpDown.Name = "CreditCardFeesUpDown";
            this.CreditCardFeesUpDown.Size = new System.Drawing.Size(171, 20);
            this.CreditCardFeesUpDown.TabIndex = 9;
            this.CreditCardFeesUpDown.Enter += new System.EventHandler(this.OnUpDownEnter);
            this.CreditCardFeesUpDown.Leave += new System.EventHandler(this.OnUpDownLeave);
            // 
            // CreditCardChargeUpDown
            // 
            this.CreditCardChargeUpDown.DecimalPlaces = 2;
            this.CreditCardChargeUpDown.Location = new System.Drawing.Point(245, 85);
            this.CreditCardChargeUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.CreditCardChargeUpDown.Name = "CreditCardChargeUpDown";
            this.CreditCardChargeUpDown.Size = new System.Drawing.Size(171, 20);
            this.CreditCardChargeUpDown.TabIndex = 7;
            this.CreditCardChargeUpDown.Enter += new System.EventHandler(this.OnUpDownEnter);
            this.CreditCardChargeUpDown.Leave += new System.EventHandler(this.OnUpDownLeave);
            // 
            // ShippingCostUpDown
            // 
            this.ShippingCostUpDown.DecimalPlaces = 2;
            this.ShippingCostUpDown.Location = new System.Drawing.Point(245, 59);
            this.ShippingCostUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.ShippingCostUpDown.Name = "ShippingCostUpDown";
            this.ShippingCostUpDown.Size = new System.Drawing.Size(171, 20);
            this.ShippingCostUpDown.TabIndex = 5;
            this.ShippingCostUpDown.Enter += new System.EventHandler(this.OnUpDownEnter);
            this.ShippingCostUpDown.Leave += new System.EventHandler(this.OnUpDownLeave);
            // 
            // ShippinCostSplitComboBox
            // 
            this.ShippinCostSplitComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ShippinCostSplitComboBox.FormattingEnabled = true;
            this.ShippinCostSplitComboBox.Location = new System.Drawing.Point(245, 32);
            this.ShippinCostSplitComboBox.Name = "ShippinCostSplitComboBox";
            this.ShippinCostSplitComboBox.Size = new System.Drawing.Size(171, 21);
            this.ShippinCostSplitComboBox.TabIndex = 3;
            // 
            // NumberOfItemsUpDown
            // 
            this.NumberOfItemsUpDown.Location = new System.Drawing.Point(245, 6);
            this.NumberOfItemsUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumberOfItemsUpDown.Name = "NumberOfItemsUpDown";
            this.NumberOfItemsUpDown.Size = new System.Drawing.Size(171, 20);
            this.NumberOfItemsUpDown.TabIndex = 1;
            this.NumberOfItemsUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.NumberOfItemsUpDown.ValueChanged += new System.EventHandler(this.OnNumberOfItemsUpDownValueChanged);
            this.NumberOfItemsUpDown.Enter += new System.EventHandler(this.OnUpDownEnter);
            this.NumberOfItemsUpDown.Leave += new System.EventHandler(this.OnUpDownLeave);
            // 
            // ItemsTabPage
            // 
            this.ItemsTabPage.Controls.Add(this.OtherCostShareLabel);
            this.ItemsTabPage.Controls.Add(this.CustomsFeeShareLabel);
            this.ItemsTabPage.Controls.Add(this.CreditCardFeeShareLabel);
            this.ItemsTabPage.Controls.Add(this.CreditCardChargeShareLabel);
            this.ItemsTabPage.Controls.Add(this.DomesticItemPriceLabel);
            this.ItemsTabPage.Controls.Add(this.ShippingCostShareLabel);
            this.ItemsTabPage.Controls.Add(this.ForeignItemPriceLabel);
            this.ItemsTabPage.Controls.Add(this.ItemPanel);
            this.ItemsTabPage.Location = new System.Drawing.Point(4, 22);
            this.ItemsTabPage.Name = "ItemsTabPage";
            this.ItemsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ItemsTabPage.Size = new System.Drawing.Size(1532, 262);
            this.ItemsTabPage.TabIndex = 1;
            this.ItemsTabPage.Text = "Items";
            this.ItemsTabPage.UseVisualStyleBackColor = true;
            // 
            // CreditCardFeeShareLabel
            // 
            this.CreditCardFeeShareLabel.AutoSize = true;
            this.CreditCardFeeShareLabel.Location = new System.Drawing.Point(780, 6);
            this.CreditCardFeeShareLabel.Name = "CreditCardFeeShareLabel";
            this.CreditCardFeeShareLabel.Size = new System.Drawing.Size(186, 13);
            this.CreditCardFeeShareLabel.TabIndex = 8;
            this.CreditCardFeeShareLabel.Text = "Credit Card Fee in Domestic Currency:";
            // 
            // CreditCardChargeShareLabel
            // 
            this.CreditCardChargeShareLabel.AutoSize = true;
            this.CreditCardChargeShareLabel.Location = new System.Drawing.Point(540, 6);
            this.CreditCardChargeShareLabel.Name = "CreditCardChargeShareLabel";
            this.CreditCardChargeShareLabel.Size = new System.Drawing.Size(202, 13);
            this.CreditCardChargeShareLabel.TabIndex = 7;
            this.CreditCardChargeShareLabel.Text = "Credit Card Charge in Domestic Currency:";
            // 
            // DomesticItemPriceLabel
            // 
            this.DomesticItemPriceLabel.AutoSize = true;
            this.DomesticItemPriceLabel.Location = new System.Drawing.Point(180, 6);
            this.DomesticItemPriceLabel.Name = "DomesticItemPriceLabel";
            this.DomesticItemPriceLabel.Size = new System.Drawing.Size(160, 13);
            this.DomesticItemPriceLabel.TabIndex = 6;
            this.DomesticItemPriceLabel.Text = "Item Price in Domestic Currency:";
            // 
            // ShippingCostShareLabel
            // 
            this.ShippingCostShareLabel.AutoSize = true;
            this.ShippingCostShareLabel.Location = new System.Drawing.Point(360, 6);
            this.ShippingCostShareLabel.Name = "ShippingCostShareLabel";
            this.ShippingCostShareLabel.Size = new System.Drawing.Size(169, 13);
            this.ShippingCostShareLabel.TabIndex = 5;
            this.ShippingCostShareLabel.Text = "Shipping Cost in Foreign Currency:";
            // 
            // ForeignItemPriceLabel
            // 
            this.ForeignItemPriceLabel.AutoSize = true;
            this.ForeignItemPriceLabel.Location = new System.Drawing.Point(10, 6);
            this.ForeignItemPriceLabel.Name = "ForeignItemPriceLabel";
            this.ForeignItemPriceLabel.Size = new System.Drawing.Size(151, 13);
            this.ForeignItemPriceLabel.TabIndex = 4;
            this.ForeignItemPriceLabel.Text = "Item Price in Foreign Currency:";
            // 
            // ItemPanel
            // 
            this.ItemPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ItemPanel.AutoScroll = true;
            this.ItemPanel.Location = new System.Drawing.Point(6, 22);
            this.ItemPanel.Name = "ItemPanel";
            this.ItemPanel.Size = new System.Drawing.Size(1520, 234);
            this.ItemPanel.TabIndex = 0;
            // 
            // CustomsFeeShareLabel
            // 
            this.CustomsFeeShareLabel.AutoSize = true;
            this.CustomsFeeShareLabel.Location = new System.Drawing.Point(1000, 6);
            this.CustomsFeeShareLabel.Name = "CustomsFeeShareLabel";
            this.CustomsFeeShareLabel.Size = new System.Drawing.Size(174, 13);
            this.CustomsFeeShareLabel.TabIndex = 9;
            this.CustomsFeeShareLabel.Text = "Customs Fee in Domestic Currency:";
            // 
            // OtherCostShareLabel
            // 
            this.OtherCostShareLabel.AutoSize = true;
            this.OtherCostShareLabel.Location = new System.Drawing.Point(1200, 6);
            this.OtherCostShareLabel.Name = "OtherCostShareLabel";
            this.OtherCostShareLabel.Size = new System.Drawing.Size(163, 13);
            this.OtherCostShareLabel.TabIndex = 10;
            this.OtherCostShareLabel.Text = "Other Cost in Domestic Currency:";
            // 
            // ItemPricesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1584, 311);
            this.Controls.Add(this.MainTabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1920, 1230);
            this.MinimumSize = new System.Drawing.Size(1280, 300);
            this.Name = "ItemPricesForm";
            this.Text = "ItemPricesBasicsForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Shown += new System.EventHandler(this.OnFormShown);
            this.MainTabControl.ResumeLayout(false);
            this.BasicsTabPage.ResumeLayout(false);
            this.BasicsTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OtherCostsUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CustomsFeesUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CreditCardFeesUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CreditCardChargeUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ShippingCostUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumberOfItemsUpDown)).EndInit();
            this.ItemsTabPage.ResumeLayout(false);
            this.ItemsTabPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage BasicsTabPage;
        private System.Windows.Forms.TabPage ItemsTabPage;
        private System.Windows.Forms.Label OtherCostsLabel;
        private System.Windows.Forms.Label CustomsFeesLabel;
        private System.Windows.Forms.Label CreditCardFeesLabel;
        private System.Windows.Forms.Label CreditCardChargeLabel;
        private System.Windows.Forms.Label ShippingCostLabel;
        private System.Windows.Forms.Label ShippinCostSplitLabel;
        private System.Windows.Forms.Label NumberOfItemsLabel;
        private System.Windows.Forms.NumericUpDown OtherCostsUpDown;
        private System.Windows.Forms.NumericUpDown CustomsFeesUpDown;
        private System.Windows.Forms.NumericUpDown CreditCardFeesUpDown;
        private System.Windows.Forms.NumericUpDown CreditCardChargeUpDown;
        private System.Windows.Forms.NumericUpDown ShippingCostUpDown;
        private System.Windows.Forms.ComboBox ShippinCostSplitComboBox;
        private System.Windows.Forms.NumericUpDown NumberOfItemsUpDown;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.Label CreditCardFeeShareLabel;
        private System.Windows.Forms.Label CreditCardChargeShareLabel;
        private System.Windows.Forms.Label DomesticItemPriceLabel;
        private System.Windows.Forms.Label ShippingCostShareLabel;
        private System.Windows.Forms.Label ForeignItemPriceLabel;
        private System.Windows.Forms.Panel ItemPanel;
        private System.Windows.Forms.Label OtherCostShareLabel;
        private System.Windows.Forms.Label CustomsFeeShareLabel;
    }
}