import React from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getJson, postJson } from '@/lib/api';
import SnippetCard from '@/components/snippets/SnippetCard';
import SnippetEditor from '@/components/snippets/SnippetEditor';

type SnippetDto = {
  id: string;
  title: string;
  code: string;
  language: string;
  tags?: string[];
  isFavorite?: boolean;
  createdAt?: string;
};

export default function SnippetsPage() {
  const qc = useQueryClient();
  const [showEditor, setShowEditor] = React.useState(false);

  const { data: snippets, isLoading, error } = useQuery<SnippetDto[]>({
    queryKey: ['snippets'],
    queryFn: () => getJson<SnippetDto[]>('/api/snippets'),
  });

  const create = useMutation((payload: Partial<SnippetDto>) => postJson<SnippetDto>('/api/snippets', payload), {
    onSuccess() {
      qc.invalidateQueries(['snippets']);
      setShowEditor(false);
    },
  });

  const toggleFavorite = useMutation(async ({ id }: { id: string }) => postJson<boolean>(`/api/snippets/${id}/favorite`, {}), {
    onMutate: async ({ id }) => {
      await qc.cancelQueries(['snippets']);
      const prev = qc.getQueryData<SnippetDto[]>(['snippets']);
      if (prev) {
        qc.setQueryData(['snippets'], prev.map(s => s.id === id ? { ...s, isFavorite: !s.isFavorite } : s));
      }
      return { prev };
    },
    onError: (err, vars, context: any) => {
      if (context?.prev) qc.setQueryData(['snippets'], context.prev);
    },
    onSettled: () => qc.invalidateQueries(['snippets']),
  });

  return (
    <div className="p-6">
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-2xl font-semibold">Snippets</h2>
        <div>
          <button className="btn btn-primary" onClick={() => setShowEditor(true)}>Create snippet</button>
        </div>
      </div>

      {isLoading && <div>Loading...</div>}
      {error && <div className="text-red-600">Error loading snippets</div>}

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {snippets?.map(s => (
          <SnippetCard key={s.id} snippet={s} onToggleFavorite={() => toggleFavorite.mutate({ id: s.id })} />
        ))}
      </div>

      {showEditor && <SnippetEditor onClose={() => setShowEditor(false)} onCreate={(payload) => create.mutate(payload)} />}
    </div>
  );
}

