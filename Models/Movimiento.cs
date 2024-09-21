namespace MovimientosContablesAPI.Models
{
    //getters / setters
    public class Movimiento
    {
        public required DateTime Fecha { get; set; }
        public required string Descripcion { get; set; }
        public required decimal Monto { get; set; }
    } // Estructura del archivo csv
}
