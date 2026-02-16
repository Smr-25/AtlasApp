import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AtlasLogo from '@/components/AtlasLogo';
import AuthInput from '@/components/auth/AuthInput';
import { getJson, putJson, logout } from '@/lib/api';

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
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [success, setSuccess] = useState<string | null>(null);

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

        <div className="glass rounded-2xl p-6">
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
        </div>
      </div>
    </div>
  );
};

export default ProfilePage;

