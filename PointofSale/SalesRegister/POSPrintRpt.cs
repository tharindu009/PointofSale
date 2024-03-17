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
using Microsoft.Reporting.WinForms;
using PointofSale.BAL;

namespace PointofSale.SalesRegister
{
    public partial class POSPrintRpt : KryptonForm
    {
        public POSPrintRpt()
        {
            InitializeComponent();
            
        }

        public POSPrintRpt(string saleno)
        {
            InitializeComponent();
            toolsaleno.Text = saleno;
        }

        bool DirectPrint = false;

        public POSPrintRpt(string saleno, bool printID)
        {
            DirectPrint = printID;
        }

        private void POSPrintRpt_Load(object sender, EventArgs e)
        {
            try
            {
                //reportViewer1.Reset();
                string sql = " SELECT  sp.sales_id AS salesid, sp.payment_type AS paytype, sp.payment_amount AS Payamount, " +
                             " sp.change_amount AS charAmt, sp.due_amount AS due, sp.dis, sp.vat, sp.sales_time AS s_time,  " +
                             " sp.c_id AS custID, sp.emp_id AS empID, sp.comment AS Note, sp.TrxType, si.sales_id,si.item_id,  " +
                             " si.itemName, si.Qty, si.RetailsPrice, si.Total,si.profit, si.sales_time , sp.Shopid, tl.*,c.* ,  " +
                             " CASE     " +
                             " WHEN si.taxapply = 1 THEN 'TX'  " +
                             " ELSE ''  " +
                             " END 'TaxApply'  " +
                             ",j.Mileage,j.VehicleNo" +
                             " FROM            sales_payment sp " +
                             " INNER JOIN   sales_item si " +
                             " ON sp.sales_id  = si.sales_id " +
                             " INNER JOIN tbl_terminalLocation tl " +
                             " ON sp.Shopid  = tl.Shopid " +
                             " INNER JOIN tbl_customer c " +
                             " ON  sp.c_id  = c.ID " +
                             "LEFT OUTER JOIN job_card j" +
                              " ON j.JobNo = sp.Job_id" +
                             " Where sp.sales_id  = '" + toolsaleno.Text + "'  ";

                DAL.DataAccessManager.ExecuteSQL(sql);
                DataTable dt = DAL.DataAccessManager.GetDataTable(sql);

                string milage = dt.Rows[0][42].ToString();
                string vehicleno = dt.Rows[0][43].ToString();

                string cusID = dt.Rows[0][8].ToString();
                string CusDetail = "";

                //ReportParameter[] parameters = new ReportParameter[2];
                //parameters[0] = new ReportParameter("Milage", milage);

                //parameters[1] = new ReportParameter("VehicleNo", vehicleno);
                //this.reportViewer1.LocalReport.SetParameters(parameters);

                ReportDataSource reportDSDetail = new ReportDataSource("POSPRINTDataSet", dt);
                this.reportViewer1.LocalReport.DataSources.Clear();
                this.reportViewer1.LocalReport.DataSources.Add(reportDSDetail);
                this.reportViewer1.LocalReport.Refresh();
                this.reportViewer1.SetDisplayMode(DisplayMode.Normal);
                this.reportViewer1.ZoomMode = ZoomMode.PageWidth;
                //this.reportViewer1.ZoomPercent = 80;
                this.reportViewer1.RefreshReport();



                if (parameter.autoprintid == "1")
                {
                    timerpregress.Interval = 100;
                    timerpregress.Start();
                    //this.reportViewer1.PrintDialog();
                }
                else
                {
                    timerpregress.Stop();
                    toolstrpProgressCount.Visible = false;
                    toolStripProgressBar1.Visible = false;
                    btnstopPrint.Visible = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void prgressbar()
        {

            toolStripProgressBar1.Increment(10);
            toolstrpProgressCount.Text = " " + toolStripProgressBar1.Value.ToString() + "%";
            if (toolStripProgressBar1.Value == toolStripProgressBar1.Maximum)
            {
                timerpregress.Stop();
                this.reportViewer1.PrintDialog();
                timerpregress.Stop();
            }
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void timerpregress_Tick(object sender, EventArgs e)
        {
            try
            {
                prgressbar();
            }
            catch
            {
            }
        }

        private void btnstopPrint_Click(object sender, EventArgs e)
        {
            if (btnstopPrint.Text != "START")
            {
                timerpregress.Stop();
                btnstopPrint.Text = "START";
            }
            else
            {
                btnstopPrint.Text = "STOP";
                timerpregress.Start();
            }
        }

        private void reportViewer1_RenderingComplete(object sender, RenderingCompleteEventArgs e)
        {
            if (parameter.autoprintid == "1")
            {
                //reportViewer1.PrintDialog();
            }
        }
    }
}
