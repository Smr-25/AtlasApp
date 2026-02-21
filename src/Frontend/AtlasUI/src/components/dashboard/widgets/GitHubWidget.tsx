import React from 'react';
import { Github } from 'lucide-react';
import { Dialog, DialogTrigger, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogFooter, DialogClose } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';

const mockPRs = [
	{ id: 'PR-123', title: 'Fix authentication bug', author: 'alice', branch: 'fix/auth' },
	{ id: 'PR-124', title: 'Add onboarding seeder', author: 'bob', branch: 'feature/onboarding' },
	{ id: 'PR-125', title: 'Improve docker startup', author: 'carol', branch: 'chore/docker' },
];

export default function GitHubWidget() {
	return (
		<Dialog>
			<DialogTrigger asChild>
				<div className="glass rounded-2xl p-4 hover:shadow-lg cursor-pointer transition-all">
					<div className="flex items-center justify-between">
						<div className="flex items-center gap-3">
							<div className="w-10 h-10 rounded-xl bg-gradient-to-br from-[#3b82f6] to-[#0ea5e9] flex items-center justify-center text-white">
								<Github className="h-5 w-5" />
							</div>
							<div>
								<div className="font-medium text-lg">GitHub</div>
								<div className="text-sm text-muted-foreground">3 Pending PRs · 5 New Commits</div>
							</div>
						</div>

						<div className="opacity-80 p-1 rounded-md border border-border text-muted-foreground text-xs">Open</div>
					</div>
				</div>
			</DialogTrigger>

			<DialogContent className="max-w-3xl">
				<div className="absolute right-4 top-4 text-sm text-muted-foreground">
					<a href="#" className="underline text-muted-foreground">Open in Browser</a>
				</div>

				<DialogHeader>
					<DialogTitle>Pending Pull Requests</DialogTitle>
					<DialogDescription>Review and take actions on the latest PRs.</DialogDescription>
				</DialogHeader>

				<div className="space-y-3 mt-2">
					{mockPRs.map(pr => (
						<div key={pr.id} className="flex items-center justify-between p-3 rounded-lg border border-border bg-secondary">
							<div>
								<div className="font-medium">{pr.title}</div>
								<div className="text-xs text-muted-foreground">{pr.author} · {pr.branch}</div>
							</div>
							<div className="flex items-center gap-2">
								<Button size="sm" variant="outline">Approve</Button>
								<Button size="sm">Merge</Button>
							</div>
						</div>
					))}
				</div>

				<DialogFooter>
					<a href="#" className="text-sm text-muted-foreground mr-auto">Open in Browser</a>
					<DialogClose asChild>
						<Button>Close</Button>
					</DialogClose>
				</DialogFooter>
			</DialogContent>
		</Dialog>
	);
}
