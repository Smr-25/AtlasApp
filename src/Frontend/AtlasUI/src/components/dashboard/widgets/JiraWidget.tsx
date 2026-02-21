import React from 'react';
import { FileText, Target } from 'lucide-react';
import { Button } from '@/components/ui/button';

const tasks = [
  { id: 'J-101', title: 'Implement search indexing' },
  { id: 'J-102', title: 'Refactor onboarding flow' },
  { id: 'J-103', title: 'Stabilize docker-compose' },
];

export default function JiraWidget(){
  return (
    <div className="glass rounded-2xl p-4 hover:shadow-lg transition-all">
      <div className="flex items-center justify-between mb-3">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-[#ef4444]/20 flex items-center justify-center text-white">
            <FileText className="h-5 w-5 text-red-400" />
          </div>
          <div>
            <div className="font-medium">Jira</div>
            <div className="text-sm text-muted-foreground">3 In Progress</div>
          </div>
        </div>
        <Button variant="ghost" size="sm">View</Button>
      </div>

      <div className="space-y-2">
        {tasks.map(t => (
          <div key={t.id} className="flex items-center justify-between p-2 rounded-md border border-border bg-secondary">
            <div>
              <div className="text-sm font-medium">{t.title}</div>
              <div className="text-xs text-muted-foreground">{t.id}</div>
            </div>
            <div className="text-xs text-primary">In Progress</div>
          </div>
        ))}
      </div>
    </div>
  );
}

