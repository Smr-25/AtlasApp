import React from 'react';

type SnippetDto = {
  id: string;
  title: string;
  code: string;
  language: string;
  tags?: string[];
  isFavorite?: boolean;
  createdAt?: string;
};

export default function SnippetCard({ snippet, onToggleFavorite }: { snippet: SnippetDto; onToggleFavorite: () => void }) {
  return (
    <div className="border rounded p-4 bg-white shadow-sm">
      <div className="flex items-start justify-between">
        <div>
          <h3 className="text-lg font-medium">{snippet.title}</h3>
          <div className="text-sm text-muted-foreground">{snippet.language}</div>
        </div>
        <div>
          <button onClick={onToggleFavorite} className="text-xl" title="Toggle favorite">{snippet.isFavorite ? '★' : '☆'}</button>
        </div>
      </div>
      <pre className="mt-3 p-3 bg-gray-50 rounded overflow-auto text-sm"><code>{snippet.code}</code></pre>
    </div>
  );
}

