using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Neura.Billing.Data;
using static Neura.Billing.GlobalVar;

namespace Neura.Billing.TariffCalcs
{
    class TariffMain
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void GetCosts(int myMeteringInterval, out int processedGroups)
        {
            int tariffId = 0;
            int componentCount = 0;
            string tariffYear = "";
            double consumed = 0;
            double max = 0;
            DateTime DateReceived;
            int nodeId = 0;
            //int readingsType = 0;

            //Current values
            double uMax = 0; //Usage

            double uAcc = 0;
            double uPeak = 0;
            double cFixed = 0;  //Currrency
            double cMax = 0; //Marginal peak for max progressive

            double cAcc = 0;

            //Updated values
            double uToDateMax = 0; //Usage
            double uToDateAcc = 0;
            double cToDateFixed = 0;  //Currrency
            double cToDateMax = 0;
            double cToDateAcc = 0;

            //Previous values
            double uPreviousMax = 0; //Usage
            double uPreviousAcc = 0;
            double uPreviousPeak = 0;
            double cPreviousFixed = 0; 
            double cPreviousMax = 0;

            double cPreviousAcc = 0;
            int myMeasurement = 0;
            int mySeason = 0;
            int myInterval = 0;
            int myFloor = 0;
            int myCeiling = 0;
            int myFlow = 0;
            int myCustomer = 0;
            string myComponentName = "";
            double myRate = 0;



            //Read unprocessed data from ServiceUsage_Groupes
            int usageCount = UtilityConnections.GetGroupedUsage(out DataTable dtGrouped);
            processedGroups = usageCount;
            foreach (DataRow dr in dtGrouped.Rows)
            {
                nodeId = Convert.ToInt32(dr["node"]);
                DateReceived = Convert.ToDateTime(dr["DateReceived"]);

                //Confirm of a customer is active for this node
                int customerNode = UtilityConnections.GetCustomerFromNode(nodeId, out DataTable dtCustomerNode);

                if (bLogTest == true)
                {
                    Log.Info("");
                    Log.Info("=======Running calculations for Node " + nodeId + " for date " + DateReceived + "======");
                    Log.Info("");
                }

                if (customerNode == 0)
                {
                    //No customers attached to this node
                    MessageBox.Show("No customers associated with node " + nodeId);

                    goto SkipNext;
                }
                string filter = "Status = 1";
                DataRow[] drS = dtCustomerNode.Select(filter);
                if (drS.Length == 0)
                {
                    //MessageBox.Show("No active customers associated with node " + nodeId);
                    if (bLogTest == true)
                    {
                        Log.Info("!!!!!! No active customers associated with node " + nodeId);
                    }
                    goto SkipNext;
                }
                else if (drS.Length > 1)
                {
                    //MessageBox.Show("Multiple customers associated with node " + nodeId);
                    if (bLogTest == true)
                    {
                        Log.Info("!!!!!! Multiple customers associated with node " + nodeId);
                    }
                    goto SkipNext;

                }
                else
                {
                    myCustomer = Convert.ToInt16(dtCustomerNode.Rows[0]["Customer"]);
                    if (bLogTest == true)
                    {
                        Log.Info("CustomerId = " + myCustomer);
                    }

                }
                //Get previous values
                //Test
                uPreviousMax = 0; //Usage
                uPreviousAcc = 0;
                uPreviousPeak = 0;
                cPreviousFixed = 0;  //Currrency
                cPreviousMax = 0;
                uToDateMax = 0; //Usage
                                //uToDateAcc = 0;
                cToDateFixed = 0;  //Currrency
                cToDateMax = 0;
                //cToDateAcc = 0;
                PreviousValues.GetPreviousValues(nodeId, DateReceived, myMeteringInterval,
                    out cPreviousFixed, out cPreviousMax, out cPreviousAcc,
                     out uPreviousMax, out uPreviousAcc, out uPreviousPeak);

                tariffId = Convert.ToInt16(dr["TariffId"]);
                int TOULookupId = UtilityConnections.SelectTOULookup(tariffId);

                //Get tariff components

                componentCount = TariffComponents.GetTariffComponents(tariffId, DateReceived, out DataTable dtComponents);
                if (componentCount == 0)
                {
                    if (bLogTest == true)
                    {
                        Log.Info("No tariff or rates or meter");
                    }
                    if (bLogResult == true)
                    {
                        Log.Info("No tariff or rates or meter");
                    }
                    goto SkipNext;

                }  //no tariff or meter

                //Meaning of values: 0, 1, 2 etc.
                //public enum TouSeasonName { High, Low, All }
                //public enum Category { Peak, Standard, OffPeak, All, PeakAndStandard }
                //public enum Service { Electricity, Water, Gas }
                //public enum Season { High, Low, All, NotApplicable }
                //public enum Measurement { Accumulated, Maximum, Fixed, MaxProgressive }
                //public enum Interval { Peak, Standard, OffPeak, All, PeakAndStandard, NonEnergy }
                //public enum Flow { NotApplicable, Import, Export, Consumption, Generate, NetImportExport, NetImportExportGenerate }
                //public enum Period { Account, Day, Month }

                ////Get block count
                //int countBlock = UtilityConnections.GetBlockComponents(tariffId,DateReceived, out DataTable dtBlocks);
                tariffYear = Convert.ToString(dtComponents.Rows[0]["Year"]);
                if (bLogTest == true)
                {
                    Log.Info("Tariff year = " + tariffYear);
                }
                int iComponent = 0;
                foreach (DataRow drT in dtComponents.Rows)
                {
                    //Check if this component is applicable in the case of TOU
                    bool myMatch = true;
                    DateTime checkDate = DateReceived;
                    int month = checkDate.Month;
                    if (checkDate.Day == 1 && checkDate.Minute == 0 && checkDate.Hour == 0) { checkDate = checkDate.AddMinutes(-1); }
                    myInterval = Convert.ToInt16(drT["Interval"]);
                    mySeason = Convert.ToInt16(drT["Season"]);
                    myMeasurement = Convert.ToInt16(drT["Measurement"]);
                    if (bLogTest == true)
                    {
                        Log.Info("");
                        Log.Info("--------- Tariff component " + iComponent + " ------");
                        switch (myMeasurement)
                        {
                            case 0:
                                Log.Info("measurement: accumulated");
                                Log.Info("season     : " + mySeason);
                                Log.Info("interval   : " + myInterval);
                                break;
                            case 1:
                                Log.Info("measurement: maximum");
                                Log.Info("season     : " + mySeason);
                                Log.Info("interval   : " + myInterval);
                                break;
                            case 2:
                                Log.Info("measurement: fixed");
                                Log.Info("season     : " + mySeason);
                                Log.Info("interval   : " + myInterval);
                                break;
                            case 3:
                                Log.Info("measurement: max progressive");
                                Log.Info("season     : " + mySeason);
                                Log.Info("interval   : " + myInterval);
                                break;
                        }
                        iComponent += 1;
                    }
                    myComponentName = Convert.ToString(drT["TariffComponentName"]);
                    if (mySeason == 0 || mySeason == 1)  //Seasonal
                    {
                        int getSeason = Seasons.GetSeason(month, TOULookupId);
                        if (getSeason != mySeason)
                        {
                            if (bLogTest == true) { Log.Info("Wrong season for this component - next component"); }
                            goto SkipNextComponent;
                        }
                    }
                    if (myInterval != 3)  //All
                    {
                        if (myInterval != 5) //Non-Energy
                        {
                            //Interval applies
                            myMatch = TOURate.CheckTouRate(DateReceived, TOULookupId, mySeason, myInterval);
                        }
                    }
                    if (myMatch == false)
                    {
                        if (bLogTest == true) { Log.Info("Wrong interval for this component - next component"); }
                        goto SkipNextComponent;
                    }


                    myRate = Convert.ToDouble(drT["Rate"]);

                    myFloor = Convert.ToInt16(drT["Floor"]);
                    myCeiling = Convert.ToInt16(drT["Ceiling"]);
                    myFlow = Convert.ToInt16(drT["Flow"]);


                    double netUsage = 0;
                    double netMax = 0;
                    consumed = 0;
                    int myReadingsType = 0;
                    // To do - code to check and match  seasonality

                    //Deal with reading types
                    int readingTypesCount = UtilityConnections.SelectUsageByNodeTariff(nodeId, tariffId,
                        DateReceived, out DataTable dtUsage);

                    foreach (DataRow drR in dtUsage.Rows)
                    {
                        consumed = Convert.ToDouble(drR["Consumed"]);
                        try
                        {
                            max = Convert.ToDouble(drR["Max"]);
                        }
                        catch (Exception)
                        {
                            max = 0;
                        }

                        if (myFlow == 5 || myFlow == 6) //5= NetImportExport, 6=NetImportExportGenerate
                        {
                            myReadingsType = Convert.ToInt16(drR["ReadingsType"]); //0=NA, 1=Import,2=Export,3=Consumption,4=Generation

                            switch (myReadingsType)
                            {
                                case 1: //Import
                                    netUsage += consumed;
                                    netMax += max;
                                    break;
                                case 2: //Export
                                    netUsage -= consumed;
                                    netMax -= max;
                                    break;
                                case 4: //Generation
                                    netUsage -= consumed;
                                    netMax -= max;
                                    break;
                                default:
                                    netUsage = consumed;
                                    netMax = max;
                                    break;

                            }
                        }
                        else
                        {
                            netUsage = consumed;
                            netMax = max;
                        }
                    }

                    switch (myMeasurement)
                    {
                        case 0: //Accumulate


                            if (bLogTest == true) { Log.Info("measurement: accumulated"); }
                            if (myCeiling > 0 || myFloor > 0)  // Block tariff
                            {
                                if (bLogTest == true) { Log.Info("block tariff: true"); }
                                BlockTariff.GetBlockTariff(DateReceived, cPreviousAcc, uPreviousAcc, netUsage,
                                    myReadingsType, myFloor, myCeiling, myRate, tariffId, nodeId, out cAcc, out uAcc);
                                if (cAcc > 0)
                                {
                                    cToDateAcc = cPreviousAcc + cAcc;
                                    uToDateAcc = uPreviousAcc + uAcc;
                                }
                                else
                                {
                                    cAcc = cToDateAcc - cPreviousAcc;
                                    uAcc = uToDateAcc - uPreviousAcc;
                                }
                            }
                            else
                            {
                                if (bLogTest == true) { Log.Info("block tariff: false"); }

                                Energy.GetEnergy(myRate, myFlow, mySeason, myInterval, myMeasurement, myReadingsType,
                                    netUsage, nodeId, DateReceived, TOULookupId, cPreviousAcc, uPreviousAcc, myMeteringInterval, out uAcc, out cAcc, out uToDateAcc, out cToDateAcc);

                            }
                            break;

                        case 1: //Maximum
                            if (bLogTest == true) { Log.Info("measurement: maximum"); }
                            Maximum.GetMaximumDemand(myRate, myFlow, mySeason, myInterval, myMeasurement,
                                myMeteringInterval, DateReceived, TOULookupId, netMax, cPreviousMax, uPreviousMax, uPreviousPeak,
                                out cMax, out uMax, out uToDateMax, out cToDateMax, out uPeak);
                            break;
                        case 2: //Fixed
                            if (bLogTest == true) { Log.Info("measurement: fixed"); }

                            Fixed.GetFixed(mySeason, myInterval, myRate, true, DateReceived,
                                cPreviousFixed, myMeteringInterval, out cFixed);  //Current value
                            cToDateFixed = cPreviousFixed + cFixed;

                            break;

                        case 3: //MaxProgressive
                            if (bLogTest == true) { Log.Info("measurement: progressive"); }

                            Maximum.GetMaximumDemand(myRate, myFlow, mySeason, myInterval, myMeasurement,
                                myMeteringInterval, DateReceived, TOULookupId, netMax, cPreviousMax, uPreviousMax, uPreviousPeak,
                                out cMax, out uMax, out uToDateMax, out cToDateMax, out uPeak);
                            cToDateFixed = cPreviousFixed + cFixed;
                            break;
                    }
                    SkipNextComponent:;

                }
                //Save values
                if (bLogTest == true) { Log.Info("All components complete - Saving data"); }
                SaveConnections.SaveMonthValues(nodeId, DateReceived, cToDateFixed, cToDateMax,
                        cToDateAcc, uToDateMax, uToDateAcc, myCustomer, uPeak);


                SaveConnections.SavePeriodValues(nodeId, DateReceived, cFixed, cMax,
                        cAcc, uMax, uAcc, myCustomer);

                double total = cAcc + cMax + cFixed;
                SaveConnections.SaveCustomerValues(DateReceived, total, myCustomer);
                SkipNext:;
                SaveConnections.UpdateServiceUsageStatus(nodeId, DateReceived);

            }


        }
    }
}
