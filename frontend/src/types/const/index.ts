export const input = {
    className: "rounded-md border border-slate-300 px-3 py-2 text-base text-slate-900 focus:outline-none focus:ring-2 focus:ring-slate-400",
}

export const bannerClases: Record<BannerProps["tipo"], string> = {
    exito: "bg-emerald-50 text-emerald-800 border-emerald-200",
    error: "bg-red-50 text-red-800 border-red-200",
    conflicto: "bg-amber-50 text-amber-900 border-amber-300",
}

export interface BannerProps {
    tipo: "error" | "exito" | "conflicto";
    mensaje: string;
    onAccion: () => void;
    etiquetaAccion?: string;
}

export const button = {
    className: "rounded-md border border-slate-300 bg-white px-3 py-1 text-sm hover:bg-slate-50 disabled:opacity-40 disabled:cursor-not-allowed",
}