using Microsoft.AspNetCore.Mvc;
using MOD9_KEL32.Models;
using MOD9_KEL32.Models.Dto;
using MOD9_KEL32.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Identity;

namespace MOD9_KEL32.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<Villa>> GetVillas()
        {
            return Ok(VillaStore.villaList);
        }
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(200, Type = typeof(VillaDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(404)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id == 0) return BadRequest();
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if (villa == null) return NotFound();
            return Ok(villa);
        }
        [HttpPost]
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
        {
            if (VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists");
                return BadRequest(ModelState);
            }
            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }
            if (villaDTO.Id == 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            villaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDTO);
            string response = "Sukses menambahkan data Vila" + "\nId : " + villaDTO.Id.ToString() + "\nNama : " + villaDTO.Name;

            return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, response);
        }
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {

                return BadRequest();

            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {

                return NotFound();

            }
            VillaStore.villaList.Remove(villa);
            return NoContent();
        }
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
        {
            if (villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            villa.Name = villaDTO.Name;
            villa.Occupancy = villaDTO.Occupancy;
            villa.Sqft = villaDTO.Sqft;
            return NoContent();
        }
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);

            if (villa == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(villa, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }

        [HttpPost("/login")]
        public ActionResult<VillaDTO> LoginAcc([FromBody] VillaDTO villaDTO)
        {
            if (villaDTO == null)
            {
                return BadRequest("Username/Password Invalid");
            }

            var villa = VillaStore.villaList.FirstOrDefault(u => u.Name == villaDTO.Name);

            if (villa == null)
            {
                return NotFound("Username tidak ditemukan");
            }

            if (villa.Id != villaDTO.Id)
            {
                return Unauthorized("Id Salah");
            }

            if (villa.Password != villaDTO.Password)
            {
                return Unauthorized("Password Salah");
            }

            return Ok("Berhasil login");
        }
    }
}