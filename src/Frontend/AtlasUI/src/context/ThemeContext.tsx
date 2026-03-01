import { createContext, useContext, useEffect, useState, useCallback, ReactNode } from "react";
import { UserRole } from "@/context/AuthContext";

type Theme = "light" | "dark";

interface ThemeContextType {
  theme: Theme;
  toggleTheme: () => void;
  setRole: (role: UserRole) => void;
  clearRole: () => void;
  currentRole: UserRole | null;
}

const ThemeContext = createContext<ThemeContextType>({
  theme: "light",
  toggleTheme: () => {},
  setRole: () => {},
  clearRole: () => {},
  currentRole: null,
});

export const useTheme = () => useContext(ThemeContext);

// Role → theme class mapping
const roleThemeClass: Record<UserRole, string> = {
  developer: "theme-developer",
  designer: "theme-designer",
  cybersecurity: "theme-cybersecurity",
  marketer: "theme-marketer",
  "team-leader": "theme-team-leader",
};

const allThemeClasses = Object.values(roleThemeClass);

// Roles that are always dark regardless of toggle
const alwaysDarkRoles: UserRole[] = ["developer", "cybersecurity", "marketer"];

// Default theme per role (for light/dark toggle starting point)
const defaultThemeForRole: Record<UserRole, Theme> = {
  developer: "dark",
  designer: "light",
  cybersecurity: "dark",
  marketer: "dark",
  "team-leader": "light",
};

export const ThemeProvider = ({ children }: { children: ReactNode }) => {
  const [theme, setThemeState] = useState<Theme>("light");
  // null = no role applied yet → default narıncı brand theme (auth pages)
  const [currentRole, setCurrentRole] = useState<UserRole | null>(null);

  // Apply theme class to <html> when role changes
  useEffect(() => {
    const root = document.documentElement;

    // Remove all existing theme classes
    allThemeClasses.forEach((cls) => root.classList.remove(cls));
    root.classList.remove("light", "dark");

    // If no role set (auth pages), use default narıncı theme
    if (!currentRole) {
      root.classList.add("light");
      setThemeState("light");
      return;
    }

    // Add role theme class
    const themeClass = roleThemeClass[currentRole];
    if (themeClass) {
      root.classList.add(themeClass);
    }

    // Set initial theme for this role
    const initialTheme = defaultThemeForRole[currentRole];
    setThemeState(initialTheme);

    // Apply light/dark class only for roles that support toggling
    if (!alwaysDarkRoles.includes(currentRole)) {
      root.classList.add(initialTheme);
    }
  }, [currentRole]);

  // Apply light/dark class when theme toggles
  useEffect(() => {
    if (!currentRole) return;
    const root = document.documentElement;
    root.classList.remove("light", "dark");

    if (!alwaysDarkRoles.includes(currentRole)) {
      root.classList.add(theme);
    }
  }, [theme, currentRole]);

  const toggleTheme = useCallback(() => {
    if (!currentRole || alwaysDarkRoles.includes(currentRole)) return;
    setThemeState((t) => (t === "light" ? "dark" : "light"));
  }, [currentRole]);

  const setRole = useCallback((role: UserRole) => {
    setCurrentRole(role);
  }, []);

  // Called on logout — revert to brand orange
  const clearRole = useCallback(() => {
    setCurrentRole(null);
  }, []);

  return (
    <ThemeContext.Provider value={{ theme, toggleTheme, setRole, clearRole, currentRole: currentRole as UserRole }}>
      {children}
    </ThemeContext.Provider>
  );
};
