using Mapster;
using NicoPasino.Core.DTO.Ventas;
using NicoPasino.Core.Errores;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Modelos.Ventas;
using System.Linq.Expressions;

namespace NicoPasino.Servicios.Servicios.Ventas
{
    public class VentaServicio : IServicioGenerico<Venta, VentaDto>
    {
        // TODO: usar uow?
        private readonly IRepositorioGenericoVentas<Venta> _repoG;
        private readonly IRepositorioGenericoVentas<Cliente> _repoCliente;
        private readonly IRepositorioGenericoVentas<Producto> _repoProducto;
        private readonly IRepositorioGenericoVentas<Ventaporproducto> _repoVpp;

        public VentaServicio(
            IRepositorioGenericoVentas<Venta> repoG,
            IRepositorioGenericoVentas<Cliente> repoCliente,
            IRepositorioGenericoVentas<Producto> repoProducto,
            IRepositorioGenericoVentas<Ventaporproducto> repoVpp) {
            _repoG = repoG ?? throw new ArgumentNullException(nameof(repoG));
            _repoCliente = repoCliente ?? throw new ArgumentNullException(nameof(repoCliente));
            _repoProducto = repoProducto ?? throw new ArgumentNullException(nameof(repoProducto));
            _repoVpp = repoVpp ?? throw new ArgumentNullException(nameof(repoVpp));
        }

        public async Task<IEnumerable<VentaDto>> GetAll(bool activo) {
            var objsDb = await _repoG.ListarAsync(
                //filtro: m => m.Activo == activo
                orden: q => q.OrderByDescending(m => m.FechaVenta)
                , incluir: "IdClienteNavigation,Ventaporproducto,Ventaporproducto.IdProductoNavigation"
            );

            // si hay...
            if (objsDb != null && objsDb.Any()) {
                var objsDto = objsDb.Adapt<IEnumerable<VentaDto>>();
                return objsDto;
            }
            else return Enumerable.Empty<VentaDto>();
        }

        public async Task<IEnumerable<VentaDto>> GetAll(string campo, string? valor) {
            if (string.IsNullOrWhiteSpace(campo)) throw new ArgumentException("Campo de búsqueda no válido.");
            campo = campo.Trim().ToLowerInvariant();
            valor = valor?.Trim();

            Expression<Func<Venta, bool>> filtro = null;
            Func<IQueryable<Venta>, IOrderedQueryable<Venta>> orden = null;

            bool valorIsNull = string.IsNullOrEmpty(valor);
            var vLower = valorIsNull ? string.Empty : valor!.ToLowerInvariant();

            switch (campo) {
                case "numero":
                    orden = q => q.OrderBy(m => m.Numero);
                    if (!valorIsNull) {
                        filtro = m => m.Numero != null
                                    && m.Numero.ToString().Contains(vLower);
                    }
                    break;

                case "nombre":
                case "nombredesc":
                    orden = (campo == "nombre")
                        ? new Func<IQueryable<Venta>, IOrderedQueryable<Venta>>(q => q.OrderBy(m => m.IdClienteNavigation.Nombre))
                        : new Func<IQueryable<Venta>, IOrderedQueryable<Venta>>(q => q.OrderByDescending(m => m.IdClienteNavigation.Nombre));
                    if (!valorIsNull) {
                        filtro = m => m.IdClienteNavigation != null
                                   && m.IdClienteNavigation.Nombre != null
                                   && m.IdClienteNavigation.Nombre.ToLower().Contains(vLower);
                    }
                    break;

                case "otro":
                case "otrodesc":
                    orden = (campo == "otro")
                        ? new Func<IQueryable<Venta>, IOrderedQueryable<Venta>>(q => q.OrderBy(m => m.Detalle))
                        : new Func<IQueryable<Venta>, IOrderedQueryable<Venta>>(q => q.OrderByDescending(m => m.Detalle));
                    if (!valorIsNull) {
                        filtro = m => m.Detalle != null
                                    && m.Detalle.ToLower().Contains(vLower);
                    }
                    break;

                default:
                    throw new DataException($"Campo de búsqueda '{campo}' no soportado. Campos soportados: numero, nombre(Cliente), otro(Detalle).");
            }

            var objsDb = await _repoG.ListarAsync(filtro: filtro, incluir: "IdClienteNavigation,Ventaporproducto,Ventaporproducto.IdProductoNavigation", orden: orden);

            if (objsDb != null && objsDb.Any()) {
                var objsDto = objsDb.Adapt<IEnumerable<VentaDto>>();
                return objsDto;
            }
            else return Enumerable.Empty<VentaDto>();
        }

        public async Task<VentaDto> GetById(int id) {
            if (id <= 0) throw new DataException("Numero de venta no válido"); // TODO: comprobar si existe Nro
            var objDb = await _repoG.GetAsync(
                filtro: m => m.Id == id
                , incluir: "IdClienteNavigation,Ventaporproducto,Ventaporproducto.IdProductoNavigation"
            );

            if (objDb != null) {
                var objDto = objDb.Adapt<VentaDto>();
                return objDto;
            }
            else return new VentaDto();
        }

        public async Task<bool> Create(VentaDto obj) {
            Random random = new();
            if (obj == null) throw new DataException("No se recibió ningún dato.");

            // Verificar Cliente
            if (obj.DNI != null) {
                var cliente = await _repoCliente.GetAsync(filtro: c => c.Documento == obj.DNI);
                if (cliente != null) {
                    // Si ya existe el cliente, reutilizar su Id y NO crear uno nuevo
                    obj.IdCliente = cliente.Id;
                }
                else {
                    // Sólo validar y crear si no existe
                    if (string.IsNullOrWhiteSpace(obj.Nombre)) {
                        throw new DataException("Nombre vacío o no válido.");
                    }

                    try {
                        var nuevoCliente = new Cliente { Documento = (int)obj.DNI, Nombre = obj.Nombre };
                        var res = await _repoCliente.Add(nuevoCliente);

                        obj.IdCliente = res.Id;
                    }
                    catch (Exception ex) {
                        throw new UpdateException("Error al crear Cliente: desde el Servicio de Venta.");
                    }
                }
            }
            else throw new DataException("No se recibió DNI.");

            // Verificar productos
            if (obj.ItemsId != null && obj.ItemsCant != null) {
                if (obj.ItemsId.ToArray().Length != obj.ItemsCant.ToArray().Length)
                    throw new DataException("ItemsId e ItemsCant deben tener la misma longitud.");
            }
            else throw new DataException("No se recibió la lista de productos.");

            // Mapear a Venta manualmente para evitar conflictos de tipos
            var venta = new Venta
            {
                IdCliente = obj.IdCliente,
                Numero = random.Next(1, 9999999),
                Detalle = obj.Detalle,
                FechaVenta = obj.FechaVenta ?? DateTime.UtcNow
            };

            // Guardar venta
            var ventaGuardada = await _repoG.Add(venta); // TODO: saveChanges al final
            if (ventaGuardada == null) throw new DataException("No se pudo guardar la venta.");

            // Guardar VentaPorProducto según ItemsId / ItemsCant
            if (obj.ItemsId != null && obj.ItemsCant != null) {
                var ids = obj.ItemsId.ToArray();
                var cants = obj.ItemsCant.ToArray();

                for (int i = 0; i < ids.Length; i++) {
                    var idPublica = ids[i];
                    var cantidad = cants[i];

                    // buscar producto por IdPublica
                    var productoDb = await _repoProducto.GetAsync(filtro: p => p.IdPublica == idPublica);
                    if (productoDb == null) throw new DataException($"Producto no encontrado (IdPublica={idPublica}).");

                    var vpp = new Ventaporproducto
                    {
                        IdVenta = ventaGuardada.Id,
                        IdProducto = productoDb.Id,
                        Cantidad = cantidad,
                        PrecioUnitario = productoDb.Precio,
                        SubTotal = productoDb.Precio * cantidad
                    };

                    await _repoVpp.Add(vpp);
                }
            }
            return true;
        }

        public Task<bool> Update(VentaDto obj) {
            throw new NotImplementedException();
        }

        public Task<bool> Enable(int id, bool estado) {
            throw new NotImplementedException();
        }
    }
}