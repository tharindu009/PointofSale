﻿using System;
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
    public partial class addNewCustomer : KryptonForm
    {
        public addNewCustomer()
        {
            InitializeComponent();
        }

        public string CustID
        {
            set
            {
                lblCustID.Text = value;
                //lnkCustomers.Visible = false;
                dtgviewCusttrxHistory.Visible = true;
                //lblcuthistorylabel.Visible = true;
                //lbltoplabel.Visible = true;

            }
            get
            {
                return lblCustID.Text;

            }
        }

        public string CustName
        {
            set
            {
                txtCustomerName.Text = value;
                btnSave.Text = "Update";
            }
            get
            {
                return txtCustomerName.Text;
            }
        }

        public string CustPhone
        {
            set
            {
                txtPhone.Text = value;

            }
            get
            {
                return txtPhone.Text;

            }
        }

        public string City
        {
            set
            {
                txtCity.Text = value;
            }
            get
            {
                return txtCity.Text;
            }
        }


        public string Email
        {
            set
            {
                txtEmailAddress.Text = value;
            }
            get
            {
                return txtEmailAddress.Text;
            }
        }

        public string CustAddress
        {
            set
            {
                txtCustomerAddress.Text = value;
            }
            get
            {
                return txtCustomerAddress.Text;
            }
        }

        public string PeopleType
        {
            set
            {

                CombPeopleType.Text = value;
            }
            get
            {
                return CombPeopleType.Text;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
                this.Close();
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void clearform()
        {
            CombPeopleType.Text = string.Empty;
            txtCity.Text = string.Empty;
            txtCustomerName.Text = string.Empty;
            txtCustomerAddress.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtEmailAddress.Text = string.Empty;
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblCustID.Text == ".")
                {
                    // if (txtPeopleID.Text == "") { MessageBox.Show("Please Fill ID"); txtPeopleID.Focus(); } else
                    if (txtCustomerName.Text == "") { MessageBox.Show("Please Fill Name"); txtCustomerName.Focus(); }
                    else if (txtPhone.Text == "") { MessageBox.Show("Please Fill Phone"); txtPhone.Focus(); }
                    else if (CombPeopleType.Text == "") { MessageBox.Show("Please Fill People Type"); CombPeopleType.Focus(); }
                    else if (txtCity.Text == "" && CombPeopleType.Text == "Customer") { MessageBox.Show("Please Fill Vehicle Reg No"); txtCity.Focus(); }
                    else if (txtCustomerAddress.Text == "") { MessageBox.Show("Please Fill  Address"); txtCustomerAddress.Focus(); }
                    else
                    {
                        string sqlCmd = "insert into tbl_customer (Name, EmailAddress, Phone, address, City, PeopleType )  values ('" + txtCustomerName.Text + "', '" + txtEmailAddress.Text + "', '" + txtPhone.Text + "', '" + txtCustomerAddress.Text + "', '" + txtCity.Text + "', '" + CombPeopleType.Text + "')";
                        DAL.DataAccessManager.ExecuteSQL(sqlCmd);
                        MessageBox.Show("Successfully saved");
                        clearform();

                    }
                }
                else  //Update 
                {
                    string sqlUpdateCmd = "update tbl_customer set Name = '" + txtCustomerName.Text + "', EmailAddress= '" + txtEmailAddress.Text + "', address = '" + txtCustomerAddress.Text + "', Phone = '" + txtPhone.Text + "', City = '" + txtCity.Text + "' , PeopleType = '" + CombPeopleType.Text + "'   where ID = '" + lblCustID.Text + "'";
                    DAL.DataAccessManager.ExecuteSQL(sqlUpdateCmd);
                    clearFields();
                    MessageBox.Show("Successfully Updated");
                }


            }
            catch (Exception exp)
            {
                MessageBox.Show("Sorry\r\n this id already added \n\n " + exp.Message);
            }

        }

        private void clearFields()
        {
            txtCity.Text = "";
            txtCustomerAddress.Text = "";
            txtCustomerName.Text = "";
            txtEmailAddress.Text = "";
            txtPhone.Text = "";
            lblCustID.Text = ".";
            btnSave.Text = "Save";

        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void addNewCustomer_Load(object sender, EventArgs e)
        {
            try
            {
                //string sql = "  select  sales_id as 'Invo_No' , sales_time as Date , payment_amount as Total , " +
                //            "   (payment_amount - due_amount) as 'Paid Amount' ,  payment_type as 'Payment Type' , " +
                //            "   due_amount as Due, emp_id as 'Sold by' ,    C_id  as Contact , Comment as 'Cust Name/Comment' " +
                //            "   from sales_payment   where C_id = '" + lblCustID.Text + "' order by  sales_id desc";
                string sql = "SELECT [ID],[Name],[EmailAddress],[Phone],[Address],[City] as [Vehicle_Reg],[PeopleType] FROM [tbl_customer] order by Name asc";
                DAL.DataAccessManager.ExecuteSQL(sql);
                DataTable dt1 = DAL.DataAccessManager.GetDataTable(sql);
                dtgviewCusttrxHistory.DataSource = dt1;
                //------
                DataGridViewButtonColumn Edit = new DataGridViewButtonColumn();
                dtgviewCusttrxHistory.Columns.Add(Edit);
                Edit.HeaderText = "Edit";
                Edit.Text = "Edit";
                Edit.Name = "Edit";
                Edit.ToolTipText = "Edit this category";
                Edit.UseColumnTextForButtonValue = true;
                Edit.Width = 30;

                DataGridViewButtonColumn del = new DataGridViewButtonColumn();
                dtgviewCusttrxHistory.Columns.Add(del);
                del.HeaderText = "Del";
                del.Text = "X";
                del.Name = "del";
                del.ToolTipText = "Delete this category";
                del.UseColumnTextForButtonValue = true;
                del.Width = 30;

                DataGridViewColumn ColID = dtgviewCusttrxHistory.Columns[0];
                //ColID.Width = 31;
                DataGridViewColumn ColName = dtgviewCusttrxHistory.Columns[1];
                //ColName.Width = 220;
            }
            catch
            {
            }
        }

        private void dtgviewCusttrxHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == dtgviewCusttrxHistory.Columns["Edit"].Index && e.RowIndex >= 0)
                {
                    foreach (DataGridViewRow row1 in dtgviewCusttrxHistory.SelectedRows)
                    {
                        DataGridViewRow row = dtgviewCusttrxHistory.Rows[e.RowIndex];
                        string Name = row.Cells["Name"].Value.ToString();
                        string Contact = row.Cells["Phone"].Value.ToString();
                        string Email = row.Cells["EmailAddress"].Value.ToString();
                        string Address = row.Cells["Address"].Value.ToString();
                        string City = row.Cells["Vehicle_Reg"].Value.ToString();
                        string CusID = row.Cells["ID"].Value.ToString();
                        string Type = row.Cells["PeopleType"].Value.ToString();


                        txtCustomerName.Text = Name;
                        txtPhone.Text = Contact;
                        txtEmailAddress.Text = Email;
                        txtCity.Text = City;
                        CombPeopleType.Text = Type;
                        lblCustID.Text = CusID;
                        txtCustomerAddress.Text = Address;

                        btnSave.Text = "Update";
                    }
                }

                //Delete
                if (e.ColumnIndex == dtgviewCusttrxHistory.Columns["del"].Index && e.RowIndex >= 0)
                {
                    foreach (DataGridViewRow rowdel in dtgviewCusttrxHistory.SelectedRows)
                    {
                        DialogResult result = MessageBox.Show("Do you want to Delete the record?", "Yes or No", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                        if (result == DialogResult.Yes)
                        {

                            string sqldel = " delete from tbl_customer where ID = '" + rowdel.Cells[2].Value.ToString() + "'";
                            DAL.DataAccessManager.ExecuteSQL(sqldel);
                            MessageBox.Show("Customer/Supplier Deleted");
                            string sql = "SELECT [ID],[Name],[EmailAddress],[Phone],[Address],[City] as [Vehicle_Reg],[PeopleType] FROM [tbl_customer] order by Name asc";
                            DAL.DataAccessManager.ExecuteSQL(sql);
                            DataTable dt1 = DAL.DataAccessManager.GetDataTable(sql);
                            dtgviewCusttrxHistory.DataSource = dt1;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dtgviewCusttrxHistory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == dtgviewCusttrxHistory.Columns["Edit"].Index && e.RowIndex >= 0)
                {
                    foreach (DataGridViewRow row1 in dtgviewCusttrxHistory.SelectedRows)
                    {
                        DataGridViewRow row = dtgviewCusttrxHistory.Rows[e.RowIndex];
                        string Name = row.Cells["Name"].Value.ToString();
                        string Contact = row.Cells["Phone"].Value.ToString();
                        string Email = row.Cells["EmailAddress"].Value.ToString();
                        string Address = row.Cells["Address"].Value.ToString();
                        string City = row.Cells["City"].Value.ToString();
                        string CusID = row.Cells["ID"].Value.ToString();
                        string Type = row.Cells["PeopleType"].Value.ToString();


                        txtCustomerName.Text = Name;
                        txtPhone.Text = Contact;
                        txtEmailAddress.Text = Email;
                        txtCity.Text = City;
                        CombPeopleType.Text = Type;
                        lblCustID.Text = CusID;
                        txtCustomerAddress.Text = Address;

                        btnSave.Text = "Update";
                    }
                }

            }
            catch
            {

            }
        }
    }
}
