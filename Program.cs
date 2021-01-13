using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AdysTech.InfluxDB.Client.Net;

namespace Tmups2InfluxDB
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            string TMUPS_IP = Environment.GetEnvironmentVariable("TMUPS_IP");
            string TMUPS_ID = Environment.GetEnvironmentVariable("TMUPS_ID");
            string INFLUXDB_IP = Environment.GetEnvironmentVariable("INFLUXDB_IP");
            string INFLUXDB_PORT = Environment.GetEnvironmentVariable("INFLUXDB_PORT");
            string INFLUXDB_DATABASE = Environment.GetEnvironmentVariable("INFLUXDB_DATABASE");
            string INFLUXDB_USERNAME = Environment.GetEnvironmentVariable("INFLUXDB_USERNAME");
            string INFLUXDB_PASSWORD = Environment.GetEnvironmentVariable("INFLUXDB_PASSWORD");
            int RETRY, READING;
            if (!Int32.TryParse(Environment.GetEnvironmentVariable("RETRY"), out RETRY))
            {
                Console.WriteLine("Invalid RETRY using 10");
                RETRY = 10 * 1000;
            }
            else
            {
                RETRY = RETRY * 1000;
            }
            if (!Int32.TryParse(Environment.GetEnvironmentVariable("READING"), out READING))
            {
                Console.WriteLine("Invalid READING using 5");
                READING = 5 * 1000;
            }
            else
            {
                READING = READING * 1000;
            }

            string statusURLString = "http://" + TMUPS_IP + "/?page=view/ups_status_getdata";
            string influxDbURL = "http://" + INFLUXDB_IP + ":" + INFLUXDB_PORT;

            Console.WriteLine("Running with below parameters");
            Console.WriteLine("\tTMUPS_IP {0}", TMUPS_IP);
            Console.WriteLine("\tTMUPS_ID {0}", TMUPS_ID);
            Console.WriteLine("\tINFLUXDB_IP {0}", INFLUXDB_IP);
            Console.WriteLine("\tINFLUXDB_PORT {0}", INFLUXDB_PORT);
            Console.WriteLine("\tINFLUXDB_DATABASE {0}", INFLUXDB_DATABASE);
            Console.WriteLine("\tINFLUXDB_USERNAME {0}", INFLUXDB_USERNAME);
            Console.WriteLine("\tINFLUXDB_PASSWORD {0}", INFLUXDB_PASSWORD);
            Console.WriteLine("\n\nUsing Status URL   : {0}", statusURLString);
            Console.WriteLine("Using InfluxDB URL : {0}", influxDbURL);


            while (true)
            {
                InfluxDBClient influxClient = new InfluxDBClient(influxDbURL, INFLUXDB_USERNAME, INFLUXDB_PASSWORD);

                try
                {
                    while (true)
                    {
                        String responseBody = await client.GetStringAsync(statusURLString);
                        int startOfContent = responseBody.IndexOf("{\"ups_data_array\"", 0);
                        if (startOfContent < 0)
                        {
                            Console.WriteLine("Error : Start of content not found");
                            break;
                        }
                        responseBody = responseBody.Substring(startOfContent);
                        int endOfContent = responseBody.IndexOf("</div>", 0);
                        if (endOfContent < 0)
                        {
                            Console.WriteLine("Error : End of content not found");
                            break;
                        }
                        responseBody = responseBody.Substring(0, endOfContent);

                        String json = responseBody;
                        UpsDataRoot upsDataRoot = JsonSerializer.Deserialize<UpsDataRoot>(json);
                        Console.WriteLine(json);

                        var valMixed = new InfluxDatapoint<InfluxValueField>();
                        valMixed.UtcTimestamp = DateTime.UtcNow;
                        valMixed.Tags.Add("id", TMUPS_ID);
                        valMixed.MeasurementName = "UPS_DATA";
                        valMixed.Precision = TimePrecision.Seconds;


                        valMixed.Fields.Add("Power", new InfluxValueField(Int32.Parse(upsDataRoot.ups_data_array["73"]) / 10000.0));
                        valMixed.Fields.Add("BatteryStatus", new InfluxValueField(Int32.Parse(upsDataRoot.ups_data_array["22"])));
                        valMixed.Fields.Add("BatteryCharge", new InfluxValueField(Int32.Parse(upsDataRoot.ups_data_array["18"])));

                        valMixed.Fields.Add("Voltage", new InfluxValueField(Int32.Parse(upsDataRoot.ups_data_array["24"]) / 10.0));
                        valMixed.Fields.Add("Current", new InfluxValueField(Int32.Parse(upsDataRoot.ups_data_array["20"]) / 10.0));
                        valMixed.Fields.Add("SecondsOnBattery", new InfluxValueField(Int32.Parse(upsDataRoot.ups_data_array["81"])));
                        valMixed.Fields.Add("DischargingTimes", new InfluxValueField(Int32.Parse(upsDataRoot.ups_data_array["46"])));
                        valMixed.Fields.Add("EstimatedCharging", new InfluxValueField(Int32.Parse(upsDataRoot.ups_data_array["36"]) / 10.0));


                        valMixed.Fields.Add("Input Voltage AB", new InfluxValueField(Int32.Parse(upsDataRoot.ups_data_array["51"]) / 10.0));
                        valMixed.Fields.Add("Input Voltage BC", new InfluxValueField(Int32.Parse(upsDataRoot.ups_data_array["52"]) / 10.0));
                        valMixed.Fields.Add("Input Voltage CA", new InfluxValueField(Int32.Parse(upsDataRoot.ups_data_array["53"]) / 10.0));


                        valMixed.Fields.Add("Output Voltage AB", new InfluxValueField(Int32.Parse(upsDataRoot.ups_data_array["77"]) / 10.0));
                        valMixed.Fields.Add("Output Voltage BC", new InfluxValueField(Int32.Parse(upsDataRoot.ups_data_array["78"]) / 10.0));
                        valMixed.Fields.Add("Output Voltage CA", new InfluxValueField(Int32.Parse(upsDataRoot.ups_data_array["79"]) / 10.0));
                        valMixed.Fields.Add("Output Current A", new InfluxValueField(Int32.Parse(upsDataRoot.ups_data_array["65"]) / 10.0));
                        valMixed.Fields.Add("Output Current B", new InfluxValueField(Int32.Parse(upsDataRoot.ups_data_array["66"]) / 10.0));
                        valMixed.Fields.Add("Output Current C", new InfluxValueField(Int32.Parse(upsDataRoot.ups_data_array["67"]) / 10.0));

                        var r = await influxClient.PostPointAsync(INFLUXDB_DATABASE, valMixed);
                        Console.WriteLine(r);
                        Thread.Sleep(5000);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
                influxClient.Dispose();

                Thread.Sleep(10000);
            }
        }
    }
}
