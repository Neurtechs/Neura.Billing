using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neura.Billing.Data;

namespace Neura.Billing.TariffCalcs
{
    class Seasons
    {
        public static int GetSeason(int imonth, int touLookupId)
        {
            int[] monthStart;
            int[] monthEnd;
            int[] season;

            int componentCount = UtilityConnections.SelectTOULookupS(touLookupId,
                out DataTable dtTOULookup);
            monthStart = new int[componentCount];
            monthEnd = new int[componentCount];
            season = new int[componentCount];
            int k = 0;
            foreach (DataRow dr in dtTOULookup.Rows)
            {
                monthStart[k] = Convert.ToInt32(dr["MonthStart"]);
                monthEnd[k] = Convert.ToInt32(dr["MonthEnd"]);
                season[k] = Convert.ToInt32(dr["TouSeasonName"]);
                k += 1;
            }
            for (var j = 0; j <= componentCount - 1; j++)
            {
                if (monthStart[j] < monthEnd[j])
                {
                    // Interval is within
                    if (imonth >= monthStart[j] && imonth <= monthEnd[j])
                    {
                        return season[j];
                    }
                }
                else if (imonth >= monthStart[j] || imonth <= monthEnd[j])
                {
                    return season[j];
                }
            }

            return 0;
        }
    }
}
