import React, { useState } from 'react';
import { Plus, GripVertical, ExternalLink, MoreHorizontal } from 'lucide-react';
import DashboardLayout from '@/components/dashboard/DashboardLayout';

interface Workspace {
  id: string;
  name: string;
  description: string;
  color: string;
  items: number;
  lastActive: string;
}

const mockWorkspaces: Workspace[] = [
  { id: '1', name: 'Frontend Dev', description: 'React, Vue, Angular layihələri', color: 'hsl(217 91% 60%)', items: 12, lastActive: '2 saat əvvəl' },
  { id: '2', name: 'Backend API', description: 'Node.js, Python servislər', color: 'hsl(152 69% 45%)', items: 8, lastActive: '5 dəqiqə əvvəl' },
  { id: '3', name: 'DevOps', description: 'CI/CD, Docker, Kubernetes', color: 'hsl(38 92% 50%)', items: 5, lastActive: '1 gün əvvəl' },
  { id: '4', name: 'Mobile Apps', description: 'iOS, Android, Flutter', color: 'hsl(280 70% 55%)', items: 3, lastActive: '3 saat əvvəl' },
];

type WidgetSize = 'small' | 'medium' | 'large';

interface WidgetCardProps {
  workspace: Workspace;
  size: WidgetSize;
  onResize: (id: string, size: WidgetSize) => void;
}

const sizeClasses: Record<WidgetSize, string> = {
  small: 'col-span-1 row-span-1',
  medium: 'col-span-1 row-span-2',
  large: 'col-span-2 row-span-2',
};

const WidgetCard: React.FC<WidgetCardProps> = ({ workspace, size, onResize }) => {
  const [showMenu, setShowMenu] = useState(false);

  return (
    <div className={`${sizeClasses[size]} glass rounded-2xl p-5 group relative hover:border-primary/30 transition-all duration-300 cursor-pointer`}>
      <div className="absolute top-3 left-3 opacity-0 group-hover:opacity-100 transition-opacity">
        <GripVertical className="h-4 w-4 text-muted-foreground" />
      </div>

      <div className="absolute top-3 right-3">
        <button
          onClick={(e) => { e.stopPropagation(); setShowMenu(!showMenu); }}
          className="opacity-0 group-hover:opacity-100 p-1 rounded-md hover:bg-secondary transition-all"
        >
          <MoreHorizontal className="h-4 w-4 text-muted-foreground" />
        </button>
        {showMenu && (
          <div className="absolute right-0 top-8 bg-card border border-border rounded-lg shadow-lg py-1 z-10 min-w-[120px] animate-fade-in">
            {(['small', 'medium', 'large'] as WidgetSize[]).map(s => (
              <button
                key={s}
                onClick={(e) => { e.stopPropagation(); onResize(workspace.id, s); setShowMenu(false); }}
                className={`w-full text-left px-3 py-2 text-sm hover:bg-secondary transition-colors ${size === s ? 'text-primary' : 'text-foreground'}`}
              >
                {s === 'small' ? 'Kiçik' : s === 'medium' ? 'Orta' : 'Böyük'}
              </button>
            ))}
          </div>
        )}
      </div>

      <div className="w-10 h-10 rounded-xl flex items-center justify-center mb-4" style={{ backgroundColor: `${workspace.color}20` }}>
        <div className="w-3 h-3 rounded-full" style={{ backgroundColor: workspace.color }} />
      </div>

      <h3 className="text-base font-semibold text-foreground mb-1">{workspace.name}</h3>
      <p className="text-xs text-muted-foreground mb-4">{workspace.description}</p>

      {size !== 'small' && (
        <div className="space-y-3 mt-auto">
          <div className="flex justify-between text-xs">
            <span className="text-muted-foreground">Elementlər</span>
            <span className="text-foreground font-medium">{workspace.items}</span>
          </div>
          <div className="flex justify-between text-xs">
            <span className="text-muted-foreground">Son aktivlik</span>
            <span className="text-foreground font-medium">{workspace.lastActive}</span>
          </div>
        </div>
      )}

      {size === 'large' && (
        <div className="mt-4 pt-4 border-t border-border">
          <div className="flex items-center gap-2">
            <button className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-primary/10 text-primary text-xs font-medium hover:bg-primary/20 transition-colors">
              <ExternalLink className="h-3 w-3" />
              Aç
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

const DashboardPage: React.FC = () => {
  const [widgetSizes, setWidgetSizes] = useState<Record<string, WidgetSize>>({
    '1': 'large',
    '2': 'medium',
    '3': 'small',
    '4': 'small',
  });

  const handleResize = (id: string, size: WidgetSize) => {
    setWidgetSizes(prev => ({ ...prev, [id]: size }));
  };

  return (
    <DashboardLayout>
      <div className="space-y-6 animate-fade-in">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-bold text-foreground">Dashboard</h1>
            <p className="text-sm text-muted-foreground mt-1">İş sahələrinizi idarə edin və monitorinq edin</p>
          </div>
          <button className="flex items-center gap-2 h-10 px-4 rounded-lg font-medium text-sm text-primary-foreground transition-all duration-200 hover:opacity-90 active:scale-[0.98]" style={{ background: 'var(--gradient-primary)' }}>
            <Plus className="h-4 w-4" />
            Yeni Workspace
          </button>
        </div>

        {/* Stats */}
        <div className="grid grid-cols-4 gap-4">
          {[
            { label: 'Ümumi Workspace', value: '4', change: '+2 bu ay' },
            { label: 'Aktiv Layihələr', value: '28', change: '+5 bu həftə' },
            { label: 'İnteqrasiyalar', value: '12', change: '3 yeni' },
            { label: 'Komanda üzvləri', value: '8', change: '+1 bu ay' },
          ].map((stat, i) => (
            <div key={i} className="glass rounded-xl p-4">
              <p className="text-xs text-muted-foreground">{stat.label}</p>
              <p className="text-2xl font-bold text-foreground mt-1">{stat.value}</p>
              <p className="text-xs text-primary mt-1">{stat.change}</p>
            </div>
          ))}
        </div>

        {/* Workspaces grid - macOS widget style */}
        <div>
          <h2 className="text-lg font-semibold text-foreground mb-4">İş sahələri</h2>
          <div className="grid grid-cols-3 auto-rows-[140px] gap-4">
            {mockWorkspaces.map(ws => (
              <WidgetCard
                key={ws.id}
                workspace={ws}
                size={widgetSizes[ws.id] || 'small'}
                onResize={handleResize}
              />
            ))}
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
};

export default DashboardPage;
