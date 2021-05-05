using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SONY.PTP700.SPP;
using SONY.PTP700.SPP.Events;
using SONY.PTP700.SPP.PacketFactory;
using SONY.PTP700.SPP.PacketFactory.Command;
using SONY.PTP700.SPP.Utils;

namespace sony_rcp_server.Services
{
    public interface IRcpService
    {
        /// <summary>
        /// 
        /// </summary>
        public void Connect();

        /// <summary>
        /// 
        /// </summary>
        public void Disconnect();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ccuId"></param>
        /// <returns></returns>
        public int AssignToCcu(int ccuId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetOnlineCCUs();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ccuId"></param>
        /// <returns></returns>
        public IPAddress GetCcuIpAddress(int ccuId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetAssignedCCU();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Message30.PermissionControl GetPermission();

        public IEnumerable<MicGainSelect.MicGainValue> GetCameraMicrophoneGain();

    }
}
