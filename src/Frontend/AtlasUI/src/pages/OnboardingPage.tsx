import React, { useEffect, useState } from 'react';
import AtlasLogo from '@/components/AtlasLogo';
import { getJson, postJson } from '@/lib/api';

type OnboardingOptionDto = {
  id: string;
  text: string;
};

type OnboardingQuestionDto = {
  id: string;
  text: string;
  isMultiSelect: boolean;
  options: OnboardingOptionDto[];
};

const OnboardingPage: React.FC = () => {
  const [questions, setQuestions] = useState<OnboardingQuestionDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [profession, setProfession] = useState<number | null>(null);
  const [jobTitle, setJobTitle] = useState<string>('');
  const [answers, setAnswers] = useState<Record<string, string[]>>({});

  useEffect(() => {
    if (profession === null) return;
    const load = async () => {
      setLoading(true);
      setError(null);
      try {
        const data = await getJson<OnboardingQuestionDto[]>(`/api/onboarding/questions?profession=${profession}`);
        setQuestions(data || []);
      } catch (err: any) {
        setError(err?.message || 'Failed to load questions.');
      } finally {
        setLoading(false);
      }
    };
    load();
  }, [profession]);

  const toggleOption = (questionId: string, optionId: string, isMulti: boolean) => {
    setAnswers(prev => {
      const arr = prev[questionId] ? [...prev[questionId]] : [];
      if (isMulti) {
        const idx = arr.indexOf(optionId);
        if (idx >= 0) arr.splice(idx, 1);
        else arr.push(optionId);
      } else {
        if (arr.length === 1 && arr[0] === optionId) return { ...prev, [questionId]: [] };
        return { ...prev, [questionId]: [optionId] };
      }
      return { ...prev, [questionId]: arr };
    });
  };

  const handleSubmit = async (e?: React.FormEvent) => {
    e?.preventDefault();
    setError(null);

    if (!profession) { setError('Please select your profession.'); return; }

    const answersPayload: { questionId: string; optionId: string }[] = [];
    for (const q of questions) {
      const sel = answers[q.id] ?? [];
      if (sel.length === 0) continue;
      for (const optId of sel) answersPayload.push({ questionId: q.id, optionId: optId });
    }

    const payload = {
      userId: '',
      profession,
      jobTitle,
      answers: answersPayload,
    };

    try {
      await postJson('/api/onboarding/complete', payload);
      window.location.href = '/dashboard';
    } catch (err: any) {
      setError(err?.message || 'Failed to complete onboarding.');
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

        <div className="glass rounded-2xl p-6 space-y-6">
          {error && <div className="text-sm text-destructive">{error}</div>}

          {!profession ? (
            <div className="space-y-4">
              <p className="font-medium">Select your profession</p>
              <div className="flex gap-3">
                <button type="button" onClick={() => setProfession(1)} className={`px-4 py-2 rounded-lg ${profession === 1 ? 'bg-primary text-primary-foreground' : 'bg-secondary'}`}>Developer</button>
                <button type="button" onClick={() => setProfession(2)} className={`px-4 py-2 rounded-lg ${profession === 2 ? 'bg-primary text-primary-foreground' : 'bg-secondary'}`}>Designer</button>
                <button type="button" onClick={() => setProfession(3)} className={`px-4 py-2 rounded-lg ${profession === 3 ? 'bg-primary text-primary-foreground' : 'bg-secondary'}`}>Product</button>
              </div>
              <p className="text-sm text-muted-foreground">Choose a profession to load the relevant questions.</p>
            </div>
          ) : (
            <form onSubmit={handleSubmit} className="space-y-6">
              <div>
                <label className="block text-sm font-medium mb-2">Job title (optional)</label>
                <input value={jobTitle} onChange={e => setJobTitle(e.target.value)} placeholder="e.g. Frontend Engineer" className="w-full px-3 py-2 rounded-lg bg-secondary border border-border" />
              </div>

              {loading ? (
                <div>Loading questions...</div>
              ) : (
                <div className="space-y-4">
                  {questions.map(q => (
                    <div key={q.id} className="p-4 rounded-lg bg-muted/5">
                      <p className="font-medium mb-3">{q.text}</p>
                      <div className="flex flex-wrap gap-2">
                        {q.options.map(opt => {
                          const selected = (answers[q.id] ?? []).includes(opt.id);
                          return (
                            <button
                              key={opt.id}
                              type="button"
                              onClick={() => toggleOption(q.id, opt.id, q.isMultiSelect)}
                              className={`px-3 py-2 rounded-lg border ${selected ? 'bg-primary text-primary-foreground' : 'bg-secondary'}`}
                            >
                              {opt.text}
                            </button>
                          );
                        })}
                      </div>
                    </div>
                  ))}
                </div>
              )}

              <div className="flex justify-between gap-3">
                <button type="button" onClick={() => setProfession(null)} className="px-4 py-2 rounded-lg bg-secondary">Back</button>
                <div className="flex gap-3">
                  <button type="button" onClick={() => { window.location.href = '/dashboard'; }} className="px-4 py-2 rounded-lg bg-secondary">Skip</button>
                  <button type="submit" className="px-4 py-2 rounded-lg bg-primary text-primary-foreground">Complete</button>
                </div>
              </div>
            </form>
          )}
        </div>
      </div>
    </div>
  );
};

export default OnboardingPage;

