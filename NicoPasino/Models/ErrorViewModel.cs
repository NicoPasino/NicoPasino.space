namespace NicoPasino.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string? Controlador { get; set; }

        public string? Mensaje { get; set; }
    }
}
