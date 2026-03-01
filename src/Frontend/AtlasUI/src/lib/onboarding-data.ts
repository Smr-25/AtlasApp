import { UserRole } from "@/context/AuthContext";

export interface OnboardingOption {
  id: string;   // UUID
  label: string; // Display text
}

export interface OnboardingQuestion {
  id: string;    // UUID
  key: string;   // Internal key (e.g., "language")
  question: string;
  options: OnboardingOption[];
  multiSelect?: boolean;
}

// ─── Deterministic UUID generator from string seed ──────────────────
// Creates a valid UUID-like string from any input string (consistent across runs)
function seedUUID(seed: string): string {
  // Simple string hash function
  function hash(str: string, salt: number): number {
    let h = salt;
    for (let i = 0; i < str.length; i++) {
      h = ((h << 5) - h + str.charCodeAt(i)) | 0;
    }
    return Math.abs(h);
  }

  const h1 = hash(seed, 0x811c9dc5);
  const h2 = hash(seed, 0x01000193);
  const h3 = hash(seed, 0xdeadbeef);
  const h4 = hash(seed, 0xcafebabe);

  const hex = (n: number, len: number) => n.toString(16).padStart(len, "0").slice(0, len);

  return `${hex(h1, 8)}-${hex(h2, 4)}-4${hex(h3, 3)}-a${hex(h4, 3)}-${hex(h1 ^ h2, 4)}${hex(h3 ^ h4, 8)}`;
}

function makeOption(questionKey: string, label: string): OnboardingOption {
  return { id: seedUUID(`opt:${questionKey}:${label}`), label };
}

function makeQuestion(role: string, key: string, question: string, optionLabels: string[], multiSelect?: boolean): OnboardingQuestion {
  return {
    id: seedUUID(`q:${role}:${key}`),
    key,
    question,
    options: optionLabels.map((l) => makeOption(`${role}:${key}`, l)),
    multiSelect,
  };
}

export const roleLabels: Record<UserRole, string> = {
  developer: "Developer",
  designer: "Designer",
  cybersecurity: "Cybersecurity",
  marketer: "Marketer",
  "team-leader": "Team Leader",
};

export const roleDescriptions: Record<UserRole, string> = {
  developer: "Build, deploy and scale applications",
  designer: "Create beautiful user experiences",
  cybersecurity: "Protect systems and data",
  marketer: "Drive growth and engagement",
  "team-leader": "Lead and manage teams effectively",
};

export const roleIcons: Record<UserRole, string> = {
  developer: "💻",
  designer: "🎨",
  cybersecurity: "🛡️",
  marketer: "📈",
  "team-leader": "👥",
};

export const onboardingQuestions: Record<UserRole, OnboardingQuestion[]> = {
  developer: [
    makeQuestion("developer", "language", "What is your primary programming language?",
      ["JavaScript/TypeScript", "Python", "Java", "C/C++", "Go", "Rust", "PHP", "Swift"]),
    makeQuestion("developer", "framework", "Which frameworks do you primarily use?",
      ["React", "Vue.js", "Angular", "Next.js", "Django", "Spring Boot", "Express.js", "Flutter"], true),
    makeQuestion("developer", "focus", "What's your development focus area?",
      ["Frontend", "Backend", "Full-Stack", "Mobile", "DevOps", "Data Engineering"]),
    makeQuestion("developer", "experience", "What's your experience level?",
      ["Junior (0-2 years)", "Mid-level (2-5 years)", "Senior (5-10 years)", "Lead (10+ years)"]),
  ],
  designer: [
    makeQuestion("designer", "specialty", "What's your design specialty?",
      ["UI/UX Design", "Graphic Design", "Motion Design", "Product Design", "Brand Identity", "3D Design"]),
    makeQuestion("designer", "tools", "Which design tools do you use?",
      ["Figma", "Sketch", "Adobe XD", "Photoshop", "Illustrator", "After Effects", "Blender"], true),
    makeQuestion("designer", "focus", "What industry do you focus on?",
      ["Tech/SaaS", "E-commerce", "Healthcare", "Finance", "Entertainment", "Education"]),
    makeQuestion("designer", "experience", "What's your experience level?",
      ["Junior (0-2 years)", "Mid-level (2-5 years)", "Senior (5-10 years)", "Lead (10+ years)"]),
  ],
  cybersecurity: [
    makeQuestion("cybersecurity", "focus", "What's your security specialization?",
      ["Network Security", "Application Security", "Cloud Security", "Digital Forensics", "Compliance/GRC", "Penetration Testing"]),
    makeQuestion("cybersecurity", "certs", "Which certifications do you hold?",
      ["CISSP", "CEH", "CompTIA Security+", "OSCP", "CISM", "AWS Security", "None yet"], true),
    makeQuestion("cybersecurity", "tools", "Which tools do you primarily use?",
      ["Burp Suite", "Wireshark", "Metasploit", "Nmap", "Splunk", "SIEM tools"], true),
    makeQuestion("cybersecurity", "experience", "What's your experience level?",
      ["Junior (0-2 years)", "Mid-level (2-5 years)", "Senior (5-10 years)", "Lead (10+ years)"]),
  ],
  marketer: [
    makeQuestion("marketer", "specialty", "What's your marketing specialty?",
      ["Digital Marketing", "Content Marketing", "SEO/SEM", "Social Media", "Email Marketing", "Growth Hacking"]),
    makeQuestion("marketer", "tools", "Which marketing tools do you use?",
      ["Google Analytics", "HubSpot", "Mailchimp", "Semrush", "Hootsuite", "Ahrefs", "Buffer"], true),
    makeQuestion("marketer", "channels", "Which channels do you focus on?",
      ["Search/Google", "Social Media", "Email", "Content/Blog", "Paid Ads", "Influencer"], true),
    makeQuestion("marketer", "experience", "What's your experience level?",
      ["Junior (0-2 years)", "Mid-level (2-5 years)", "Senior (5-10 years)", "Lead (10+ years)"]),
  ],
  "team-leader": [
    makeQuestion("team-leader", "team-size", "What's your typical team size?",
      ["2-5 members", "6-10 members", "11-20 members", "20+ members"]),
    makeQuestion("team-leader", "methodology", "Which management methodology do you follow?",
      ["Agile/Scrum", "Kanban", "Waterfall", "Hybrid", "SAFe", "Lean"]),
    makeQuestion("team-leader", "tools", "Which project management tools do you use?",
      ["Jira", "Asana", "Trello", "Monday.com", "Notion", "ClickUp", "Linear"], true),
    makeQuestion("team-leader", "experience", "What's your leadership experience?",
      ["Junior (0-2 years)", "Mid-level (2-5 years)", "Senior (5-10 years)", "Director (10+ years)"]),
  ],
};
