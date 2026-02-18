import React, { useEffect, useState } from 'react';
import AtlasLogo from '@/components/AtlasLogo';
import { postJson, apiFetch } from '@/lib/api';

type OnboardingOptionDto = {
  id: string;
  text: string;
  value?: number; // optional profession enum value provided by backend
  recommendedIntegration?: string | null;
};

type OnboardingQuestionDto = {
  id: string;
  text: string;
  order?: number;
  isMultiSelect: boolean;
  isRequired?: boolean; // optional required flag
  targetProfession?: number | null;
  options: OnboardingOptionDto[];
};

enum UserProfession {
  General = 0,
  Developer = 1,
  Designer = 2,
  DevOps = 3,
  DataScientist = 4,
  CyberSecurity = 5,
  AiEngineer = 6,
  ProductManager = 7
}

const mapOptionTextToProfession = (option: OnboardingOptionDto): UserProfession => {
  // If backend provides a numeric value directly, prefer it
  if (typeof option.value === 'number') return option.value as UserProfession;

  const text = option?.text?.toLowerCase?.().trim() ?? '';
  if (text.includes('developer')) return UserProfession.Developer;
  if (text.includes('designer')) return UserProfession.Designer;
  if (text.includes('devops')) return UserProfession.DevOps;
  if (text.includes('data')) return UserProfession.DataScientist;
  if (text.includes('cyber') || text.includes('security')) return UserProfession.CyberSecurity;
  if (text.includes('ai') || text.includes('ml') || text.includes('machine')) return UserProfession.AiEngineer;
  if (text.includes('product')) return UserProfession.ProductManager;
  return UserProfession.General;
};

const Stepper: React.FC<{ step: number; steps: string[] }> = ({ step, steps }) => {
  return (
    <div className="flex items-center gap-4 mb-4">
      {steps.map((s, i) => (
        <div key={s} className="flex items-center gap-3">
          <div className={`w-8 h-8 rounded-full flex items-center justify-center ${i + 1 === step ? 'bg-primary text-primary-foreground' : 'bg-secondary text-muted-foreground'}`}>{i + 1}</div>
          <div className={`text-sm ${i + 1 === step ? 'font-medium text-foreground' : 'text-muted-foreground'}`}>{s}</div>
        </div>
      ))}
    </div>
  );
};

const OnboardingPage: React.FC = () => {
  const [professionQuestion, setProfessionQuestion] = useState<OnboardingQuestionDto | null>(null);
  const [questions, setQuestions] = useState<OnboardingQuestionDto[]>([]);
  const [selectedProfession, setSelectedProfession] = useState<UserProfession | null>(null);
  const [jobTitle, setJobTitle] = useState<string>('');
  const [answers, setAnswers] = useState<Record<string, string[]>>({});
  const [loading, setLoading] = useState(false);
  const [loadingProfession, setLoadingProfession] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [currentStep, setCurrentStep] = useState<number>(1);
  const [questionIndex, setQuestionIndex] = useState<number>(0);

  useEffect(() => {
    const load = async () => {
      setLoadingProfession(true);
      try {
        const pq = await (async (): Promise<OnboardingQuestionDto | null> => {
          const res = await apiFetch('/api/onboarding/profession-question', { method: 'GET' });
          const text = await res.text();
          let json: any;
          try { json = text ? JSON.parse(text) : null; } catch (e) { return null; }
          // Accept multiple shapes:
          if (json && (json.success !== undefined || json.isSuccess !== undefined)) return json.data as OnboardingQuestionDto;
          if (json && json.data && json.data.id) return json.data as OnboardingQuestionDto;
          if (json && json.id) return json as OnboardingQuestionDto;
          return null;
        })();
        if (!pq) {
          setError('Unexpected response from profession-question endpoint');
        } else {
          setProfessionQuestion(pq);
        }
      } catch (err: any) {
        // if ApiError with payload
        const msg = err?.data ? parseApiError(err.data, err?.status ? { status: err.status, statusText: '' } as any : undefined) : (err?.message || 'Failed to load profession question.');
        setError(msg);
      } finally {
        setLoadingProfession(false);
      }
    };
    load();
  }, []);

  const selectProfessionOption = (option: OnboardingOptionDto) => {
    setError(null);
    const prof = mapOptionTextToProfession(option);
    setSelectedProfession(prof);
    if (professionQuestion) setAnswers(prev => ({ ...prev, [professionQuestion.id]: [option.id] }));
    // Don't auto-advance here; wait for Next click to fetch questions and go to step 2
  };

  const fetchQuestionsForProfession = async (prof: UserProfession) => {
    setLoading(true);
    setError(null);
    try {
      const res = await apiFetch(`/api/onboarding/questions?profession=${prof}`, { method: 'GET' });
      const text = await res.text();
      let json: any;
      try { json = text ? JSON.parse(text) : null; } catch (e) { json = null; }
      let qs: OnboardingQuestionDto[] | null = null;
      if (json && (json.success !== undefined || json.isSuccess !== undefined)) qs = json.data as OnboardingQuestionDto[];
      else if (json && json.data && Array.isArray(json.data)) qs = json.data as OnboardingQuestionDto[];
      else if (Array.isArray(json)) qs = json as OnboardingQuestionDto[];
      if (!qs) {
        setError('Unexpected response from onboarding questions endpoint');
        setQuestions([]);
        return false;
      }
      const arr = (qs || []).filter(q => q.id !== professionQuestion?.id).sort((a, b) => (a.order ?? 0) - (b.order ?? 0));
      setQuestions(arr);
      setQuestionIndex(0);
      return true;
    } catch (err: any) {
      const msg = err?.data ? parseApiError(err.data, err?.status ? { status: err.status, statusText: '' } as any : undefined) : (err?.message || 'Failed to load questions for selected profession.');
      setError(msg);
      return false;
    } finally {
      setLoading(false);
    }
  };

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

  // helpers to navigate single-question pager
  const goToPrevQuestion = () => setQuestionIndex(i => Math.max(0, i - 1));
  const goToNextQuestion = () => setQuestionIndex(i => Math.min(questions.length - 1, i + 1));

  const answeredCount = () => {
    let cnt = 0;
    if (professionQuestion && answers[professionQuestion.id] && answers[professionQuestion.id].length) cnt++;
    for (const q of questions) if ((answers[q.id] ?? []).length) cnt++;
    return cnt;
  };

  const totalCount = () => (professionQuestion ? 1 + questions.length : questions.length);

  const progressPct = () => {
    const total = totalCount();
    if (!total) return 0;
    return Math.round((answeredCount() / total) * 100);
  };

  const validateRequired = (): string[] => {
    const missing: string[] = [];
    for (const q of questions) {
      if (q.isRequired && !(answers[q.id] && answers[q.id].length)) missing.push(q.text);
    }
    // profession question required by definition
    if (professionQuestion && !(answers[professionQuestion.id] && answers[professionQuestion.id].length)) missing.unshift(professionQuestion.text);
    return missing;
  };

  const handleNext = async () => {
    setError(null);
    if (currentStep === 1) {
      if (!selectedProfession) { setError('Please select your profession to continue.'); return; }
      const ok = await fetchQuestionsForProfession(selectedProfession);
      if (ok) setCurrentStep(2);
      return;
    }
    setCurrentStep(s => Math.min(3, s + 1));
  };

  const handleBack = () => setCurrentStep(s => Math.max(1, s - 1));

  const handleComplete = async (e?: React.FormEvent) => {
    e?.preventDefault();
    setError(null);

    const missing = validateRequired();
    if (missing.length) { setError('Please answer required questions: ' + missing.join('; ')); setCurrentStep(2); return; }

    if (selectedProfession === null) { setError('Please select your profession.'); return; }

    const answersPayload: { questionId: string; optionId: string }[] = [];
    if (professionQuestion && answers[professionQuestion.id]) {
      for (const optId of answers[professionQuestion.id]) answersPayload.push({ questionId: professionQuestion.id, optionId: optId });
    }
    for (const q of questions) {
      const sel = answers[q.id] ?? [];
      if (sel.length === 0) continue;
      for (const optId of sel) answersPayload.push({ questionId: q.id, optionId: optId });
    }

    const payload = { profession: selectedProfession, jobTitle, answers: answersPayload };

    try {
      await postJson('/api/onboarding/complete', payload);
      window.location.href = '/dashboard';
    } catch (err: any) {
      const msg = err?.data ? parseApiError(err.data, err?.status ? { status: err.status, statusText: '' } as any : undefined) : (err?.message || 'Failed to complete onboarding.');
      setError(msg);
    }
  };

  if (loadingProfession) return <div className="min-h-screen flex items-center justify-center">Loading...</div>;

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

          <Stepper step={currentStep} steps={[ 'Profession', 'Questions', 'Review' ]} />

          <div className="w-full h-2 bg-muted rounded-full overflow-hidden">
            <div className="h-full bg-primary" style={{ width: `${progressPct()}%` }} />
          </div>

          {currentStep === 1 && professionQuestion && (
            <div className="space-y-4">
              <p className="font-medium">{professionQuestion.text}</p>
              <div className="flex flex-wrap gap-3">
                {professionQuestion.options.map(opt => (
                  <button
                    key={opt.id}
                    type="button"
                    onClick={() => selectProfessionOption(opt)}
                    className={`px-4 py-2 rounded-sm border transition-all duration-150 focus:outline-none focus:ring-2 focus:ring-primary/50 ${answers[professionQuestion.id] && answers[professionQuestion.id].includes(opt.id) ? 'bg-transparent text-primary border-2 border-primary' : 'bg-secondary hover:shadow-sm'}`}>
                    {opt.text}
                  </button>
                ))}
              </div>
              <div className="flex justify-end gap-3">
                <button className="px-4 py-2 rounded-lg bg-secondary" onClick={() => window.location.href = '/dashboard'}>Skip</button>
                <button
                  className={`px-4 py-2 rounded-lg ${answers[professionQuestion.id] && answers[professionQuestion.id].length ? 'bg-primary text-primary-foreground' : 'bg-muted text-muted-foreground cursor-not-allowed'}`}
                  onClick={handleNext}
                  disabled={!(answers[professionQuestion.id] && answers[professionQuestion.id].length)}
                >
                  Next
                </button>
              </div>
            </div>
          )}

          {currentStep === 2 && (
            <form onSubmit={handleComplete} className="space-y-6">
              <div>
                <label className="block text-sm font-medium mb-2">Job title (optional)</label>
                <input value={jobTitle} onChange={e => setJobTitle(e.target.value)} placeholder="e.g. Senior Software Developer" className="w-full px-3 py-2 rounded-lg bg-secondary border border-border" />
              </div>

              {loading ? (
                <div>Loading questions...</div>
              ) : (
                <div className="space-y-4">
                  {questions.length === 0 ? (
                    <div className="text-sm text-muted-foreground">No questions for this profession.</div>
                  ) : (
                    // single-question pager view
                    (() => {
                      const q = questions[questionIndex];
                      const total = questions.length;
                      return (
                        <div className="p-4 rounded-lg bg-muted/5">
                          <div className="flex items-center justify-between mb-3">
                            <p className="font-medium">{q.text}{q.isRequired ? ' *' : ''}</p>
                            <div className="text-sm text-muted-foreground">{questionIndex + 1} / {total}</div>
                          </div>
                          <div className="flex flex-wrap gap-2 mb-4">
                            {q.options.map(opt => {
                              const selected = (answers[q.id] ?? []).includes(opt.id);
                              return (
                                <button
                                  key={opt.id}
                                  type="button"
                                  onClick={() => toggleOption(q.id, opt.id, q.isMultiSelect)}
                                  className={`px-3 py-2 rounded-sm border transition-all duration-150 focus:outline-none focus:ring-2 focus:ring-primary/50 ${selected ? 'bg-transparent text-primary border-2 border-primary' : 'bg-secondary'}`}>
                                  {opt.text}
                                </button>
                               );
                             })()}
                          </div>

                          <div className="flex justify-between">
                            <div>
                              <button type="button" onClick={handleBack} className="px-4 py-2 rounded-lg bg-secondary mr-2">Back</button>
                              <button type="button" onClick={() => { window.location.href = '/dashboard'; }} className="px-4 py-2 rounded-lg bg-secondary">Skip</button>
                            </div>
                            <div className="flex gap-2">
                              <button type="button" onClick={goToPrevQuestion} disabled={questionIndex === 0} className={`px-4 py-2 rounded-lg ${questionIndex === 0 ? 'bg-muted text-muted-foreground' : 'bg-secondary'}`}>Prev</button>
                              {questionIndex < total - 1 ? (
                                <button type="button" onClick={goToNextQuestion} className="px-4 py-2 rounded-lg bg-primary text-primary-foreground">Next</button>
                              ) : (
                                <button type="button" onClick={() => setCurrentStep(3)} className="px-4 py-2 rounded-lg bg-primary text-primary-foreground">Review</button>
                              )}
                            </div>
                          </div>
                        </div>
                      );
                    })()
                  )}
                </div>
              )}

            </form>
          )}

          {currentStep === 3 && (
            <div className="space-y-4">
              <h3 className="font-medium">Review your answers</h3>
              <div className="space-y-2">
                <div>
                  <div className="text-sm text-muted-foreground">Profession</div>
                  <div className="font-medium">{professionQuestion ? (professionQuestion.options.find(o => answers[professionQuestion.id]?.includes(o.id))?.text ?? '-') : '-'}</div>
                </div>

                <div>
                  <div className="text-sm text-muted-foreground">Job title</div>
                  <div className="font-medium">{jobTitle || '-'}</div>
                </div>

                <div>
                  <div className="text-sm text-muted-foreground">Answers</div>
                  <div className="mt-2 space-y-2">
                    {questions.map(q => (
                      <div key={q.id} className="p-2 rounded border bg-secondary">
                        <div className="text-sm text-muted-foreground">{q.text}</div>
                        <div className="font-medium">{(answers[q.id] ?? []).map(id => q.options.find(o => o.id === id)?.text).filter(Boolean).join(', ') || '-'}</div>
                      </div>
                    ))}
                  </div>
                </div>
              </div>

              <div className="flex justify-between gap-3">
                <button onClick={handleBack} className="px-4 py-2 rounded-lg bg-secondary">Back</button>
                <button onClick={handleComplete} className="px-4 py-2 rounded-lg bg-primary text-primary-foreground">Complete</button>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

function parseApiError(result: any, response?: Response) {
  if (!result) return response?.statusText || 'An unknown error occurred.';
  const errs = result.errors ?? result.Errors ?? null;
  if (Array.isArray(errs) && errs.length > 0) return errs.join('\n');
  if (errs && typeof errs === 'object') {
    try {
      const parts: string[] = [];
      for (const key of Object.keys(errs)) {
        const v = errs[key];
        if (Array.isArray(v)) parts.push(`${key}: ${v.join(', ')}`);
        else parts.push(`${key}: ${String(v)}`);
      }
      if (parts.length) return parts.join('\n');
    } catch (e) {}
  }
  if (result.message) return String(result.message);
  if (result.error) return String(result.error);
  if (response) return `${response.status} ${response.statusText}`;
  return 'An unexpected server error occurred.';
}

export default OnboardingPage;

