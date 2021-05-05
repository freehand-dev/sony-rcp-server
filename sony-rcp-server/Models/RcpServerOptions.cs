using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sony_rcp_server.Models
{
    public class RcpServerOptions
    {
        public MCUSettings MCU { get; set; }

        public RCPSettings RCP { get; set; }

        public RcpServerOptions()
        {
            this.MCU = new MCUSettings();
            this.RCP = new RCPSettings();
        }
    }

    public class MCUSettings
    {
        public string IPAddress { get; set; } = "192.168.0.1";
    }

    public class RCPSettings
    {
        public uint SerialNumber { get; set; } = 0x0001b034;
        public byte RcpId { get; set; } = 25;
    }
}
