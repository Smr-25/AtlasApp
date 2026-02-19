import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AtlasLogo from '@/components/AtlasLogo';
import AuthInput from '@/components/auth/AuthInput';
import { getJson, putJson, apiFetch, logout } from '@/lib/api';
import TelegramIcon from '@/components/icons/Telegram';

type Profile = {
  id: string;
  userName: string;
  email: string;
  fullName: string;
  phoneNumber?: string | null;
  emailConfirmed: boolean;
  phoneNumberConfirmed: boolean;
  createdAt: string;
  status: number;
  lastLoginAt?: string | null;
};

const ProfilePage: React.FC = () => {
  const nav = useNavigate();
  const [profile, setProfile] = useState<Profile | null>(null);
  const [loading, setLoading] = useState(false);
  const [form, setForm] = useState({ fullName: '', userName: '' });

  const [telegramId, setTelegramId] = useState('');
  const [telegramError, setTelegramError] = useState<string | null>(null);
  const [telegramSuccess, setTelegramSuccess] = useState<string | null>(null);

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      try {
        const data = await getJson<Profile>('/api/accounts/profile');
        setProfile(data);
        setForm({ fullName: data.fullName || '', userName: data.userName || '' });
      } catch (err: any) {
        await logout();
        nav('/login');
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  const validate = () => {
    const e: Record<string, string> = {};
    if (form.fullName && form.fullName.length > 100) e.fullName = 'Full name cannot exceed 100 characters.';
    if (form.userName) {
      if (form.userName.length < 3) e.userName = 'Username must be at least 3 characters.';
      else if (form.userName.length > 50) e.userName = 'Username cannot exceed 50 characters.';
      else if (!/^[a-zA-Z0-9._-]+$/.test(form.userName)) e.userName = 'Username can only contain letters, numbers, dots, underscores, and hyphens.';
    }
    return e;
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    const v = validate();
    if (Object.keys(v).length) { alert(Object.values(v).join('\n')); return; }
    setLoading(true);
    try {
      const updated = await putJson<Profile>('/api/accounts/profile', { fullName: form.fullName, userName: form.userName });
      setProfile(updated);
      alert('Profile updated successfully.');
    } catch (err: any) {
      const msg = err?.details ?? err?.message ?? 'Update failed.';
      alert(String(msg));
    } finally {
      setLoading(false);
    }
  };

  const handleSetTelegram = async (e?: React.FormEvent) => {
    e?.preventDefault();
    setTelegramError(null);
    setTelegramSuccess(null);
    if (!telegramId || !telegramId.trim()) { setTelegramError('Telegram Chat ID is required.'); return; }
    try {
      await apiFetch('/api/accounts/set-telegram-chat-id', { method: 'POST', body: JSON.stringify({ telegramChatId: telegramId }) });
      setTelegramSuccess('Telegram Chat ID saved.');
    } catch (err: any) {
      setTelegramError(err?.message ?? 'Failed to set Telegram Chat ID.');
    }
  };

  if (loading && !profile) return (
    <div className="min-h-screen flex items-center justify-center">Loading...</div>
  );

  return (
    <div className="min-h-screen flex items-center justify-center bg-background p-6">
      <div className="w-full max-w-3xl">
        <div className="text-center mb-6">
          <AtlasLogo size="lg" />
          <h2 className="text-xl font-semibold mt-4">Profile</h2>
        </div>

        <div className="glass-strong rounded-2xl p-6 space-y-6">
          <form onSubmit={handleSave} className="space-y-4">
            <AuthInput label="Full name" value={form.fullName} onChange={e => setForm(f => ({ ...f, fullName: e.target.value }))} error={undefined} />
            <AuthInput label="Username" value={form.userName} onChange={e => setForm(f => ({ ...f, userName: e.target.value }))} error={undefined} />
            <div className="flex justify-end">
              <button type="submit" disabled={loading} className="px-4 py-2 rounded-lg bg-primary text-primary-foreground">Save</button>
            </div>
          </form>

          <div className="pt-4 border-t border-border">
            <h3 className="font-medium">Telegram</h3>
            <p className="text-sm text-muted-foreground mt-1">Link your Telegram for notifications and phone verification.</p>
            <div className="mt-3 flex gap-3 items-center">
              <TelegramIcon className="w-6 h-6 text-primary" />
              <input value={telegramId} onChange={e => setTelegramId(e.target.value)} placeholder="Telegram Chat ID" className="px-3 py-2 rounded-lg bg-secondary border border-border flex-1" />
              <button onClick={handleSetTelegram} className="px-3 py-2 rounded-lg bg-primary text-primary-foreground">Set</button>
            </div>
            {telegramError && <div className="text-sm text-destructive mt-2">{telegramError}</div>}
            {telegramSuccess && <div className="text-sm text-success mt-2">{telegramSuccess}</div>}
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProfilePage;

