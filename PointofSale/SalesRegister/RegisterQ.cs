﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using PointofSale.BAL;
using PointofSale.SalesRegister;

namespace PointofSale
{
    public partial class RegisterQ : KryptonForm
    {

        string CurrentJobNo = "0";

        public RegisterQ()
        {
            InitializeComponent();
            txtBarcodeReaderBox.Focus();
        }

        public RegisterQ(string JobNo)
        {
            InitializeComponent();
            lbluser.Text = UserInfo.UserName;
            txtBarcodeReaderBox.Focus();

            CurrentJobNo = JobNo;

        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        

        //Show Products image
        public void ItemList_with_images(string value)
        {
            flowLayoutPanelItemList.Controls.Clear();
            //string img_directory = Application.StartupPath + @"\ITEMIMAGE\";
            //string[] files = Directory.GetFiles(img_directory, "*.png *.jpg");
            try
            {
                string sql = " select  *  from  purchase  where  ( product_name like '%" + value + "%' and product_quantity >= 1) " +
                " OR ( product_id like '" + value + "%'  and product_quantity >= 1) " +
                " OR (category like '%" + value + "%' and   product_quantity >= 1)  ";
                //" ORDER BY RANDOM() LIMIT 12 "; // Sqlite  //View vw_itemdisplay_sr   purchase
                // " ORDER BY RAND() LIMIT 12 "; // MySQL
                //  " ORDER BY NEWID() "; // SQL server and use -- top 12 after select  

                DAL.DataAccessManager.ExecuteSQL(sql);
                DataTable dt = DAL.DataAccessManager.GetDataTable(sql);

                int currentImage = 0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dataReader = dt.Rows[i];

                    Button b = new Button();
                    //Image i = Image.FromFile(img_directory + dataReader["name"]);
                    b.Tag = dataReader["product_id"];
                    b.Click += new EventHandler(b_Click);

                    string taxapply;
                    if (dataReader["taxapply"].ToString() == "1")
                    {
                        taxapply = "YES";
                    }
                    else
                    {
                        taxapply = "NO";
                    }

                    string details = dataReader["product_id"] +
                     "\n Name: " + dataReader["product_name"].ToString() +
                     //   "\n Buy price: " + dataReader["cost_price"].ToString() +
                     "\n Stock Qty: " + dataReader["product_quantity"].ToString() +
                     "\n Retail price: " + dataReader["retail_price"].ToString() +
                     "\n Discount: " + dataReader["discount"].ToString() +
                     "\n Category: " + dataReader["category"].ToString() +
                     "\n Supplier: " + dataReader["supplier"].ToString() +
                     //  "\n Branch: "  + dataReader["Shopid"].ToString() +
                     "\n Tax Apply: " + taxapply;
                    b.Name = details;
                    toolTip2.ToolTipTitle = "Item Details";  // If you want to Show tooltip please uncomment
                    toolTip2.SetToolTip(b, details);          //Umncomment


                    //byte[] MyData = new byte[0];
                    //MyData = (byte[])dataReader["imagename"];
                    //MemoryStream stream = new MemoryStream(MyData);

                    ImageList il = new ImageList();
                    il.ColorDepth = ColorDepth.Depth32Bit;
                    il.TransparentColor = Color.Transparent;
                    il.ImageSize = new Size(55, 45);
                    //il.Images.Add(Image.FromFile(img_directory + dataReader["imagename"]));
                    il.Images.Add(PointofSale.Properties.Resources.product);

                    b.Image = il.Images[0];
                    b.Margin = new Padding(3, 3, 3, 3);

                    b.Size = new Size(208, 100);
                    b.Text.PadRight(4);

                    //b.Text += " " + dataReader["product_id"] + "\n ";
                    b.Text += dataReader["product_name"].ToString();
                    //b.Text += "\n Buy: " + dataReader["cost_price"];
                    b.Text += "\n Stock: " + dataReader["product_quantity"];
                    b.Text += "\n R.Price: " + dataReader["retail_price"];
                    //b.Text += "\n Dis: " + dataReader["discount"] + "% ";   //"Tax: " + taxapply;

                    b.Font = new Font("Poppins", 9, FontStyle.Regular, GraphicsUnit.Point);
                    b.TextAlign = ContentAlignment.TopLeft;
                    b.TextImageRelation = TextImageRelation.ImageAboveText;
                    //  b.FlatStyle = FlatStyle.Flat;
                    b.FlatAppearance.BorderSize = 0;
                    flowLayoutPanelItemList.Controls.Add(b);
                    currentImage++;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //throw;
            }
        }

        //Click add to cart
        protected void b_Click(object sender, EventArgs e)
        {
            Button b = sender as Button;
            string s;
            s = " ID: ";
            s += b.Tag;
            s += "\n Name: ";
            s += b.Name.ToString();

            txtBarcodeReaderBox.Text = b.Tag.ToString();

        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            
        }

        private void btnInvoice_Click(object sender, EventArgs e)
        {
           
        }

        private void btnFinished_Click(object sender, EventArgs e)
        {
           
        }

        private void btnSuspend_Click(object sender, EventArgs e)
        {

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
           
        }

        private void txtBarcodeReaderBox_TextChanged(object sender, EventArgs e)
        {
            if (txtBarcodeReaderBox.Text == "")
            {
                //  MessageBox.Show("Please Insert Product id : ");
                //textBox1.Focus();
                return;
            }
            else
            {
                try
                {
                    dgrvSalesItemList.Visible = true;
                    // Default tax rate 
                    double Taxrate = Convert.ToDouble(BAL.vatdisvalue.vat);

                    //- new in 8.1 version // Default Product QTY is 1
                    string sql = "SELECT  product_name as Name , retail_price as Price , 1.00  as QTY, (retail_price * 1.00 ) * 1.00  as 'Total' ,  " +
                            " (((retail_price * 1.00 ) * discount) / 100.00) as 'dis amt' , " +
                            " CASE     " +
                            " WHEN taxapply = 1 THEN   (((retail_price * 1.00 )  - (((retail_price * 1.00 ) * discount) / 100.00))  * " + Taxrate + " ) / 100.00   " +
                            " ELSE '0.00'  " +
                            " END 'taxamt' , product_id as ID , discount , taxapply, status, product_quantity  " +
                            " FROM  purchase  where product_id = '" + txtBarcodeReaderBox.Text + "'  and product_quantity >= 1 ";
                    DAL.DataAccessManager.ExecuteSQL(sql);
                    DataTable dt = DAL.DataAccessManager.GetDataTable(sql);

                    string ItemsName = dt.Rows[0].ItemArray[0].ToString();
                    double Rprice = Convert.ToDouble(dt.Rows[0].ItemArray[1].ToString());
                    double Qty = Convert.ToDouble(dt.Rows[0].ItemArray[2].ToString());
                    double Total = Convert.ToDouble(dt.Rows[0].ItemArray[3].ToString()) * Qty;
                    string Itemid = dt.Rows[0].ItemArray[6].ToString();
                    double Disamt = Convert.ToDouble(dt.Rows[0].ItemArray[4].ToString());       //  Total Discount amount of this item
                    double Taxamt = Convert.ToDouble(dt.Rows[0].ItemArray[5].ToString());       //  Total Tax amount  of this item
                    double Dis = Convert.ToDouble(dt.Rows[0].ItemArray[7].ToString());       //  Discount Rate
                    double Taxapply = Convert.ToDouble(dt.Rows[0].ItemArray[8].ToString());       //  VAT/TAX/TPS/TVQ apply or not
                    int kitchendisplay = Convert.ToInt32(dt.Rows[0].ItemArray[9].ToString());        //  kitchen display 3= show 1= not display in kitchen 
                    double Stockqty = Convert.ToDouble(dt.Rows[0].ItemArray[10].ToString());        // 

                    //Add to Item list
                    // long i = 1;
                    int n = Finditem(ItemsName);
                    if (n == -1)  //If new item
                    {
                        dgrvSalesItemList.Rows.Add(ItemsName, Rprice, Qty, Rprice, Itemid, Disamt, Taxamt, Dis, Taxapply, kitchendisplay);
                    }
                    else  // if same item Quantity increase by 1 
                    {
                        //// if given Qty > stock qty { Stcok exceed from stock  }                      
                        if (Convert.ToDouble(dgrvSalesItemList.Rows[n].Cells[2].Value) >= Stockqty)
                        {
                            MessageBox.Show("Quantity Exceed from Stcok Qty");
                        }
                        else
                        {
                            //  dgrvSalesItemList.Rows[n].Cells[0].Value = ItemsName;
                            // dgrvSalesItemList.Rows[n].Cells[1].Value = Rprice;
                            int QtyInc = Convert.ToInt32(dgrvSalesItemList.Rows[n].Cells[2].Value);
                            dgrvSalesItemList.Rows[n].Cells[2].Value = (QtyInc + 1);  //Qty Increase
                            dgrvSalesItemList.Rows[n].Cells[3].Value = Rprice * (QtyInc + 1);   // Total price
                            //  dgrvSalesItemList.Rows[n].Cells[4].Value = Itemid;                     

                            double qty = Convert.ToDouble(dgrvSalesItemList.Rows[n].Cells[2].Value);
                            double disrate = Convert.ToDouble(dgrvSalesItemList.Rows[n].Cells[7].Value);

                            if (disrate != 0)  // if discount has
                            {
                                double DisamtInc = (((Rprice * qty) * disrate) / 100.00);      // Total Discount amount of this item
                                dgrvSalesItemList.Rows[n].Cells[5].Value = DisamtInc;
                            }

                            if (Taxapply != 0)   // If apply  tax 
                            {
                                // Total Tax amount  of this item  (Rprice - disamount) * taxRate / 100
                                double TaxamtInc = ((((Rprice * qty) - (((Rprice * qty) * disrate) / 100.00)) * Taxrate) / 100.00);
                                dgrvSalesItemList.Rows[n].Cells[6].Value = TaxamtInc;
                            }

                            // dgrvSalesItemList.Rows[n].Cells[7].Value = Dis; // Discount rate
                            //  dgrvSalesItemList.Rows[n].Cells[8].Value = Taxapply;  //Tax apply
                            //  dgrvSalesItemList.Rows[n].Cells[9].Value = kitchendisplay;
                        }


                    }


                    //Hide fields
                    dgrvSalesItemList.Columns[4].Visible = false; // ID             // new in 8.1 version
                    dgrvSalesItemList.Columns[5].Visible = false; // Disamt         // new in 8.1 version
                    dgrvSalesItemList.Columns[6].Visible = false; // taxamt         // new in 8.1 version
                    dgrvSalesItemList.Columns[7].Visible = false; // Discount rate  // new in 8.1 version
                    dgrvSalesItemList.Columns[9].Visible = false; // kitdisplay    // new in 8.3.1 version



                    btnSuspend.Enabled = true;
                    btnPayment.Enabled = true;
                    btnCompleteSalesAndPrint.Enabled = true;
                    btnSaveOnly.Enabled = true;
                    btnPrintDirect.Enabled = true; // complete sale and direct print

                    DiscountCalculation();
                    vatcal();
                    txtDiscountRate.Text = "0";

                    txtBarcodeReaderBox.Text = "";
                    txtBarcodeReaderBox.Focus();
                    // lbloveralldiscount.Text = "0";

                    if (dt.Rows.Count > 0)
                    {
                        lblNotFound.Visible = false;
                    }

                    else
                    {
                        lblNotFound.Visible = true;
                    }
                }

                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    //MessageBox.Show("sorry");
                    return;
                }
            }
        }

        // Check duplicate item 
        public int Finditem(string item)
        {
            int k = -1;
            if (dgrvSalesItemList.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgrvSalesItemList.Rows)
                {
                    if (row.Cells[0].Value.ToString().Equals(item))
                    {
                        k = row.Index;
                        break;
                    }
                }
            }
            return k;
        }

        // Discount Calculation - Change in 8.1 version
        public void DiscountCalculation()
        {
            // // subtotal without dis vat sum 
            double totalsum = 0.00;
            for (int i = 0; i < dgrvSalesItemList.Rows.Count; ++i)
            {
                totalsum += Convert.ToDouble(dgrvSalesItemList.Rows[i].Cells[3].Value);
            }
            lblTotal.Text = Math.Round(totalsum, 2).ToString();
            lblTotalItems.Text = dgrvSalesItemList.RowCount.ToString();

            ////    Discount amount sum
            double total = Convert.ToDouble(totalsum.ToString());
            double DisCount = 0.00;
            for (int i = 0; i < dgrvSalesItemList.Rows.Count; ++i)
            {
                DisCount += Convert.ToDouble(dgrvSalesItemList.Rows[i].Cells[5].Value);
            }

            DisCount = Math.Round(DisCount, 2);
            double sum = total - DisCount;
            sum = Math.Round(sum, 2);
            lblsubtotal.Text = sum.ToString();

            double payable = sum + Convert.ToDouble(lblTotalVAT.Text);
            payable = Math.Round(payable, 2);
            lblTotalPayable.Text = payable.ToString();

            lblTotalDisCount.Text = DisCount.ToString();
            lbloveralldiscount.Text = DisCount.ToString();
            // btnPayment.Text = "Pay = " + payable.ToString();

            //tabPageSR_Counter.Text = "Terminal (" + dgrvSalesItemList.RowCount.ToString() + ")";
            //  tabPageSR_Payment.Text = "Payment (" + payable.ToString() + ")";
        }

        //VAT amount sum calculation - Change in 8.1 version
        public void vatcal()
        {
            //Subtotal = total - (discount + Globaldiscount)
            double Subtotal = Convert.ToDouble(lblsubtotal.Text);
            //double Subtotal = Convert.ToDouble(lbloveralldiscount.Text)  ;

            //VAT amount
            double VAT = 0.00;
            for (int i = 0; i < dgrvSalesItemList.Rows.Count; ++i)
            {
                VAT += Convert.ToDouble(dgrvSalesItemList.Rows[i].Cells[6].Value);
            }

            VAT = Math.Round(VAT, 2);
            lblTotalVAT.Text = VAT.ToString();

            double payable = Subtotal + VAT;
            payable = Math.Round(payable, 2);
            lblTotalPayable.Text = payable.ToString();

            // btnPayment.Text = "Pay = " + payable.ToString();

            ///////Pole shows Price value  | if you have pole device please UnComment   below code
            //SerialPort sp = new SerialPort();
            //sp.PortName = "COM1";  ////Insert your pole Device Port Name E.g. COM4  -- you can find  from pole device manual  
            //sp.BaudRate = 9600;     // Pole Bound Rate 
            //sp.Parity = Parity.None;
            //sp.DataBits = 8;   // Data Bits
            //sp.StopBits = StopBits.One;
            //sp.Open();                 
            //sp.WriteLine(lblTotalPayable.Text);

            //sp.Close();
            //sp.Dispose();
            //sp = null;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }

        private void btnCompleteSalesAndPrint_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to Complete this transaction?", "Yes or No", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (txtPaidAmount.Text == "00" || txtPaidAmount.Text == "0" || txtPaidAmount.Text == string.Empty)
                {
                    MessageBox.Show("Please insert paid amount. \n  If you want full due transaction \n Please insert 0.00 ", "Yes or No", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                }
                //  else if (Convert.ToInt32(txtInvoice.Text) >= 53)
                // {
                //   MessageBox.Show("Sorry ! Demo version has limited transaction \n Please buy it \n contact at : citkar@live.com \n https://goo.gl/Hs7XsD", "Yes or No", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                //  }
                else
                {
                    try
                    {
                        //Save payment info into sales_payment table
                        payment_item(lblTotalPayable.Text, txtChangeAmount.Text, txtDueAmount.Text, CombPayby.Text, dtSalesDate.Text, lblCustID.Text, txtCustName.Text);

                        ///// save sales items one by one 
                        sales_item(dtSalesDate.Text);
                        DiscountCalculation();
                        vatcal();

                        ///// // Open Print Invoice
                        parameter.autoprintid = "0";
                        POSPrintRpt go = new POSPrintRpt(txtInvoice.Text);
                        go.ShowDialog();

                        //Clean Datagridview and Back to sales cart
                        dgrvSalesItemList.Rows.Clear();

                        //    this.tabPageSR_Payment.Parent = null; //Hide payment tab
                        //tabSRcontrol.SelectedTab = tabPageSR_Counter;
                        btnCompleteSalesAndPrint.Enabled = false;
                        btnPayment.Enabled = false;
                        txtPaidAmount.Text = "00";
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message);
                    }
                }
            }
        }


        /// //// Add sales item  ////////////Store into sales_item table //////////
        public bool sales_item(string salesdate)
        {
            int rows = dgrvSalesItemList.Rows.Count;
            for (int i = 0; i < rows; i++)
            {
                //string SalesDate = dtSalesDate.Text;  
                string trno = txtInvoice.Text;
                string itemid = dgrvSalesItemList.Rows[i].Cells[4].Value.ToString();
                string itNam = dgrvSalesItemList.Rows[i].Cells[0].Value.ToString();
                double qty = Convert.ToDouble(dgrvSalesItemList.Rows[i].Cells[2].Value.ToString());
                double Rprice = Convert.ToDouble(dgrvSalesItemList.Rows[i].Cells[1].Value.ToString());
                double total = Convert.ToDouble(dgrvSalesItemList.Rows[i].Cells[3].Value.ToString());
                double dis = Convert.ToDouble(dgrvSalesItemList.Rows[i].Cells[7].Value.ToString()); //discount rate
                double taxapply = Convert.ToDouble(dgrvSalesItemList.Rows[i].Cells[8].Value.ToString());
                int kitchendisplay = Convert.ToInt32(dgrvSalesItemList.Rows[i].Cells[9].Value.ToString());



                // =================================Start=====  Profit calculation =============== Start ========= 
                // Discount_amount = (Retail_price * discount) / 100                    -- 49 * 3 / 100 = 1.47
                // Retail_priceAfterDiscount = Retail_price - Discount_amount           -- 49 - 1.47 = 47.53
                // Profit = (Retail_priceAfterDiscount * QTY )   - (cost_price * qty);  ---( 47.53 * 1 ) - ( 45 * 1) = 2.53

                string sqlprofit = "Select cost_price , discount  from  purchase  where product_id  = '" + itemid + "'";
                DAL.DataAccessManager.ExecuteSQL(sqlprofit);
                DataTable dt1 = DAL.DataAccessManager.GetDataTable(sqlprofit);

                double cost_price = Convert.ToDouble(dt1.Rows[0].ItemArray[0].ToString());
                double discount = Convert.ToDouble(dt1.Rows[0].ItemArray[1].ToString());

                double Discount_amount = (Rprice * discount) / 100.00;
                double Retail_priceAfterDiscount = Rprice - Discount_amount;
                double Profit = Math.Round((Retail_priceAfterDiscount - cost_price), 2); // old calculation (Retail_priceAfterDiscount * qty) - (cost_price * qty);
                // =================================Start=====  Profit calculation =============== Start =========  


                string sql1 = " insert into sales_item (sales_id,itemName,Qty,RetailsPrice,Total, profit,sales_time, itemcode , discount, taxapply, status) " +
                              " values ('" + trno + "', '" + itNam + "', '" + qty + "', '" + Rprice + "', '" + total + "', '" + Profit + "', " +
                              " '" + salesdate + "','" + itemid + "','" + dis + "','" + taxapply + "','" + kitchendisplay + "')";
                DAL.DataAccessManager.ExecuteSQL(sql1);

                //update quantity Decrease from Stock Qty |  purchase Table
                if (txtInvoice.Text == "")
                {
                    MessageBox.Show("please check sales no ");
                }
                else
                {

                    string itemids = dgrvSalesItemList.Rows[i].Cells[4].Value.ToString();
                    double qtyupdate = Convert.ToDouble(dgrvSalesItemList.Rows[i].Cells[2].Value.ToString());

                    // Update Quantity
                    string sqlupdateQty = "select product_quantity  from purchase where product_id = '" + itemids + "'";
                    DAL.DataAccessManager.ExecuteSQL(sqlupdateQty);
                    DataTable dtUqty = DAL.DataAccessManager.GetDataTable(sqlupdateQty);
                    double product_quantity = Convert.ToDouble(dtUqty.Rows[0].ItemArray[0].ToString()) - qtyupdate;

                    string sql = " update purchase set " +
                                    " product_quantity = '" + product_quantity + "' " +
                                    " where product_id = '" + itemids + "' ";
                    DAL.DataAccessManager.ExecuteSQL(sql);
                }

            }
            return true;

        }

        /// //// Payment items Add  ///////////Store into Sales_payment table //////// 
        public void payment_item(string payamount, string changeamount, string dueamount, string salestype, string salesdate, string custid, string Comment)
        {
            try
            {
                string trno = lblTotalPayable.Text;
                // string payamount        = lblTotalPayable.Text;
                //  string changeamount     = "0";
                //string due              =  "0";
                if(changeamount == "")
                {
                    changeamount = "0";
                }
                if(dueamount == "")
                {
                    dueamount = "0";
                }
                string vat = lblTotalVAT.Text;
                string DiscountTotal = lbloveralldiscount.Text; // Total discount = item wise discount + counter discount
                                                                // string Comment          = "Guest";
                string overalldisRate = txtDiscountRate.Text;
                string vatRate = txtVATRate.Text;

                string sql1 = " insert into sales_payment (sales_id, payment_type,payment_amount,change_amount,due_amount, dis, vat, " +
                               " sales_time,c_id,emp_id,comment, TrxType, Shopid , ovdisrate , vaterate,Job_id ) " +
                               "  values ('" + txtInvoice.Text + "','" + CombPayby.Text + "', '" + payamount + "', '" + changeamount + "', " +
                               " '" + dueamount + "', '" + DiscountTotal + "', '" + vat + "', '" + salesdate + "', '" + lblCustID.Text + "', " +
                               "  '" + UserInfo.UserName + "','" + Comment + "','POS','" + UserInfo.Shopid + "' , '" + overalldisRate + "' , '" + vatRate + "','" + CurrentJobNo + "' )";
                DAL.DataAccessManager.ExecuteSQL(sql1);

                //string sql1 = " insert into sales_payment (sales_id, payment_type,payment_amount,change_amount,due_amount, dis, vat, " +
                //                " sales_time,c_id,emp_id,comment, TrxType, Shopid , ovdisrate , vaterate ) " +
                //                " values ('" + txtInvoice.Text + "','Cash', '" + payamount + "', '" + changeamount + "', " +
                //                " '" + dueamount + "', '" + DiscountTotal + "', '" + vat + "', '" + DateTime.Now.ToString("yyyy-MM-dd") + "', '10000009', " +
                //                " '" + UserInfo.UserName + "','" + Comment + "','POS','" + UserInfo.Shopid + "' , '" + overalldisRate + "' , '" + vatRate + "' )";
                //DataAccess.ExecuteSQL(sql1);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void RegisterQ_Load(object sender, EventArgs e)
        {
            try
            {
                ItemList_with_images("");

                dtSalesDate.Format = DateTimePickerFormat.Custom;
                dtSalesDate.CustomFormat = "yyyy-MM-dd";


                //Customer Databind 
                string sqlCust = "select   DISTINCT  *   from tbl_customer where PeopleType = 'Customer'";
                DAL.DataAccessManager.ExecuteSQL(sqlCust);
                DataTable dtCust = DAL.DataAccessManager.GetDataTable(sqlCust);
                ComboCustID.DataSource = dtCust;
                ComboCustID.DisplayMember = "Name";
                ComboCustID.Text = "Guest";

                btnCompleteSalesAndPrint.Focus();

                //Load Vat and Discount rate
                // string sqlVatdis = "select * from storeconfig";
                //  DataAccess.ExecuteSQL(sqlVatdis);
                //DataTable dtVatdis = DataAccess.GetDataTable(sqlVatdis);
                //txtVATRate.Text = vatdisvalue.vat;
                // txtDiscountRate.Text    = vatdisvalue.dis;            


                this.dgrvSalesItemList.Columns.Add("itm", "Items Name");
                this.dgrvSalesItemList.Columns.Add("Am", "Price");
                this.dgrvSalesItemList.Columns.Add("Qty", "Qty");
                this.dgrvSalesItemList.Columns.Add("Total", "Total");
                this.dgrvSalesItemList.Columns.Add("ID", "ID");
                this.dgrvSalesItemList.Columns.Add("disamt", "Disamt");     // 5. new in 8.1 version
                this.dgrvSalesItemList.Columns.Add("taxamt", "taxamt");     // 6. new in 8.1 version
                this.dgrvSalesItemList.Columns.Add("dis", "Dis");           // 7. new in 8.1 version
                this.dgrvSalesItemList.Columns.Add("taxapply", "Tax");      // 8. new in 8.1 version
                this.dgrvSalesItemList.Columns.Add("kitdisplay", "KD");      // 8. new in 8.3.1 version

                //Hide fields
                dgrvSalesItemList.Columns[4].Visible = false; // ID             // new in 8.1 version
                dgrvSalesItemList.Columns[5].Visible = false; // Disamt         // new in 8.1 version
                dgrvSalesItemList.Columns[6].Visible = false; // taxamt         // new in 8.1 version
                dgrvSalesItemList.Columns[7].Visible = false; // Discount rate  // new in 8.1 version
                dgrvSalesItemList.Columns[9].Visible = false; // kitdisplay    // new in 8.3.1 version

                //Font size of columns and aligmnet  // add in from version 8.3
                dgrvSalesItemList.Columns["itm"].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 9);
                dgrvSalesItemList.Columns["Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgrvSalesItemList.Columns["taxapply"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


                ///// dataGridView1.Rows.Add(1);         

                DataGridViewButtonColumn inc = new DataGridViewButtonColumn();
                dgrvSalesItemList.Columns.Add(inc);
                inc.HeaderText = "Inc";
                inc.Text = "+";
                inc.Name = "inc";
                inc.ToolTipText = "Increase Item Qty";
                inc.UseColumnTextForButtonValue = true;

                DataGridViewButtonColumn minus = new DataGridViewButtonColumn();
                dgrvSalesItemList.Columns.Add(minus);
                minus.HeaderText = "Dec";
                minus.Text = "-";
                minus.Name = "minus";
                minus.ToolTipText = "minus Item Qty";
                minus.UseColumnTextForButtonValue = true;

                DataGridViewButtonColumn del = new DataGridViewButtonColumn();
                dgrvSalesItemList.Columns.Add(del);
                del.HeaderText = "Del";
                del.Text = "x";
                del.Name = "del";
                del.ToolTipText = "Delete this Item";
                del.UseColumnTextForButtonValue = true;


                // this.dgrvSalesItemList.Rows[0].Cells[2].Value = "1";
                //  dgrvSalesItemList.ReadOnly = true;
                dgrvSalesItemList.Columns[0].ReadOnly = true;
                dgrvSalesItemList.Columns[1].ReadOnly = true;
                dgrvSalesItemList.Columns[2].ReadOnly = false;
                dgrvSalesItemList.Columns[3].ReadOnly = true;
                dgrvSalesItemList.Columns[4].ReadOnly = true;
                dgrvSalesItemList.Columns[5].ReadOnly = true;
                dgrvSalesItemList.Columns[6].ReadOnly = true;
                dgrvSalesItemList.Columns[7].ReadOnly = true;
                dgrvSalesItemList.Columns[8].ReadOnly = true;
                dgrvSalesItemList.Columns[9].ReadOnly = true;

                //Qty column row color
                dgrvSalesItemList.Columns["Qty"].DefaultCellStyle.ForeColor = Color.Black;
                dgrvSalesItemList.Columns["Qty"].DefaultCellStyle.BackColor = Color.Silver;
                dgrvSalesItemList.Columns["Qty"].DefaultCellStyle.SelectionForeColor = Color.Black;
                dgrvSalesItemList.Columns["Qty"].DefaultCellStyle.SelectionBackColor = Color.Silver;
                dgrvSalesItemList.Columns["Qty"].DefaultCellStyle.Font = new Font(DataGridView.DefaultFont, FontStyle.Bold);



                //  Column width
                dgrvSalesItemList.Columns["itm"].Width = 200;
                dgrvSalesItemList.Columns["Del"].Width = 11;
                dgrvSalesItemList.Columns["inc"].Width = 35;
                dgrvSalesItemList.Columns["minus"].Width = 35;

                // dgrvSalesItemList.Rows[0].Cells[2].Style.BackColor = Color.Red;
                // DataGridViewColumn ColQty = dgrvSalesItemList.Columns[2];
                // ColQty.Width = 45;


                //Load Invoice No / Receipt No for current transaction
                string sql = "select sales_id from sales_payment order by sales_id desc";
                DataTable dt = DAL.DataAccessManager.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    double id = Convert.ToDouble(dt.Rows[0].ItemArray[0].ToString()) + 1;
                    //  txtInvoiceNo.Text = Convert.ToString(id);
                    txtInvoice.Text = Convert.ToString(Convert.ToInt32(id));
                    // btnInvoiceNo.Text = id.ToString();
                }
                else
                {
                    double id = 1;
                    // txtInvoiceNo.Text = Convert.ToString(id);
                    txtInvoice.Text = Convert.ToString(Convert.ToInt32(id));
                    // btnInvoiceNo.Text = id.ToString();
                }

            }
            catch //(Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }


            if (CurrentJobNo != "0")
            {
                //Bind Items
                try
                {
                    string dataDS = "SELECT JobNo,EngineOil,GearOil,OilFilter,AirFilter,FuelFilter,ATFFilter,CabinFilter,VehicleNo,DiffOil,Customer,Mileage FROM job_card where JobNo = '" + CurrentJobNo + "'";

                    DataSet ds = DAL.DataAccessManager.GetDataSet(dataDS);

                    int rows = ds.Tables[0].Rows.Count;

                    if (rows > 0)
                    {
                        string EngineOil = ds.Tables[0].Rows[0][1].ToString();
                        string GearOil = ds.Tables[0].Rows[0][2].ToString();
                        string OilFilter = ds.Tables[0].Rows[0][3].ToString();
                        string AirFilter = ds.Tables[0].Rows[0][4].ToString();
                        string FuelFilter = ds.Tables[0].Rows[0][5].ToString();
                        string AtfFilter = ds.Tables[0].Rows[0][6].ToString();
                        string CabinFilter = ds.Tables[0].Rows[0][7].ToString();
                        string DiffOil = ds.Tables[0].Rows[0][9].ToString();




                        label8.Text = ds.Tables[0].Rows[0][8].ToString();//Vehicle no
                        ComboCustID.Text = ds.Tables[0].Rows[0][10].ToString();//Customer
                        label19.Text = ds.Tables[0].Rows[0][0].ToString();//Job no
                        label14.Text = ds.Tables[0].Rows[0][11].ToString();//Milage
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void txtDiscountRate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (lblTotalPayable.Text == "")
                {
                    MessageBox.Show("Please Add at least One Item");
                }
                else
                {
                    double Discountvalue = Convert.ToDouble(txtDiscountRate.Text);
                    txtDiscountRate.Text = Discountvalue.ToString();
                    double subtotal = Convert.ToDouble(lblTotal.Text) - Convert.ToDouble(lblTotalDisCount.Text); // total - item discount  100 - 5 = 95        
                    double totaldiscount = (subtotal * Discountvalue) / 100;  //Counter discount  // 95 * 5 /100 = 4.75  
                                                                              // double totaldiscount = Convert.ToDouble(lblTotalDisCount.Text) + Discountvalue;   // Uncomment this line if you want to discount value and comment/delete above line
                    double disPlusOverallDiscount = totaldiscount + Convert.ToDouble(lblTotalDisCount.Text); // 4.75 + 5 = 9.75
                    disPlusOverallDiscount = Math.Round(disPlusOverallDiscount, 2);
                    lbloveralldiscount.Text = disPlusOverallDiscount.ToString();  // Overall discount 9.75

                    double subtotalafteroveralldiscount = subtotal - totaldiscount; // 95 - 4.75 = 90.25
                    subtotalafteroveralldiscount = Math.Round(subtotalafteroveralldiscount, 2);
                    lblsubtotal.Text = subtotalafteroveralldiscount.ToString();

                    double payable = subtotalafteroveralldiscount + Convert.ToDouble(lblTotalVAT.Text);
                    payable = Math.Round(payable, 2);
                    lblTotalPayable.Text = payable.ToString();

                    //  btnPayment.Text = "Pay = " + payable.ToString();

                }
            }
            catch
            {
                txtDiscountRate.Text = "0";
            }

        }

        private void txtPaidAmount_TextChanged(object sender, EventArgs e)
        {
            if (lblTotalPayable.Text == "")
            {
                // MessageBox.Show("please insert Amount ");
            }
            else
            {
                try
                {
                    if (Convert.ToDouble(txtPaidAmount.Text) >= Convert.ToDouble(lblTotalPayable.Text))
                    {
                        double changeAmt = Convert.ToDouble(txtPaidAmount.Text) - Convert.ToDouble(lblTotalPayable.Text);
                        changeAmt = Math.Round(changeAmt, 2);
                        txtChangeAmount.Text = changeAmt.ToString();
                        txtDueAmount.Text = "0";
                        this.AcceptButton = btnPrintDirect;
                    }
                    if (Convert.ToDouble(txtPaidAmount.Text) <= Convert.ToDouble(lblTotalPayable.Text))
                    {
                        double changeAmt = Convert.ToDouble(lblTotalPayable.Text) - Convert.ToDouble(txtPaidAmount.Text);
                        changeAmt = Math.Round(changeAmt, 2);
                        txtDueAmount.Text = changeAmt.ToString();
                        txtChangeAmount.Text = "0";
                        this.AcceptButton = btnPrintDirect;
                    }

                }
                catch //(Exception exp)
                {
                    // MessageBox.Show(exp.Message);
                }

            }
        }

        private void dgrvSalesItemList_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Increase Item Quantity with Edited cell
                if (e.ColumnIndex == dgrvSalesItemList.Columns["Qty"].Index && e.RowIndex >= 0)
                {
                    foreach (DataGridViewRow row in dgrvSalesItemList.SelectedRows)
                    {
                        // Total Price
                        // double totalPrice = Convert.ToDouble(row.Cells[2].Value) * Convert.ToDouble(row.Cells[1].Value);
                        // row.Cells[3].Value = totalPrice;
                        if (Convert.ToDouble(row.Cells[2].Value) > CheckStockQty(Convert.ToString(dgrvSalesItemList.Rows[e.RowIndex].Cells[4].Value.ToString())))
                        {
                            MessageBox.Show("You don't have sufficient item Quantity \n\n Your  Item Quantity is greater than Stock Qty");
                            row.Cells[2].Value = CheckStockQty(Convert.ToString(dgrvSalesItemList.Rows[e.RowIndex].Cells[4].Value.ToString()));

                            double qty = Convert.ToDouble(row.Cells[2].Value);
                            double Rprice = Convert.ToDouble(row.Cells[1].Value);
                            double disrate = Convert.ToDouble(row.Cells[7].Value);
                            double Taxrate = Convert.ToDouble(vatdisvalue.vat);

                            //// show total price   Qty  * Rprice
                            double totalPrice = qty * Rprice;
                            row.Cells[3].Value = totalPrice;

                            if (Convert.ToDouble(row.Cells[7].Value) != 0)  // IF discount is not zero then apply discount
                            {
                                double Disamt = (((Rprice * qty) * disrate) / 100.00);      // Total Discount amount of this item
                                row.Cells[5].Value = Disamt;
                            }

                            if (Convert.ToDouble(row.Cells[8].Value) != 0)  // IF tax is not zero then apply tax
                            {
                                double Taxamt = ((((Rprice * qty) - (((Rprice * qty) * disrate) / 100.00)) * Taxrate) / 100.00); // Total Tax amount  of this item
                                row.Cells[6].Value = Taxamt;
                            }
                        }
                        else
                        {
                            double qty = Convert.ToDouble(row.Cells[2].Value);
                            double Rprice = Convert.ToDouble(row.Cells[1].Value);
                            double disrate = Convert.ToDouble(row.Cells[7].Value);
                            double Taxrate = Convert.ToDouble(vatdisvalue.vat);

                            //// show total price   Qty  * Rprice
                            double totalPrice = qty * Rprice;
                            row.Cells[3].Value = totalPrice;

                            if (Convert.ToDouble(row.Cells[7].Value) != 0)  // IF discount is not zero then apply discount
                            {
                                double Disamt = (((Rprice * qty) * disrate) / 100.00);      // Total Discount amount of this item
                                row.Cells[5].Value = Disamt;
                            }

                            if (Convert.ToDouble(row.Cells[8].Value) != 0)  // IF tax is not zero then apply tax
                            {
                                double Taxamt = ((((Rprice * qty) - (((Rprice * qty) * disrate) / 100.00)) * Taxrate) / 100.00); // Total Tax amount  of this item
                                row.Cells[6].Value = Taxamt;
                            }
                        }

                        DiscountCalculation();
                        vatcal();
                        txtDiscountRate.Text = "0";
                    }
                }
            }
            catch
            {
            }
        }

        private void dgrvSalesItemList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

                // Delete items From Gridview
                if (e.ColumnIndex == dgrvSalesItemList.Columns["del"].Index && e.RowIndex >= 0)
                {
                    foreach (DataGridViewRow row2 in dgrvSalesItemList.SelectedRows)
                    {
                        //  DialogResult result = MessageBox.Show("Do you want to Delete?", "Yes or No", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                        //  if (result == DialogResult.Yes)
                        //  {
                        if (!row2.IsNewRow)
                            dgrvSalesItemList.Rows.Remove(row2);
                        DiscountCalculation();
                        vatcal();
                        txtDiscountRate.Text = "0";
                        // lbloveralldiscount.Text = "0";
                        // }
                    }
                }

                // Increase Item Quantity
                if (e.ColumnIndex == dgrvSalesItemList.Columns["inc"].Index && e.RowIndex >= 0)
                {
                    foreach (DataGridViewRow row in dgrvSalesItemList.SelectedRows)
                    {

                        //  dgrvSalesItemList.Rows[0][0].Convert.ToDouble(row.Cells[10].Value))Convert.ToString(row.Cells[4].Value.ToString() // Convert.ToString(dgrvSalesItemList.Rows[e.RowIndex].Cells[4].Value.ToString()
                        if (Convert.ToDouble(row.Cells[2].Value) >= CheckStockQty(Convert.ToString(dgrvSalesItemList.Rows[e.RowIndex].Cells[4].Value.ToString())))
                        {
                            MessageBox.Show("You don't have sufficient item Quantity \n\n Your  Item Quantity is greater than Stock Qty");
                            row.Cells[2].Value = CheckStockQty(Convert.ToString(dgrvSalesItemList.Rows[e.RowIndex].Cells[4].Value.ToString()));

                            double qtySum = Convert.ToDouble(row.Cells[2].Value);
                            row.Cells[2].Value = qtySum;

                            double qty = Convert.ToDouble(row.Cells[2].Value);
                            double Rprice = Convert.ToDouble(row.Cells[1].Value);
                            double disrate = Convert.ToDouble(row.Cells[7].Value);
                            double Taxrate = Convert.ToDouble(vatdisvalue.vat);

                            //// show total price   Qty  * Rprice
                            double totalPrice = qty * Rprice;
                            row.Cells[3].Value = totalPrice;

                            if (Convert.ToDouble(row.Cells[7].Value) != 0)
                            {
                                double Disamt = (((Rprice * qty) * disrate) / 100.00);      // Total Discount amount of this item
                                row.Cells[5].Value = Disamt;
                            }

                            if (Convert.ToDouble(row.Cells[8].Value) != 0)
                            {
                                double Taxamt = ((((Rprice * qty) - (((Rprice * qty) * disrate) / 100.00)) * Taxrate) / 100.00); // Total Tax amount  of this item
                                row.Cells[6].Value = Taxamt;
                            }
                        }
                        else
                        {
                            //// Increase by 1
                            double qtySum = Convert.ToDouble(row.Cells[2].Value) + 1;
                            row.Cells[2].Value = qtySum;

                            double qty = Convert.ToDouble(row.Cells[2].Value);
                            double Rprice = Convert.ToDouble(row.Cells[1].Value);
                            double disrate = Convert.ToDouble(row.Cells[7].Value);
                            double Taxrate = Convert.ToDouble(vatdisvalue.vat);

                            //// show total price   Qty  * Rprice
                            double totalPrice = qty * Rprice;
                            row.Cells[3].Value = totalPrice;

                            if (Convert.ToDouble(row.Cells[7].Value) != 0)
                            {
                                double Disamt = (((Rprice * qty) * disrate) / 100.00);      // Total Discount amount of this item
                                row.Cells[5].Value = Disamt;
                            }

                            if (Convert.ToDouble(row.Cells[8].Value) != 0)
                            {
                                double Taxamt = ((((Rprice * qty) - (((Rprice * qty) * disrate) / 100.00)) * Taxrate) / 100.00); // Total Tax amount  of this item
                                row.Cells[6].Value = Taxamt;
                            }

                        }

                        DiscountCalculation();
                        vatcal();

                        txtDiscountRate.Text = "0";

                    }
                }

                // Decrease Item Quantity  -- Add new from 8.3.2
                if (e.ColumnIndex == dgrvSalesItemList.Columns["minus"].Index && e.RowIndex >= 0)
                {
                    foreach (DataGridViewRow row in dgrvSalesItemList.SelectedRows)
                    {
                        if (Convert.ToDouble(row.Cells[2].Value) > 1)
                        {
                            //// Decrease by 1 
                            double qtySum = Convert.ToDouble(row.Cells[2].Value) - 1;
                            row.Cells[2].Value = qtySum;

                            double qty = Convert.ToDouble(row.Cells[2].Value);
                            double Rprice = Convert.ToDouble(row.Cells[1].Value);
                            double disrate = Convert.ToDouble(row.Cells[7].Value);
                            double Taxrate = Convert.ToDouble(vatdisvalue.vat);

                            //// show total price   Qty  * Rprice
                            double totalPrice = qty * Rprice;
                            row.Cells[3].Value = totalPrice;

                            if (Convert.ToDouble(row.Cells[7].Value) != 0)
                            {
                                double Disamt = (((Rprice * qty) * disrate) / 100.00);      // Total Discount amount of this item
                                row.Cells[5].Value = Disamt;
                            }

                            if (Convert.ToDouble(row.Cells[8].Value) != 0)
                            {
                                double Taxamt = ((((Rprice * qty) - (((Rprice * qty) * disrate) / 100.00)) * Taxrate) / 100.00); // Total Tax amount  of this item
                                row.Cells[6].Value = Taxamt;
                            }

                            DiscountCalculation();
                            vatcal();

                            txtDiscountRate.Text = "0";
                        }

                    }
                }

            }
            catch //(Exception exp)
            {
                // MessageBox.Show("Sorry" + exp.Message);
            }
        }


        public double CheckStockQty(string itemcode)
        {
            string sql = "SELECT   product_quantity  " +
                        " FROM  purchase  where product_id = '" + itemcode + "'  ";
            DAL.DataAccessManager.ExecuteSQL(sql);
            DataTable dt = DAL.DataAccessManager.GetDataTable(sql);

            double totalstockQty = Convert.ToDouble(dt.Rows[0].ItemArray[0].ToString());
            return totalstockQty;
        }

        private void txtPaidAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                bool ignoreKeyPress = false;

                bool matchString = Regex.IsMatch(txtPaidAmount.Text.ToString(), @"\.\d\d\d");

                if (e.KeyChar == '\b') // Always allow a Backspace
                    ignoreKeyPress = false;
                else if (matchString)
                    ignoreKeyPress = true;
                else if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                    ignoreKeyPress = true;
                else if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                    ignoreKeyPress = true;

                e.Handled = ignoreKeyPress;
                //using System.Text.RegularExpressions;
            }
            catch
            {
            }
        }

        private void btnPrintDirect_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to Complete this transaction and Direct Print?", "Yes or No", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (txtPaidAmount.Text == "00" || txtPaidAmount.Text == "0" || txtPaidAmount.Text == string.Empty)
                {
                    MessageBox.Show("Please insert paid amount. \n  If you want full due transaction \n Please insert 0.00 ", "Yes or No", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                }
                else
                {
                    try
                    {
                        //Save payment info into sales_payment table
                        payment_item(lblTotalPayable.Text, txtChangeAmount.Text, txtDueAmount.Text, CombPayby.Text, dtSalesDate.Text, lblCustID.Text, ComboCustID.Text);

                        ///// save sales items one by one 
                        sales_item(dtSalesDate.Text);

                        PrintReceiptWithoutPrintDialog();

                        //Clean Datagridview and Back to sales cart
                        dgrvSalesItemList.Rows.Clear();
                        DiscountCalculation();
                        vatcal();
                        //    this.tabPageSR_Payment.Parent = null; //Hide payment tab
                        //tabSRcontrol.SelectedTab = tabPageSR_Counter;
                        btnCompleteSalesAndPrint.Enabled = false;
                        btnPayment.Enabled = false;
                        btnSaveOnly.Enabled = false;
                        //UpdateJobCard();
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message);
                    }
                }
            }
        }


        private void PrintReceiptWithoutPrintDialog()
        {
            PrintDialog printDialog = new PrintDialog();

            PrintDocument printDocument = new PrintDocument();
            printDocument.DocumentName = "Receipt_direct_" + txtInvoice.Text + "_" + DateTime.Now.ToString("yyyyMMddhhmmss");
            printDialog.Document = printDocument;

            printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);
            printDocument.Print();

            //DialogResult result = printDialog.ShowDialog();

            //if(result == DialogResult.OK)
            //{
            //    printDocument.Print();
            //}
        }


        void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics graphic = e.Graphics;

                Font font = new Font("Courier New", 10);

                float fontHeight = font.GetHeight();

                int startX = 10;
                int startY = 10;
                int offset = 13;

                string sql = " SELECT  sp.sales_id AS salesid, sp.payment_type AS paytype, sp.payment_amount AS Payamount, " +
                                " sp.change_amount AS charAmt, sp.due_amount AS due, sp.dis, sp.vat, sp.sales_time AS s_time,  " +
                                " sp.c_id AS custID, sp.emp_id AS empID, sp.comment AS Note, sp.TrxType, si.sales_id,si.item_id,  " +
                                " si.itemName, si.Qty, si.RetailsPrice, si.Total,si.profit, si.sales_time , sp.Shopid, tl.*, c.* ,  " +
                                " CASE     " +
                                " WHEN si.taxapply = 1 THEN 'TX'  " +
                                " ELSE ''  " +
                                " END 'TaxApply'  " +
                                " FROM            sales_payment sp " +
                                " INNER JOIN   sales_item si " +
                                " ON sp.sales_id  = si.sales_id " +
                                " INNER JOIN tbl_terminalLocation tl " +
                                " ON sp.Shopid  = tl.Shopid " +
                                " INNER JOIN tbl_customer c " +
                                " ON  sp.c_id  = c.ID " +
                                " Where sp.sales_id  = '" + txtInvoice.Text + "'  ";
                DAL.DataAccessManager.ExecuteSQL(sql);
                DataTable dt = DAL.DataAccessManager.GetDataTable(sql);

                string storename = dt.Rows[0]["companyname"].ToString(); //"Doglus Coffee Shop"
                string Address = dt.Rows[0]["location"].ToString(); ///// "34 Dandus street ON M7H R5T CA"
                string Phone = dt.Rows[0]["phone"].ToString();  //// "+1(416) 111 1234"
                string vatregino = dt.Rows[0]["vatregino"].ToString(); //// "803060284RT0003"
                string Salesid = "Invoice " + dt.Rows[0]["salesid"].ToString() + "-" + dt.Rows[0]["empID"].ToString();

                string VehicleNo = label8.Text;

                offset = offset + (int)fontHeight;
                graphic.DrawString(storename, new Font("Courier New", 19), new SolidBrush(Color.Black), startX + 100, offset);

                offset = offset + (int)fontHeight + 9;
                //RectangleF rectFaddr = new RectangleF(startX, offset, 180, 55);
                RectangleF rectFaddr = new RectangleF(startX + 120, offset, 280, 55);
                graphic.DrawString("".PadRight(3) + Address, new Font("Courier New", 11), new SolidBrush(Color.Black), rectFaddr);

                offset = offset + (int)fontHeight + 7;  // +Convert.ToInt32(rectFaddr.Height);
                graphic.DrawString("".PadRight(3) + "TEL:" + Phone, new Font("Courier New", 11), new SolidBrush(Color.Black), startX + 120, startY + offset);

                offset = offset + (int)fontHeight + 4;
                graphic.DrawString("".PadRight(3) + "Vehicle No:" + VehicleNo, new Font("Courier New", 11), new SolidBrush(Color.Black), startX + 120, startY + offset);

                offset = offset + (int)fontHeight + 3;
                graphic.DrawString("".PadRight(3) + "Customer Name:" + ComboCustID.Text, new Font("Courier New", 11), new SolidBrush(Color.Black), startX + 120, startY + offset);

                offset = offset + (int)fontHeight + 3;
                graphic.DrawString("".PadRight(3) + "Milage:" + label14.Text, new Font("Courier New", 11), new SolidBrush(Color.Black), startX + 120, startY + offset);

                offset = offset + (int)fontHeight + 5;
                graphic.DrawString(DateTime.Now.ToString(), new Font("Courier New", 11), new SolidBrush(Color.Black), startX + 120, startY + offset);

                offset = offset + (int)fontHeight + 3;
                graphic.DrawString(Salesid, new Font("Courier New", 11), new SolidBrush(Color.Black), startX + 120, startY + offset);

                offset = offset + (int)fontHeight + 32;
                //graphic.DrawString("-----------------------------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string productdescription = dt.Rows[i]["itemname"].ToString(); // "Product-" + i.ToString();
                    string productTotal = dt.Rows[i]["total"].ToString();  /// String.Format("{0:c}", "$20");
                    string productQty = dt.Rows[i]["Qty"].ToString();
                    string product1Line = productQty + "x   " + productdescription + "  " + productTotal;

                    // graphic.DrawString(product1Line, font, new SolidBrush(Color.Black), startX, startY + offset);
                    //offset = offset + (int)fontHeight + 5;

                    //RectangleF rectProdline = new RectangleF(startX, offset, 215, 57);
                    RectangleF rectProdline = new RectangleF(startX, offset, 415, 57);
                    graphic.DrawString(product1Line, font, new SolidBrush(Color.Black), rectProdline);
                    offset = offset + (int)fontHeight + 13;
                }

                decimal dis = Convert.ToDecimal(dt.Rows[0]["dis"].ToString());
                decimal TAX = Convert.ToDecimal(dt.Rows[0]["vat"].ToString());
                decimal Subtotal = Convert.ToDecimal(dt.Rows[0]["Payamount"].ToString()) - (TAX);
                string payment_amount = dt.Rows[0]["Payamount"].ToString();
                decimal Change = Convert.ToDecimal(dt.Rows[0]["charAmt"].ToString());
                decimal due = Convert.ToDecimal(dt.Rows[0]["due"].ToString());
                string Payment = dt.Rows[0]["paytype"].ToString();
                string footermsg = dt.Rows[0]["footermsg"].ToString();
                //if(footermsg.Length > 75)
                //{
                //    footermsg = footermsg.Substring(0, 28) + "\n" + footermsg.Substring(29, 30) + "\n" + footermsg.Substring(60, 14);  ///"THANK YOU & COME BACK"
                //}
                //else
                //{
                //    footermsg = "THANK YOU & COME BACK";
                //}

                graphic.DrawString("----------------------------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset);

                var format = new StringFormat() { Alignment = StringAlignment.Far };

                offset = offset + (int)fontHeight + 5;
                var rect = new RectangleF(startX, startY + offset, 415, 97);
                //graphic.DrawString("Sub-Total ".PadRight(15) + String.Format("{0:c}", Subtotal.ToString()), font, new SolidBrush(Color.Black), startX, startY + offset);
                graphic.DrawString("Sub-Total ".PadRight(15) + String.Format("{0:c}", Subtotal.ToString()), font, new SolidBrush(Color.Black), rect, format);


                offset = offset + (int)fontHeight + 5;
                var RECT1 = new RectangleF(startX, startY + offset, 415, 97);
                //graphic.DrawString("TAX ".PadRight(15) + String.Format("{0:c}", TAX.ToString()), font, new SolidBrush(Color.Black), startX, startY + offset);
                graphic.DrawString("TAX ".PadRight(15) + String.Format("{0:c}", TAX.ToString()), font, new SolidBrush(Color.Black), RECT1, format);

                //  offset = offset + (int)fontHeight + 5;
                // graphic.DrawString("Total ".PadRight(8) + String.Format("{0:c}", payment_amount), new Font("Courier New", 15, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + offset);

                offset = offset + (int)fontHeight + 15;
                //RectangleF rectTotal = new RectangleF(startX, offset, 215, 127);
                RectangleF rectTotal = new RectangleF(startX, offset, 415, 127);
                graphic.DrawString("Total ".PadRight(8) + String.Format("{0:c}", payment_amount), new Font("Courier New", 15, FontStyle.Bold), new SolidBrush(Color.Black), rectTotal, format);

                if (Change > 0)
                {
                    offset = offset + (int)fontHeight + 17;
                    //graphic.DrawString("Change  ".PadRight(15) + String.Format("{0:c}", Change.ToString()), new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + offset);

                }

                if (due > 0)
                {

                    offset = offset + (int)fontHeight + 17;
                    var rect1 = new RectangleF(startX, startY + offset, 415, 97);
                    //graphic.DrawString("Due  ".PadRight(15) + String.Format("{0:c}", due.ToString()), new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + offset);
                    graphic.DrawString("Due  ".PadRight(15) + String.Format("{0:c}", due.ToString()), new Font("Courier New", 11), new SolidBrush(Color.Black), rect1, format);
                }

                if (dis > 0)
                {
                    offset = offset + (int)fontHeight + 17;
                    var rect1 = new RectangleF(startX, startY + offset, 415, 97);
                    //   graphic.DrawString("Discount ".PadRight(15) + String.Format("{0:c}", dis.ToString()), font, new SolidBrush(Color.Black), startX, startY + offset);
                    graphic.DrawString("Discount ".PadRight(15) + String.Format("{0:c}", dis.ToString()), font, new SolidBrush(Color.Black), rect1, format);
                }

                offset = offset + (int)fontHeight + 5;
                var rect2 = new RectangleF(startX, startY + offset, 415, 97);
                //graphic.DrawString("Payment ".PadRight(15) + String.Format("{0:N}", Payment), font, new SolidBrush(Color.Black), startX, startY + offset);
                graphic.DrawString("Payment ".PadRight(15) + String.Format("{0:N}", Payment), font, new SolidBrush(Color.Black), rect2, format);


                offset = offset + (int)fontHeight + 13;
                //RectangleF rectF1 = new RectangleF(startX, startY + offset, 210, 106);
                RectangleF rectF1 = new RectangleF(startX + 110, startY + offset, 410, 106);
                graphic.DrawString(footermsg, font, new SolidBrush(Color.Black), rectF1);

                //offset = offset + Convert.ToInt32(rectF1.Height) + 7;
                //graphic.DrawString(footermsg, font, new SolidBrush(Color.Black), startX, startY + offset);
                //////Logo Here Draw icon to screen.
                ////offset = offset + (int)fontHeight + 7;
                ////e.Graphics.DrawIcon(new Icon("Rockettheme-Ecommerce-Sale.ico"), startX, startY + offset);

            }
            catch (Exception ex)
            {
                string aaaa = ex.Message;
            }

        }

        private void btnSaveOnly_Click(object sender, EventArgs e)
        {
            if (txtPaidAmount.Text == "00" || txtPaidAmount.Text == "0" || txtPaidAmount.Text == string.Empty)
            {
                MessageBox.Show("Please insert paid amount. \n  If you want full due transaction \n Please insert 0.00 ", "Yes or No", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            }
            //else if (Convert.ToInt32(txtInvoice.Text) >= 53)
            //{
            //    MessageBox.Show("Sorry ! Demo version has limited transaction \n Please buy it \n contact at : citkar@live.com \n https://goo.gl/Hs7XsD", "Yes or No", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            //}
            else
            {
                try
                {
                    //Save payment info into sales_payment table
                    payment_item(lblTotalPayable.Text, txtChangeAmount.Text, txtDueAmount.Text, CombPayby.Text, dtSalesDate.Text, lblCustID.Text, txtCustName.Text);

                    ///// save sales items one by one 
                    sales_item(dtSalesDate.Text);
                    MessageBox.Show("Successfully has been saved ");
                    //btnCompleteSalesAndPrint.Enabled = false;
                    //btnSaveOnly.Text = "Done";
                    //btnSaveOnly.Enabled = false;

                    //Clean Datagridview and Back to sales cart
                    dgrvSalesItemList.Rows.Clear();
                    DiscountCalculation();
                    vatcal();
                    //   this.tabPageSR_Payment.Parent = null; //Hide payment tab
                    //tabSRcontrol.SelectedTab = tabPageSR_Counter;

                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }
            }
        }

        private void btnSuspend_Click_1(object sender, EventArgs e)
        {
            try
            {
                dgrvSalesItemList.Rows.Clear();
                dgrvSalesItemList.Visible = false;
                // lblTotalItems.Text = "0";
                txtDiscountRate.Text = "0";
                lbloveralldiscount.Text = "0";
                DiscountCalculation();
                vatcal();
                btnCompleteSalesAndPrint.Enabled = false;
                btnSaveOnly.Enabled = false;
                btnPayment.Enabled = false;
                //tabPageSR_Counter.Text = "Terminal";
                txtBarcodeReaderBox.Focus();
                //  this.tabPageSR_Payment.Parent = null; //Hide payment tab
            }
            catch
            {
            }
        }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            if (lblTotalPayable.Text == "00" || lblTotalPayable.Text == "0" || lblTotalPayable.Text == string.Empty)
            {
                MessageBox.Show("Sorry ! You don't have enough product in Item cart \n  Please Add to cart", "Yes or No", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            }
            else
            {
                ////Save payment info into sales_payment table
                payment_item(lblTotalPayable.Text, "0", "0", "Cash", DateTime.Now.ToString("yyyy-MM-dd").ToString(), "10000009", "Guest");

                ///// save sales items one by one  
                sales_item(DateTime.Now.ToString("yyyy-MM-dd").ToString());

                //btnPayment.Enabled = false;


                ///// // Open Print Invoice
                parameter.autoprintid = "1";
                POSPrintRpt go = new POSPrintRpt(txtInvoice.Text);
                go.ShowDialog();

                dgrvSalesItemList.Rows.Clear();
                // lblTotalItems.Text = "0";
                DiscountCalculation();
                vatcal();
                btnCompleteSalesAndPrint.Enabled = false;
                btnPayment.Enabled = false;
                btnSaveOnly.Enabled = false;
                //  this.tabPageSR_Payment.Parent = null; //Hide payment tab
            }
        }
    }
}
