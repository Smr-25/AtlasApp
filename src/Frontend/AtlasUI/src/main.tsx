import { createRoot } from "react-dom/client";
import App from "./App.tsx";
import "./index.css";

// Enable lightweight mocking when ?mock=1 is present so UI can be explored without backend
if (typeof window !== "undefined") {
  try {
    const qs = new URLSearchParams(window.location.search || "");
    if (qs.get("mock") === "1") {
      // dynamic import so it doesn't affect production bundles unless requested
      import("./lib/mockApi")
        .then((m) => m.enableMocking())
        .catch(() => {});
    }
  } catch (e) {}
}

createRoot(document.getElementById("root")!).render(<App />);
