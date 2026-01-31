using System.Reflection;
using System.Text.Json;

namespace NicoPasino.Core.Mapper.Ventas
{
    // hecho con IA
    public static class MapperGenerico
    {
        /// <summary>
        /// Mapea propiedades públicas por nombre desde TSource a TTarget.
        /// Intenta asignación directa, Convert.ChangeType y, como último recurso, serialización JSON.
        /// </summary>
        public static TTarget Map<TSource, TTarget>(TSource source)
            where TTarget : new() {
            if (source is null) {
                return default!;
            }

            var target = new TTarget();

            try {
                var sourceProps = typeof(TSource)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead);

                var targetProps = typeof(TTarget)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanWrite)
                    .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                };

                foreach (var sProp in sourceProps) {
                    if (!targetProps.TryGetValue(sProp.Name, out var tProp)) {
                        continue;
                    }

                    var value = sProp.GetValue(source);
                    if (value is null) {
                        tProp.SetValue(target, null);
                        continue;
                    }

                    // Asignación directa si es compatible
                    if (tProp.PropertyType.IsAssignableFrom(sProp.PropertyType)) {
                        tProp.SetValue(target, value);
                        continue;
                    }

                    // Intentar conversión simple para tipos primitivos/convertibles
                    try {
                        var converted = Convert.ChangeType(value, Nullable.GetUnderlyingType(tProp.PropertyType) ?? tProp.PropertyType);
                        tProp.SetValue(target, converted);
                        continue;
                    }
                    catch {
                        // Ignorar y probar con JSON
                    }

                    // Intentar deserializar el valor (por ejemplo de un DTO anidado)
                    try {
                        var serialized = JsonSerializer.Serialize(value, jsonOptions);
                        var deserialized = JsonSerializer.Deserialize(serialized, tProp.PropertyType, jsonOptions);
                        tProp.SetValue(target, deserialized);
                    }
                    catch {
                        // Si todo falla, no propagar excepción (seguimos el patrón del archivo original).
                    }
                }
            }
            catch {
                // No propagar excepción por diseño del mapper existente.
            }

            return target;
        }

        /// <summary>
        /// Mapea una colección de TSource a una lista de TTarget.
        /// </summary>
        public static IEnumerable<TTarget> MapList<TSource, TTarget>(IEnumerable<TSource> source)
            where TTarget : new() {
            if (source is null) {
                return Enumerable.Empty<TTarget>();
            }

            var list = new List<TTarget>();
            try {
                foreach (var item in source) {
                    list.Add(Map<TSource, TTarget>(item));
                }
            }
            catch {
                // No propagar
            }

            return list;
        }

        // Compatibilidad con las firmas anteriores (opcional):
        // ConvertToDto/ConvertToModel para Producto <-> ProductoDto siguen funcionando si se desea.
    }
}
