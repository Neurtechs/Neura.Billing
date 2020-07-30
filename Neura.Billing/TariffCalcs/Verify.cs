using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Neura.Billing.Data;
using static Neura.Billing.GlobalVar;

namespace Neura.Billing.TariffCalcs
{
    class Verify
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
           (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// This method verifies incoming readings and generates missing values for cumulative meters
        /// </summary>
        /// <remarks>
        /// Called from the main start page timer event </remarks>
        /// <returns>Number of readings</returns>
        /// <param name="myMeteringInterval"></param>
        /// <returns>
        /// Number of incoming readings</returns>
        public static int VerifyIncoming(int myMeteringInterval)
        {

            int readingCount = IncomingConnections.ConnectIncomingReadings(out DataTable dtReadingsIn);
            if (readingCount == 0)
            {
                return 0;
            }
            if (bLogTest == true)
            {
                Log.Info("Number of readings found in Readings table = " + readingCount);
            }
            int myNodeId = 0;
            int myId = 0; //record Id
            double myReading = 0;
            double myPreviousReading = 0;
            double myDiffReading = 0;
            int myUTC = 0;
            int myReadingsType = 0;
            DateTime myTimeStamp;
            DateTime myReadingDate;
            DateTime myPreviousReadingDate;
            int myMeterType = 0;
            double timeDiff = 0;
            double periods = 0;
            int comment = 1; //0=NotRead,1=DataRead, 2=DuplicateReadingSkipped

            foreach (DataRow dr in dtReadingsIn.Rows)
            {
                myId = Convert.ToInt32(dr["Oid"]);
                myNodeId = Convert.ToInt32(dr["NodeId"]);
                myReading = Convert.ToDouble(dr["Reading"]);
                myUTC = Convert.ToInt16(dr["UTC"]);
                myReadingsType = Convert.ToInt16(dr["ReadingsType"]);
                myTimeStamp = Convert.ToDateTime(dr["TimeStamp"]);
                if (bLogTest == true)
                {
                    Log.Info("-------------------------------------------------");
                    Log.Info("New reading at " + myTimeStamp + " For " + myNodeId);
                }
                if (myUTC == 1) //calculate readingDate in local time
                {
                    myReadingDate = CalcTime.ToNearest(myTimeStamp, myMeteringInterval, true, true);
                    //(DateTime timeStamp, bool getNearest, bool UTC = false)

                    if (bLogTest == true)
                    {

                        Log.Info("UTC Timestamp " + myTimeStamp + " converted to local time ReadingDate " + myReadingDate);
                    }
                    myTimeStamp = CalcTime.ToNearest(myTimeStamp, myMeteringInterval, false, true);

                }
                else
                {
                    myReadingDate = CalcTime.ToNearest(myTimeStamp, myMeteringInterval, true);
                    if (bLogTest == true)
                    {
                        Log.Info("Timestamp " + myTimeStamp + " converted to ReadingDate " + myReadingDate);
                    }
                }

                IncomingConnections.SelectNode(myNodeId, myReadingsType, out DataTable dtSelectNode); //Get all node data

                myMeterType = Convert.ToInt16(dtSelectNode.Rows[0]["MeterType"]);  //Get meter type
                                                                                   //MeterType 0=kWhAcc,1=kWhP,2=kW,3=klAcc,4=klP,5=NA
                                                                                   //myReadingsType = Convert.ToInt16(dtSelectNode.Rows[0]["ReadingsType"]);
                myPreviousReadingDate = UtilityConnections.FindlLastReading(myNodeId, myReadingsType); //Previous reading for this meter/channel
                                                                                                       // If no reading found, 1980/01/01 is returned

                if (myPreviousReadingDate == Convert.ToDateTime("1980/01/01"))
                {
                    //Find Start Readings
                    UtilityConnections.GetStartReading(myNodeId, myReadingsType, out double myStartReading);
                    if (bLogTest == true)
                    {
                        Log.Info("For Node = " + myNodeId + " ReadingsType = " + myReadingsType + " ===========");
                        Log.Info("No previous readings found. Attempting to get start readings ------");
                        Log.Info("Start Reading= " + myStartReading);

                    }
                    myPreviousReadingDate = myReadingDate.AddMinutes(-myMeteringInterval);  //Set up a dummy reading date for this reading
                }
                else
                {
                    //Get Previous Reading
                    UtilityConnections.SelectIntermediateByReadingDate(myNodeId, myPreviousReadingDate, myReadingsType,
                        out DataTable dtSelectReading);
                    myPreviousReading = Convert.ToDouble(dtSelectReading.Rows[0]["Reading"]);
                    myPreviousReadingDate = Convert.ToDateTime(dtSelectReading.Rows[0]["TempDate"]);



                    if (bLogTest == true)
                    {
                        Log.Info("");
                        Log.Info("For Node = " + myNodeId);
                        Log.Info("Previous readings found.------");
                        Log.Info("ReadingDate= " + myPreviousReadingDate);
                        Log.Info("Reading= " + myPreviousReading);

                    }
                }
                timeDiff = myReadingDate.Subtract(myPreviousReadingDate).TotalMinutes;
                double tempReading = myPreviousReading;

                if ((timeDiff - myMeteringInterval) != 0 && timeDiff != 0)
                {
                    if (bLogTest == true)
                    {
                        Log.Info("Missing Readings.------");
                    }
                    //Get number of periods between readings
                    periods = timeDiff / myMeteringInterval;
                    //MeterType:  0=kWhAcc,1=kWhP,2=kW,3=klAcc,4=klP,5=NA
                    if (myMeterType == 0 || myMeterType == 3)
                    {
                        myDiffReading = (myReading - myPreviousReading) / periods;
                        if (bLogTest == true)
                        {
                            Log.Info("Cumulative meter. Spread across intervals------");
                            Log.Info("Missing periods = " + periods);
                            Log.Info("Average diff in reading = " + myDiffReading);

                        }
                        for (int i = 1; i < periods; i++) //All but last
                        {
                            tempReading += myDiffReading;
                            myPreviousReadingDate = myPreviousReadingDate.AddMinutes(myMeteringInterval);
                            SaveConnections.SaveIntermediateReadings(myNodeId, tempReading, myPreviousReadingDate, myReadingsType, myMeterType, 1);


                            if (bLogTest == true)
                            {
                                Log.Info("Saving intermediate values for  = " + myPreviousReadingDate);
                            }
                            //tempReading += myDiffReading;
                            myPreviousReading = tempReading;
                        }

                    }
                    else
                    {
                        //Missing Data for non-cumulative meter
                        //To do
                    }
                }
                else if (timeDiff == 0)
                {
                    //Skip
                    comment = 2;
                    if (bLogTest == true)
                    {
                        Log.Info("----------------");
                        Log.Info("Duplicate reading / too close to previous reading. Skipped timestamp = " + myTimeStamp);
                    }

                    goto UpdateComment;
                }
                else
                {
                    //Valid metering interval
                    comment = 1;
                }
                SaveConnections.SaveIntermediateReadings(myNodeId, myReading, myReadingDate, myReadingsType, myMeterType, 0, myTimeStamp);

                if (bLogTest == true)
                {
                    Log.Info("Saving intermediate values for  = " + myReadingDate);
                }

                UpdateComment:;
                //Set comment
                SaveConnections.UpdateReading(myId, comment);
            }



            return readingCount;


        }
    }
}
