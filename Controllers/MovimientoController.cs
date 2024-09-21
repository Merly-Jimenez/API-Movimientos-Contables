using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using CsvHelper;
using System.IO;
using MovimientosContablesAPI.Models;

namespace MovimientosContablesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimientosController : ControllerBase
    {
        [HttpPost("procesar-movimientos")]
        public IActionResult ProcesarMovimientos([FromForm] IFormFile archivoCsv)
        {
            if (archivoCsv == null || archivoCsv.Length == 0)
            {
                return BadRequest("Archivo no proporcionado o está vacío");
            }
            var movimientos = new List<Movimiento>();
            decimal total = 0;
            int count = 0;

            try
            {
                using (var reader = new StreamReader(archivoCsv.OpenReadStream()))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<Movimiento>().ToList();
                    movimientos = records;
                    total = movimientos.Sum(m => m.Monto);
                    count = movimientos.Count;
                }
                var promedio = movimientos.Any() ? total / count : 0;

                return Ok(new
                {
                    total = total,
                    promedio = Math.Round(promedio, 2),
                    movimientosProcesados = count
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al procesar el archivo: {ex.Message}");
            }
        }
    }
}