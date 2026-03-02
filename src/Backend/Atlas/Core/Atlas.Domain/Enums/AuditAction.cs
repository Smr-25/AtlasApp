namespace Atlas.Domain.Enums;

public enum AuditAction
{
    Login = 1,
    Logout = 2,
    PasswordChanged = 3,
    ProfileUpdated = 4,
    WorkspaceCreated = 10,
    WorkspaceDeleted = 11,
    WorkspaceShared = 12,
    MemberAdded = 13,
    MemberRemoved = 14,
    MemberRoleChanged = 15,
    IntegrationConnected = 20,
    IntegrationDisconnected = 21,
    IntegrationToggled = 22,
    ScriptExecuted = 30,
    DeploymentTriggered = 31,
    DockerAction = 32,
    SubscriptionChanged = 40,
    TokenCreated = 50,
    TokenRevoked = 51,
    WebhookCreated = 52,
    WebhookDeleted = 53,
    SettingsChanged = 60
}

