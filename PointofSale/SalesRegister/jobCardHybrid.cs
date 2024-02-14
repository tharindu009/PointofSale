using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace PointofSale
{
    public partial class jobCardHybrid : KryptonForm
    {
        public jobCardHybrid()
        {
            InitializeComponent();
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void jobCard_Load(object sender, EventArgs e)
        {
            pnlItemStock.Visible = false;
            generate_inv();
            BindCurrentJobs();
            ClearField();
            BindCustomer();
            BindEmployee();
            BindEmployee2();
            //cmbDiff.Visible = false;
            //ComboCategory.Visible = false;
        }

        private void BindCustomer()
        {
            string sqlCust = "select   DISTINCT  *   from tbl_customer where PeopleType = 'Customer'";
            DAL.DataAccessManager.ExecuteSQL(sqlCust);
            DataTable dtCust = DAL.DataAccessManager.GetDataTable(sqlCust);
            cmbCustomer.DataSource = dtCust;
            cmbCustomer.DisplayMember = "Name";
            cmbCustomer.ValueMember = "ID";
            cmbCustomer.Text = "Guest";
        }

        private void BindEmployee()
        {
            string sql3 = "select * from usermgt";
            DAL.DataAccessManager.ExecuteSQL(sql3);
            DataTable dt1 = DAL.DataAccessManager.GetDataTable(sql3);
            cmbEmployee.DataSource = dt1;
            cmbEmployee.DisplayMember = "Name";

            cmbEmployee2.DataSource = dt1;
            cmbEmployee2.DisplayMember = "Name";

        }

        private void BindEmployee2()
        {
            string sql3 = "select * from usermgt";
            DAL.DataAccessManager.ExecuteSQL(sql3);
            DataTable dt1 = DAL.DataAccessManager.GetDataTable(sql3);

            cmbEmployee2.DataSource = dt1;
            cmbEmployee2.DisplayMember = "Name";

        }


        private void BindCurrentJobs()
        {
            string JobQuery = @"SELECT [JobNo],[CustomerName],[VehicleReg],[CurrentMilage],[DateIn],[ActionTaken],[Status]
                                FROM [kts].[dbo].[JobHybridMaster] where [Status] = 'Open'";

            DataTable dtJob = DAL.DataAccessManager.GetDataTable(JobQuery);
            dgrvSalesItemList.DataSource = dtJob;
        }


        private void ClearField()
        {
            txtMilage.Text = "";
            txtVehicleType.Text = "";
            txtRegNo.Text = "";
            txtOther.Text = "";
            chk12v.Checked = false;
            chkAlternator.Checked = false;
            chkBattery.Checked = false;
            chkBelt.Checked = false;
            chkBoot.Checked = false;
            chkBreak.Checked = false;
            chkCoolant.Checked = false;
            chkDashboard.Checked = false;
            chkElectric.Checked = false;
            chkEngine.Checked = false;
            chkFuse.Checked = false;
            chkGear.Checked = false;
            chkOil.Checked = false;
            chkSound.Checked = false;
            
            btnFinished.Enabled = false;
            
            btnInvoice.Enabled = false;
            pnlItemStock.Visible = false;

        }

        public void generate_inv()
        {
            int id_tmp;
            string Query = "SELECT top 1 JobNo FROM JobHybridMaster order by JobNo DESC";

            DataTable dt = DAL.DataAccessManager.GetDataTable(Query);

            if (dt.Rows.Count == 0)
            {
                id_tmp = 1000;
            }
            else
            {
                id_tmp = Convert.ToInt32(dt.Rows[0][0]) + 1;
            }

            txtJobNo.Text = id_tmp.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //Insert Job Master
                int JobNo = Convert.ToInt32(txtJobNo.Text);
                string customerName = cmbCustomer.Text;
                string vehicleReg = txtRegNo.Text;
                string currentMilage = txtMilage.Text;
                DateTime DateIn = dtDateIn.Value;
                string ActionTaken = txtActionTaken.Text;
                string JobMasterQuery = "INSERT INTO[dbo].[JobHybridMaster] ([JobNo],[CustomerName],[VehicleReg],[CurrentMilage],[DateIn],[ActionTaken],[Status])" +
                                         "VALUES ('"+JobNo+"','"+customerName+"','"+vehicleReg+"','"+currentMilage+"','"+DateIn+"','"+ActionTaken+"','Open')";
                DAL.DataAccessManager.ExecuteSQL(JobMasterQuery);

                //Insert Job Detail
                //Dictionary<string, string> vehicleComponents = new Dictionary<string, string>
                //{
                //    { "hybridSystem", "No" },
                //    { "engine", "No" },
                //    { "gearBox", "No" },
                //    { "breakSystem", "No" },
                //    { "dashBoard", "No" },
                //    { "battery12", "No" },
                //    { "fuseBox", "No" },
                //    { "oil", "No" },
                //    { "electrical", "No" },
                //    { "sound", "No" },
                //    { "belts", "No" },
                //    { "coolant", "No" },
                //    { "alternator", "No" },
                //    { "shock", "No" },

                //};
                string hybridSystem = "No", engine = "No", gearBox = "No", breakSystem = "No", dashBoard = "No", battery12 = "No", fuseBox = "No", oil = "No",
                    electrical = "No", sound = "No", belts = "No", coolent = "No", alternator = "No", shock = "No";





                if (chkBattery.Checked) hybridSystem = "Yes";
                if (chkEngine.Checked) engine = "Yes";
                if (chkGear.Checked) gearBox = "Yes";
                if(chkBreak.Checked) breakSystem = "Yes";
                if (chkDashboard.Checked) dashBoard = "Yes";
                if (chk12v.Checked) battery12 = "Yes";
                if (chkFuse.Checked) fuseBox = "Yes";
                if (chkOil.Checked) oil = "Yes";
                if (chkElectric.Checked) electrical = "Yes";
                if (chkSound.Checked) sound = "Yes";
                if (chkBelt.Checked) belts = "Yes";
                if (chkCoolant.Checked) coolent = "Yes";
                if (chkAlternator.Checked) alternator = "Yes";
                if (chkBoot.Checked) shock = "Yes";

                //vehicleComponents["otherIssues"] = txtOther.Text;
                string otherIssues = "";
                otherIssues = txtOther.Text;

                string JobCardDetailQuery = "INSERT INTO[dbo].[JobHybridDetail] ([JobNo],[HybridSystem],[Engine],[GearBox],[BreakSystem],[DashBoard],[Battery12v],[FuseBox],[Oil],[Electrical],[Sound],[Belts],[Coolant],[Alternator],[Shock],[OtherIssues]) " +
                                                    "VALUES ('"+JobNo+"','"+ hybridSystem + "','"+ engine +"','"+ gearBox + "','"+  breakSystem + "','"+  dashBoard + "'" +
                                                            ",'"+  battery12 + "','"+  fuseBox + "','"+  oil + "','"+  electrical + "','"+  sound + "','"+  belts + "'" +
                                                            ",'"+  coolent + "','"+  alternator + "','"+  shock + "','"+ otherIssues + "')";
                DAL.DataAccessManager.ExecuteSQL(JobCardDetailQuery);


                //Insert Technician
                string Tec1Name = "";
                DateTime assignDate;
                string assignTime = "";
                if(cmbEmployee.Text != "-Select-")
                {
                    Tec1Name = cmbEmployee.Text;
                    assignDate = Convert.ToDateTime(AssignDt1.Value.ToString());
                    assignTime = AssignTime1.Value.ToString();

                    string tecQuery1 = "INSERT INTO [dbo].[JobTechAssign] ([JobNo],[TechName],[AssignDate],[AssignTime]) " +
                    "VALUES ('" + JobNo + "','" + Tec1Name + "','" + assignDate + "','" + assignTime + "')";

                    DAL.DataAccessManager.ExecuteSQL(tecQuery1);
                }

                string Tec2Name = "";
                DateTime assignDate2;
                string assignTime2 = "";
                if (cmbEmployee2.Text != "-Select-")
                {
                    Tec2Name = cmbEmployee2.Text;
                    assignDate2 = Convert.ToDateTime(AssignDt2.Value.ToString());
                    assignTime2 = AssignTime2.Value.ToString();

                    string tecQuery2 = "INSERT INTO [dbo].[JobTechAssign] ([JobNo],[TechName],[AssignDate],[AssignTime]) " +
                    "VALUES ('" + JobNo + "','" + Tec2Name + "','" + assignDate2 + "','" + assignTime2 + "')";

                    DAL.DataAccessManager.ExecuteSQL(tecQuery2);
                }

                //Insert Job Cart Item
                int itmCount = grdCategory.Rows.Count;
                if (itmCount != 0)
                {
                    for (int i = 0; i < itmCount-1; i++)
                    {
                        string ItemNo = grdCategory.Rows[i].Cells[0].Value.ToString();
                        int Qty = Convert.ToInt32(grdCategory.Rows[i].Cells[3].Value.ToString());
                        double Cost = Convert.ToDouble(grdCategory.Rows[i].Cells[2].Value.ToString());
                        string ItemLoc = "";

                        if (rdoOutside.Checked)
                        {
                            ItemLoc = "OutSide";
                        }
                        if (rdoStock.Checked)
                        {
                            ItemLoc = "InHouse Stock";
                        }

                        string ItemQuery = "INSERT INTO [dbo].[JobCardItems] ([JobNo],[ReplaceItemNo],[Qty],[Cost],[ItemLocation]) " +
                                            "VALUES ('" + JobNo + "','" + ItemNo + "','" + Qty + "','" + Cost + "','" + ItemLoc + "')";

                        DAL.DataAccessManager.ExecuteSQL(ItemQuery);
                    }
                }


                MessageBox.Show("Job Save Successful");
                BindCurrentJobs();
                generate_inv();
                ClearField();
            }
            catch (Exception x)
            {

                MessageBox.Show(x.Message);
            }
        }

        private void btnInvoice_Click(object sender, EventArgs e)
        {
            this.Hide();
            string JobNo = lblSelectJob.Text;
            RegisterQ qc = new RegisterQ(JobNo);
            //SalesRegister qc = new SalesRegister(JobNo);
            qc.MdiParent = this.ParentForm;
            qc.Show();
        }

        private void btnFinished_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Do you want to Complete the Job Number: "+ lblSelectJob.Text +"? ", "Yes or No", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    int JobNoUpdate = Convert.ToInt32(lblSelectJob.Text);
                    string JobUpdtQuery = "UPDATE [dbo].[JobHybridMaster] SET [Status] = 'Finished' WHERE [JobNo] = '" + JobNoUpdate + "'";
                    int a = DAL.DataAccessManager.ExecuteSQL(JobUpdtQuery);
                    btnFinished.Enabled = false;

                    BindCurrentJobs();

                    MessageBox.Show("Job Number:" + JobNoUpdate + " Successfully Completed");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSuspend_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to Suspend the Job Number: " + lblSelectJob.Text + "? ", "Yes or No", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                int JobNoUpdate = Convert.ToInt32(lblSelectJob.Text);
                string JobUpdtQuery = "UPDATE [dbo].[JobHybridMaster] SET [Status] = 'Suspend' WHERE [JobNo] = '" + JobNoUpdate + "'";
                int a = DAL.DataAccessManager.ExecuteSQL(JobUpdtQuery);
                btnFinished.Enabled = false;

                BindCurrentJobs();

                MessageBox.Show("Job Number:" + JobNoUpdate + " Successfully Suspended");
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            generate_inv();
            ClearField();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnItemList_Click(object sender, EventArgs e)
        {
            //pnlItemStock.Visible = true;
            if(rdoOutside.Checked == true)
            {
                rdoStock.Checked = false;
                pnlOutside.Visible = true;
                pnlItemStock.Visible = false;
            }
            else if(rdoStock.Checked == true)
            {
                rdoOutside.Checked = false;
                pnlItemStock.Visible = true;
                pnlOutside.Visible = false;
                string ItemQuery = "SELECT [product_id],[product_name],[product_quantity],[retail_price] FROM [purchase] where [product_quantity] > 0";
                DataTable dt = DAL.DataAccessManager.GetDataTable(ItemQuery);
                grdStockItem.DataSource = dt;

            }

        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (tabSRcontrol.TabPages.Contains(tabPageSR_Payment))//tab already present
            {
                tabSRcontrol.SelectTab(tabPage1);
                btnSave.Visible = true;
            }
            else
            {
                //tabControlMdi.TabPages.Add(tabProduct); // add removed tab
                //tabControlMdi.SelectTab(tabProduct);    // select by name
            }
        }

        private void rdoOutside_CheckedChanged(object sender, EventArgs e)
        {
            if(rdoOutside.Checked == true)
            {
                rdoStock.Checked = false;
            }
        }

        private void rdoStock_CheckedChanged(object sender, EventArgs e)
        {
            if(rdoStock.Checked == true)
            {
                rdoOutside.Checked = false;
            }
        }

        private void kryptonButton1_Click_1(object sender, EventArgs e)
        {
            lblItemNo.Text = txtOutsideItemNo.Text.ToString();
            txtItemName.Text = txtOutsideItemName.Text.ToString();
            lblQty.Text = txtOutsideItemQty.Text.ToString();
            pnlOutside.Visible = false;
            this.grdCategory.Rows.Add(lblItemNo.Text, txtItemName.Text, txtSellPrice.Text,lblQty.Text);
        }

        private void kryptonButton4_Click(object sender, EventArgs e)
        {
            pnlItemStock.Visible = false;
        }

        private void kryptonButton3_Click(object sender, EventArgs e)
        {
            pnlOutside.Visible = false;
        }

        private void kryptonTextBox1_TextChanged(object sender, EventArgs e)
        {
            string ItemQuery = "SELECT [product_id],[product_name],[product_quantity],[retail_price] FROM [purchase] where [product_quantity] > 0 and ([product_name] like '%" + txtItemSearch.Text + "%' or [product_id] like '%" + txtItemSearch.Text + "%')";
            DataTable dtItem = DAL.DataAccessManager.GetDataTable(ItemQuery);
            grdStockItem.DataSource = dtItem;
        }

        private void grdStockItem_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {

        }

        private void label42_Click(object sender, EventArgs e)
        {

        }

        private void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            string customerID = cmbCustomer.SelectedValue.ToString();
            string Query = "select sum(due_amount) as Due from sales_payment where c_id = '" + customerID + "' and due_amount != '0'";
            DataTable dtDueAmount = DAL.DataAccessManager.GetDataTable(Query);
            if (dtDueAmount.Rows.Count != 0)
            {
                lblDueAmount.Text = dtDueAmount.Rows[0][0].ToString();
            }
            else
            {
                lblDueAmount.Text = "0.00";
            }
        }

        private void kryptonButton5_Click(object sender, EventArgs e)
        {

        }

        private void grdStockItem_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow row = grdStockItem.Rows[e.RowIndex];
                lblProID.Text = row.Cells[0].Value.ToString();
                lblProName.Text = row.Cells[1].Value.ToString();
                lblSelPrice.Text = row.Cells[3].Value.ToString();
                //lbpaidamt.Text = row.Cells[4].Value.ToString();
                //lbDueAmount.Text = row.Cells[6].Value.ToString();
                //lbcontact.Text = row.Cells[9].Value.ToString();
                //pnlReceiveDue.Visible = true;
                this.grdCategory.Rows.Add(lblProID.Text, lblProName.Text, lblSelPrice.Text, "1");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void kryptonButton5_Click_1(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void dgrvSalesItemList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgrvSalesItemList.Rows[e.RowIndex];
            string jobNo = row.Cells[0].Value.ToString();
            lblSelectJob.Text = jobNo;
            btnFinished.Enabled = true;
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void kryptonButton6_Click(object sender, EventArgs e)
        {
            string JobUpdtQuery = "UPDATE [dbo].[JobHybridMaster] SET [Status] = 'Suspend' WHERE [JobNo] = '" + lblSelectJob + "'";
            int a = DAL.DataAccessManager.ExecuteSQL(JobUpdtQuery);
            btnFinished.Enabled = false;

            BindCurrentJobs();
        }

        private void dgrvSalesItemList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgrvSalesItemList.Rows[e.RowIndex];
            string jobNo = row.Cells[0].Value.ToString();
            lblSelectJob.Text = jobNo;
            btnFinished.Enabled = true;
        }
    }
}