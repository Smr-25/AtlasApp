import { UserRole } from "@/context/AuthContext";

export interface OnboardingQuestion {
  id: string;
  question: string;
  options: string[];
  multiSelect?: boolean;
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
    {
      id: "language",
      question: "What is your primary programming language?",
      options: ["JavaScript/TypeScript", "Python", "Java", "C/C++", "Go", "Rust", "PHP", "Swift"],
    },
    {
      id: "framework",
      question: "Which frameworks do you primarily use?",
      options: ["React", "Vue.js", "Angular", "Next.js", "Django", "Spring Boot", "Express.js", "Flutter"],
      multiSelect: true,
    },
    {
      id: "focus",
      question: "What's your development focus area?",
      options: ["Frontend", "Backend", "Full-Stack", "Mobile", "DevOps", "Data Engineering"],
    },
    {
      id: "experience",
      question: "What's your experience level?",
      options: ["Junior (0-2 years)", "Mid-level (2-5 years)", "Senior (5-10 years)", "Lead (10+ years)"],
    },
  ],
  designer: [
    {
      id: "specialty",
      question: "What's your design specialty?",
      options: ["UI/UX Design", "Graphic Design", "Motion Design", "Product Design", "Brand Identity", "3D Design"],
    },
    {
      id: "tools",
      question: "Which design tools do you use?",
      options: ["Figma", "Sketch", "Adobe XD", "Photoshop", "Illustrator", "After Effects", "Blender"],
      multiSelect: true,
    },
    {
      id: "focus",
      question: "What industry do you focus on?",
      options: ["Tech/SaaS", "E-commerce", "Healthcare", "Finance", "Entertainment", "Education"],
    },
    {
      id: "experience",
      question: "What's your experience level?",
      options: ["Junior (0-2 years)", "Mid-level (2-5 years)", "Senior (5-10 years)", "Lead (10+ years)"],
    },
  ],
  cybersecurity: [
    {
      id: "focus",
      question: "What's your security specialization?",
      options: ["Network Security", "Application Security", "Cloud Security", "Digital Forensics", "Compliance/GRC", "Penetration Testing"],
    },
    {
      id: "certs",
      question: "Which certifications do you hold?",
      options: ["CISSP", "CEH", "CompTIA Security+", "OSCP", "CISM", "AWS Security", "None yet"],
      multiSelect: true,
    },
    {
      id: "tools",
      question: "Which tools do you primarily use?",
      options: ["Burp Suite", "Wireshark", "Metasploit", "Nmap", "Splunk", "SIEM tools"],
      multiSelect: true,
    },
    {
      id: "experience",
      question: "What's your experience level?",
      options: ["Junior (0-2 years)", "Mid-level (2-5 years)", "Senior (5-10 years)", "Lead (10+ years)"],
    },
  ],
  marketer: [
    {
      id: "specialty",
      question: "What's your marketing specialty?",
      options: ["Digital Marketing", "Content Marketing", "SEO/SEM", "Social Media", "Email Marketing", "Growth Hacking"],
    },
    {
      id: "tools",
      question: "Which marketing tools do you use?",
      options: ["Google Analytics", "HubSpot", "Mailchimp", "Semrush", "Hootsuite", "Ahrefs", "Buffer"],
      multiSelect: true,
    },
    {
      id: "channels",
      question: "Which channels do you focus on?",
      options: ["Search/Google", "Social Media", "Email", "Content/Blog", "Paid Ads", "Influencer"],
      multiSelect: true,
    },
    {
      id: "experience",
      question: "What's your experience level?",
      options: ["Junior (0-2 years)", "Mid-level (2-5 years)", "Senior (5-10 years)", "Lead (10+ years)"],
    },
  ],
  "team-leader": [
    {
      id: "team-size",
      question: "What's your typical team size?",
      options: ["2-5 members", "6-10 members", "11-20 members", "20+ members"],
    },
    {
      id: "methodology",
      question: "Which management methodology do you follow?",
      options: ["Agile/Scrum", "Kanban", "Waterfall", "Hybrid", "SAFe", "Lean"],
    },
    {
      id: "tools",
      question: "Which project management tools do you use?",
      options: ["Jira", "Asana", "Trello", "Monday.com", "Notion", "ClickUp", "Linear"],
      multiSelect: true,
    },
    {
      id: "experience",
      question: "What's your leadership experience?",
      options: ["Junior (0-2 years)", "Mid-level (2-5 years)", "Senior (5-10 years)", "Director (10+ years)"],
    },
  ],
};
