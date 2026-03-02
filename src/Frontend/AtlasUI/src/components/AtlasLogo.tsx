import { cn } from "@/lib/utils";

interface AtlasLogoProps {
  size?: "sm" | "md" | "lg" | "xl";
  variant?: "full" | "icon" | "text";
  className?: string;
}

const sizes = {
  sm: { icon: 20, text: "text-base", gap: "gap-1.5" },
  md: { icon: 24, text: "text-lg", gap: "gap-2" },
  lg: { icon: 32, text: "text-2xl", gap: "gap-2.5" },
  xl: { icon: 40, text: "text-3xl", gap: "gap-3" },
};

const AtlasLogo = ({ size = "md", variant = "full", className }: AtlasLogoProps) => {
  const s = sizes[size];

  const IconMark = () => (
    <div
      className="relative flex items-center justify-center rounded-lg bg-gradient-to-br from-orange-500 to-orange-600 shadow-lg shadow-orange-500/20"
      style={{ width: s.icon + 8, height: s.icon + 8 }}
    >
      <svg
        width={s.icon}
        height={s.icon}
        viewBox="0 0 24 24"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        {/* Stylized A for Atlas */}
        <path
          d="M12 3L4 21H8.5L10 17H14L15.5 21H20L12 3Z"
          fill="white"
          fillOpacity="0.95"
        />
        <path
          d="M10.8 14.5L12 11L13.2 14.5H10.8Z"
          fill="currentColor"
          className="text-orange-600"
        />
      </svg>
    </div>
  );

  const TextMark = () => (
    <span
      className={cn(
        s.text,
        "font-black tracking-tight select-none",
        "bg-gradient-to-r from-foreground via-foreground to-foreground/70 bg-clip-text text-transparent"
      )}
    >
      ATLAS
    </span>
  );

  if (variant === "icon") return <IconMark />;
  if (variant === "text") return <TextMark />;

  return (
    <div className={cn("flex items-center", s.gap, className)}>
      <IconMark />
      <TextMark />
    </div>
  );
};

export default AtlasLogo;

