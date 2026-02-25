import * as React from "react";
import { useEffect, useState } from "react";
import { onboarding } from "@/lib/api";
import { useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { useAuth } from "@/hooks/useAuth";

interface Question {
  id: string;
  text: string;
  options?: { id: string; text: string }[];
  isMultiSelect?: boolean;
  required?: boolean;
}

export default function OnboardingPage() {
  const navigate = useNavigate();
  const auth = useAuth();
  const [professionQuestion, setProfessionQuestion] = useState<any | null>(null);
  const [profession, setProfession] = useState<string | null>(null);
  const [questions, setQuestions] = useState<Question[]>([]);
  const [answers, setAnswers] = useState<Record<string, any>>({});
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  // stepper state
  const [stepIndex, setStepIndex] = useState<number>(0); // 0 = profession, 1.. = questions
  const [animKey, setAnimKey] = useState(0);

  useEffect(() => {
    onboarding.getProfessionQuestion().then(setProfessionQuestion).catch(() => {});
  }, []);

  useEffect(() => {
    if (!profession) return;
    onboarding.getQuestionsByProfession(profession).then((qs: any) => setQuestions(qs || [])).catch(() => {});
  }, [profession]);

  // When questions load, ensure stepIndex is at least 1 if profession chosen
  useEffect(() => {
    if (profession && questions.length > 0 && stepIndex === 0) setStepIndex(1);
  }, [profession, questions]);

  const totalSteps = 1 + questions.length; // profession + each question

  const goToStep = (idx: number) => {
    if (idx < 0) idx = 0;
    if (idx > totalSteps) idx = totalSteps;
    setStepIndex(idx);
    // trigger simple remount animation by changing key
    setAnimKey(k => k + 1);
  };

  const toggleOption = (qId: string, optId: string) => {
    const q = questions.find(q => q.id === qId);
    if (!q) return;
    if (q.isMultiSelect) {
      setAnswers(a => ({ ...a, [qId]: (a[qId] || []).includes(optId) ? (a[qId] || []).filter((x: string) => x !== optId) : [...(a[qId] || []), optId] }));
    } else {
      setAnswers(a => ({ ...a, [qId]: optId }));
    }
  };

  const validateStep = (idx: number) => {
    if (idx === 0) return profession ? null : 'Please select a profession';
    const q = questions[idx - 1];
    if (!q) return null;
    if (q.required) {
      const ans = answers[q.id];
      if (q.isMultiSelect) {
        if (!Array.isArray(ans) || ans.length === 0) return `Please answer: ${q.text}`;
      } else {
        if (!ans) return `Please answer: ${q.text}`;
      }
    }
    return null;
  };

  const submit = async () => {
    setLoading(true);
    setMessage(null);
    // validate all required before submit
    for (const q of questions) {
      if (q.required) {
        const ans = answers[q.id];
        if (q.isMultiSelect) {
          if (!Array.isArray(ans) || ans.length === 0) {
            setMessage(`Please answer: ${q.text}`);
            setLoading(false);
            return;
          }
        } else {
          if (!ans) { setMessage(`Please answer: ${q.text}`); setLoading(false); return; }
        }
      }
    }

    const payload = { Answers: Object.entries(answers).map(([questionId, ans]) => ({ QuestionId: questionId, Answer: ans })) };

    try {
      const token = auth.state?.accessToken;
      await onboarding.complete(payload, token);
      navigate('/');
    } catch (err: any) {
      if (err?.status === 401) {
        try {
          await auth.refresh();
          const token2 = auth.state?.accessToken;
          await onboarding.complete(payload, token2);
          navigate('/');
          return;
        } catch (refreshErr: any) {
          navigate('/login');
          return;
        }
      }
      setMessage(err?.data?.errors?.map((x: any) => x.message).join('\n') || 'Failed to complete onboarding');
    } finally {
      setLoading(false);
    }
  };

  const currentStepLabel = () => {
    if (stepIndex === 0) return 'Choose profession';
    if (stepIndex > 0 && stepIndex <= questions.length) return questions[stepIndex - 1]?.text || 'Question';
    return 'Summary';
  };

  // accessibility: announce step changes
  useEffect(() => {
    const el = document.getElementById('onboarding-step-announcer');
    if (el) el.textContent = `Step ${Math.min(stepIndex + 1, totalSteps)} of ${totalSteps}: ${currentStepLabel()}`;
  }, [stepIndex, questions, profession]);

  return (
    <div className="min-h-screen flex items-start justify-center bg-background py-12">
      <div className="w-full max-w-3xl p-8 rounded-lg shadow bg-card">
        <h1 className="text-2xl font-semibold mb-2">Complete your onboarding</h1>
        <p className="text-sm text-muted-foreground mb-4">Help us tailor Atlas for you — quick & friendly questions.</p>

        <div className="mb-4">
          <div className="flex items-center justify-between mb-2">
            <div className="text-sm text-muted-foreground">Progress</div>
            <div className="text-sm font-medium">{Math.max(0, stepIndex)} / {totalSteps - 1}</div>
          </div>
          <div className="w-full bg-muted rounded h-2 overflow-hidden">
            <div className="h-2 bg-primary transition-all" style={{ width: `${(stepIndex / (totalSteps - 1)) * 100}%` }} />
          </div>
        </div>

        <div id="onboarding-step-announcer" aria-live="polite" className="sr-only" />

        {/* Step content: key by animKey to animate on change */}
        <div key={animKey} className="transition-opacity duration-250 ease-in-out">
          {stepIndex === 0 && professionQuestion && (
            <div className="space-y-3">
              <div className="text-sm text-muted-foreground">{professionQuestion.text}</div>
              <div className="flex flex-wrap gap-3">
                {professionQuestion.options?.map((opt: any) => (
                  <button key={opt.id} onClick={() => { setProfession(opt.value || opt.id); goToStep(1); }} className="px-4 py-2 rounded-md border hover:shadow-sm transition">
                    {opt.text}
                  </button>
                ))}
              </div>
            </div>
          )}

          {stepIndex > 0 && stepIndex <= questions.length && (
            <div className="space-y-6">
              <div className="text-lg font-medium">{questions[stepIndex - 1]?.text}</div>
              <div className="flex flex-wrap gap-3">
                {questions[stepIndex - 1]?.options?.map((opt: any) => {
                  const q = questions[stepIndex - 1];
                  const sel = q?.isMultiSelect ? (answers[q.id] || []).includes(opt.id) : answers[q.id] === opt.id;
                  return (
                    <button key={opt.id} type="button" onClick={() => toggleOption(q.id, opt.id)} className={`px-4 py-2 rounded-sm border transition-all duration-150 ${sel ? 'border-2 border-primary text-primary' : 'border border-border bg-secondary'}`} aria-pressed={sel} aria-label={opt.text}>
                      {opt.text}
                    </button>
                  );
                })}
              </div>

              <div className="flex items-center justify-between">
                <Button variant="ghost" onClick={() => goToStep(stepIndex - 1)}>Back</Button>
                <div className="flex items-center gap-2">
                  <Button onClick={() => {
                    const err = validateStep(stepIndex);
                    if (err) { setMessage(err); return; }
                    if (stepIndex < totalSteps - 1) goToStep(stepIndex + 1);
                    else goToStep(totalSteps);
                  }}>{stepIndex < totalSteps - 1 ? 'Next' : 'Summary'}</Button>
                </div>
              </div>

            </div>
          )}

          {/* Summary / final step */}
          {stepIndex === totalSteps && (
            <div className="space-y-4">
              <div className="text-lg font-medium">Summary</div>
              <div className="space-y-2">
                {questions.map(q => (
                  <div key={q.id} className="p-3 rounded border bg-secondary">
                    <div className="text-sm font-medium">{q.text}</div>
                    <div className="text-sm text-muted-foreground">{Array.isArray(answers[q.id]) ? answers[q.id].join(', ') : answers[q.id]}</div>
                  </div>
                ))}
              </div>

              <div className="flex items-center justify-between">
                <Button variant="ghost" onClick={() => goToStep(totalSteps - 1)}>Back</Button>
                <Button onClick={submit} disabled={loading}>{loading ? 'Saving...' : 'Finish'}</Button>
              </div>
            </div>
          )}
        </div>

        {message && <div className="mt-4 text-sm text-destructive">{message}</div>}
      </div>
    </div>
  );
}
