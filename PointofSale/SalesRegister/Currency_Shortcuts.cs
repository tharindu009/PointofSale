﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PointofSale.SalesRegister
{
    public partial class Currency_Shortcuts : UserControl
    {
        public Currency_Shortcuts()
        {
            InitializeComponent();
        }


        public Delegate NumaricKeypad;

        #region Numaric Key pad
        private void btnNum1_Click(object sender, EventArgs e)
        {
            NumaricKeypad.DynamicInvoke("1");
        }

        private void btnNum2_Click(object sender, EventArgs e)
        {
            NumaricKeypad.DynamicInvoke("2");
        }

        private void btnNum3_Click(object sender, EventArgs e)
        {
            NumaricKeypad.DynamicInvoke("3");
        }

        private void btnNum4_Click(object sender, EventArgs e)
        {
            NumaricKeypad.DynamicInvoke("4");
        }

        private void btnNum5_Click(object sender, EventArgs e)
        {
            NumaricKeypad.DynamicInvoke("5");
        }

        private void btnNum6_Click(object sender, EventArgs e)
        {
            NumaricKeypad.DynamicInvoke("6");
        }

        private void btnNum7_Click(object sender, EventArgs e)
        {
            NumaricKeypad.DynamicInvoke("7");
        }

        private void btnNum8_Click(object sender, EventArgs e)
        {
            NumaricKeypad.DynamicInvoke("8");
        }

        private void btnNum9_Click(object sender, EventArgs e)
        {
            NumaricKeypad.DynamicInvoke("9");
        }

        private void btnDecimal_Click(object sender, EventArgs e)
        {
            NumaricKeypad.DynamicInvoke(".");
        }

        private void btnNum0_Click(object sender, EventArgs e)
        {
            NumaricKeypad.DynamicInvoke("0");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            //CoinandNotesFunctionPointer.DynamicInvoke("XX");
        }

        private void btnBackSpace_Click(object sender, EventArgs e)
        {
            //CoinandNotesFunctionPointer.DynamicInvoke("BXC");
        }

        #endregion
    }
}
