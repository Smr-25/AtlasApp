import { useQuery, useMutation } from '@tanstack/react-query'
import { motion } from 'framer-motion'
import { Crown, Check, Zap, Shield, CreditCard, ExternalLink, BarChart3 } from 'lucide-react'
import AppShell from '@/components/layout/AppShell'
import { subscriptions } from '@/lib/apiClient'
import { queryKeys } from '@/lib/queryKeys'
import { toast } from 'sonner'

const PLANS = [
  {
    id: 'free',
    name: 'Free',
    price: '$0',
    description: 'Get started with the basics',
    icon: Zap,
    features: [
      '3 workspaces',
      '5 integrations',
      'Basic dashboard',
      'Community support',
    ],
    color: 'from-slate-600 to-slate-500',
    tier: 0,
  },
  {
    id: 'pro',
    name: 'Pro',
    price: '$12',
    period: '/month',
    description: 'Everything you need to be productive',
    icon: Crown,
    features: [
      '10 workspaces',
      '20 integrations',
      'Custom hotkeys',
      'AI agents access',
      'Priority support',
      'All role dashboards',
    ],
    color: 'from-primary to-primary/70',
    tier: 1,
    popular: true,
    priceId: import.meta.env.VITE_STRIPE_PRO_PRICE_ID ?? 'price_pro',
  },
  {
    id: 'team',
    name: 'Team',
    price: '$39',
    period: '/month',
    description: 'For growing teams that need more',
    icon: Shield,
    features: [
      'Unlimited workspaces',
      'Unlimited integrations',
      'Full team features',
      'Squad Arena & Radar',
      'Dedicated support',
      'Custom billing',
    ],
    color: 'from-purple-600 to-violet-500',
    tier: 2,
    priceId: import.meta.env.VITE_STRIPE_TEAM_PRICE_ID ?? 'price_team',
  },
]

export default function SubscriptionPage() {
  const { data: current, isLoading } = useQuery({
    queryKey: queryKeys.subscription.current(),
    queryFn: () => subscriptions.current(),
    staleTime: 60000,
    retry: false,
  })

  const { data: usage } = useQuery({
    queryKey: queryKeys.subscription.usage(),
    queryFn: () => subscriptions.usage(),
    staleTime: 60000,
    retry: false,
  })

  const checkoutMutation = useMutation({
    mutationFn: (priceId: string) =>
      subscriptions.checkout({
        priceId,
        successUrl: `${window.location.origin}/subscription?success=1`,
        cancelUrl: `${window.location.origin}/subscription`,
      }),
    onSuccess: (data) => {
      if (data?.url) window.location.href = data.url
    },
    onError: () => toast.error('Could not start checkout'),
  })

  const portalMutation = useMutation({
    mutationFn: () =>
      subscriptions.portal({ returnUrl: `${window.location.origin}/subscription` }),
    onSuccess: (data) => {
      if (data?.url) window.location.href = data.url
    },
    onError: () => toast.error('Could not open billing portal'),
  })

  const cancelMutation = useMutation({
    mutationFn: () => subscriptions.cancel(),
    onSuccess: () => toast.success('Subscription cancelled'),
    onError: () => toast.error('Could not cancel subscription'),
  })

  const currentTier = current?.tier === 'Pro' ? 1 : current?.tier === 'Team' ? 2 : 0
  const currentStatus = current?.status ?? 'Active'

  return (
    <AppShell>
      <div className="max-w-5xl mx-auto space-y-8">
        {/* Header */}
        <motion.div
          initial={{ opacity: 0, y: -10 }}
          animate={{ opacity: 1, y: 0 }}
          className="text-center"
        >
          <h1 className="text-3xl font-bold text-foreground">Plans & Billing</h1>
          <p className="text-muted-foreground mt-2">Choose the plan that fits your workflow</p>
        </motion.div>

        {/* Current Status */}
        {!isLoading && current && (
          <motion.div
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            className="rounded-2xl border border-border bg-card p-5"
          >
            <div className="flex items-center justify-between flex-wrap gap-4">
              <div>
                <p className="text-xs text-muted-foreground mb-1">Current Plan</p>
                <h3 className="text-lg font-bold text-foreground">{current?.tier ?? 'Free'}</h3>
                <div className={`inline-flex items-center gap-1 text-xs px-2 py-0.5 rounded-full mt-1 ${
                  currentStatus === 'Active' ? 'bg-emerald-500/10 text-emerald-600' : 'bg-red-500/10 text-red-500'
                }`}>
                  <div className={`w-1.5 h-1.5 rounded-full ${currentStatus === 'Active' ? 'bg-emerald-500' : 'bg-red-500'}`} />
                  {currentStatus}
                </div>
              </div>

              {usage && (
                <div className="grid grid-cols-2 gap-6">
                  <div>
                    <p className="text-xs text-muted-foreground">Workspaces</p>
                    <div className="flex items-center gap-2 mt-1">
                      <div className="flex-1 h-1.5 bg-muted rounded-full overflow-hidden">
                        <div
                          className="h-full bg-primary rounded-full"
                          style={{ width: `${((usage.currentWorkspaces ?? 0) / (usage.maxWorkspaces ?? 1)) * 100}%` }}
                        />
                      </div>
                      <span className="text-xs text-foreground whitespace-nowrap">
                        {usage.currentWorkspaces ?? 0} / {usage.maxWorkspaces ?? '∞'}
                      </span>
                    </div>
                  </div>
                  <div>
                    <p className="text-xs text-muted-foreground">Integrations</p>
                    <div className="flex items-center gap-2 mt-1">
                      <div className="flex-1 h-1.5 bg-muted rounded-full overflow-hidden">
                        <div
                          className="h-full bg-primary rounded-full"
                          style={{ width: `${((usage.currentIntegrations ?? 0) / (usage.maxIntegrations ?? 1)) * 100}%` }}
                        />
                      </div>
                      <span className="text-xs text-foreground whitespace-nowrap">
                        {usage.currentIntegrations ?? 0} / {usage.maxIntegrations ?? '∞'}
                      </span>
                    </div>
                  </div>
                </div>
              )}

              <div className="flex gap-2">
                <motion.button
                  whileTap={{ scale: 0.95 }}
                  onClick={() => portalMutation.mutate()}
                  className="flex items-center gap-1.5 px-3 py-2 rounded-xl bg-muted text-foreground text-xs font-medium hover:bg-muted/80"
                >
                  <CreditCard className="w-3.5 h-3.5" />
                  Manage Billing
                  <ExternalLink className="w-3 h-3" />
                </motion.button>
              </div>
            </div>
          </motion.div>
        )}

        {/* Plans */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-5">
          {PLANS.map((plan, idx) => {
            const isCurrentPlan = currentTier === plan.tier
            const PlanIcon = plan.icon

            return (
              <motion.div
                key={plan.id}
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: idx * 0.1 }}
                whileHover={{ y: -4 }}
                className={`relative rounded-2xl border bg-card p-6 flex flex-col ${
                  plan.popular
                    ? 'border-primary shadow-lg shadow-primary/10'
                    : 'border-border'
                }`}
              >
                {plan.popular && (
                  <div className="absolute -top-3 left-1/2 -translate-x-1/2">
                    <span className="bg-primary text-primary-foreground text-[10px] font-bold px-3 py-1 rounded-full">
                      MOST POPULAR
                    </span>
                  </div>
                )}

                <div className={`w-10 h-10 rounded-xl bg-gradient-to-br ${plan.color} flex items-center justify-center mb-4`}>
                  <PlanIcon className="w-5 h-5 text-white" />
                </div>

                <h3 className="text-lg font-bold text-foreground">{plan.name}</h3>
                <p className="text-xs text-muted-foreground mb-3">{plan.description}</p>

                <div className="flex items-end gap-1 mb-5">
                  <span className="text-3xl font-bold text-foreground">{plan.price}</span>
                  {plan.period && (
                    <span className="text-sm text-muted-foreground mb-1">{plan.period}</span>
                  )}
                </div>

                <ul className="space-y-2 flex-1 mb-6">
                  {plan.features.map((feature) => (
                    <li key={feature} className="flex items-center gap-2 text-sm text-foreground">
                      <Check className="w-3.5 h-3.5 text-emerald-500 shrink-0" />
                      {feature}
                    </li>
                  ))}
                </ul>

                {isCurrentPlan ? (
                  <div className="w-full h-10 rounded-xl bg-emerald-500/10 text-emerald-600 text-sm font-medium flex items-center justify-center gap-2">
                    <Check className="w-4 h-4" />
                    Current Plan
                  </div>
                ) : (
                  <motion.button
                    whileTap={{ scale: 0.97 }}
                    onClick={() => plan.priceId && checkoutMutation.mutate(plan.priceId)}
                    disabled={!plan.priceId || checkoutMutation.isPending}
                    className={`w-full h-10 rounded-xl text-sm font-semibold transition-all disabled:opacity-50 ${
                      plan.popular
                        ? 'bg-primary text-primary-foreground hover:bg-primary/90'
                        : 'bg-muted text-foreground hover:bg-muted/80'
                    }`}
                  >
                    {plan.tier === 0 ? 'Downgrade to Free' : `Upgrade to ${plan.name}`}
                  </motion.button>
                )}
              </motion.div>
            )
          })}
        </div>

        {/* Cancel */}
        {currentTier > 0 && (
          <div className="text-center">
            <button
              onClick={() => {
                if (confirm('Are you sure you want to cancel your subscription?')) {
                  cancelMutation.mutate()
                }
              }}
              className="text-xs text-muted-foreground hover:text-red-500 transition-colors"
            >
              Cancel subscription
            </button>
          </div>
        )}
      </div>
    </AppShell>
  )
}

