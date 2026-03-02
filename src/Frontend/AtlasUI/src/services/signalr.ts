import * as signalR from "@microsoft/signalr";
import { TokenService } from "./api";

let connection: signalR.HubConnection | null = null;

export function getHubConnection(): signalR.HubConnection | null {
  return connection;
}

export async function startSignalR(): Promise<signalR.HubConnection> {
  if (connection && connection.state === signalR.HubConnectionState.Connected) {
    return connection;
  }

  connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/atlas", {
      accessTokenFactory: () => TokenService.getAccessToken() || "",
    })
    .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
    .configureLogging(signalR.LogLevel.Warning)
    .build();

  try {
    await connection.start();
    console.log("[Atlas] SignalR connected");
  } catch (err) {
    console.warn("[Atlas] SignalR connection failed:", err);
  }

  return connection;
}

export async function stopSignalR(): Promise<void> {
  if (connection) {
    await connection.stop();
    connection = null;
  }
}

// ─── Event Types ────────────────────────────────────────────────
export interface SignalRNotification {
  id: string;
  category: string;
  priority: string;
  title: string;
  body: string;
  actionType?: string;
}

export interface SignalRPresenceUpdate {
  userId: string;
  status: string;
  lastSeen: string;
}

export interface SignalRFocusState {
  userId: string;
  isActive: boolean;
  sessionType?: string;
}

export interface SignalRJobCompleted {
  jobType: string;
  result: unknown;
}

export interface SignalRFeedUpdate {
  eventType: string;
  item: unknown;
}

export interface SignalRAlert {
  alertType: string;
  payload: unknown;
}

// ─── Event Helpers ──────────────────────────────────────────────
export function onNotification(cb: (data: SignalRNotification) => void): void {
  connection?.on("NotificationReceived", cb);
}

export function onAlert(cb: (data: SignalRAlert) => void): void {
  connection?.on("AlertReceived", cb);
}

export function onPresenceUpdate(cb: (data: SignalRPresenceUpdate) => void): void {
  connection?.on("PresenceUpdate", cb);
}

export function onFocusStateChanged(cb: (data: SignalRFocusState) => void): void {
  connection?.on("FocusStateChanged", cb);
}

export function onJobCompleted(cb: (data: SignalRJobCompleted) => void): void {
  connection?.on("JobCompleted", cb);
}

export function onFeedUpdate(cb: (data: SignalRFeedUpdate) => void): void {
  connection?.on("FeedUpdate", cb);
}

export function offAll(): void {
  if (!connection) return;
  connection.off("NotificationReceived");
  connection.off("AlertReceived");
  connection.off("PresenceUpdate");
  connection.off("FocusStateChanged");
  connection.off("JobCompleted");
  connection.off("FeedUpdate");
}

