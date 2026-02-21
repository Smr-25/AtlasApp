import React from 'react';
import { Activity } from 'lucide-react';

export default function DockerWidget(){
  return (
    <div className="glass rounded-2xl p-4 flex items-center justify-between hover:shadow-lg transition-all">
      <div className="flex items-center gap-3">
        <div className="w-8 h-8 rounded-full bg-[#10b981]/20 flex items-center justify-center">
          <span className="w-2 h-2 rounded-full bg-emerald-400 animate-pulse block" />
        </div>
        <div>
          <div className="font-medium">Docker</div>
          <div className="text-sm text-muted-foreground">4 Containers Running</div>
        </div>
      </div>
      <Activity className="h-5 w-5 text-muted-foreground" />
    </div>
  );
}

