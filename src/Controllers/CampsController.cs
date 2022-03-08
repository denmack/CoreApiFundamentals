using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Models;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")] // Kein Hard-Code, damit der Name automatisch genommen wird. Es wird der Name vor Controller verwendet!
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;

        public CampsController(ICampRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet] // Stellt sicher dass ein Get-Request gemacht wird
        public async Task<ActionResult<CampModel[]>> Get(bool includeTalks = false/*Query-string*/) // --> Endpoint für eine Route
        {
            try
            {
                var results = await _repository.GetAllCampsAsync(includeTalks);
                CampModel[] models = _mapper.Map<CampModel[]>(results);
                /* Mapper vereinfachen die geholten Objekte, so können nicht-gebrauchte Attribute ausgelassen werden*/
                return models;
            } catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("{moniker}")] // Hier machen wir klar, dass ein Camp nach moniker zurückgegeben werden soll
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try
            {
                var result = await _repository.GetCampAsync(moniker);

                if (result == null) return NotFound(); // Abfrage wenn Camp nicht gefunden worden ist

                return _mapper.Map<CampModel>(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        // Suche anhand Datum nach einem Camp
        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var results = await _repository.GetAllCampsByEventDate(theDate, includeTalks);
                if(!results.Any()) return NotFound(); // --> 404 wenn nichts gefunden wird
                return _mapper.Map<CampModel[]>(results);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }
    }
}
