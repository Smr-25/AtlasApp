import { createRoot } from "react-dom/client";
import App from "./App";
import "./index.css";

if (import.meta.env?.DEV) {
  import('./lib/mockBackendAdapter').then(m => m.enableMockBackend()).catch(() => {})
}

createRoot(document.getElementById("root")!).render(<App />);
