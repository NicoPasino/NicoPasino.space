using System.ComponentModel.DataAnnotations;

namespace NicoPasino.Core.DTO.Movies
{
    public class MovieDto
    {
        public int? idPublica { get; set; }

        [Display(Name = "Título")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Requiere entre 3 y 255 carácteres.")]
        [Required(ErrorMessage = "Requiere un título.")]
        public string title { get; set; }

        [Display(Name = "Año")]
        [Range(1900, 2026, ErrorMessage = "Requiere un número entre 1900 y 2026.")]
        [Required(ErrorMessage = "El año es requerido.")]
        public int year { get; set; }
        
        [Display(Name = "Director")]
        [StringLength(255, ErrorMessage = "Máximo 255 carácteres.")]
        public string? director { get; set; }

        [Display(Name = "Duración")]
        [Range(30, 300, ErrorMessage = "Requiere un número entre 30 y 300.")]
        [Required(ErrorMessage = "Requiere una duración.")]
        public int duration { get; set; }

        [Url(ErrorMessage = "El Url no es válido.")]
        public string? poster { get; set; }

        [Display(Name = "Calificación")]
        [Required(ErrorMessage = "Requiere una calificación.")]
        public decimal rate { get; set; }

        [Display(Name = "Géneros")]
        [Required(ErrorMessage = "Requiere al menos un género.")]
        public IEnumerable<int> genreIds { get; set; }

        public IEnumerable<string>? genreNames { get; set; }
    }
}

// Custom Annotation
/*
public class EvenNumberAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
        if (value != null && (int)value % 2 != 0) {
            return new ValidationResult("The value must be an even number.");
        }
        return ValidationResult.Success;
    }
}
*/