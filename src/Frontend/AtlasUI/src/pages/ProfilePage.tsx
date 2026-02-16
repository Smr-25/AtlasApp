import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AtlasLogo from '@/components/AtlasLogo';
import AuthInput from '@/components/auth/AuthInput';
import { getJson, putJson, postJson, apiFetch, logout } from '@/lib/api';

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

const phoneRegex = /^\+\d{1,3}\d{4,14}(?:x.+)?$/;

const ProfilePage: React.FC = () => {
  const nav = useNavigate();
  const [profile, setProfile] = useState<Profile | null>(null);
  const [loading, setLoading] = useState(false);
  const [form, setForm] = useState({ fullName: '', userName: '' });
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [success, setSuccess] = useState<string | null>(null);

  const [phoneInput, setPhoneInput] = useState('');
  const [phoneChannel, setPhoneChannel] = useState<number>(1);
  const [phoneError, setPhoneError] = useState<string | null>(null);
  const [phoneSuccess, setPhoneSuccess] = useState<string | null>(null);

  const [telegramId, setTelegramId] = useState('');
  const [telegramError, setTelegramError] = useState<string | null>(null);
  const [telegramSuccess, setTelegramSuccess] = useState<string | null>(null);

  const [generatedLinkCode, setGeneratedLinkCode] = useState<string | null>(null);
  const [linkError, setLinkError] = useState<string | null>(null);

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
    setSuccess(null);
    const v = validate();
    if (Object.keys(v).length) { setErrors(v); return; }
    setLoading(true);
    try {
      const updated = await putJson<Profile>('/api/accounts/profile', { fullName: form.fullName, userName: form.userName });
      setProfile(updated);
      setSuccess('Profile updated successfully.');
      setErrors({});
    } catch (err: any) {
      const msg = err?.details ?? err?.message ?? 'Update failed.';
      setErrors({ global: String(msg) });
    } finally {
      setLoading(false);
    }
  };

  const handleAddPhone = async (e?: React.FormEvent) => {
    if (e) e.preventDefault();
    setPhoneError(null);
    setPhoneSuccess(null);
    if (!phoneInput || phoneInput.trim() === '') { setPhoneError('PhoneNumber is required.'); return; }
    if (!phoneRegex.test(phoneInput.trim())) { setPhoneError('PhoneNumber must be in valid international format.'); return; }
    if (![1, 2].includes(phoneChannel)) { setPhoneError('VerificationChannel must be a valid enum value.'); return; }
    try {
      await postJson('/api/accounts/add-phone-number', { phoneNumber: phoneInput.trim(), verificationChannel: phoneChannel });
      setPhoneSuccess('Phone added. Verification code sent.');
      // refresh profile
      const data = await getJson<Profile>('/api/accounts/profile');
      setProfile(data);
    } catch (err: any) {
      setPhoneError(err?.message ?? 'Failed to add phone.');
    }
  };

  const handleSetTelegram = async (e?: React.FormEvent) => {
    if (e) e.preventDefault();
    setTelegramError(null);
    setTelegramSuccess(null);
    if (!telegramId || telegramId.trim() === '') { setTelegramError('Telegram Chat ID is required.'); return; }
    try {
      await postJson('/api/accounts/set-telegram-chat-id', { telegramChatId: telegramId.trim() });
      setTelegramSuccess('Telegram Chat ID saved.');
    } catch (err: any) {
      setTelegramError(err?.message ?? 'Failed to set Telegram Chat ID.');
    }
  };

  const handleGenerateTelegramLink = async () => {
    setGeneratedLinkCode(null);
    setLinkError(null);
    try {
      const code = await postJson<string>('/api/accounts/generate-telegram-link-code', {});
      setGeneratedLinkCode(String(code));
    } catch (err: any) {
      setLinkError(err?.message ?? 'Failed to generate link code.');
    }
  };

  const handleDeleteAccount = async () => {
    if (!confirm('Are you sure you want to delete your account? This action is irreversible.')) return;
    try {
      const res = await apiFetch('/api/accounts/delete-account', { method: 'DELETE' });
      if (res.ok) {
        await logout();
        nav('/register');
      } else {
        const json = await res.json().catch(() => null);
        alert(json?.message ?? 'Failed to delete account.');
      }
    } catch (err: any) {
      alert(err?.message ?? 'Failed to delete account.');
    }
  };

  if (loading && !profile) return (
    <div className="min-h-screen flex items-center justify-center">Loading...</div>
  );

  return (
    <div className="min-h-screen flex items-center justify-center bg-background p-6">
      <div className="w-full max-w-xl">
        <div className="text-center mb-6">
          <AtlasLogo size="lg" />
          <h2 className="text-xl font-semibold mt-4">Profile</h2>
        </div>

        <div className="glass rounded-2xl p-6 space-y-6">
          {profile && (
            <form onSubmit={handleSave} className="space-y-4">
              <AuthInput label="Full name" placeholder="Full name" value={form.fullName} onChange={e => setForm(f => ({ ...f, fullName: e.target.value }))} error={errors.fullName} />
              <AuthInput label="Username" placeholder="Username" value={form.userName} onChange={e => setForm(f => ({ ...f, userName: e.target.value }))} error={errors.userName} />

              <div className="text-sm text-muted-foreground">Email: {profile.email}</div>
              <div className="text-sm text-muted-foreground">Phone: {profile.phoneNumber ?? '-'}</div>

              {errors.global && <p className="text-sm text-destructive">{errors.global}</p>}
              {success && <p className="text-sm text-primary">{success}</p>}

              <div className="flex gap-3 mt-4">
                <button type="submit" disabled={loading} className="px-4 py-2 rounded-lg bg-primary text-primary-foreground">Save</button>
                <button type="button" onClick={() => { setForm({ fullName: profile.fullName, userName: profile.userName }); setErrors({}); }} className="px-4 py-2 rounded-lg bg-secondary">Cancel</button>
                <button type="button" onClick={() => { sessionStorage.clear(); logout(); nav('/login'); }} className="ml-auto px-4 py-2 rounded-lg bg-destructive text-destructive-foreground">Logout</button>
              </div>
            </form>
          )}

          <hr />

          <div>
            <h3 className="text-lg font-medium">Add Phone Number</h3>
            <form onSubmit={e => handleAddPhone(e)} className="mt-3 space-y-3">
              <AuthInput label="Phone number" placeholder="+994501234567" value={phoneInput} onChange={e => setPhoneInput(e.target.value)} error={phoneError ?? undefined} />
              <div className="flex gap-2">
                <button type="button" onClick={() => setPhoneChannel(1)} className={`px-3 py-2 rounded-lg ${phoneChannel === 1 ? 'bg-primary text-primary-foreground' : 'bg-secondary'}`}>SMS</button>
                <button type="button" onClick={() => setPhoneChannel(2)} className={`px-3 py-2 rounded-lg ${phoneChannel === 2 ? 'bg-primary text-primary-foreground' : 'bg-secondary'}`}>Telegram</button>
              </div>
              <div className="flex gap-3">
                <button type="submit" className="px-4 py-2 rounded-lg bg-primary text-primary-foreground">Add phone</button>
                <div className="text-sm text-muted-foreground mt-2">{phoneSuccess && <span className="text-primary">{phoneSuccess}</span>}</div>
              </div>
            </form>
          </div>

          <hr />

          <div>
            <h3 className="text-lg font-medium">Telegram</h3>
            <form onSubmit={e => handleSetTelegram(e)} className="mt-3 space-y-3">
              <AuthInput label="Telegram Chat ID" placeholder="123456789" value={telegramId} onChange={e => setTelegramId(e.target.value)} error={telegramError ?? undefined} />
              <div className="flex gap-3">
                <button type="submit" className="px-4 py-2 rounded-lg bg-primary text-primary-foreground">Save Telegram ID</button>
                <button type="button" onClick={handleGenerateTelegramLink} className="px-4 py-2 rounded-lg bg-secondary">Generate link code</button>
              </div>
              {generatedLinkCode && <div className="text-sm text-muted-foreground mt-2">Generated code: <span className="font-medium">{generatedLinkCode}</span></div>}
              {linkError && <div className="text-sm text-destructive mt-2">{linkError}</div>}
              {telegramSuccess && <div className="text-sm text-primary mt-2">{telegramSuccess}</div>}
            </form>
          </div>

          <hr />

          <div>
            <h3 className="text-lg font-medium">Danger zone</h3>
            <div className="mt-3">
              <button onClick={handleDeleteAccount} className="px-4 py-2 rounded-lg bg-destructive text-destructive-foreground">Delete account</button>
            </div>
          </div>

        </div>
      </div>
    </div>
  );
};

export default ProfilePage;

