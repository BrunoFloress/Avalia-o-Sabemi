import type { Config } from "tailwindcss";

const config: Config = {
  content: ["./app/**/*.{ts,tsx}", "./components/**/*.{ts,tsx}"],
  theme: {
    extend: {
      colors: {
        base: "#0F1419",       // fundo geral, tom "sala de operações"
        surface: "#161C26",    // cartões e tabela
        surfaceAlt: "#1D2530", // linhas alternadas / hover
        border: "#2A323D",
        ink: "#E6EAEF",        // texto primário
        muted: "#8A94A3",      // texto secundário
        accent: "#3FB6A8",     // teal — identidade do produto, usado com moderação
        success: "#34D399",
        error: "#F87171",
        pending: "#FBBF24",
      },
      fontFamily: {
        sans: ["var(--font-inter)", "system-ui", "sans-serif"],
        mono: ["var(--font-jbmono)", "ui-monospace", "monospace"],
      },
    },
  },
  plugins: [],
};
export default config;
