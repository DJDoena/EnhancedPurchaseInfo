using System;
using System.Windows.Forms;

namespace DoenaSoft.DVDProfiler.EnhancedPurchaseInfo
{
    public partial class ItemPriceRow : UserControl
    {
        public ItemPriceRow()
        {
            InitializeComponent();
        }

        private void OnUpDownLeave(Object sender, System.EventArgs e)
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