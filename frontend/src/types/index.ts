export type EstadoCliente = "Activo" | "Inactivo" | "Suspendido"

export interface ClienteDto {
    id: string;
    nombre: string;
    rfc: string;
    email: string;
    telefono: string | null;
    estado: EstadoCliente;
    fechaAltaUtc: string;
    version: number;
    transicionesPermitidas: EstadoCliente[];
}

export interface ResultadoPaginado<T> {
    items: T[];
    total: number;
    pagina: number;
    tamanoPagina: number;
}

export interface ErrorApi {
    codigo: string;
    mensaje: string;
}

export interface CrearClientePayload {
    nombre: string;
    rfc: string;
    email: string;
    telefono: string | null;
}

export interface ActualizarClientePayload extends CrearClientePayload {
    version: number;
}