using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars.Ribbon.Drawing;
using Newtonsoft.Json;

namespace Neura.Billing.Calls
{
    public class Info
    {

        /// <summary>
        /// Examples: "49e876a2-ccc8-4047-98b8-cad20f1dcbd4"
        /// </summary>
        [JsonProperty("_postman_id")]
        public static string PostmanId { get; set; }

        /// <summary>
        /// Examples: "Neura"
        /// </summary>
        [JsonProperty("name")]
        public static string Name { get; set; }

        /// <summary>
        /// Examples: "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
        /// </summary>
        [JsonProperty("schema")]
        public static string Schema { get; set; }
    }

    public class Body
    {

        /// <summary>
        /// Examples: "raw"
        /// </summary>
        [JsonProperty("mode")]
        public static string Mode { get; set; }

        /// <summary>
        /// Examples: "[\"modbuse1\"]"
        /// </summary>
        [JsonProperty("raw")]
        public static string Raw { get; set; }
    }

    public class Url
    {

        /// <summary>
        /// Examples: "http://34.69.70.151:8080/api/v1/switch/on", "http://34.69.70.151:8080/api/v1/switch/off", "http://34.69.70.151:8080/api/v1/statuses", "http://34.69.70.151:8080/api/v1/meters", "http://34.69.70.151:8080/api/v1/gateways"
        /// </summary>
        [JsonProperty("raw")]
        public static string Raw { get; set; }

        /// <summary>
        /// Examples: "http"
        /// </summary>
        [JsonProperty("protocol")]
        public static string Protocol { get; set; }

        /// <summary>
        /// Examples: ["34","69","70","151"], ["34","69","70","151"], ["34","69","70","151"], ["34","69","70","151"], ["34","69","70","151"]
        /// </summary>
        [JsonProperty("host")]
        public static IList<string> Host { get; set; }

        /// <summary>
        /// Examples: "8080"
        /// </summary>
        [JsonProperty("port")]
        public static string Port { get; set; }

        /// <summary>
        /// Examples: ["api","v1","switch","on"], ["api","v1","switch","off"], ["api","v1","statuses"], ["api","v1","meters"], ["api","v1","gateways"]
        /// </summary>
        [JsonProperty("path")]
        public static IList<string> Path { get; set; }
    }

    public class Request
    {

        /// <summary>
        /// Examples: "POST", "GET"
        /// </summary>
        [JsonProperty("method")]
        public static string Method { get; set; }

        /// <summary>
        /// Examples: [], [], [], [], []
        /// </summary>
        [JsonProperty("header")]
        public static IList<object> Header { get; set; }

        /// <summary>
        /// Examples: {"mode":"raw","raw":"[\"modbuse1\"]"}, {"mode":"raw","raw":"[\"modbuse1\"]"}
        /// </summary>
        [JsonProperty("body")]
        public static Body Body { get; set; }

        /// <summary>
        /// Examples: {"raw":"http://34.69.70.151:8080/api/v1/switch/on","protocol":"http","host":["34","69","70","151"],"port":"8080","path":["api","v1","switch","on"]}, {"raw":"http://34.69.70.151:8080/api/v1/switch/off","protocol":"http","host":["34","69","70","151"],"port":"8080","path":["api","v1","switch","off"]}, {"raw":"http://34.69.70.151:8080/api/v1/statuses","protocol":"http","host":["34","69","70","151"],"port":"8080","path":["api","v1","statuses"]}, {"raw":"http://34.69.70.151:8080/api/v1/meters","protocol":"http","host":["34","69","70","151"],"port":"8080","path":["api","v1","meters"]}, {"raw":"http://34.69.70.151:8080/api/v1/gateways","protocol":"http","host":["34","69","70","151"],"port":"8080","path":["api","v1","gateways"]}
        /// </summary>
        [JsonProperty("url")]
        public Url Url { get; set; }
    }

    public class Item
    {

        /// <summary>
        /// Examples: "Switch on", "Switch off", "Statuses", "Meters", "Gateways"
        /// </summary>
        [JsonProperty("name")]
        public static string Name { get; set; }

        /// <summary>
        /// Examples: {"method":"POST","header":[],"body":{"mode":"raw","raw":"[\"modbuse1\"]"},"url":{"raw":"http://34.69.70.151:8080/api/v1/switch/on","protocol":"http","host":["34","69","70","151"],"port":"8080","path":["api","v1","switch","on"]}}, {"method":"POST","header":[],"body":{"mode":"raw","raw":"[\"modbuse1\"]"},"url":{"raw":"http://34.69.70.151:8080/api/v1/switch/off","protocol":"http","host":["34","69","70","151"],"port":"8080","path":["api","v1","switch","off"]}}, {"method":"GET","header":[],"url":{"raw":"http://34.69.70.151:8080/api/v1/statuses","protocol":"http","host":["34","69","70","151"],"port":"8080","path":["api","v1","statuses"]}}, {"method":"GET","header":[],"url":{"raw":"http://34.69.70.151:8080/api/v1/meters","protocol":"http","host":["34","69","70","151"],"port":"8080","path":["api","v1","meters"]}}, {"method":"GET","header":[],"url":{"raw":"http://34.69.70.151:8080/api/v1/gateways","protocol":"http","host":["34","69","70","151"],"port":"8080","path":["api","v1","gateways"]}}
        /// </summary>
        [JsonProperty("request")]
        public static Request Request { get; set; }

        /// <summary>
        /// Examples: [], [], [], [], []
        /// </summary>
        [JsonProperty("response")]
        public static IList<object> Response { get; set; }
    }

    public class ProtocolProfileBehavior
    {
    }

    public class Switch
    {
        public static List<string> listItems { get; set; }
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
       
        public static void CallString(string refFrom, int count, out string sResult)
        {
            sResult = "Invalid nType";
            if (refFrom == "last" || refFrom == "from")
            {
               //OK
            }
            else
            {
                 MessageBox.Show("Invalid parameter");
                goto ExitHere;
            }
           
            string url = "http://34.122.10.49:8080/";
            url = url + refFrom;
            url = url + "/" + count;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            httpWebRequest.PreAuthenticate = true;

            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("dale:liebenberg"));
            HttpWebResponse responseObj = null;

            Log.Info(DateTime.UtcNow + " UTC, Attempting: " + url);
            try
            {
                responseObj = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (Exception e)
            {
                sResult = e.ToString();
                goto ExitHere;
            }

            sResult = null;
            using (Stream stream = responseObj.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                sResult = sr.ReadToEnd();
                sr.Close();
            }

            ExitHere:;
            Log.Info(DateTime.UtcNow + " UTC, Result: " + sResult);

        }
        
        
        public static void nodeSwitch( string gatewayId, 
            string nodeId, out string sResult, string onOrOff = "off")
        {
            //nType = water or electricity
            //onOrOff = on or off
            sResult = "Invalid nType";

            string url = "http://34.122.10.49:8080/switch/";


            url =url + gatewayId + "/" + nodeId + "/" + onOrOff;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            httpWebRequest.PreAuthenticate = true;

            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("dale:liebenberg"));
            HttpWebResponse responseObj = null;

            Log.Info(DateTime.UtcNow + " UTC, Attempting: " + url);
            try
            {
                responseObj = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (Exception e)
            {
                sResult = e.ToString();
                goto ReturnHere;
            }

            sResult = null;
            using (Stream stream = responseObj.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                sResult = sr.ReadToEnd();
                sr.Close();
            }

            ReturnHere:;
            Log.Info(DateTime.UtcNow + " UTC, Result: " + sResult);

        }
    }



    public class NeuraCalls
    {

        /// <summary>
        /// Examples: {"_postman_id":"49e876a2-ccc8-4047-98b8-cad20f1dcbd4","name":"Neura","schema":"https://schema.getpostman.com/json/collection/v2.1.0/collection.json"}
        /// </summary>
        [JsonProperty("info")]
        public static Info Info { get; set; }

        /// <summary>
        /// Examples: [{"name":"Switch on","request":{"method":"POST","header":[],"body":{"mode":"raw","raw":"[\"modbuse1\"]"},"url":{"raw":"http://34.69.70.151:8080/api/v1/switch/on","protocol":"http","host":["34","69","70","151"],"port":"8080","path":["api","v1","switch","on"]}},"response":[]},{"name":"Switch off","request":{"method":"POST","header":[],"body":{"mode":"raw","raw":"[\"modbuse1\"]"},"url":{"raw":"http://34.69.70.151:8080/api/v1/switch/off","protocol":"http","host":["34","69","70","151"],"port":"8080","path":["api","v1","switch","off"]}},"response":[]},{"name":"Statuses","request":{"method":"GET","header":[],"url":{"raw":"http://34.69.70.151:8080/api/v1/statuses","protocol":"http","host":["34","69","70","151"],"port":"8080","path":["api","v1","statuses"]}},"response":[]},{"name":"Meters","request":{"method":"GET","header":[],"url":{"raw":"http://34.69.70.151:8080/api/v1/meters","protocol":"http","host":["34","69","70","151"],"port":"8080","path":["api","v1","meters"]}},"response":[]},{"name":"Gateways","request":{"method":"GET","header":[],"url":{"raw":"http://34.69.70.151:8080/api/v1/gateways","protocol":"http","host":["34","69","70","151"],"port":"8080","path":["api","v1","gateways"]}},"response":[]},{"name":"Meters list","request":{"method":"GET","header":[],"url":{"raw":"http://34.69.70.151:8080/api/v1/meters/list","protocol":"http","host":["34","69","70","151"],"port":"8080","path":["api","v1","meters","list"]}},"response":[]},{"name":"Statuses list","request":{"method":"GET","header":[],"url":{"raw":"http://34.69.70.151:8080/api/v1/statuses/list","protocol":"http","host":["34","69","70","151"],"port":"8080","path":["api","v1","statuses","list"]}},"response":[]}]
        /// </summary>
        [JsonProperty("item")]
        public static IList<Item> Item { get; set; }

        /// <summary>
        /// Examples: {}
        /// </summary>
        [JsonProperty("protocolProfileBehavior")]
        public ProtocolProfileBehavior ProtocolProfileBehavior { get; set; }
    }
}
