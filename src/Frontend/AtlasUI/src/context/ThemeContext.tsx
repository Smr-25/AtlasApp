import { createContext, useContext, useEffect, useState, ReactNode } from "react";
import { UserRole } from "@/context/AuthContext";

type Theme = "light" | "dark";

interface ThemeContextType {
  theme: Theme;
  toggleTheme: () => void;
  setRole: (role: UserRole) => void;
  currentRole: UserRole;
}

const ThemeContext = createContext<ThemeContextType>({
  theme: "light",
  toggleTheme: () => {},
  setRole: () => {},
  currentRole: "team-leader",
});

export const useTheme = () => useContext(ThemeContext);

// Role-based color configurations (HSL values)
const roleThemes: Record<UserRole, {
  primary: string;
  ring: string;
  sidebarPrimary: string;
  momentumAccent: string;
}> = {
  "team-leader": {
    primary: "19 100% 50%",
    ring: "19 100% 50%",
    sidebarPrimary: "19 100% 50%",
    momentumAccent: "19 100% 50%",
  },
  developer: {
    primary: "220 90% 56%",
    ring: "220 90% 56%",
    sidebarPrimary: "220 90% 56%",
    momentumAccent: "220 90% 56%",
  },
  designer: {
    primary: "350 85% 55%",
    ring: "350 85% 55%",
    sidebarPrimary: "350 85% 55%",
    momentumAccent: "350 85% 55%",
  },
  cybersecurity: {
    primary: "145 63% 42%",
    ring: "145 63% 42%",
    sidebarPrimary: "145 63% 42%",
    momentumAccent: "145 63% 42%",
  },
  marketer: {
    primary: "45 93% 47%",
    ring: "45 93% 47%",
    sidebarPrimary: "45 93% 47%",
    momentumAccent: "45 93% 47%",
  },
};

export const ThemeProvider = ({ children }: { children: ReactNode }) => {
  const [theme, setTheme] = useState<Theme>("light");
  const [currentRole, setCurrentRole] = useState<UserRole>("team-leader");

  useEffect(() => {
    const root = document.documentElement;
    root.classList.remove("light", "dark");
    root.classList.add(theme);
  }, [theme]);

  useEffect(() => {
    const root = document.documentElement;
    const colors = roleThemes[currentRole];
    root.style.setProperty("--primary", colors.primary);
    root.style.setProperty("--ring", colors.ring);
    root.style.setProperty("--sidebar-primary", colors.sidebarPrimary);
    root.style.setProperty("--sidebar-ring", colors.ring);
    root.style.setProperty("--momentum-orange", colors.momentumAccent);

    // For marketer (yellow), use dark foreground on primary
    if (currentRole === "marketer") {
      root.style.setProperty("--primary-foreground", "18 100% 4%");
      root.style.setProperty("--sidebar-primary-foreground", "18 100% 4%");
    } else {
      root.style.setProperty("--primary-foreground", "0 0% 100%");
      root.style.setProperty("--sidebar-primary-foreground", "0 0% 100%");
    }
  }, [currentRole]);

  const toggleTheme = () => setTheme((t) => (t === "light" ? "dark" : "light"));
  const setRole = (role: UserRole) => setCurrentRole(role);

  return (
    <ThemeContext.Provider value={{ theme, toggleTheme, setRole, currentRole }}>
      {children}
    </ThemeContext.Provider>
  );
};
