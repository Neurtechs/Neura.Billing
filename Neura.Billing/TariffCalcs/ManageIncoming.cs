using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Neura.Billing.GlobalVar;
using System.Data;
using Neura.Billing.Data;

namespace Neura.Billing.TariffCalcs
{
    class ManageIncoming
    {
        private static readonly log4net.ILog Log =
           log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static int GetIncomingReadings(int myMeteringInterval)
        {
            int myNodeId;

            //Read the data from the Intermediate table
            int readingCount = IncomingConnections.ConnectIntermediateReadings(out DataTable dtIntermediate);
            //int myReadingsType = 0;
            int myMeterType = 0;
            int myRecordId = 0;
            double usage = 0;  //Acc usage
            double max = 0; //demand
            double myReading = 0;
            //double myPreviousReading = 0;
            DateTime myReadingDate;
            DateTime myPreviousReadingDate;
            if (readingCount > 0)
            {
                foreach (DataRow dr in dtIntermediate.Rows)
                {
                    int myReadingsType = 0;
                    //Get number of ReadingTypes
                    myNodeId = Convert.ToInt32(dr["NodeId"]);
                    myReadingsType = Convert.ToInt16(dr["ReadingsType"]);
                    myReadingDate = Convert.ToDateTime(dr["TempDate"]);
                    myRecordId = Convert.ToInt16(dr["Oid"]);
                    myReading = Convert.ToDouble(dr["Reading"]);


                    myMeterType = Convert.ToInt16(dr["MeterType"]);
                    int countReadingsTypes = UtilityConnections.CountReadingType(myNodeId);

                    if (countReadingsTypes == 1)
                    {
                        //get the node data
                        IncomingConnections.SelectNode(myNodeId, myReadingsType, out DataTable dtSelectNode);

                        //metertype: 0 = kWhAcc, 1 = kWhP, 2 = kW, 3 = klAcc, 4 = klP, 5 = NA
                        if (myMeterType == 0 || myMeterType == 3)
                        {
                            if (bLogTest == true) { Log.Info("Node " + myNodeId + " : Meter type = accumulated value"); }
                            usage = CalcDiffAcc.usageDelta(myNodeId, myReadingsType, myReadingDate, myReading, myMeteringInterval);

                        }
                        else if (myMeterType == 2)
                        {
                            if (bLogTest == true) { Log.Info("NodeId " + myNodeId + " : Meter type = integrated kW"); }
                            int iPeriod = 60 / myMeteringInterval;
                            usage = myReading / iPeriod;
                            max = myReading;
                        }
                        else
                        {
                            if (bLogTest == true) { Log.Info("NodId " + myNodeId + " : Meter type = period value"); }
                            usage = myReading;
                        }
                        if (bLogTest == true) { Log.Info("Usage calculated = " + usage); }



                        //To do - code for MD calcs as necessary

                        // Update ReadingIntermediate Status
                        SaveConnections.UpdateIntermediateStats(myRecordId);
                        if (bLogTest == true) { Log.Info("Update intermediate table Status to 2"); }


                        // Delete old records
                        myPreviousReadingDate = myReadingDate.AddMinutes(-10 * myMeteringInterval);
                        if (bLogTest == true) { Log.Info("Delete intermediate records older than 10 periods"); }
                        SaveConnections.DeleteIntermediate(myNodeId, myReadingsType, myPreviousReadingDate);

                        //Save to Usage Table
                        SaveConnections.SaveUsage(myReadingsType, usage, myReadingDate, myNodeId, max);
                        if (bLogTest == true) { Log.Info("Save data to Usage table ++++++++++++++++++"); }
                    }
                    else
                    {
                        //To Do code to deal with this - multiple reading types
                        //Make sure all are received per period before marking as processed

                        //Temporary -- use above code here for testing


                        //get the node data
                        IncomingConnections.SelectNode(myNodeId, myReadingsType, out DataTable dtSelectNode);

                        //metertype: 0 = kWhAcc, 1 = kWhP, 2 = kW, 3 = klAcc, 4 = klP, 5 = NA
                        if (myMeterType == 0 || myMeterType == 3)
                        {
                            if (bLogTest == true) { Log.Info("Node " + myNodeId + " : Meter type = accumulated value"); }
                            usage = CalcDiffAcc.usageDelta(myNodeId, myReadingsType, myReadingDate, myReading, myMeteringInterval);

                        }
                        else if (myMeterType == 2)
                        {
                            if (bLogTest == true) { Log.Info("NodeId " + myNodeId + " : Meter type = integrated kW"); }
                            int iPeriod = 60 / myMeteringInterval;
                            usage = myReading / iPeriod;
                            max = myReading;
                        }
                        else
                        {
                            if (bLogTest == true) { Log.Info("NodId " + myNodeId + " : Meter type = period value"); }
                            usage = myReading;
                        }
                        if (bLogTest == true) { Log.Info("Usage calculated = " + usage); }



                        //To do - code for MD calcs as necessary

                        // Update ReadingIntermediate Status
                        SaveConnections.UpdateIntermediateStats(myRecordId);
                        if (bLogTest == true) { Log.Info("Update intermediate table Status to 2"); }


                        // Delete old records
                        myPreviousReadingDate = myReadingDate.AddMinutes(-10 * myMeteringInterval);
                        if (bLogTest == true) { Log.Info("Delete intermediate records older than 10 periods"); }
                        SaveConnections.DeleteIntermediate(myNodeId, myReadingsType, myPreviousReadingDate);

                        //Save to Usage Table
                        SaveConnections.SaveUsage(myReadingsType, usage, myReadingDate, myNodeId, max);
                        if (bLogTest == true) { Log.Info("Save data to Usage table ++++++++++++++++++"); }

                    }
                    //Generate  lookuptables as necessary
                    int lookUpCount = 0;
                    //int lastBool = 0;
                    int iBool = 0;

                    lookUpCount = UtilityConnections.SelectTouGroups(out
                        DataTable dtTouGroups);
                    if (lookUpCount > 0)
                    {
                        int[] LookUpId = new int[lookUpCount];
                        foreach (DataRow drR in dtTouGroups.Rows)
                        {
                            //TOU Lookup table
                            LookUpId[iBool] = Convert.ToInt16(drR["TOULookupId"]);
                            LookUpTable.GenerateLookUp(LookUpId[iBool], 0, myMeteringInterval);
                            LookUpTable.GenerateLookUp(LookUpId[iBool], 1, myMeteringInterval);

                            //Periods in Month
                            int month = myReadingDate.Month;
                            string sMonth = month.ToString();
                            if (sMonth.Length == 1) { sMonth = "0" + sMonth; }

                            int year = myReadingDate.Year;
                            string sYear = year.ToString();

                            DateTime myDate = Convert.ToDateTime(sYear + "/" + sMonth + "/01");
                            PeriodsInMonth.GetPeriodsInMonth(myDate, LookUpId[iBool], myMeteringInterval, out int a,
                                out int b, out int c, out int d);
                            iBool += 1;
                        }
                    }


                }
            }



            return readingCount;
        }
    }
}
