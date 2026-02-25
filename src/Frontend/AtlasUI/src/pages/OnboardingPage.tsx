import * as React from "react";
import { useEffect, useState } from "react";
import { onboarding } from "@/lib/api";
import { useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

interface Question {
  id: string;
  text: string;
  options?: { id: string; text: string }[];
  isMultiSelect?: boolean;
}

export default function OnboardingPage() {
  const navigate = useNavigate();
  const [professionQuestion, setProfessionQuestion] = useState<any | null>(null);
  const [profession, setProfession] = useState<string | null>(null);
  const [questions, setQuestions] = useState<Question[]>([]);
  const [answers, setAnswers] = useState<Record<string, any>>({});
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  useEffect(() => {
    onboarding.getProfessionQuestion().then(setProfessionQuestion).catch(() => {});
  }, []);

  useEffect(() => {
    if (!profession) return;
    onboarding.getQuestionsByProfession(profession).then((qs: any) => setQuestions(qs || [])).catch(() => {});
  }, [profession]);

  const toggleOption = (qId: string, optId: string) => {
    const q = questions.find(q => q.id === qId);
    if (!q) return;
    if (q.isMultiSelect) {
      setAnswers(a => ({ ...a, [qId]: (a[qId] || []).includes(optId) ? (a[qId] || []).filter((x: string) => x !== optId) : [...(a[qId] || []), optId] }));
    } else {
      setAnswers(a => ({ ...a, [qId]: optId }));
    }
  };

  const submit = async () => {
    setLoading(true);
    setMessage(null);
    try {
      const payload = { Answers: Object.entries(answers).map(([questionId, ans]) => ({ QuestionId: questionId, Answer: ans })) };
      const res = await onboarding.complete(payload);
      // On success, navigate to profile or dashboard
      navigate('/');
    } catch (err: any) {
      setMessage(err?.data?.errors?.map((x: any) => x.message).join('\n') || 'Failed to complete onboarding');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-start justify-center bg-background py-12">
      <div className="w-full max-w-3xl p-8 rounded-lg shadow bg-card">
        <h1 className="text-2xl font-semibold mb-4">Complete your onboarding</h1>
        <p className="text-sm text-muted-foreground mb-6">Help us tailor Atlas for you — a couple of quick questions.</p>

        {professionQuestion && !profession && (
          <div className="space-y-3">
            <div className="text-sm text-muted-foreground">{professionQuestion.text}</div>
            <div className="flex flex-wrap gap-3">
              {professionQuestion.options?.map((opt: any) => (
                <button key={opt.id} onClick={() => setProfession(opt.value)} className="px-4 py-2 rounded-md border">{opt.text}</button>
              ))}
            </div>
          </div>
        )}

        {profession && (
          <div className="space-y-4">
            <div className="text-sm text-muted-foreground">Profession: <strong>{profession}</strong></div>

            {questions.map(q => (
              <div key={q.id} className="p-4 rounded border bg-secondary">
                <div className="text-sm text-muted-foreground">{q.text}</div>
                <div className="mt-2 flex flex-wrap gap-3">
                  {q.options?.map(opt => {
                    const sel = q.isMultiSelect ? (answers[q.id] || []).includes(opt.id) : answers[q.id] === opt.id;
                    return (
                      <button key={opt.id} type="button" onClick={() => toggleOption(q.id, opt.id)} className={`px-4 py-2 rounded-sm border transition-all duration-150 ${sel ? 'border-2 border-primary text-primary' : 'border border-border bg-secondary'}`}>
                        {opt.text}
                      </button>
                    );
                  })}
                </div>
              </div>
            ))}

            <div className="flex items-center justify-between">
              <Button onClick={() => setProfession(null)}>Back</Button>
              <Button onClick={submit} disabled={loading}>{loading ? 'Saving...' : 'Finish'}</Button>
            </div>

            {message && <div className="mt-4 text-sm text-destructive">{message}</div>}
          </div>
        )}

      </div>
    </div>
  );
}

