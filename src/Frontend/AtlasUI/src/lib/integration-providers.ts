import { UserRole } from "@/context/AuthContext";

export interface IntegrationProviderInfo {
  provider: string;       // Backend enum adı
  name: string;           // Göstərilən ad
  description: string;    // Qısa açıqlama
  icon: string;           // Emoji icon
  category: string;       // Kateqoriya
  roles: UserRole[];      // Hansı rollara aiddir
  apiUrl?: string;        // API URL (varsa)
}

export const integrationProviders: IntegrationProviderInfo[] = [
  // Developer
  { provider: "GitHub", name: "GitHub", description: "Code repositories & collaboration", icon: "🐙", category: "Developer", roles: ["developer", "team-leader"], apiUrl: "https://api.github.com" },
  { provider: "Jira", name: "Jira", description: "Project & issue tracking", icon: "📋", category: "Developer", roles: ["developer", "team-leader"], apiUrl: "https://api.atlassian.com" },
  { provider: "Trello", name: "Trello", description: "Kanban boards & task management", icon: "📌", category: "Developer", roles: ["developer", "team-leader"] },
  { provider: "Notion", name: "Notion", description: "Docs, wikis & project management", icon: "📝", category: "Developer", roles: ["developer", "team-leader"] },

  // Designer
  { provider: "Figma", name: "Figma", description: "UI/UX design & prototyping", icon: "🎨", category: "Designer", roles: ["designer"], apiUrl: "https://api.figma.com" },
  { provider: "AdobeCc", name: "Adobe CC", description: "Creative Cloud suite", icon: "🅰️", category: "Designer", roles: ["designer"] },
  { provider: "Miro", name: "Miro", description: "Whiteboard & brainstorming", icon: "🖼️", category: "Designer", roles: ["designer"], apiUrl: "https://api.miro.com" },
  { provider: "LottieFiles", name: "LottieFiles", description: "Animations & motion design", icon: "✨", category: "Designer", roles: ["designer"] },
  { provider: "Dribbble", name: "Dribbble", description: "Design portfolio & inspiration", icon: "🏀", category: "Designer", roles: ["designer"] },
  { provider: "Zeplin", name: "Zeplin", description: "Design handoff & specs", icon: "📐", category: "Designer", roles: ["designer"], apiUrl: "https://api.zeplin.dev" },

  // DevOps
  { provider: "Aws", name: "AWS", description: "Amazon Web Services cloud", icon: "☁️", category: "DevOps", roles: ["developer", "cybersecurity"] },
  { provider: "Azure", name: "Azure", description: "Microsoft Azure cloud", icon: "🔷", category: "DevOps", roles: ["developer", "cybersecurity"] },
  { provider: "DockerHub", name: "Docker Hub", description: "Container registry", icon: "🐳", category: "DevOps", roles: ["developer"] },
  { provider: "Datadog", name: "Datadog", description: "Monitoring & analytics", icon: "🐕", category: "DevOps", roles: ["developer"] },

  // Communication
  { provider: "Slack", name: "Slack", description: "Team messaging & notifications", icon: "💬", category: "Communication", roles: ["developer", "designer", "cybersecurity", "marketer", "team-leader"] },
  { provider: "Discord", name: "Discord", description: "Community & voice chat", icon: "🎮", category: "Communication", roles: ["developer", "designer", "cybersecurity", "marketer", "team-leader"] },

  // Monitoring
  { provider: "Sentry", name: "Sentry", description: "Error tracking & performance", icon: "🐛", category: "Monitoring", roles: ["developer"], apiUrl: "https://sentry.io/api" },
  { provider: "SonarQube", name: "SonarQube", description: "Code quality & security", icon: "🔍", category: "Monitoring", roles: ["developer"], apiUrl: "https://sonarcloud.io/api" },
  { provider: "Perplexity", name: "Perplexity", description: "AI-powered research", icon: "🤖", category: "AI", roles: ["developer", "designer", "cybersecurity", "marketer", "team-leader"] },

  // SecOps
  { provider: "Cloudflare", name: "Cloudflare", description: "CDN, DNS & security", icon: "🛡️", category: "SecOps", roles: ["cybersecurity"] },
  { provider: "Snyk", name: "Snyk", description: "Vulnerability scanning", icon: "🔒", category: "SecOps", roles: ["cybersecurity"] },
  { provider: "AwsGuardDuty", name: "AWS GuardDuty", description: "Threat detection", icon: "🔎", category: "SecOps", roles: ["cybersecurity"] },
  { provider: "OnePassword", name: "1Password", description: "Password & secrets management", icon: "🔐", category: "SecOps", roles: ["cybersecurity"] },
  { provider: "VirusTotal", name: "VirusTotal", description: "Malware analysis", icon: "🦠", category: "SecOps", roles: ["cybersecurity"] },
  { provider: "Shodan", name: "Shodan", description: "Internet intelligence", icon: "🌐", category: "SecOps", roles: ["cybersecurity"] },
  { provider: "PagerDuty", name: "PagerDuty", description: "Incident management", icon: "🚨", category: "SecOps", roles: ["cybersecurity"] },

  // Marketing
  { provider: "MetaAds", name: "Meta Ads", description: "Facebook & Instagram advertising", icon: "📘", category: "Marketing", roles: ["marketer"] },
  { provider: "GoogleSearchConsole", name: "Google Search Console", description: "SEO & search analytics", icon: "🔍", category: "Marketing", roles: ["marketer"] },
  { provider: "Mailchimp", name: "Mailchimp", description: "Email marketing campaigns", icon: "📧", category: "Marketing", roles: ["marketer"] },
  { provider: "SocialListening", name: "Social Listening", description: "Brand monitoring", icon: "👂", category: "Marketing", roles: ["marketer"] },
  { provider: "Ga4", name: "Google Analytics 4", description: "Website analytics", icon: "📊", category: "Marketing", roles: ["marketer"] },
  { provider: "StripeMkt", name: "Stripe", description: "Payment & revenue analytics", icon: "💳", category: "Marketing", roles: ["marketer"] },
  { provider: "HubSpot", name: "HubSpot", description: "CRM & marketing automation", icon: "🧲", category: "Marketing", roles: ["marketer"] },
];

/** Get providers relevant to a specific role */
export function getProvidersForRole(role?: UserRole): IntegrationProviderInfo[] {
  if (!role) return integrationProviders;
  return integrationProviders.filter((p) => p.roles.includes(role));
}

/** Get all unique categories */
export function getProviderCategories(): string[] {
  return [...new Set(integrationProviders.map((p) => p.category))];
}

/** Find provider info by name */
export function getProviderInfo(provider: string): IntegrationProviderInfo | undefined {
  return integrationProviders.find((p) => p.provider === provider);
}

