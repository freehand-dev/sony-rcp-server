using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using sony_rcp_server.Models;
using SONY.PTP700.SPP;
using SONY.PTP700.SPP.Events;
using SONY.PTP700.SPP.PacketFactory;
using SONY.PTP700.SPP.PacketFactory.Command;
using SONY.PTP700.SPP.Utils;

namespace sony_rcp_server.Services
{
    public class RcpService: IRcpService, IDisposable
    {
        private MsuClient _msuClient;

        readonly private ILogger<RcpService> _logger;
        private readonly RcpServerOptions _settings;

        public RcpService(
            IServiceProvider serviceProvider,
            IOptions<RcpServerOptions> settings,
            ILogger<RcpService> logger)
        {
            this._logger = logger;
            this._settings = settings.Value;

            var _loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            //
            _logger.LogDebug($"* RcpServerOptions.MCU.IPAddress={ this._settings.MCU.IPAddress }");
            _logger.LogDebug($"* RcpServerOptions.RCP.RcpId={ this._settings.RCP.RcpId }");
            _logger.LogDebug($"* RcpServerOptions.RCP.SerialNumber={ this._settings.RCP.SerialNumber }");

            _msuClient = new MsuClient(_loggerFactory)
            {
                Host = IPAddress.Parse(_settings.MCU.IPAddress),
                SerialNumber = _settings.RCP.SerialNumber,
                RcpId = _settings.RCP.RcpId,
            };

            _msuClient.CcuClient.OnHandShake += EventOnHandShake;
            _msuClient.OnHandShake += EventOnHandShake;
            _msuClient.OnChangeAssigment += EventOnChangeAssigment;
            _msuClient.OnChangePermisionControl += EventOnChangePermisionControl;
            _msuClient.OnError += EventOnError;
            _msuClient.CcuClient.OnError += EventOnError;
            _msuClient.CcuClient.OnChangeCameraPowerState += EventOnChangeCameraPowerState;
        }

        public void Dispose()
        {
            this.Disconnect();
        }

        private void EventOnHandShake(Object sender, PacketReceivedEventArgs args)
        {
            var _handshake = (args.Packet as HandShake);
            this._logger.LogInformation($"SerialNumber is { _handshake?.SerialNumber.ToString() }");
            this._logger.LogInformation($"DeviceType is { _handshake?.Type.ToString() }");
            this._logger.LogInformation($"DeviceModel is { _handshake?.Model.ToString() }");
        }

        private void EventOnChangeAssigment(Object sender, RcpAssigmentEventArgs args)
        {
            this._logger.LogDebug($"[OnChangeAssigment] { Convert.ToString(args.CcuNo) }");
        }

        private void EventOnChangePermisionControl(Object sender, PermissionControlEventArgs args)
        {
            this._logger.LogDebug($"[OnChangePermisionControl] { args.Permisions.ToString() }");
        }


        private void EventOnChangeCameraPowerState(Object sender, CameraPowerStateEventArgs args)
        {
            this._logger.LogDebug($"[OnChangeCameraPowerState] { args.State.ToString() }");
        }


        private void EventOnError(Object sender, PacketReceivedEventArgs args)
        {
            this._logger.LogError($"[ { ((sender is CcuClient) ? "CCU" : "MSU") } ][{ args.Packet.PacketType.ToString() }] { ByteUtils.ToHexString(args.Packet.Payload) }");
        }

        public void Connect()
        {
            _msuClient.Connect();
        }
        public void Disconnect()
        {
            _msuClient.Disconnect();
        }

        public int AssignToCcu(int ccuId)
        {
            return _msuClient.Assign((byte)ccuId);
        }

        public IEnumerable<int> GetOnlineCCUs()
        {
            return _msuClient.GetOnlineCcuList();
        }

        public IPAddress GetCcuIpAddress(int ccuId)
        {
            return _msuClient.GetCcuIpAddress((byte)ccuId);
        }

        public int GetAssignedCCU()
        {
            return _msuClient.GetAssignedCCU();
        }

        public Message30.PermissionControl GetPermission()
        {
            return _msuClient.GetPermission();
        }

        public IEnumerable<MicGainSelect.MicGainValue> GetCameraMicrophoneGain()
        {
            var micGainSelect = _msuClient.CcuClient.mic_gain_select(MicGainSelect.MicGainValue.QueryStatus);
            var result = new List<MicGainSelect.MicGainValue>
            {
                micGainSelect.Item1.Value,
                micGainSelect.Item2.Value,
            };
            return result;
        }
    }
}
