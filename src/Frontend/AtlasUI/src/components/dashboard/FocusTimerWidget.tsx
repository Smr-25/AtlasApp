import { useState, useEffect, useRef } from 'react'
import { motion, AnimatePresence } from 'framer-motion'
import { Play, Pause, Square, RotateCcw, Timer } from 'lucide-react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { focus } from '@/lib/apiClient'
import { queryKeys } from '@/lib/queryKeys'
import { toast } from 'sonner'

const POMODORO_MINS = 25
const DEEP_WORK_MINS = 90

export default function FocusTimerWidget() {
  const qc = useQueryClient()
  const [localSeconds, setLocalSeconds] = useState(0)
  const [running, setRunning] = useState(false)
  const [mode, setMode] = useState<'pomodoro' | 'deepwork'>('pomodoro')
  const [sessionId, setSessionId] = useState<string | null>(null)
  const intervalRef = useRef<NodeJS.Timeout | null>(null)

  const totalSeconds = mode === 'pomodoro' ? POMODORO_MINS * 60 : DEEP_WORK_MINS * 60

  const { data: active } = useQuery({
    queryKey: queryKeys.focus.active(),
    queryFn: () => focus.active(),
    refetchInterval: 30000,
  })

  const startMutation = useMutation({
    mutationFn: () => focus.start({
      type: mode === 'pomodoro' ? 1 : 2,
      durationMinutes: mode === 'pomodoro' ? POMODORO_MINS : DEEP_WORK_MINS,
      label: `${mode === 'pomodoro' ? 'Pomodoro' : 'Deep Work'} session`,
    }),
    onSuccess: (data) => {
      if (data?.id) setSessionId(data.id)
    },
  })

  const completeMutation = useMutation({
    mutationFn: (id: string) => focus.complete(id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: queryKeys.focus.stats() })
      toast.success('Focus session completed! 🎉')
    },
  })

  const pauseMutation = useMutation({
    mutationFn: (id: string) => focus.pause(id),
  })

  const resumeMutation = useMutation({
    mutationFn: (id: string) => focus.resume(id),
  })

  useEffect(() => {
    if (running) {
      intervalRef.current = setInterval(() => {
        setLocalSeconds((s) => {
          if (s >= totalSeconds - 1) {
            setRunning(false)
            if (sessionId) completeMutation.mutate(sessionId)
            return totalSeconds
          }
          return s + 1
        })
      }, 1000)
    } else {
      if (intervalRef.current) clearInterval(intervalRef.current)
    }
    return () => { if (intervalRef.current) clearInterval(intervalRef.current) }
  }, [running, totalSeconds, sessionId])

  const handleStart = async () => {
    if (!sessionId) {
      const res = await startMutation.mutateAsync()
      if (res?.id) setSessionId(res.id)
    } else if (sessionId) {
      resumeMutation.mutate(sessionId)
    }
    setRunning(true)
  }

  const handlePause = () => {
    setRunning(false)
    if (sessionId) pauseMutation.mutate(sessionId)
  }

  const handleReset = () => {
    setRunning(false)
    setLocalSeconds(0)
    setSessionId(null)
  }

  const remaining = totalSeconds - localSeconds
  const mins = Math.floor(remaining / 60).toString().padStart(2, '0')
  const secs = (remaining % 60).toString().padStart(2, '0')
  const progress = (localSeconds / totalSeconds) * 100

  const circumference = 2 * Math.PI * 45

  return (
    <div className="rounded-2xl border border-border bg-card p-5">
      <div className="flex items-center justify-between mb-4">
        <div className="flex items-center gap-2">
          <Timer className="w-4 h-4 text-primary" />
          <h3 className="text-sm font-semibold text-foreground">Focus Timer</h3>
        </div>
        <div className="flex gap-1">
          {(['pomodoro', 'deepwork'] as const).map((m) => (
            <button
              key={m}
              onClick={() => { setMode(m); setLocalSeconds(0); setRunning(false); setSessionId(null) }}
              className={`text-[10px] px-2 py-0.5 rounded-md font-medium transition-colors ${
                mode === m
                  ? 'bg-primary text-primary-foreground'
                  : 'text-muted-foreground hover:text-foreground'
              }`}
            >
              {m === 'pomodoro' ? '🍅 25m' : '🧠 90m'}
            </button>
          ))}
        </div>
      </div>

      <div className="flex flex-col items-center">
        {/* Circular timer */}
        <div className="relative w-28 h-28 mb-4">
          <svg className="w-full h-full -rotate-90" viewBox="0 0 100 100">
            <circle cx="50" cy="50" r="45" fill="none" stroke="currentColor" strokeWidth="4" className="text-muted/30" />
            <motion.circle
              cx="50" cy="50" r="45"
              fill="none"
              stroke="currentColor"
              strokeWidth="4"
              strokeLinecap="round"
              strokeDasharray={circumference}
              strokeDashoffset={circumference * (1 - progress / 100)}
              className="text-primary"
              transition={{ duration: 0.5 }}
            />
          </svg>
          <div className="absolute inset-0 flex flex-col items-center justify-center">
            <span className="text-2xl font-bold text-foreground font-mono">{mins}:{secs}</span>
            <span className="text-[9px] text-muted-foreground uppercase tracking-wider">
              {running ? 'Focus' : localSeconds > 0 ? 'Paused' : 'Ready'}
            </span>
          </div>
        </div>

        {/* Controls */}
        <div className="flex items-center gap-2">
          <motion.button
            whileHover={{ scale: 1.05 }}
            whileTap={{ scale: 0.95 }}
            onClick={running ? handlePause : handleStart}
            className="flex items-center gap-1.5 px-4 py-2 rounded-xl bg-primary text-primary-foreground text-sm font-medium hover:bg-primary/90 transition-colors"
          >
            <AnimatePresence mode="wait">
              {running ? (
                <motion.span key="pause" initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }}>
                  <Pause className="w-3.5 h-3.5" />
                </motion.span>
              ) : (
                <motion.span key="play" initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }}>
                  <Play className="w-3.5 h-3.5" />
                </motion.span>
              )}
            </AnimatePresence>
            {running ? 'Pause' : localSeconds > 0 ? 'Resume' : 'Start'}
          </motion.button>
          <motion.button
            whileHover={{ scale: 1.05 }}
            whileTap={{ scale: 0.95 }}
            onClick={handleReset}
            className="w-9 h-9 rounded-xl bg-muted flex items-center justify-center text-muted-foreground hover:text-foreground transition-colors"
          >
            <RotateCcw className="w-3.5 h-3.5" />
          </motion.button>
        </div>
      </div>
    </div>
  )
}

