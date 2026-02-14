import React, { useState } from 'react';
import { Search, Plus, Settings2, ExternalLink, Trash2 } from 'lucide-react';
import DashboardLayout from '@/components/dashboard/DashboardLayout';

interface Integration {
  id: string;
  name: string;
  description: string;
  icon: string;
  connected: boolean;
  lastSync?: string;
}

const availableIntegrations: Integration[] = [
  { id: 'github', name: 'GitHub', description: 'Repo və PR idarəetməsi', icon: '🐙', connected: true, lastSync: '5 dəq əvvəl' },
  { id: 'gitlab', name: 'GitLab', description: 'CI/CD pipeline inteqrasiyası', icon: '🦊', connected: false },
  { id: 'slack', name: 'Slack', description: 'Komanda bildirişləri', icon: '💬', connected: true, lastSync: '1 saat əvvəl' },
  { id: 'jira', name: 'Jira', description: 'Tapşırıq izləmə sistemi', icon: '📋', connected: false },
  { id: 'docker', name: 'Docker', description: 'Konteyner idarəetməsi', icon: '🐳', connected: false },
  { id: 'aws', name: 'AWS', description: 'Bulud infrastrukturu', icon: '☁️', connected: true, lastSync: '30 dəq əvvəl' },
  { id: 'figma', name: 'Figma', description: 'Dizayn faylları sinxronizasiyası', icon: '🎨', connected: false },
  { id: 'notion', name: 'Notion', description: 'Sənəd və qeyd idarəetməsi', icon: '📝', connected: false },
  { id: 'vercel', name: 'Vercel', description: 'Deployment idarəetməsi', icon: '▲', connected: false },
];

const IntegrationsPage: React.FC = () => {
  const [search, setSearch] = useState('');
  const [integrations, setIntegrations] = useState(availableIntegrations);

  const connected = integrations.filter(i => i.connected);
  const available = integrations.filter(i => !i.connected && i.name.toLowerCase().includes(search.toLowerCase()));

  const toggleConnection = (id: string) => {
    setIntegrations(prev => prev.map(i => i.id === id ? { ...i, connected: !i.connected, lastSync: !i.connected ? 'İndicə' : undefined } : i));
  };

  return (
    <DashboardLayout>
      <div className="space-y-8 animate-fade-in max-w-4xl">
        <div>
          <h1 className="text-2xl font-bold text-foreground">İnteqrasiyalar</h1>
          <p className="text-sm text-muted-foreground mt-1">Xarici xidmətləri qoşun və idarə edin</p>
        </div>

        {/* Connected */}
        {connected.length > 0 && (
          <div>
            <h2 className="text-base font-semibold text-foreground mb-3">Qoşulmuş inteqrasiyalar</h2>
            <div className="space-y-3">
              {connected.map(item => (
                <div key={item.id} className="glass rounded-xl p-4 flex items-center justify-between group">
                  <div className="flex items-center gap-4">
                    <div className="w-10 h-10 rounded-xl bg-secondary flex items-center justify-center text-xl">
                      {item.icon}
                    </div>
                    <div>
                      <h3 className="text-sm font-semibold text-foreground">{item.name}</h3>
                      <p className="text-xs text-muted-foreground">{item.description}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-3">
                    {item.lastSync && (
                      <span className="text-xs text-muted-foreground">Son sinx: {item.lastSync}</span>
                    )}
                    <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                      <button className="p-2 rounded-lg hover:bg-secondary transition-colors" title="Parametrlər">
                        <Settings2 className="h-4 w-4 text-muted-foreground" />
                      </button>
                      <button className="p-2 rounded-lg hover:bg-secondary transition-colors" title="Aç">
                        <ExternalLink className="h-4 w-4 text-muted-foreground" />
                      </button>
                      <button onClick={() => toggleConnection(item.id)} className="p-2 rounded-lg hover:bg-destructive/10 transition-colors" title="Sil">
                        <Trash2 className="h-4 w-4 text-muted-foreground hover:text-destructive" />
                      </button>
                    </div>
                    <div className="w-2 h-2 rounded-full bg-success" />
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Available */}
        <div>
          <div className="flex items-center justify-between mb-3">
            <h2 className="text-base font-semibold text-foreground">Mövcud inteqrasiyalar</h2>
            <div className="flex items-center gap-2 bg-secondary rounded-lg px-3 py-2 w-56">
              <Search className="h-4 w-4 text-muted-foreground" />
              <input
                placeholder="Axtar..."
                value={search}
                onChange={e => setSearch(e.target.value)}
                className="bg-transparent text-sm text-foreground placeholder:text-muted-foreground outline-none flex-1"
              />
            </div>
          </div>
          <div className="grid grid-cols-3 gap-3">
            {available.map(item => (
              <div key={item.id} className="glass rounded-xl p-4 hover:border-primary/30 transition-all duration-200 group">
                <div className="flex items-start justify-between mb-3">
                  <div className="w-10 h-10 rounded-xl bg-secondary flex items-center justify-center text-xl">
                    {item.icon}
                  </div>
                  <button
                    onClick={() => toggleConnection(item.id)}
                    className="px-3 py-1 rounded-lg text-xs font-medium bg-primary/10 text-primary hover:bg-primary/20 transition-colors"
                  >
                    Qoş
                  </button>
                </div>
                <h3 className="text-sm font-semibold text-foreground">{item.name}</h3>
                <p className="text-xs text-muted-foreground mt-1">{item.description}</p>
              </div>
            ))}
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
};

export default IntegrationsPage;
