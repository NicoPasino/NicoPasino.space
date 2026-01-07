namespace NicoPasino.Core.DTO.Ventas
{
    public class VentaNuevaDto
    {
        // sin uso (se iba usar en VentaServicio.cs pero recibe VentaDto)
        public int DNI { get; set; }
        public string? ItemsId { get; set; }
        public string? ItemsCant { get; set; }
    }
}
