using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointofSale.DAL
{
    class DataAccessManager
    {
        // Use for PointofSale.exe.config file   you can change Database server info after compile/Debug 


        //static string _ConnectionString = "Data Source=(local);Initial Catalog=kts;User Id=sa;Password=Welcome@2020;"; //-----------------------------Production
        static string _ConnectionString = "Data Source=DESKTOP-4GPK025;Initial Catalog=kts;Trusted_Connection=True;"; //---------------------------Test

       
        //If your MSSQL server have window authentication (MSSQL server open without Password) please use this one 
        //static string _ConnectionString = "Data Source=(local); Initial Catalog=APOSDB; "; 

        static SqlConnection _Connection = null;
        public static SqlConnection Connection
        {
            get
            {
                if (_Connection == null)
                {
                    _Connection = new SqlConnection(_ConnectionString);
                    _Connection.Open();

                    return _Connection;
                }
                else if (_Connection.State != System.Data.ConnectionState.Open)
                {
                    _Connection.Open();

                    return _Connection;
                }
                else
                {
                    return _Connection;
                }
            }
        }

        public static DataSet GetDataSet(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, Connection);
            SqlDataAdapter adp = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();
            adp.Fill(ds);
            Connection.Close();

            return ds;
        }

        public static DataTable GetDataTable(string sql)
        {
            Console.WriteLine(sql);
            DataSet ds = GetDataSet(sql);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            return null;
        }

        public static int ExecuteSQL(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, Connection);
            return cmd.ExecuteNonQuery();
        }


        public static int AddItem(string pid, string pname, double quan, double cprice, double sprice, double ctotalpri, double rtotalpri, string ComboCategory, string cmbSupplier, byte[] ItemImage,
                                    double discount, int taxapply, string cmboShopid, int kitchenDisplaythisitem)
        {
            string sql1 = " insert into purchase (product_id, product_name, product_quantity, cost_price, retail_price, total_cost_price, " +
                                       " total_retail_price, category, supplier , imagename, discount, taxapply, Shopid , status) " +
                                       " values (@pid,@pname,@quan,@cprice,@sprice,@ctotalpri, " +
                                       " @rtotalpri,@ComboCategory,@cmbSupplier,@ItemImage, " +
                                       " @discount,@taxapply ,@cmboShopid,@kitchenDisplaythisitem)";

            SqlCommand cmd = new SqlCommand(sql1, Connection);

            cmd.Parameters.AddWithValue("@pid", pid);
            cmd.Parameters.AddWithValue("@pname", pname);
            cmd.Parameters.AddWithValue("@quan", quan);
            cmd.Parameters.AddWithValue("@cprice", cprice);
            cmd.Parameters.AddWithValue("@sprice", sprice);
            cmd.Parameters.AddWithValue("@ctotalpri", ctotalpri);
            cmd.Parameters.AddWithValue("@rtotalpri", rtotalpri);
            cmd.Parameters.AddWithValue("@ComboCategory", ComboCategory);
            cmd.Parameters.AddWithValue("@cmbSupplier", cmbSupplier);
            cmd.Parameters.AddWithValue("@ItemImage", ItemImage);
            cmd.Parameters.AddWithValue("@discount", discount);
            cmd.Parameters.AddWithValue("@taxapply", taxapply);
            cmd.Parameters.AddWithValue("@cmboShopid", cmboShopid);
            cmd.Parameters.AddWithValue("@kitchenDisplaythisitem", kitchenDisplaythisitem);


            int row = cmd.ExecuteNonQuery();
            Connection.Close();

            return row;
        }


        // convert image to byte array
        public static byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }
    }
}
