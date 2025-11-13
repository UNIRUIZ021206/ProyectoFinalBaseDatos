namespace SuperEsperanzaApi.Dto
{
    public class ErrorResponse
    {
        public string Error { get; set; } = string.Empty;
    }

    public class ErrorDetailResponse
    {
        public string Error { get; set; } = string.Empty;
        public object? Detalles { get; set; }
    }

    public class MensajeResponse
    {
        public string Mensaje { get; set; } = string.Empty;
    }
}

