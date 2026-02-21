import React, { useEffect, useState } from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { LayoutDashboard, Puzzle, Settings, HelpCircle, Bell, Search, User, ChevronLeft, Moon } from 'lucide-react';
import AtlasLogo from '@/components/AtlasLogo';
import { getJson } from '@/lib/api';

interface DashboardLayoutProps {
  children: React.ReactNode;
}

const navItems = [
  { to: '/dashboard', icon: LayoutDashboard, label: 'Dashboard' },
  { to: '/profile', icon: User, label: 'Profile' },
  { to: '/dashboard/integrations', icon: Puzzle, label: 'Integrations' },
];

const bottomItems = [
  { to: '/dashboard/settings', icon: Settings, label: 'Settings' },
  { to: '/dashboard/help', icon: HelpCircle, label: 'Help' },
];

const DashboardLayout: React.FC<DashboardLayoutProps> = ({ children }) => {
  const navigate = useNavigate();
  const [profile, setProfile] = useState<{ fullName?: string | null; userName?: string | null; email?: string | null } | null>(null);
  const [collapsed, setCollapsed] = useState(false);
  const [focusMode, setFocusMode] = useState(false);
  const [pomodoro, setPomodoro] = useState(24 * 60); // seconds

  useEffect(() => {
    let mounted = true;
    const load = async () => {
      try {
        const data = await getJson('/api/accounts/profile');
        if (mounted) setProfile(data);
      } catch (err) {
        // ignore
      }
    };
    load();
    return () => { mounted = false; };
  }, []);

  useEffect(() => {
    if (!focusMode) return;
    const t = setInterval(() => setPomodoro(p => Math.max(0, p - 1)), 1000);
    return () => clearInterval(t);
  }, [focusMode]);

  const fmt = (s: number) => {
    const m = Math.floor(s / 60).toString().padStart(2, '0');
    const sec = (s % 60).toString().padStart(2, '0');
    return `${m}:${sec}`;
  };

  return (
    <div className={`flex h-screen ${focusMode ? 'bg-[#0b0c0f]' : 'bg-background'} overflow-hidden`}>
      <aside className={`flex flex-col ${collapsed ? 'w-20' : 'w-20'} border-r border-border glass-strong shrink-0 transition-all duration-300`}>
        <div className="p-4 flex items-center justify-center">
          <AtlasLogo size={collapsed ? 'sm' : 'md'} />
        </div>

        <nav className="flex-1 px-1 space-y-1 flex flex-col items-center">
          {navItems.map(item => (
            <NavLink
              key={item.to}
              to={item.to}
              end={item.to === '/dashboard'}
              title={item.label}
              className={({ isActive }) =>
                `w-full flex items-center justify-center gap-3 my-1 py-2.5 rounded-lg text-sm font-medium transition-all duration-200 ${isActive ? 'bg-primary/10 text-primary' : 'text-muted-foreground hover:text-foreground hover:bg-secondary'}`
              }
            >
              <item.icon className="h-5 w-5" />
            </NavLink>
          ))}

          <div className="mt-6 border-t border-border w-full" />

          <div className="pt-4 flex flex-col items-center w-full">
            {bottomItems.map(item => (
              <NavLink
                key={item.to}
                to={item.to}
                title={item.label}
                className={({ isActive }) =>
                  `w-full flex items-center justify-center gap-3 my-1 py-2.5 rounded-lg text-sm font-medium transition-all duration-200 ${isActive ? 'bg-primary/10 text-primary' : 'text-muted-foreground hover:text-foreground hover:bg-secondary'}`
                }
              >
                <item.icon className="h-5 w-5" />
              </NavLink>
            ))}
          </div>
        </nav>

        <div className="p-3 flex items-center justify-center border-t border-border">
          <button
            onClick={() => navigate('/login')}
            className="w-full flex items-center justify-center gap-2 px-3 py-2 rounded-lg text-sm font-medium text-muted-foreground hover:text-foreground hover:bg-secondary"
            title="Logout"
          >
            Logout
          </button>
        </div>
      </aside>

      <div className="flex-1 flex flex-col overflow-hidden">
        <header className="h-16 border-b border-border flex items-center justify-between px-6 shrink-0 glass-strong">
          <div className="flex items-center gap-3">
            <button
              onClick={() => setCollapsed(c => !c)}
              className="p-2 rounded-md hover:bg-secondary transition-colors"
              title="Toggle sidebar"
            >
              <ChevronLeft className="h-4 w-4 text-muted-foreground transform ${collapsed ? 'rotate-180' : ''}" />
            </button>

            <div className="hidden sm:flex items-center gap-3 bg-secondary rounded-lg px-3 py-2 w-60">
              <Search className="h-4 w-4 text-muted-foreground" />
              <input placeholder="Command (⌘K)" className="bg-transparent text-sm text-foreground placeholder:text-muted-foreground outline-none flex-1" />
            </div>
          </div>

          <div className="flex items-center gap-4">
            <div className="text-sm font-medium px-3 py-1 rounded-md glass-strong">{fmt(pomodoro)}</div>
            <button
              onClick={() => setFocusMode(m => !m)}
              className={`p-2 rounded-md hover:bg-secondary transition-colors flex items-center gap-2 ${focusMode ? 'bg-primary/10 text-primary' : 'text-muted-foreground'}`}
              title="Toggle Focus Mode"
            >
              <Moon className="h-4 w-4" />
              <span className="hidden sm:inline text-sm">Focus</span>
            </button>

            <button className="relative p-2 rounded-lg hover:bg-secondary transition-colors">
              <Bell className="h-5 w-5 text-muted-foreground" />
              <span className="absolute top-1.5 right-1.5 w-2 h-2 rounded-full bg-primary" />
            </button>

            <NavLink to="/profile" className="flex items-center gap-3 hover:bg-secondary rounded-lg px-2 py-1 transition-colors">
              <div className="w-8 h-8 rounded-full bg-primary/20 flex items-center justify-center text-sm font-semibold text-primary">
                {profile?.fullName ? profile.fullName.charAt(0).toUpperCase() : 'A'}
              </div>
              <div className="text-sm leading-tight hidden md:block">
                <p className="font-medium text-foreground">{profile?.fullName ?? 'Atlas User'}</p>
                <p className="text-xs text-muted-foreground">{profile?.userName ? `@${profile.userName}` : (profile?.email ?? '')}</p>
              </div>
            </NavLink>
          </div>
        </header>

        <main className="flex-1 overflow-auto p-6">
          {children}
        </main>
      </div>
    </div>
  );
};

export default DashboardLayout;
