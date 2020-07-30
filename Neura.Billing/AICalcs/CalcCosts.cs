using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using static Neura.Billing.GlobalVar;
using Neura.Billing.Data;
using Neura.Billing.TariffCalcs;

namespace Neura.Billing.AICalcs
{
    class CalcCosts
    {
        public static void GetCosts(DataTable dtComponents, int myMeteringInterval, double netUsage, double cPreviousAcc, double cPreviousFixed,
            double cPreviousMax, double uPreviousAcc, int tariffId, int myReadingsType, double uPreviousMax, double netMax, double uPreviousPeak,
            int nodeId, int TOULookupId, DateTime DateReceived, out double cToDateAcc, out double cToDateFixed, out double cToDateMax, out double uToDateAcc,
            out double uToDatePeak)
        {

            //Current values
            double uMax = 0; //Usage

            double uAcc = 0;
            double uPeak = 0;
            double cFixed = 0;  //Currrency
            double cMax = 0; //Marginal peak for max progressive
            uToDatePeak = 0;
            double cAcc = 0;

            //Updated values
            double uToDateMax = 0; //Usage
            uToDateAcc = 0;
            cToDateFixed = 0;  //Currrency
            cToDateMax = 0;
            cToDateAcc = 0;


            int myMeasurement = 0;
            int mySeason = 0;
            int myInterval = 0;
            int myFloor = 0;
            int myCeiling = 0;
            int myFlow = 0;


            double myRate = 0;

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

                if (mySeason == 0 || mySeason == 1)  //Seasonal
                {
                    int getSeason = Seasons.GetSeason(month, TOULookupId);
                    if (getSeason != mySeason)
                    {

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
                    goto SkipNextComponent;
                }

                myRate = Convert.ToDouble(drT["Rate"]);

                myFloor = Convert.ToInt16(drT["Floor"]);
                myCeiling = Convert.ToInt16(drT["Ceiling"]);
                myFlow = Convert.ToInt16(drT["Flow"]);

                switch (myMeasurement)
                {
                    case 0: //Accumulate
                        if (myCeiling > 0 || myFloor > 0)  // Block tariff
                        {

                            BlockTariff.GetBlockTariff(DateReceived, cPreviousAcc, uPreviousAcc, netUsage,
                                myReadingsType, myFloor, myCeiling, myRate, tariffId, nodeId, out cAcc, out uAcc, false);
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

                            Energy.GetEnergy(myRate, myFlow, mySeason, myInterval, myMeasurement, myReadingsType,
                                netUsage, nodeId, DateReceived, TOULookupId, cPreviousAcc, uPreviousAcc, myMeteringInterval, out uAcc, out cAcc, out uToDateAcc, out cToDateAcc);

                        }
                        break;

                    //Note:  Max not used in this application
                    case 1: //Maximum
                        Maximum.GetMaximumDemand(myRate, myFlow, mySeason, myInterval, myMeasurement,
                            myMeteringInterval, DateReceived, TOULookupId, netMax, cPreviousMax, uPreviousMax, uPreviousPeak,
                            out cMax, out uMax, out uToDateMax, out cToDateMax, out uPeak);
                        uToDatePeak = uPeak;
                        break;
                    case 2: //Fixed                      

                        Fixed.GetFixed(mySeason, myInterval, myRate, true, DateReceived,
                            cPreviousFixed, myMeteringInterval, out cFixed);  //Current value
                        cToDateFixed = cPreviousFixed + cFixed;

                        break;

                    case 3: //MaxProgressive


                        Maximum.GetMaximumDemand(myRate, myFlow, mySeason, myInterval, myMeasurement,
                            myMeteringInterval, DateReceived, TOULookupId, netMax, cPreviousMax, uPreviousMax, uPreviousPeak,
                            out cMax, out uMax, out uToDateMax, out cToDateMax, out uPeak);
                        cToDateFixed = cPreviousFixed + cFixed;
                        uToDatePeak = uPeak;
                        break;

                }


                SkipNextComponent:;
            }
            //Return values



        }

    }
}
