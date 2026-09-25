// Scripts generales del sistema. Las ventanas que se usan acá están en Views/Shared/_Mensajes.cshtml

document.addEventListener('DOMContentLoaded', function () {

    // 0) Menú principal: al tocar un botón se deja una marca para que, en la página siguiente,
    //    la barra de navegación aparezca bajando con una animación (ver _Layout.cshtml)
    document.querySelectorAll('.gym-main-menu .gym-menu-tile').forEach(function (boton) {
        boton.addEventListener('click', function () {
            try { sessionStorage.setItem('gymAnimarBarra', '1'); } catch (e) { }
        });
    });

    // 1) Si la página trae un mensaje de éxito o de error, se muestra la ventana automáticamente
    const modalMensaje = document.getElementById('modalMensaje');
    if (modalMensaje) {
        new bootstrap.Modal(modalMensaje).show();
    }

    // 2) Confirmación antes de enviar un formulario (por ejemplo, antes de eliminar).
    //    Uso: <form ... data-confirmar="¿Está seguro que desea eliminar...?">
    const modalConfirmar = document.getElementById('modalConfirmar');
    if (!modalConfirmar) return;

    const ventanaConfirmar = new bootstrap.Modal(modalConfirmar);
    let formularioPendiente = null;

    document.querySelectorAll('form[data-confirmar]').forEach(function (formulario) {
        formulario.addEventListener('submit', function (evento) {
            evento.preventDefault();                 // frenamos el envío hasta que el usuario confirme
            formularioPendiente = formulario;
            document.getElementById('modalConfirmarTexto').textContent = formulario.dataset.confirmar;
            ventanaConfirmar.show();
        });
    });

    document.getElementById('modalConfirmarSi').addEventListener('click', function () {
        if (formularioPendiente) {
            formularioPendiente.submit();            // submit() envía el formulario sin volver a preguntar
        }
    });

    // 3) Casillas "Seleccionar todos" (se usan al asignar una rutina a varios alumnos).
    //    Uso: <input type="checkbox" data-seleccionar-todos="alumnoIds">
    document.querySelectorAll('[data-seleccionar-todos]').forEach(function (casilla) {
        casilla.addEventListener('change', function () {
            const nombre = casilla.dataset.seleccionarTodos;
            document.querySelectorAll('input[name="' + nombre + '"]').forEach(function (c) {
                c.checked = casilla.checked;
            });
        });
    });
});
