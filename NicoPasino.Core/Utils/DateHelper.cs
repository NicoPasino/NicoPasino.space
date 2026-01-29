namespace NicoPasino.Core.Utils
{
    public class DateHelper
    {
        public static (string fecha, TimeOnly hora) GetDate() {
            DateTime now = DateTime.Now;

            string[] meses = {
                "enero", "febrero", "marzo", "abril", "mayo", "junio",
                "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre"
            };

            // Formateo de fecha: d/mes/yyyy
            string d = now.Day.ToString("D2"); // "D2" asegura 2 dígitos (como padStart)
            string m = meses[now.Month - 1];   // Los meses en C# van de 1 a 12
            string y = now.Year.ToString();
            string fecha = $"{d}/{m}/{y}";

            // Formateo de hora: HH:mm:ss
            //string hora = now.ToString("HH:mm:ss");
            TimeOnly hora = TimeOnly.FromDateTime(now);

            return (fecha, hora);
        }
    }
}
