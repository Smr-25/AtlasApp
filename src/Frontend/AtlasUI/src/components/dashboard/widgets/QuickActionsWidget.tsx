import React from 'react';
import { Terminal, Trash2, Play } from 'lucide-react';
import { Button } from '@/components/ui/button';

export default function QuickActionsWidget(){
  return (
    <div className="glass rounded-2xl p-4 hover:shadow-lg transition-all">
      <div className="flex items-center justify-between">
        <div>
          <div className="font-medium">Quick Actions</div>
          <div className="text-sm text-muted-foreground">Terminal shortcuts</div>
        </div>
      </div>

      <div className="flex gap-2 mt-3">
        <Button size="sm" className="flex items-center gap-2"><Play className="h-4 w-4"/> Spin Up Env</Button>
        <Button size="sm" variant="outline" className="flex items-center gap-2"><Trash2 className="h-4 w-4"/> Flush Cache</Button>
        <Button size="sm" variant="destructive" className="flex items-center gap-2"><Terminal className="h-4 w-4"/> Nuke DB</Button>
      </div>
    </div>
  );
}

