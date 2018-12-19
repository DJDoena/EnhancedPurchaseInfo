namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    partial class ItemPriceRow
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ForeignPriceUpDown = new System.Windows.Forms.NumericUpDown();
            this.ShippingCostShareTextBox = new System.Windows.Forms.TextBox();
            this.DomesticPriceTextBox = new System.Windows.Forms.TextBox();
            this.CreditCardChargeShareTextBox = new System.Windows.Forms.TextBox();
            this.CreditCardFeeShareTextBox = new System.Windows.Forms.TextBox();
            this.CustomsFeeShareTextBox = new System.Windows.Forms.TextBox();
            this.OtherCostShareTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.ForeignPriceUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // ForeignPriceUpDown
            // 
            this.ForeignPriceUpDown.DecimalPlaces = 2;
            this.ForeignPriceUpDown.Location = new System.Drawing.Point(2, 4);
            this.ForeignPriceUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.ForeignPriceUpDown.Name = "ForeignPriceUpDown";
            this.ForeignPriceUpDown.Size = new System.Drawing.Size(148, 20);
            this.ForeignPriceUpDown.TabIndex = 11;
            this.ForeignPriceUpDown.Leave += new System.EventHandler(this.OnUpDownLeave);
            // 
            // ShippingCostShareTextBox
            // 
            this.ShippingCostShareTextBox.Location = new System.Drawing.Point(352, 3);
            this.ShippingCostShareTextBox.Name = "ShippingCostShareTextBox";
            this.ShippingCostShareTextBox.ReadOnly = true;
            this.ShippingCostShareTextBox.Size = new System.Drawing.Size(148, 20);
            this.ShippingCostShareTextBox.TabIndex = 14;
            this.ShippingCostShareTextBox.TabStop = false;
            // 
            // DomesticPriceTextBox
            // 
            this.DomesticPriceTextBox.Location = new System.Drawing.Point(172, 3);
            this.DomesticPriceTextBox.Name = "DomesticPriceTextBox";
            this.DomesticPriceTextBox.ReadOnly = true;
            this.DomesticPriceTextBox.Size = new System.Drawing.Size(148, 20);
            this.DomesticPriceTextBox.TabIndex = 15;
            this.DomesticPriceTextBox.TabStop = false;
            // 
            // CreditCardChargeShareTextBox
            // 
            this.CreditCardChargeShareTextBox.Location = new System.Drawing.Point(532, 3);
            this.CreditCardChargeShareTextBox.Name = "CreditCardChargeShareTextBox";
            this.CreditCardChargeShareTextBox.ReadOnly = true;
            this.CreditCardChargeShareTextBox.Size = new System.Drawing.Size(148, 20);
            this.CreditCardChargeShareTextBox.TabIndex = 16;
            this.CreditCardChargeShareTextBox.TabStop = false;
            // 
            // CreditCardFeeShareTextBox
            // 
            this.CreditCardFeeShareTextBox.Location = new System.Drawing.Point(772, 3);
            this.CreditCardFeeShareTextBox.Name = "CreditCardFeeShareTextBox";
            this.CreditCardFeeShareTextBox.ReadOnly = true;
            this.CreditCardFeeShareTextBox.Size = new System.Drawing.Size(148, 20);
            this.CreditCardFeeShareTextBox.TabIndex = 17;
            this.CreditCardFeeShareTextBox.TabStop = false;
            // 
            // CustomsFeeShareTextBox
            // 
            this.CustomsFeeShareTextBox.Location = new System.Drawing.Point(1002, 5);
            this.CustomsFeeShareTextBox.Name = "CustomsFeeShareTextBox";
            this.CustomsFeeShareTextBox.ReadOnly = true;
            this.CustomsFeeShareTextBox.Size = new System.Drawing.Size(148, 20);
            this.CustomsFeeShareTextBox.TabIndex = 18;
            this.CustomsFeeShareTextBox.TabStop = false;
            // 
            // OtherCostShareTextBox
            // 
            this.OtherCostShareTextBox.Location = new System.Drawing.Point(1202, 5);
            this.OtherCostShareTextBox.Name = "OtherCostShareTextBox";
            this.OtherCostShareTextBox.ReadOnly = true;
            this.OtherCostShareTextBox.Size = new System.Drawing.Size(148, 20);
            this.OtherCostShareTextBox.TabIndex = 19;
            this.OtherCostShareTextBox.TabStop = false;
            // 
            // ItemPriceRow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.OtherCostShareTextBox);
            this.Controls.Add(this.CustomsFeeShareTextBox);
            this.Controls.Add(this.CreditCardFeeShareTextBox);
            this.Controls.Add(this.CreditCardChargeShareTextBox);
            this.Controls.Add(this.DomesticPriceTextBox);
            this.Controls.Add(this.ShippingCostShareTextBox);
            this.Controls.Add(this.ForeignPriceUpDown);
            this.Name = "ItemPriceRow";
            this.Size = new System.Drawing.Size(1370, 30);
            ((System.ComponentModel.ISupportInitialize)(this.ForeignPriceUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.NumericUpDown ForeignPriceUpDown;
        public System.Windows.Forms.TextBox ShippingCostShareTextBox;
        public System.Windows.Forms.TextBox DomesticPriceTextBox;
        public System.Windows.Forms.TextBox CreditCardChargeShareTextBox;
        public System.Windows.Forms.TextBox CreditCardFeeShareTextBox;
        public System.Windows.Forms.TextBox CustomsFeeShareTextBox;
        public System.Windows.Forms.TextBox OtherCostShareTextBox;
    }
}
