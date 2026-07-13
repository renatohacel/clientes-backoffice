import { useState } from "react";
import { cerrarSesion, haySesion } from "./services/api";
import { Login } from "./components/login";
import type { ClienteDto } from "./types";
import { useClientes } from "./hooks/useClientes";
import { obtenerCliente } from "./services/clientes";
import { FormularioCliente } from "./components/form-cliente";
import { CambioEstado } from "./components/cambio-estado";
import { ListadoClientes } from "./components/listado-clientes";
import { Banner } from "./components/banner";

type Vista =
  | { modo: "listado" }
  | { modo: "alta" }
  | { modo: "edicion"; cliente: ClienteDto };

function App() {

  const [autenticado, setAutenticado] = useState(haySesion());
  function handleCerrarSesion() {
    cerrarSesion();
    setAutenticado(false);
  }

  if (!autenticado) {
    return <Login onLogin={() => setAutenticado(true)} />;
  }
  return <Clientes handleCerrarSesion={handleCerrarSesion} />
}

export default App;

function Clientes({ handleCerrarSesion }: { handleCerrarSesion: () => void }) {
  const [vista, setVista] = useState<Vista>({ modo: "listado" });
  const [seleccionado, setSeleccionado] = useState<ClienteDto | null>(null);
  const [mensaje, setMensaje] = useState<string | null>(null);
  const [conflicto, setConflicto] = useState(false);


  const clientes = useClientes();

  async function resolverConflicto() {
    setConflicto(false);
    if (seleccionado) {
      const fresco = await obtenerCliente(seleccionado.id).catch(() => null);
      setSeleccionado(fresco);
      if (vista.modo === "edicion" && fresco) setVista({ modo: "edicion", cliente: fresco });
    }
    void clientes.recargar();
  }

  function alGuardar(cliente: ClienteDto) {
    setVista({ modo: "listado" });
    setSeleccionado(cliente);
    setMensaje(`Cliente "${cliente.nombre}" guardado correctamente.`);
    void clientes.recargar();
  }

  function alCambiarEstado(actualizado: ClienteDto) {
    setSeleccionado(actualizado);
    setMensaje(`Estado cambiado a ${actualizado.estado}.`);
    void clientes.recargar();
  }

  return (
    <div className="min-h-screen bg-slate-100">
      <div className="max-w-5xl mx-auto p-4">
        <header className="flex items-center justify-between mb-6">
          <h1 className="text-xl font-semibold text-slate-800">Mantenimiento de Clientes</h1>
          <button
            onClick={handleCerrarSesion}
            className="text-sm text-slate-500 hover:text-slate-800"
          >
            Cerrar sesión
          </button>
        </header>
        <main>
          {conflicto && (
            <Banner
              tipo="conflicto"
              mensaje="Otro operador modificó este cliente mientras lo editabas. Recarga para ver los datos actuales."
              etiquetaAccion="Recargar datos"
              onAccion={() => void resolverConflicto()}
            />
          )}
          {mensaje && !conflicto && (
            <Banner tipo="exito" mensaje={mensaje} etiquetaAccion="✕" onAccion={() => setMensaje(null)} />
          )}
          {clientes.error && <Banner tipo="error" mensaje={clientes.error} />}

          {vista.modo === "listado" && (
            <>
              <ListadoClientes
                items={clientes.items}
                total={clientes.total}
                pagina={clientes.pagina}
                totalPaginas={clientes.totalPaginas}
                busqueda={clientes.busqueda}
                cargando={clientes.cargando}
                onBusqueda={clientes.setBusqueda}
                onPagina={clientes.setPagina}
                onEditar={(c) => { setMensaje(null); setVista({ modo: "edicion", cliente: c }); }}
                onNuevo={() => { setMensaje(null); setVista({ modo: "alta" }); }}
                seleccionado={seleccionado}
                onSeleccionar={setSeleccionado}
              />
              {seleccionado && (
                <aside className="mt-4 rounded-xl bg-white shadow-sm p-4">
                  <h3 className="font-semibold text-slate-800">{seleccionado.nombre}</h3>
                  <p className="text-sm text-slate-500 mb-3">
                    Estado actual: <strong className="text-slate-700">{seleccionado.estado}</strong>
                  </p>
                  <CambioEstado
                    cliente={seleccionado}
                    onCambiado={alCambiarEstado}
                    onConflicto={() => setConflicto(true)}
                  />
                </aside>
              )}
            </>
          )}

          {(vista.modo === "alta" || vista.modo === "edicion") && (
            <FormularioCliente
              clienteInicial={vista.modo === "edicion" ? vista.cliente : null}
              onGuardado={alGuardar}
              onCancelar={() => setVista({ modo: "listado" })}
              onConflicto={() => setConflicto(true)}
            />
          )}
        </main>
      </div>
    </div>
  )
}



