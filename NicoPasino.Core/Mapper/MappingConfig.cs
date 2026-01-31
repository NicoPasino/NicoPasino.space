using Mapster;
using NicoPasino.Core.DTO.Notas;
using NicoPasino.Core.DTO.Ventas;
using NicoPasino.Core.Modelos.Notas;
using NicoPasino.Core.Modelos.Ventas;

namespace NicoPasino.Core.Mapper
{
    public static class MappingConfig
    {
        public static void VentasMappings() {
            TypeAdapterConfig<Producto, ProductoDto>.NewConfig()
                .TwoWays() // de modelo a dto / de dto a modelo.
                .Map(dest => dest.Categoria, src => src.IdCategoriaNavigation.Nombre); // prop calculada

            TypeAdapterConfig<Venta, VentaDto>.NewConfig()
                .TwoWays()
                .Map(dest => dest.Cliente, src => src.IdClienteNavigation.Nombre)
                .Map(dest => dest.Productos, src => src.Ventaporproducto);

            TypeAdapterConfig<Ventaporproducto, VentaporproductoDto>.NewConfig()
                .Map(dest => dest.Producto, src => src.IdProductoNavigation.Nombre);

            TypeAdapterConfig<Cliente, ClienteDto>.NewConfig()
                .Map(dest => dest.NroCompras, src => src.Venta.Count());
        }

        public static void NotasMappings() {
            TypeAdapterConfig<Cards, CardsDto>.NewConfig()
                .TwoWays()
                .Map(dest => dest.Id, src => src.IdPublica);
        }
    }
}
