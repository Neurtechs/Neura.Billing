using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static Neura.Billing.GlobalVar;

namespace Neura.Billing
{
    public partial class frmLogin : DevExpress.XtraEditors.XtraForm
    {
        private string connectionString;
        public frmLogin()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //textBoxUser.Text = "Dale"; //on server
            //textBoxServer.Text = "localhost";
            //textBoxPassword.Text = "D@lelieb01";
            textBoxUser.Text = "Dale";
            textBoxServer.Text = "neura.dyndns.org,3306";
            textBoxPassword.Text = "D@lelieb01";
            //textBoxUser.Text = "root";
            //textBoxServer.Text = "localhost";
            //textBoxPassword.Text = "D@lelieb01";
        }

        

        private void simpleButtonLogin_Click_1(object sender, EventArgs e)
        {
            connectionString = "Server = " + textBoxServer.Text + "; User ID = " + textBoxUser.Text + "; Password = " +
                               textBoxPassword.Text;
            connectionString = connectionString +
                               ";  Persist Security Info = true; Charset = utf8; Database = Neura; Connection Timeout=1800 ";

            //connectionString =
            //     "Server = localhost; User ID = root; Password = D@lelieb01;  Persist Security Info = true; Charset = utf8; Database = Neura; Connect Timeout=360";
            mySqlConnection = new MySqlConnection(connectionString);
            
            if (mySqlConnection.State == ConnectionState.Closed)
            {
                try
                {
                    mySqlConnection.Open();
                    MessageBox.Show("Connected to DB");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Check you connection string and comms to DB");
                    MessageBox.Show(ex.Message);
                    goto ExitHere;
                }
            }

           
            Main f = new Main();
            f.ShowDialog();
            ExitHere:;
        }
    }
}
