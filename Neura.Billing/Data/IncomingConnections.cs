using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;
using static Neura.Billing.GlobalVar;

namespace Neura.Billing.Data
{
    public class IncomingConnections
    {
        public static int ConnectIncomingReadings(out DataTable dtReadingsIn)
        {
            int readingCount = 0;
            //string str = "SELECT Oid, NodeId, Reading,ReadingsType,TimeStamp,UTC,Comment from Readings1  " +
            //                "Where (Comment=0) " +
            //             "ORDER BY NodeId, TimeStamp, ReadingsType  LIMIT 4";
            ////string str = "SELECT Oid, NodeId, Reading,ReadingsType,TimeStamp,UTC,Comment from Readings " +
            ////                "Where (Comment=0) " +
            ////             "ORDER BY NodeId, ReadingsType, TimeStamp";

            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("ConnectIncomingReadings", mySqlConnection);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;

            dtReadingsIn = new DataTable();
            da.Fill(dtReadingsIn);
            readingCount = dtReadingsIn.Rows.Count;

            return readingCount;
        }
        /// <summary>
        /// Returns nodeinfo
        /// </summary>
        /// <param name="NodeId"></param>
        /// <param name="dtSelectNode"></param>
        public static void SelectNode(int NodeId, int ReadingsType, out DataTable dtSelectNode)
        {
            //string str = "SELECT vnode_readingtype.* from vnode_readingtype Where NodeId = "
            //    + NodeId + " AND ReadingsType = " + ReadingsType;

            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("SelectNode", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_NodeId", NodeId);
            cmd.Parameters.AddWithValue("_ReadingsType", ReadingsType);
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;

            dtSelectNode = new DataTable();
            da.Fill(dtSelectNode);
        }
        public static int ConnectIntermediateReadings(out DataTable dtI)
        {
            //string str = "Select * from ReadingIntermediate Where Status < 2 Order by NodeId, TimeStamp ";
            //MySqlDataAdapter da = new MySqlDataAdapter(str, mySqlConnection);

            MySqlCommand cmd = new MySqlCommand("ConnectIntermediateReadings", mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = cmd;

            dtI = new DataTable();
            da.Fill(dtI);
            int count = dtI.Rows.Count;
            //int r = 0;
            //for (int i = 0; i < count ; i++)
            //{
            //    r = Convert.ToInt16(dtI.Rows[i]["ReadingsType"]);
            //    MessageBox.Show(Convert.ToString (r));
            //}

            return count;
        }
    }
}
