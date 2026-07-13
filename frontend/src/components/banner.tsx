import { bannerClases, type BannerProps } from "@/types/const";


export function Banner({ tipo, mensaje, onAccion, etiquetaAccion }: BannerProps) {
    return (
        <div
            role={tipo === "exito" ? "status" : "alert"}
            className={`flex items-center justify-between gap-4 rounded-md border px-4 py-3 mb-4 text-sm ${bannerClases[tipo]}`}
        >
            <span>{mensaje}</span>
            {onAccion && etiquetaAccion && (
                <button
                    type="button"
                    onClick={onAccion}
                    className="shrink-0 rounded-md bg-white/60 px-3 py-1 font-medium hover:bg-white"
                >
                    {etiquetaAccion}
                </button>
            )}
        </div>
    );
}