import { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";
import { Loader2 } from "lucide-react";

/**
 * Universal OAuth callback handler.
 * Backend OAuth flow tamamlandıqdan sonra bu səhifəyə redirect olunur.
 * URL pattern: /auth/callback?accessToken=...&refreshToken=...&provider=...
 * Error case: /auth/callback?error=...
 */
const OAuthCallback = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const { finalizeAuthFromTokens } = useAuth();
  const [error, setError] = useState("");

  useEffect(() => {
    const handleCallback = async () => {
      const accessToken = searchParams.get("accessToken") || "";
      const refreshToken = searchParams.get("refreshToken") || "";
      const provider = searchParams.get("provider") || "external";
      const errorParam = searchParams.get("error");
      const isNewUser = searchParams.get("isNewUser") === "true";

      if (errorParam) {
        setError(decodeURIComponent(errorParam));
        setTimeout(() => navigate("/login", { replace: true }), 3000);
        return;
      }

      if (!accessToken || !refreshToken) {
        setError("Missing tokens from OAuth callback");
        setTimeout(() => navigate("/login", { replace: true }), 3000);
        return;
      }

      try {
        await finalizeAuthFromTokens({ AccessToken: accessToken, RefreshToken: refreshToken }, isNewUser);
        if (isNewUser) {
          navigate("/onboarding", { replace: true });
        } else {
          navigate("/dashboard", { replace: true });
        }
      } catch (err: any) {
        setError(err?.message || `${provider} login failed`);
        setTimeout(() => navigate("/login", { replace: true }), 3000);
      }
    };

    handleCallback();
  }, [searchParams, finalizeAuthFromTokens, navigate]);

  return (
    <div className="min-h-screen flex items-center justify-center bg-background">
      <div className="flex flex-col items-center gap-4">
        {error ? (
          <>
            <div className="p-4 rounded-xl bg-destructive/10 border border-destructive/20 text-destructive text-sm text-center max-w-md">
              {error}
            </div>
            <p className="text-sm text-muted-foreground">Redirecting to login...</p>
          </>
        ) : (
          <>
            <Loader2 className="w-8 h-8 animate-spin text-primary" />
            <p className="text-sm text-muted-foreground">
              Completing login...
            </p>
          </>
        )}
      </div>
    </div>
  );
};

export default OAuthCallback;

