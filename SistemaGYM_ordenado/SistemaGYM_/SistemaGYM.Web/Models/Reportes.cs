namespace SistemaGYM.Web.Models;

// Tipos chiquitos que usan los reportes para pasarle los datos ya calculados a la vista.

// Un valor por mes (ej: cantidad de suscriptos o total cobrado en ese mes)
public record DatoMensual(DateTime Mes, decimal Valor);

// Un valor por categoría (ej: cantidad de clientes de un plan, o total cobrado por método de pago)
public record DatoCategoria(string Nombre, decimal Valor);
