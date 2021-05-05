using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using sony_rcp_server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sony_rcp_server.Models;
using System.Net;
using SONY.PTP700.SPP;
using SONY.PTP700.SPP.Events;
using SONY.PTP700.SPP.PacketFactory;
using SONY.PTP700.SPP.PacketFactory.Command;
using SONY.PTP700.SPP.Utils;

namespace sony_rcp_server.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RcpController : ControllerBase
    {
        private readonly ILogger<RcpController> _logger;
        private readonly IRcpService _rcpService;

        public RcpController(ILogger<RcpController> logger, RcpService rcpService)
        {
            _logger = logger;
            _rcpService = rcpService;
        }

        // GET: /api/v1/rcp/test
        [HttpGet("test")]
        public ActionResult<BasicResult<int>> Test()
        {
            return Ok(
                new BasicResult<int>(44));
        }

        // GET: /api/v1/rcp/connect
        [HttpGet("connect")]
        public ActionResult Connect()
        {
            _rcpService.Connect();
            return Ok();
        }

        // GET: /api/v1/rcp/disconnect
        [HttpGet("disconnect")]
        public ActionResult Disconnect()
        {
            _rcpService.Disconnect();
            return Ok();
        }

        // PUT: /api/v1/rcp/ccu/assign/{ccuId}
        [HttpPut("ccu/assign/{ccuId}")]
        public ActionResult<BasicResult<int>> Assign(int ccuId)
        {
            if (!_rcpService.GetOnlineCCUs().Contains(ccuId))
            {
                return NotFound($"Failed assign to CCU { Convert.ToString(ccuId) }, CCU is offline.");
            }

            var result = _rcpService.AssignToCcu(ccuId);

            if (result != ccuId)
            {
                return Problem(
                    title: $"Failed assign to CCU { Convert.ToString(ccuId) }, currently assigned is { Convert.ToString(result) }");
            }

            return Ok(
                new BasicResult<int>(result));
        }

        // DELETE: /api/v1/rcp/ccu/assign
        [HttpDelete("ccu/assign")]
        public ActionResult<int> Unassign()
        {

            var ccuIdNew = _rcpService.AssignToCcu(0);

            if (ccuIdNew != 0)
            {
                return Problem(
                    title: $"Failed unassign from CCU, currently assigned is { Convert.ToString(ccuIdNew) }");
            }

            return Ok();
        }

        // GET: /api/v1/rcp/ccu/assign
        // GET: /api/v1/rcp/ccu/assigned
        [HttpGet("ccu/assign")]
        [HttpGet("ccu/assigned")]
        public ActionResult<BasicResult<int>> Assigned()
        {
            var result = _rcpService.GetAssignedCCU();
            return Ok(
                new BasicResult<int>(result));
        }

        // GET: /api/v1/rcp/ccu
        [HttpGet("ccu")]
        public ActionResult<IEnumerable<int>> GetOnlineCCUs()
        {
            var result = _rcpService.GetOnlineCCUs();
            return Ok(result);
        }

        // GET: /api/v1/rcp/ccu/ip/{ccuId}
        [HttpGet("ccu/ip/{ccuId}")]
        public ActionResult<BasicResult<string>> GetCcuIpAddress(int ccuId)
        {
            var result = _rcpService.GetCcuIpAddress(ccuId);
            return Ok(
                new BasicResult<string>(result.ToString()));
        }

        // GET: /api/v1/rcp/permision
        [HttpGet("permision")]
        public ActionResult<BasicResult<Message30.PermissionControl>> GetPermision()
        {
            var result = _rcpService.GetPermission();
            return Ok(
                new BasicResult<Message30.PermissionControl>(result));
        }

        // GET: /api/v1/rcp/operatio/camera/mic_gain
        [HttpGet("operatio/camera/mic_gain")]
        public ActionResult<IEnumerable<MicGainSelect.MicGainValue>> GetCameraMicrophoneGain()
        {
            var result = _rcpService.GetCameraMicrophoneGain();
            return Ok(result);
        }

    }

}
