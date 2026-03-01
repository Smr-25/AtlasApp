import { useState, useEffect, useCallback } from "react";
import {
  workspaceApi,
  integrationApi,
  WorkspaceDto,
  IntegrationDto,
} from "@/services/api";

export function useWorkspaces() {
  const [workspaces, setWorkspaces] = useState<WorkspaceDto[]>([]);
  const [integrations, setIntegrations] = useState<IntegrationDto[]>([]);
  const [pendingIntegrations, setPendingIntegrations] = useState<IntegrationDto[]>([]);
  const [activeWorkspace, setActiveWorkspace] = useState<WorkspaceDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchAll = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);

      // Fetch all three in parallel, catch each individually so one failure doesn't block others
      const [wsRes, intRes, pendRes] = await Promise.allSettled([
        workspaceApi.getAll(),
        integrationApi.getAll(),
        integrationApi.getPending(),
      ]);

      // Workspaces
      if (wsRes.status === "fulfilled") {
        const d = wsRes.value.data;
        if (d.isSuccess && Array.isArray(d.data)) {
          setWorkspaces(d.data);
          const def = d.data.find((w) => w.isDefault) || d.data[0] || null;
          setActiveWorkspace(def);
        }
      }

      // Active integrations
      if (intRes.status === "fulfilled") {
        const d = intRes.value.data;
        if (d.isSuccess && Array.isArray(d.data)) {
          setIntegrations(d.data);
        }
      }

      // Pending integrations
      if (pendRes.status === "fulfilled") {
        const d = pendRes.value.data;
        if (d.isSuccess && Array.isArray(d.data)) {
          setPendingIntegrations(d.data);
        }
      }
    } catch (err: any) {
      setError(err?.message || "Failed to load workspace data");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchAll();
  }, [fetchAll]);

  const switchWorkspace = useCallback((ws: WorkspaceDto) => {
    setActiveWorkspace(ws);
  }, []);

  const createWorkspace = useCallback(
    async (name: string, description?: string) => {
      const res = await workspaceApi.create({ name, description });
      if (res.data.isSuccess) {
        await fetchAll();
        return { error: null };
      }
      return { error: res.data.errors?.[0] || "Failed to create workspace" };
    },
    [fetchAll]
  );

  const deleteWorkspace = useCallback(
    async (id: string) => {
      await workspaceApi.remove(id);
      await fetchAll();
    },
    [fetchAll]
  );

  const setDefaultWorkspace = useCallback(
    async (id: string) => {
      await workspaceApi.setDefault(id);
      await fetchAll();
    },
    [fetchAll]
  );

  const toggleIntegration = useCallback(
    async (workspaceId: string, integrationId: string, enable: boolean) => {
      await workspaceApi.toggleIntegration(workspaceId, integrationId, enable);
      await fetchAll();
    },
    [fetchAll]
  );

  return {
    workspaces,
    integrations,
    pendingIntegrations,
    activeWorkspace,
    switchWorkspace,
    createWorkspace,
    deleteWorkspace,
    setDefaultWorkspace,
    toggleIntegration,
    loading,
    error,
    refresh: fetchAll,
  };
}

