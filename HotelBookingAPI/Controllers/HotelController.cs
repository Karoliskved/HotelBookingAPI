using hotelBooking.Models;
using HotelBookingAPI.Models;
using HotelBookingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace HotelBookingAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        public readonly HotelService _hotelService;
        public HotelController(HotelService hotelService)
        {
            _hotelService = hotelService;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllHotels()
        {
            var hotels = await _hotelService.GetAllHotels();
            if (hotels is not null)
            {
                return Ok(hotels);
            }
            return NotFound("There are no hotels in the database.");
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHotelById(string id)
        {
            if (ObjectId.TryParse(id, out _))
            {
                var hotel = await _hotelService.GetHotelById(id);
                if (hotel is not null)
                {
                    return Ok(hotel);
                }
                return NotFound($"Room with id: {id} doesn't exist.");
            }
            return BadRequest($"Invalid id: {id} provided.");
        }
        [HttpGet("dist/{dist}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHotelByDistToBeach(double dist)

        {
            var hotel = await _hotelService.GetHotelByDistToBeach(dist);
            if (hotel is not null)
            {

                return Ok(hotel);

            }
            return NotFound($"Room with id: {dist} doesn't exist.");

        }
        [HttpGet("closebeach")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHotelByClosestTotheBeach()

        {
            var hotel = await _hotelService.GetHotelByClosestTotheBeach();
            if (hotel is not null)
            {
                return Ok(hotel);
            }
            return NotFound($"No hotels");
        }
        [HttpPost("multi/test")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHotelByMultiParam([FromBody] FilterInput input)
        {
            /*string[] atributes = { "geographicData.distToBeach", "geographicData.distToMountains" };
            double[] distances = { 26, 21 };*/
            /*
                     {
     "atributes": [
    "geographicData.distToBeach", "geographicData.distToCity"
  ],
  "distances": [
    26, 20
  ],
  "operators": [
    "Lt", "Gt"
  ]

     ***************
      "atributes": [
    "geographicData.distToBeach", "geographicData.distToBeach"
  ],
  "distances": [
    30, 10
  ],
  "operators": [
    "Lt", "Gt"
  ]
}*/
            string[]? atributes = input.Atributes;
            double[]? distances = input.Distances;
            string[]? operators = input.Operators;
            var hotel = await _hotelService.GetHotelByMultiParam(atributes, distances, operators);
            if (hotel is not null)
            {
                return Ok(hotel);
            }
            return NotFound($"No hotels");
        }
    }




}
