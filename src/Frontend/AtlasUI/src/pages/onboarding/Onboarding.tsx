import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { motion, AnimatePresence } from "framer-motion";
import { ArrowRight, ArrowLeft, Check, Sparkles } from "lucide-react";
import { useAuth, UserRole } from "@/context/AuthContext";
import { onboardingQuestions, roleLabels, roleDescriptions, roleIcons } from "@/lib/onboarding-data";

const roles: UserRole[] = ["developer", "designer", "cybersecurity", "marketer", "team-leader"];

const roleColors: Record<UserRole, string> = {
  developer: "from-blue-500 to-blue-600",
  designer: "from-rose-500 to-rose-600",
  cybersecurity: "from-emerald-500 to-emerald-600",
  marketer: "from-amber-500 to-amber-600",
  "team-leader": "from-orange-500 to-orange-600",
};

const roleAccentBg: Record<UserRole, string> = {
  developer: "bg-blue-500/10 border-blue-500/30 text-blue-600",
  designer: "bg-rose-500/10 border-rose-500/30 text-rose-600",
  cybersecurity: "bg-emerald-500/10 border-emerald-500/30 text-emerald-600",
  marketer: "bg-amber-500/10 border-amber-500/30 text-amber-600",
  "team-leader": "bg-orange-500/10 border-orange-500/30 text-orange-600",
};

const roleAccentSelected: Record<UserRole, string> = {
  developer: "bg-blue-500 text-white border-blue-500 shadow-blue-500/25",
  designer: "bg-rose-500 text-white border-rose-500 shadow-rose-500/25",
  cybersecurity: "bg-emerald-500 text-white border-emerald-500 shadow-emerald-500/25",
  marketer: "bg-amber-500 text-white border-amber-500 shadow-amber-500/25",
  "team-leader": "bg-orange-500 text-white border-orange-500 shadow-orange-500/25",
};

const Onboarding = () => {
  const navigate = useNavigate();
  const { user, setUserRole, completeOnboarding } = useAuth();
  const [selectedRole, setSelectedRole] = useState<UserRole | null>(user?.role || null);
  const [currentStep, setCurrentStep] = useState(0); // 0 = role selection, 1+ = questions
  const [answers, setAnswers] = useState<Record<string, string | string[]>>({});

  const questions = selectedRole ? onboardingQuestions[selectedRole] : [];
  const totalSteps = questions.length + 1;
  const progress = ((currentStep + 1) / totalSteps) * 100;

  const handleRoleSelect = (role: UserRole) => {
    setSelectedRole(role);
    setUserRole(role);
    setAnswers({});
  };

  const handleAnswer = (questionId: string, option: string, multiSelect?: boolean) => {
    if (multiSelect) {
      const current = (answers[questionId] as string[]) || [];
      const updated = current.includes(option)
        ? current.filter((o) => o !== option)
        : [...current, option];
      setAnswers({ ...answers, [questionId]: updated });
    } else {
      setAnswers({ ...answers, [questionId]: option });
    }
  };

  const isAnswered = (questionId: string) => {
    const answer = answers[questionId];
    if (Array.isArray(answer)) return answer.length > 0;
    return !!answer;
  };

  const isOptionSelected = (questionId: string, option: string) => {
    const answer = answers[questionId];
    if (Array.isArray(answer)) return answer.includes(option);
    return answer === option;
  };

  const handleNext = async () => {
    if (currentStep === 0 && !selectedRole) return;
    if (currentStep > 0 && !isAnswered(questions[currentStep - 1].id)) return;

    if (currentStep < totalSteps - 1) {
      setCurrentStep(currentStep + 1);
    } else {
      // Complete onboarding
      const flatAnswers: Record<string, string> = {};
      Object.entries(answers).forEach(([key, val]) => {
        flatAnswers[key] = Array.isArray(val) ? val.join(", ") : val;
      });
      flatAnswers.role = selectedRole!;
      const ok = await completeOnboarding(flatAnswers);
      if (ok) navigate("/");
    }
  };

  const handleBack = () => {
    if (currentStep > 0) setCurrentStep(currentStep - 1);
  };

  const canProceed = currentStep === 0 ? !!selectedRole : isAnswered(questions[currentStep - 1]?.id);

  return (
    <div className="min-h-screen bg-background flex flex-col">
      {/* Header */}
      <div className="border-b border-border bg-card px-6 py-4">
        <div className="max-w-3xl mx-auto flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="w-9 h-9 rounded-lg bg-primary flex items-center justify-center shadow-md shadow-primary/30">
              <span className="text-primary-foreground font-semibold text-sm">M</span>
            </div>
            <span className="text-foreground font-semibold">Momentum</span>
          </div>
          <span className="text-xs text-muted-foreground">
            Step {currentStep + 1} of {totalSteps}
          </span>
        </div>
        {/* Progress bar */}
        <div className="max-w-3xl mx-auto mt-3">
          <div className="h-1.5 bg-muted rounded-full overflow-hidden">
            <motion.div
              className={`h-full rounded-full bg-gradient-to-r ${selectedRole ? roleColors[selectedRole] : "from-primary to-primary"}`}
              animate={{ width: `${progress}%` }}
              transition={{ duration: 0.4, ease: "easeOut" }}
            />
          </div>
        </div>
      </div>

      {/* Content */}
      <div className="flex-1 flex items-center justify-center px-6 py-12">
        <div className="w-full max-w-2xl">
          <AnimatePresence mode="wait">
            {currentStep === 0 ? (
              <motion.div
                key="role-select"
                initial={{ opacity: 0, x: 20 }}
                animate={{ opacity: 1, x: 0 }}
                exit={{ opacity: 0, x: -20 }}
                transition={{ duration: 0.3 }}
              >
                <div className="text-center mb-8">
                  <motion.div
                    initial={{ scale: 0 }}
                    animate={{ scale: 1 }}
                    className="w-14 h-14 rounded-2xl bg-primary/10 flex items-center justify-center mx-auto mb-4"
                  >
                    <Sparkles className="w-7 h-7 text-primary" />
                  </motion.div>
                  <h2 className="text-2xl font-bold text-foreground mb-2">What's your role?</h2>
                  <p className="text-muted-foreground text-sm">
                    Select your specialization to personalize your workspace
                  </p>
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                  {roles.map((role, i) => (
                    <motion.button
                      key={role}
                      initial={{ opacity: 0, y: 10 }}
                      animate={{ opacity: 1, y: 0 }}
                      transition={{ delay: i * 0.08 }}
                      onClick={() => handleRoleSelect(role)}
                      className={`relative p-5 rounded-2xl border-2 text-left transition-all ${
                        selectedRole === role
                          ? roleAccentSelected[role] + " shadow-lg"
                          : "border-border hover:border-primary/30 bg-card"
                      }`}
                    >
                      {selectedRole === role && (
                        <motion.div
                          initial={{ scale: 0 }}
                          animate={{ scale: 1 }}
                          className="absolute top-3 right-3 w-6 h-6 rounded-full bg-white/20 flex items-center justify-center"
                        >
                          <Check className="w-4 h-4" />
                        </motion.div>
                      )}
                      <span className="text-2xl mb-2 block">{roleIcons[role]}</span>
                      <h3 className={`font-semibold text-sm mb-1 ${selectedRole === role ? "text-white" : "text-foreground"}`}>
                        {roleLabels[role]}
                      </h3>
                      <p className={`text-xs ${selectedRole === role ? "text-white/70" : "text-muted-foreground"}`}>
                        {roleDescriptions[role]}
                      </p>
                    </motion.button>
                  ))}
                </div>
              </motion.div>
            ) : (
              <motion.div
                key={`question-${currentStep}`}
                initial={{ opacity: 0, x: 20 }}
                animate={{ opacity: 1, x: 0 }}
                exit={{ opacity: 0, x: -20 }}
                transition={{ duration: 0.3 }}
              >
                {(() => {
                  const q = questions[currentStep - 1];
                  return (
                    <>
                      <div className="text-center mb-8">
                        <div className={`inline-flex px-3 py-1 rounded-full text-xs font-medium border mb-4 ${selectedRole ? roleAccentBg[selectedRole] : ""}`}>
                          {roleLabels[selectedRole!]}
                        </div>
                        <h2 className="text-2xl font-bold text-foreground mb-2">{q.question}</h2>
                        {q.multiSelect && (
                          <p className="text-muted-foreground text-sm">Select all that apply</p>
                        )}
                      </div>

                      <div className="grid grid-cols-2 gap-3">
                        {q.options.map((option, i) => (
                          <motion.button
                            key={option}
                            initial={{ opacity: 0, y: 10 }}
                            animate={{ opacity: 1, y: 0 }}
                            transition={{ delay: i * 0.05 }}
                            onClick={() => handleAnswer(q.id, option, q.multiSelect)}
                            className={`p-4 rounded-xl border-2 text-left text-sm font-medium transition-all ${
                              isOptionSelected(q.id, option)
                                ? selectedRole
                                  ? roleAccentSelected[selectedRole] + " shadow-lg"
                                  : "bg-primary text-primary-foreground border-primary"
                                : "border-border bg-card text-foreground hover:border-primary/30"
                            }`}
                          >
                            <div className="flex items-center justify-between">
                              <span className={isOptionSelected(q.id, option) ? "text-white" : ""}>{option}</span>
                              {isOptionSelected(q.id, option) && (
                                <motion.div initial={{ scale: 0 }} animate={{ scale: 1 }}>
                                  <Check className="w-4 h-4" />
                                </motion.div>
                              )}
                            </div>
                          </motion.button>
                        ))}
                      </div>
                    </>
                  );
                })()}
              </motion.div>
            )}
          </AnimatePresence>

          {/* Navigation */}
          <div className="flex justify-between mt-10">
            <button
              onClick={handleBack}
              disabled={currentStep === 0}
              className="flex items-center gap-2 px-5 h-11 rounded-xl border border-border text-sm text-foreground hover:bg-muted transition-colors disabled:opacity-30 disabled:cursor-not-allowed"
            >
              <ArrowLeft className="w-4 h-4" />
              Back
            </button>
            <motion.button
              whileHover={{ scale: canProceed ? 1.02 : 1 }}
              whileTap={{ scale: canProceed ? 0.98 : 1 }}
              onClick={handleNext}
              disabled={!canProceed}
              className={`flex items-center gap-2 px-6 h-11 rounded-xl font-medium text-sm shadow-lg transition-all disabled:opacity-40 disabled:cursor-not-allowed bg-gradient-to-r text-white ${
                selectedRole ? roleColors[selectedRole] : "from-primary to-primary"
              }`}
            >
              {currentStep === totalSteps - 1 ? "Get Started" : "Continue"}
              <ArrowRight className="w-4 h-4" />
            </motion.button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Onboarding;
