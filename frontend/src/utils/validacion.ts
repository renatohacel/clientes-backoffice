const RFC_REGEX = /^[A-ZÑ&]{3,4}\d{6}[A-Z0-9]{3}$/i;
const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const TELEFONO_REGEX = /^\+?\d{10,15}$/;


export interface ErroresFormulario {
    nombre?: string;
    rfc?: string;
    email?: string;
    telefono?: string;
}

export interface DataFormulario {
    nombre: string;
    rfc: string;
    email: string;
    telefono: string;
}

export function validarCliente(data: DataFormulario): ErroresFormulario {
    const errores: ErroresFormulario = {};

    const nombre = data.nombre.trim();
    if (!nombre) errores.nombre = "El nombre es requerido.";
    else if (nombre.length < 3 || nombre.length > 120)
        errores.nombre = "El nombre debe tener entre 3 y 120 caracteres.";

    if (!data.rfc.trim()) errores.rfc = "El RFC es requerido.";
    else if (!RFC_REGEX.test(data.rfc.trim()))
        errores.rfc = "El RFC no tiene un formato válido.";

    if (!data.email.trim()) errores.email = "El email es requerido.";
    else if (!EMAIL_REGEX.test(data.email.trim()))
        errores.email = "El email no tiene un formato válido.";

    if (data.telefono.trim() && !TELEFONO_REGEX.test(data.telefono.trim()))
        errores.telefono = "El teléfono debe tener de 10 a 15 dígitos.";

    return errores;
}

export function hayErrores(errores: ErroresFormulario): boolean {
    return Object.keys(errores).length > 0;
}