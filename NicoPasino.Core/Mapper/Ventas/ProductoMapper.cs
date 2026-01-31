using NicoPasino.Core.DTO.Ventas;
using NicoPasino.Core.Modelos.Ventas;

namespace NicoPasino.Core.Mapper.Ventas
{
    public static class ProductoMapper
    {
        public static ProductoDto ConvertToDto(Producto modelo) {
            var objetoDTO = new ProductoDto();
            try {
                objetoDTO.IdPublica = modelo.IdPublica;
                objetoDTO.IdCategoria = modelo.IdCategoria;
                objetoDTO.Nombre = modelo.Nombre;
                objetoDTO.Descripcion = modelo.Descripcion;
                objetoDTO.Cantidad = modelo.Cantidad;
                objetoDTO.Precio = modelo.Precio;
                objetoDTO.FechaCreacion = modelo.FechaCreacion;
                objetoDTO.FechaModificacion = modelo.FechaModificacion;
                objetoDTO.Activo = modelo.Activo;

                /*var cat = modelo.Moviegenres ?? Enumerable.Empty<Categoria>();
                objetoDTO.IdCategoria = cat.Select(g => g.GenreId).ToList();
                objetoDTO.genreNames = cat
                    .Select(g => g.Genre?.Name)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Distinct()
                    .ToList();*/
            }
            catch (Exception) {
                // no propagar
            }
            return objetoDTO;
        }

        public static IEnumerable<ProductoDto> ConvertToDtoList(IEnumerable<Producto> objeto) {
            var listaDTO = new List<ProductoDto>();
            try {
                foreach (var item in objeto) {
                    var dto = ConvertToDto(item);
                    listaDTO.Add(dto);
                }
            }
            catch (Exception ex) {
            }

            return listaDTO;
        }

        public static Producto ConvertToModel(ProductoDto objeto) {
            var model = new Producto();

            try {
                model.IdPublica = objeto.IdPublica;
                model.IdCategoria = objeto.IdCategoria;
                model.Nombre = objeto.Nombre;
                model.Descripcion = objeto.Descripcion;
                model.Cantidad = objeto.Cantidad;
                model.Precio = objeto.Precio;
                model.Activo = true;
                model.FechaCreacion = objeto.FechaCreacion;
                model.FechaModificacion = DateTime.Now;

                // NOTA: el mapeo de genre a entidades se realiza en el servicio (MovieServicio),
                // porque el mapper no tiene acceso al contexto/repositorio.
            }
            catch (Exception ex) {

            }

            return model;
        }
    }
}
