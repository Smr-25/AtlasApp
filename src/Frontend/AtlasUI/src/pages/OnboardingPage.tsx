import React, { useEffect, useState } from 'react';
import AtlasLogo from '@/components/AtlasLogo';
import { postJson, getJson } from '@/lib/api';

type Question = {
  id: string;
  text: string;
  options?: { id: string; text: string }[];
  profession: number;
};

const OnboardingPage: React.FC = () => {
  const [questions, setQuestions] = useState<Question[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [newQuestion, setNewQuestion] = useState('');
  const [newOption, setNewOption] = useState<Record<string, string>>({});

  const fetchQuestions = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await getJson<Question[]>('/api/onboarding/questions?profession=0');
      setQuestions(data || []);
    } catch (e: any) {
      setError(e?.message || 'Failed to load questions');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchQuestions(); }, []);

  const handleCreateQuestion = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newQuestion || !newQuestion.trim()) return setError('Question text is required.');
    setError(null);
    try {
      const id = await postJson<string>('/api/onboarding/questions', { text: newQuestion });
      setNewQuestion('');
      fetchQuestions();
    } catch (e: any) {
      setError(e?.message || 'Failed to create question');
    }
  };

  const handleAddOption = async (qId: string) => {
    const text = (newOption[qId] || '').trim();
    if (!text) return setError('Option text is required.');
    setError(null);
    try {
      const optionId = await postJson<string>(`/api/onboarding/questions/${qId}/options`, { questionId: qId, text });
      setNewOption(prev => ({ ...prev, [qId]: '' }));
      fetchQuestions();
    } catch (e: any) {
      setError(e?.message || 'Failed to add option');
    }
  };

  const handleComplete = async () => {
    setError(null);
    try {
      const profileId = await postJson<string>('/api/onboarding/complete', {});
      // navigate to dashboard/profile
      window.location.href = '/dashboard';
    } catch (e: any) {
      setError(e?.message || 'Failed to complete onboarding');
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background py-10">
      <div className="w-full max-w-3xl mx-4">
        <div className="text-center mb-6">
          <AtlasLogo />
          <h2 className="text-xl font-semibold mt-3">Onboarding</h2>
          <p className="text-sm text-muted-foreground">Answer a few quick questions to complete your profile</p>
        </div>

        <div className="glass rounded-2xl p-6 space-y-4">
          {error && <div className="text-sm text-destructive">{error}</div>}

          <form onSubmit={handleCreateQuestion} className="flex gap-2">
            <input className="flex-1 input" value={newQuestion} onChange={e => setNewQuestion(e.target.value)} placeholder="New question text" />
            <button className="btn" type="submit">Create question</button>
          </form>

          <div className="space-y-4">
            {loading ? <div>Loading...</div> : (
              questions.length === 0 ? <div className="text-sm text-muted-foreground">No questions yet</div> : (
                questions.map(q => (
                  <div key={q.id} className="p-4 border rounded-md">
                    <div className="font-medium">{q.text}</div>
                    <div className="mt-2 flex gap-2 flex-wrap">
                      {q.options && q.options.map(o => (
                        <div key={o.id} className="px-3 py-1 rounded-full bg-secondary text-sm">{o.text}</div>
                      ))}
                    </div>

                    <div className="mt-3 flex gap-2">
                      <input className="flex-1 input" placeholder="Option text" value={newOption[q.id] || ''} onChange={e => setNewOption(prev => ({ ...prev, [q.id]: e.target.value }))} />
                      <button className="btn" type="button" onClick={() => handleAddOption(q.id)}>Add</button>
                    </div>
                  </div>
                ))
              )
            )}
          </div>

          <div className="flex justify-end">
            <button className="btn btn-primary" onClick={handleComplete}>Finish onboarding</button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default OnboardingPage;

