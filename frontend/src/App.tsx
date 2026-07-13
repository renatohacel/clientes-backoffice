import { useState } from "react";
import { cerrarSesion, haySession } from "./services/api";
import Login from "./components/login";

function App() {

  const [autenticado, setAutenticado] = useState(haySession());
  if (!autenticado) {
    return <Login onLogin={() => setAutenticado(true)} />;
  }

  return (
    <div className="min-h-screen bg-slate-100">
      <div className="max-w-5xl mx-auto p-4">
        <header className="flex items-center justify-between mb-6">
          <h1 className="text-xl font-semibold text-slate-800">Mantenimiento de Clientes</h1>
          <button
            onClick={() => { cerrarSesion(); setAutenticado(false); }}
            className="text-sm text-slate-500 hover:text-slate-800"
          >
            Cerrar sesión
          </button>
        </header>
        <main>
          {/* Fase 5B: listado, formulario y cambio de estado */}
          <p className="text-slate-500">Sesión iniciada ✓</p>
        </main>
      </div>
    </div>
  )
}

export default App
