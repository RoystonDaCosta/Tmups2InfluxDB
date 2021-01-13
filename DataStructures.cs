using System.Collections.Generic;

namespace Tmups2InfluxDB
{

    public class LedStatus
    {
        public bool load_on_inv { get; set; }
        public bool bat_op { get; set; }
        public bool load_on_byp { get; set; }
        public bool over_load { get; set; }
        public bool ups_fault { get; set; }
    }

    public class StatusBit
    {
        public bool cb1 { get; set; }
        public bool cb2 { get; set; }
        public bool inverter { get; set; }
        public bool sync_driving { get; set; }
        public bool async_driving { get; set; }
    }

    public class UpsDataRoot
    {
        public Dictionary<string,string> ups_data_array { get; set; }
        public bool communication_error { get; set; }
        //public List<object> current_alarms { get; set; }
        public LedStatus led_status { get; set; }
        public StatusBit status_bit { get; set; }
    }

}