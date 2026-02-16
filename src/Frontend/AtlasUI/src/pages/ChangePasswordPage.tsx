import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AtlasLogo from '@/components/AtlasLogo';
import AuthInput from '@/components/auth/AuthInput';
import { putJson, logout } from '@/lib/api';
import { Eye, Lock } from 'lucide-react';
import ClosedEye from '@/components/icons/ClosedEye';

const ChangePasswordPage: React.FC = () => {
  const nav = useNavigate();
  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [showCurrent, setShowCurrent] = useState(false);
  const [showNew, setShowNew] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);

  const validate = () => {
    if (!currentPassword) return 'Current password is required.';
    if (!newPassword) return 'New password is required.';
    if (newPassword.length < 8) return 'New password must be at least 8 characters long.';
    if (!/[A-Z]/.test(newPassword)) return 'New password must contain at least one uppercase letter.';
    if (!/[a-z]/.test(newPassword)) return 'New password must contain at least one lowercase letter.';
    if (!/[0-9]/.test(newPassword)) return 'New password must contain at least one digit.';
    if (!/[^a-zA-Z0-9]/.test(newPassword)) return 'New password must contain at least one special character.';
    if (newPassword !== confirmPassword) return 'Confirm password must match the new password.';
    return null;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);
    const v = validate();
    if (v) { setError(v); return; }
    setLoading(true);
    try {
      await putJson('/api/accounts/change-password', { currentPassword, newPassword, confirmPassword });
      setSuccess('Password changed successfully. Please login again.');
      setTimeout(async () => { await logout(); nav('/login'); }, 1200);
    } catch (err: any) {
      setError(err?.message ?? 'Change password failed.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background p-6">
      <div className="w-full max-w-md">
        <div className="text-center mb-6">
          <AtlasLogo size="lg" />
          <h2 className="text-xl font-semibold mt-4">Change password</h2>
        </div>

        <div className="glass rounded-2xl p-6">
          <form onSubmit={handleSubmit} className="space-y-4">
            <AuthInput label="Current password" icon={Lock} type={showCurrent ? 'text' : 'password'} value={currentPassword} onChange={e => setCurrentPassword(e.target.value)} suffix={<button type="button" onClick={() => setShowCurrent(s => !s)} className="p-1 text-muted-foreground">{showCurrent ? <ClosedEye className="w-4 h-4" /> : <Eye className="w-4 h-4" />}</button>} />
            <AuthInput label="New password" icon={Lock} type={showNew ? 'text' : 'password'} value={newPassword} onChange={e => setNewPassword(e.target.value)} suffix={<button type="button" onClick={() => setShowNew(s => !s)} className="p-1 text-muted-foreground">{showNew ? <ClosedEye className="w-4 h-4" /> : <Eye className="w-4 h-4" />}</button>} />
            <AuthInput label="Confirm password" icon={Lock} type={showConfirm ? 'text' : 'password'} value={confirmPassword} onChange={e => setConfirmPassword(e.target.value)} suffix={<button type="button" onClick={() => setShowConfirm(s => !s)} className="p-1 text-muted-foreground">{showConfirm ? <ClosedEye className="w-4 h-4" /> : <Eye className="w-4 h-4" />}</button>} />

            {error && <p className="text-sm text-destructive">{error}</p>}
            {success && <p className="text-sm text-primary">{success}</p>}

            <div className="flex gap-3 mt-4">
              <button type="submit" disabled={loading} className="px-4 py-2 rounded-lg bg-primary text-primary-foreground">Change</button>
              <button type="button" onClick={() => nav('/dashboard')} className="px-4 py-2 rounded-lg bg-secondary">Cancel</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default ChangePasswordPage;

