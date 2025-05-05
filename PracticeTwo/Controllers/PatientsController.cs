using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using PracticeTwo.Dtos;
using Serilog;
using Domain.Manager;
using System.Net.Http;
using System.Threading.Tasks;

namespace PracticeTwo.Controllers
{
    [ApiController]
    [Route("api/patients")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientManager _manager;
        private readonly IGiftManager _giftManager;
        private readonly Serilog.ILogger _logger;
        private readonly HttpClient _http;

        public PatientsController(IPatientManager manager, IGiftManager gift)
        {
            _manager = manager;
            _giftManager = gift;
            _logger = Log.ForContext<PatientsController>();
            _http = new HttpClient();
        }

        // CREATE
        [HttpPost]
        public IActionResult Create([FromBody] PatientDto dto)
        {
            _logger.Information("Creating patient {CI}", dto.CI);
            var p = new Patient
            {
                Name = dto.Name,
                LastName = dto.LastName,
                CI = dto.CI
            };
            _manager.Create(p);
            return CreatedAtAction(nameof(GetByCi), new { ci = p.CI }, p);
        }

        // READ ALL
        [HttpGet]
        public IActionResult GetAll()
        {
            _logger.Information("Getting all patients");
            return Ok(_manager.GetAll());
        }

        // READ ONE
        [HttpGet("{ci}")]
        public IActionResult GetByCi(string ci)
        {
            _logger.Information("Getting patient {CI}", ci);
            var p = _manager.GetByCi(ci);
            if (p == null)
            {
                _logger.Warning("Patient {CI} not found", ci);
                return NotFound("Patient not found");
            }
            return Ok(p);
        }

        // UPDATE
        [HttpPut("{ci}")]
        public IActionResult Update(string ci, [FromBody] PatientUpdateDto dto)
        {
            _logger.Information("Updating patient {CI}", ci);
            if (!_manager.Update(ci, dto.Name, dto.LastName))
            {
                _logger.Warning("Patient {CI} not found for update", ci);
                return NotFound("Patient not found");
            }
            return NoContent();
        }

        // DELETE
        [HttpDelete("{ci}")]
        public IActionResult Delete(string ci)
        {
            _logger.Information("Deleting patient {CI}", ci);
            if (!_manager.Delete(ci))
            {
                _logger.Warning("Patient {CI} not found for deletion", ci);
                return NotFound("Patient not found");
            }
            return NoContent();
        }

        // GIFT ENDPOINT
        [HttpGet("{ci}/gift")]
        public async Task<IActionResult> GetGift(string ci)
        {
            if (_manager.GetByCi(ci) == null)
                return NotFound("Patient not found");

            var gift = await _giftManager.GetRandomGiftAsync(ci);
            if (gift == null)
                return StatusCode(502, "No gift available");

            return Ok(gift);
        }
    }
}