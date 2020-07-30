using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neura.Billing.Data;

namespace Neura.Billing.TariffCalcs
{
    class TariffComponents
    {
        public static int GetTariffComponents(int tariffId, DateTime myReadingDate, out DataTable dtTComponents)
        {
            //This loads the tariff components into the tariff arrays
            //Calculate date filter
            int day = myReadingDate.Day;
            int hour = myReadingDate.Hour;
            int minute = myReadingDate.Minute;
            DateTime tempDateTime = myReadingDate;
            if (day == 1 && hour == 0 && minute == 0) { tempDateTime = tempDateTime.AddMinutes(-1); }
            int componentCount = UtilityConnections.SelectTariffComponents(tariffId, tempDateTime,
                out DataTable dtComponents);  //Read tariff components into dtTariffComponents
            dtTComponents = dtComponents;
            return componentCount;
        }
    }
}
