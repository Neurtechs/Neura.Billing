using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neura.Billing.Data;
using static Neura.Billing.GlobalVar;
using System.Data;
using Neura.Billing.TariffCalcs;

namespace Neura.Billing.DEHW
{
    class CalcRates
    {
        public static double GetRate(int tariffID, DateTime DateReceived)
        {
            int componentCount = 0;
            string tariffYear = "";
          
            int myInterval = 0;
            int mySeason = 0;
            int myMeasurement = 0;
            double myRate = 0;
            componentCount = TariffComponents.GetTariffComponents(tariffID, DateReceived, out DataTable dtComponents);
            tariffYear = Convert.ToString(dtComponents.Rows[0]["Year"]);
            int TOULookupId = UtilityConnections.SelectTOULookup(tariffID);
            foreach (DataRow drT in dtComponents.Rows )
            {
                //Get tariff components
                bool myMatch = true;
                DateTime checkDate = DateReceived;
                int month = checkDate.Month;
                if (checkDate.Day == 1 && checkDate.Minute == 0 && checkDate.Hour == 0) { checkDate = checkDate.AddMinutes(-1); }
                myInterval = Convert.ToInt16(drT["Interval"]);
                mySeason = Convert.ToInt16(drT["Season"]);
                myMeasurement = Convert.ToInt16(drT["Measurement"]);
                if (myMeasurement!=0) {goto SkipNextComponent;}
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
                return myRate;
                break;
                SkipNextComponent: ;
            }

            return myRate;
        }


        
    }
}
