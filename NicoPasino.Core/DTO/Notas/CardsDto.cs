namespace NicoPasino.Core.DTO.Notas;

public partial class CardsDto
{
    public int Id { get; set; } // es "IdPublica"

    //public int IdPublica { get; set; }

    public string Header { get; set; }

    public string Text { get; set; }

    public string? Fecha { get; set; }

    public string? Hora { get; set; }

    public string Name { get; set; }

    public string Color { get; set; }
}