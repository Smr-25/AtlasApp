import React from 'react';

import DashboardLayout from '@/components/dashboard/DashboardLayout';
import GitHubWidget from '@/components/dashboard/widgets/GitHubWidget';
import DockerWidget from '@/components/dashboard/widgets/DockerWidget';
import JiraWidget from '@/components/dashboard/widgets/JiraWidget';
import QuickActionsWidget from '@/components/dashboard/widgets/QuickActionsWidget';

const DashboardPage: React.FC = () => {
  return (
    <DashboardLayout>
      <div className="min-h-screen p-6 bg-[radial-gradient(ellipse_at_top_left,_var(--tw-gradient-stops))] from-[#0f1115] to-[#07070a] text-foreground">
        <div className="max-w-7xl mx-auto">
          <div className="flex items-center justify-between mb-6">
            <div>
              <h1 className="text-2xl font-bold">Command Center</h1>
              <p className="text-sm text-muted-foreground">Control center for engineers — workspaces, integrations and live tools</p>
            </div>
            <div className="text-sm text-muted-foreground">Welcome back — good to see you</div>
          </div>

          <div className="grid grid-cols-12 gap-4 auto-rows-[120px]">
            <div className="col-span-8 row-span-2">
              <div className={`h-full`}>
                <GitHubWidget />
              </div>
            </div>

            <div className="col-span-2 row-span-1">
              <DockerWidget />
            </div>

            <div className="col-span-4 row-span-2">
              <JiraWidget />
            </div>

            <div className="col-span-2 row-span-1">
              <QuickActionsWidget />
            </div>
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
};

export default DashboardPage;
