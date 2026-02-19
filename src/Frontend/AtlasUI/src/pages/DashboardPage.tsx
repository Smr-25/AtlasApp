import React, { useState, useRef } from 'react';
import { Plus, GripVertical, ExternalLink, MoreHorizontal } from 'lucide-react';
import { Link } from 'react-router-dom';
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
  const dragRef = useRef<HTMLDivElement | null>(null);
  const startXRef = useRef<number | null>(null);
  const startWidthRef = useRef<number | null>(null);

  const handlePointerDown = (e: React.PointerEvent) => {
    startXRef.current = e.clientX;
    const el = dragRef.current?.parentElement as HTMLElement | null;
    startWidthRef.current = el ? el.getBoundingClientRect().width : null;
    (e.target as Element).setPointerCapture(e.pointerId);
  };

  const handlePointerMove = (e: React.PointerEvent) => {
    if (startXRef.current == null || !startWidthRef.current) return;
    const dx = e.clientX - startXRef.current;
    const newW = Math.max(200, startWidthRef.current + dx);
    // Map width to size categories
    const newSize: WidgetSize = newW < 280 ? 'small' : newW < 520 ? 'medium' : 'large';
    if (newSize !== size) onResize(workspace.id, newSize);
  };

  const handlePointerUp = (e: React.PointerEvent) => {
    startXRef.current = null;
    startWidthRef.current = null;
    try { (e.target as Element).releasePointerCapture(e.pointerId); } catch(e){}
  };

  return (
    <div className={`${sizeClasses[size]} glass rounded-2xl p-5 group relative hover:border-primary/30 transition-all duration-300`}>
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
      </div>

      <div className="w-10 h-10 rounded-xl flex items-center justify-center mb-4" style={{ backgroundColor: `${workspace.color}20` }}>
        <div className="text-white font-semibold">{workspace.name[0]}</div>
      </div>

      <div>
        <div className="font-medium text-lg">{workspace.name}</div>
        <div className="text-sm text-muted-foreground">{workspace.description}</div>
      </div>

      <div className="mt-4 pt-4 border-t border-border">
        <div className="flex items-center gap-2">
          <button className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-primary/10 text-primary text-xs font-medium hover:bg-primary/20 transition-colors">
            <ExternalLink className="h-3 w-3" />
            Aç
          </button>
        </div>
      </div>

      <div ref={dragRef} onPointerDown={handlePointerDown} onPointerMove={handlePointerMove} onPointerUp={handlePointerUp} className="absolute right-0 top-0 h-full w-2 cursor-ew-resize" />
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
      <div className="min-h-screen p-6 bg-background">
        <div className="max-w-6xl mx-auto">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-2xl font-bold text-foreground">Dashboard</h1>
              <p className="text-sm text-muted-foreground mt-1">İş sahələrinizi idarə edin və monitorinq edin</p>
            </div>
            <Link to="/workspaces" className="flex items-center gap-2 h-10 px-4 rounded-lg font-medium text-sm text-primary-foreground transition-all duration-200 hover:opacity-90 active:scale-[0.98]" style={{ background: 'var(--gradient-primary)' }}>
              <Plus className="h-4 w-4" />
              Yeni Workspace
            </Link>
          </div>

          {/* Quick Links */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-6">
            <Link to="/workspaces" className="block p-4 rounded-lg border border-border bg-secondary hover:shadow-md">
              <div className="text-lg font-semibold">Workspaces</div>
              <div className="text-sm text-muted-foreground">Manage workspaces and integrations</div>
            </Link>
            <Link to="/dashboard/integrations" className="block p-4 rounded-lg border border-border bg-secondary hover:shadow-md">
              <div className="text-lg font-semibold">Integrations</div>
              <div className="text-sm text-muted-foreground">Configure global integrations</div>
            </Link>
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
      </div>
    </DashboardLayout>
  );
};

export default DashboardPage;
