import { useState, useEffect } from "react";
import { motion, AnimatePresence } from "framer-motion";
import {
  User,
  Mail,
  Phone,
  Shield,
  Calendar,
  Briefcase,
  Edit3,
  Save,
  X,
  Key,
  Trash2,
  CheckCircle2,
  XCircle,
  Loader2,
  Crown,
  Activity,
  Clock,
  AlertTriangle,
  Send,
  Eye,
  EyeOff,
} from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import {
  authApi,
  profileApi,
  subscriptionApi,
  AccountDto,
  ProfileDto,
  SubscriptionDto,
  UsageDto,
} from "@/services/api";
import { toast } from "sonner";

const container = {
  hidden: { opacity: 0 },
  show: { opacity: 1, transition: { staggerChildren: 0.06 } },
};
const item = {
  hidden: { opacity: 0, y: 12 },
  show: { opacity: 1, y: 0, transition: { type: "spring" as const, stiffness: 200, damping: 20 } },
};

type Tab = "general" | "security" | "subscription";

const ProfilePanel = () => {
  const { user, logout } = useAuth();
  const [account, setAccount] = useState<AccountDto | null>(null);
  const [profile, setProfile] = useState<ProfileDto | null>(null);
  const [subscription, setSubscription] = useState<SubscriptionDto | null>(null);
  const [usage, setUsage] = useState<UsageDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<Tab>("general");

  // Edit states
  const [editing, setEditing] = useState(false);
  const [editFullName, setEditFullName] = useState("");
  const [editUserName, setEditUserName] = useState("");
  const [editJobTitle, setEditJobTitle] = useState("");
  const [editBio, setEditBio] = useState("");
  const [saving, setSaving] = useState(false);

  // Password change states
  const [changingPassword, setChangingPassword] = useState(false);
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [showCurrentPass, setShowCurrentPass] = useState(false);
  const [showNewPass, setShowNewPass] = useState(false);
  const [passwordSaving, setPasswordSaving] = useState(false);

  // Phone states
  const [addingPhone, setAddingPhone] = useState(false);
  const [phoneNumber, setPhoneNumber] = useState("");
  const [phoneSaving, setPhoneSaving] = useState(false);

  // Delete account
  const [deleteConfirm, setDeleteConfirm] = useState(false);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    setLoading(true);
    const [accRes, profRes, subRes, usageRes] = await Promise.allSettled([
      authApi.getProfile(),
      profileApi.getMe(),
      subscriptionApi.getCurrent(),
      subscriptionApi.getUsage(),
    ]);

    if (accRes.status === "fulfilled" && accRes.value.data.isSuccess) {
      setAccount(accRes.value.data.data);
    }
    if (profRes.status === "fulfilled" && profRes.value.data.isSuccess) {
      setProfile(profRes.value.data.data);
    }
    if (subRes.status === "fulfilled" && subRes.value.data.isSuccess) {
      setSubscription(subRes.value.data.data);
    }
    if (usageRes.status === "fulfilled" && usageRes.value.data.isSuccess) {
      setUsage(usageRes.value.data.data);
    }
    setLoading(false);
  };

  const startEditing = () => {
    setEditFullName(account?.fullName || "");
    setEditUserName(account?.userName || "");
    setEditJobTitle(profile?.jobTitle || "");
    setEditBio(profile?.bio || "");
    setEditing(true);
  };

  const cancelEditing = () => {
    setEditing(false);
  };

  const handleSave = async () => {
    setSaving(true);
    try {
      // Update account profile (fullName, userName)
      await authApi.updateProfile({
        fullName: editFullName,
        userName: editUserName,
      });

      // Update developer profile (jobTitle, bio)
      if (profile) {
        await profileApi.updateMe({
          jobTitle: editJobTitle,
          bio: editBio,
        });
      }

      toast.success("Profile updated successfully");
      setEditing(false);
      await loadData();
    } catch (err: any) {
      const msg = err?.response?.data?.errors?.[0] || "Failed to update profile";
      toast.error(msg);
    } finally {
      setSaving(false);
    }
  };

  const handleChangePassword = async () => {
    if (newPassword !== confirmPassword) {
      toast.error("Passwords do not match");
      return;
    }
    if (newPassword.length < 8) {
      toast.error("Password must be at least 8 characters");
      return;
    }
    setPasswordSaving(true);
    try {
      const res = await authApi.changePassword({
        currentPassword,
        newPassword,
        confirmPassword,
      });
      if (res.data.isSuccess) {
        toast.success("Password changed successfully");
        setChangingPassword(false);
        setCurrentPassword("");
        setNewPassword("");
        setConfirmPassword("");
      } else {
        toast.error(res.data.errors?.[0] || "Failed to change password");
      }
    } catch (err: any) {
      const msg = err?.response?.data?.errors?.[0] || "Failed to change password";
      toast.error(msg);
    } finally {
      setPasswordSaving(false);
    }
  };

  const handleAddPhone = async () => {
    if (!phoneNumber || phoneNumber.length < 10) {
      toast.error("Please enter a valid phone number");
      return;
    }
    setPhoneSaving(true);
    try {
      const res = await authApi.addPhoneNumber(phoneNumber);
      if (res.data.isSuccess) {
        toast.success("Phone number added! Check your phone for verification code.");
        setAddingPhone(false);
        setPhoneNumber("");
        await loadData();
      } else {
        toast.error(res.data.errors?.[0] || "Failed to add phone number");
      }
    } catch (err: any) {
      const msg = err?.response?.data?.errors?.[0] || "Failed to add phone number";
      toast.error(msg);
    } finally {
      setPhoneSaving(false);
    }
  };

  const handleDeleteAccount = async () => {
    setDeleting(true);
    try {
      const res = await authApi.deleteAccount();
      if (res.data.isSuccess) {
        toast.success("Account deleted");
        await logout();
      } else {
        toast.error(res.data.errors?.[0] || "Failed to delete account");
      }
    } catch (err: any) {
      toast.error("Failed to delete account");
    } finally {
      setDeleting(false);
      setDeleteConfirm(false);
    }
  };

  const initials = (account?.fullName || user?.fullName || "U")
    .split(" ")
    .map((n) => n[0])
    .join("")
    .slice(0, 2)
    .toUpperCase();

  const professionLabel: Record<number, string> = {
    1: "Developer",
    2: "Designer",
    3: "CyberSecurity",
    4: "Digital Marketing",
    5: "Product Manager",
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center py-20">
        <div className="flex flex-col items-center gap-3">
          <Loader2 className="w-8 h-8 animate-spin text-primary" />
          <p className="text-xs text-muted-foreground">Loading profile...</p>
        </div>
      </div>
    );
  }

  const tabs: { id: Tab; label: string; icon: typeof User }[] = [
    { id: "general", label: "General", icon: User },
    { id: "security", label: "Security", icon: Shield },
    { id: "subscription", label: "Subscription", icon: Crown },
  ];

  return (
    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">
      {/* Header */}
      <motion.div variants={item} className="flex items-start justify-between">
        <div className="flex items-center gap-4">
          <div className="w-16 h-16 rounded-2xl bg-gradient-to-br from-primary/30 to-primary/10 border-2 border-primary/20 flex items-center justify-center shadow-lg shadow-primary/10">
            <span className="text-2xl font-bold text-primary">{initials}</span>
          </div>
          <div>
            <h1 className="text-xl font-bold text-foreground tracking-tight">
              {account?.fullName || user?.fullName}
            </h1>
            <p className="text-sm text-muted-foreground">@{account?.userName || user?.userName}</p>
            <div className="flex items-center gap-2 mt-1">
              {account?.profession && (
                <span className="text-[10px] font-semibold bg-primary/10 text-primary px-2 py-0.5 rounded-full">
                  {professionLabel[account.profession] || "User"}
                </span>
              )}
              <span
                className={`text-[10px] font-semibold px-2 py-0.5 rounded-full ${
                  account?.status === "Active"
                    ? "bg-emerald-500/10 text-emerald-500"
                    : "bg-amber-500/10 text-amber-500"
                }`}
              >
                {account?.status || "Active"}
              </span>
            </div>
          </div>
        </div>
        {!editing && (
          <motion.button
            whileHover={{ scale: 1.05 }}
            whileTap={{ scale: 0.95 }}
            onClick={startEditing}
            className="flex items-center gap-2 px-3 py-2 rounded-lg bg-primary/10 text-primary text-sm font-medium hover:bg-primary/20 transition-colors"
          >
            <Edit3 className="w-3.5 h-3.5" />
            Edit Profile
          </motion.button>
        )}
      </motion.div>

      {/* Bio */}
      {profile?.bio && !editing && (
        <motion.div variants={item} className="text-sm text-muted-foreground leading-relaxed">
          {profile.bio}
        </motion.div>
      )}

      {/* Tags */}
      {profile?.tags && profile.tags.length > 0 && !editing && (
        <motion.div variants={item} className="flex flex-wrap gap-1.5">
          {profile.tags.map((tag) => (
            <span
              key={tag}
              className="text-[11px] bg-primary/5 text-primary/80 border border-primary/10 px-2.5 py-1 rounded-full font-medium"
            >
              {tag}
            </span>
          ))}
        </motion.div>
      )}

      {/* Tabs */}
      <motion.div variants={item} className="flex gap-1 p-1 bg-muted/40 rounded-xl border border-border">
        {tabs.map((tab) => (
          <button
            key={tab.id}
            onClick={() => setActiveTab(tab.id)}
            className={`flex-1 flex items-center justify-center gap-2 py-2 rounded-lg text-xs font-medium transition-all ${
              activeTab === tab.id
                ? "bg-card text-foreground shadow-sm border border-border"
                : "text-muted-foreground hover:text-foreground"
            }`}
          >
            <tab.icon className="w-3.5 h-3.5" />
            {tab.label}
          </button>
        ))}
      </motion.div>

      <AnimatePresence mode="wait">
        {/* ─── GENERAL TAB ─── */}
        {activeTab === "general" && (
          <motion.div
            key="general"
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
            className="space-y-4"
          >
            {editing ? (
              /* Edit Mode */
              <div className="space-y-4 rounded-xl border border-border p-5 bg-card/50">
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div>
                    <label className="text-xs text-muted-foreground font-medium mb-1.5 block">Full Name</label>
                    <input
                      value={editFullName}
                      onChange={(e) => setEditFullName(e.target.value)}
                      className="w-full h-9 px-3 rounded-lg bg-muted/40 border border-border text-sm text-foreground focus:outline-none focus:ring-1 focus:ring-primary/30 transition-all"
                    />
                  </div>
                  <div>
                    <label className="text-xs text-muted-foreground font-medium mb-1.5 block">Username</label>
                    <input
                      value={editUserName}
                      onChange={(e) => setEditUserName(e.target.value)}
                      className="w-full h-9 px-3 rounded-lg bg-muted/40 border border-border text-sm text-foreground focus:outline-none focus:ring-1 focus:ring-primary/30 transition-all"
                    />
                  </div>
                  <div>
                    <label className="text-xs text-muted-foreground font-medium mb-1.5 block">Job Title</label>
                    <input
                      value={editJobTitle}
                      onChange={(e) => setEditJobTitle(e.target.value)}
                      className="w-full h-9 px-3 rounded-lg bg-muted/40 border border-border text-sm text-foreground focus:outline-none focus:ring-1 focus:ring-primary/30 transition-all"
                    />
                  </div>
                </div>
                <div>
                  <label className="text-xs text-muted-foreground font-medium mb-1.5 block">Bio</label>
                  <textarea
                    value={editBio}
                    onChange={(e) => setEditBio(e.target.value)}
                    rows={3}
                    className="w-full px-3 py-2 rounded-lg bg-muted/40 border border-border text-sm text-foreground focus:outline-none focus:ring-1 focus:ring-primary/30 transition-all resize-none"
                  />
                </div>
                <div className="flex items-center gap-2">
                  <motion.button
                    whileHover={{ scale: 1.02 }}
                    whileTap={{ scale: 0.98 }}
                    onClick={handleSave}
                    disabled={saving}
                    className="flex items-center gap-2 px-4 py-2 rounded-lg bg-primary text-primary-foreground text-sm font-medium hover:bg-primary/90 transition-colors disabled:opacity-50"
                  >
                    {saving ? (
                      <Loader2 className="w-3.5 h-3.5 animate-spin" />
                    ) : (
                      <Save className="w-3.5 h-3.5" />
                    )}
                    Save Changes
                  </motion.button>
                  <button
                    onClick={cancelEditing}
                    className="flex items-center gap-2 px-4 py-2 rounded-lg border border-border text-sm text-muted-foreground hover:bg-muted transition-colors"
                  >
                    <X className="w-3.5 h-3.5" />
                    Cancel
                  </button>
                </div>
              </div>
            ) : (
              /* View Mode */
              <div className="space-y-3">
                {[
                  { icon: Mail, label: "Email", value: account?.email, verified: account?.emailConfirmed },
                  { icon: User, label: "Username", value: `@${account?.userName}` },
                  {
                    icon: Phone,
                    label: "Phone",
                    value: account?.phoneNumber || "Not set",
                    verified: account?.phoneNumberConfirmed,
                    action: !account?.phoneNumber
                      ? () => setAddingPhone(true)
                      : undefined,
                  },
                  { icon: Briefcase, label: "Job Title", value: profile?.jobTitle || "Not set" },
                  {
                    icon: Calendar,
                    label: "Member Since",
                    value: account?.createdAt
                      ? new Date(account.createdAt).toLocaleDateString("en-US", {
                          year: "numeric",
                          month: "long",
                          day: "numeric",
                        })
                      : "—",
                  },
                  {
                    icon: Clock,
                    label: "Last Login",
                    value: account?.lastLoginAt
                      ? new Date(account.lastLoginAt).toLocaleDateString("en-US", {
                          year: "numeric",
                          month: "short",
                          day: "numeric",
                          hour: "2-digit",
                          minute: "2-digit",
                        })
                      : "—",
                  },
                ].map((field) => (
                  <div
                    key={field.label}
                    className="flex items-center gap-3 px-4 py-3 rounded-xl border border-border bg-card/50 hover:bg-card transition-colors group"
                  >
                    <div className="w-8 h-8 rounded-lg bg-primary/5 flex items-center justify-center shrink-0">
                      <field.icon className="w-4 h-4 text-primary/60" />
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-[10px] text-muted-foreground font-medium uppercase tracking-wider">
                        {field.label}
                      </p>
                      <p className="text-sm text-foreground truncate">{field.value}</p>
                    </div>
                    {field.verified !== undefined && (
                      <div className="shrink-0">
                        {field.verified ? (
                          <div className="flex items-center gap-1 text-emerald-500">
                            <CheckCircle2 className="w-3.5 h-3.5" />
                            <span className="text-[10px] font-medium">Verified</span>
                          </div>
                        ) : (
                          <div className="flex items-center gap-1 text-amber-500">
                            <XCircle className="w-3.5 h-3.5" />
                            <span className="text-[10px] font-medium">Unverified</span>
                          </div>
                        )}
                      </div>
                    )}
                    {field.action && (
                      <button
                        onClick={field.action}
                        className="shrink-0 text-[10px] font-medium text-primary hover:underline"
                      >
                        + Add
                      </button>
                    )}
                  </div>
                ))}
              </div>
            )}

            {/* Add Phone Modal Inline */}
            <AnimatePresence>
              {addingPhone && (
                <motion.div
                  initial={{ opacity: 0, height: 0 }}
                  animate={{ opacity: 1, height: "auto" }}
                  exit={{ opacity: 0, height: 0 }}
                  className="overflow-hidden"
                >
                  <div className="rounded-xl border border-border p-4 bg-card/50 space-y-3">
                    <h3 className="text-sm font-semibold text-foreground">Add Phone Number</h3>
                    <input
                      value={phoneNumber}
                      onChange={(e) => setPhoneNumber(e.target.value)}
                      placeholder="+994501234567"
                      className="w-full h-9 px-3 rounded-lg bg-muted/40 border border-border text-sm text-foreground focus:outline-none focus:ring-1 focus:ring-primary/30 transition-all"
                    />
                    <div className="flex items-center gap-2">
                      <button
                        onClick={handleAddPhone}
                        disabled={phoneSaving}
                        className="flex items-center gap-2 px-3 py-1.5 rounded-lg bg-primary text-primary-foreground text-xs font-medium hover:bg-primary/90 disabled:opacity-50 transition-colors"
                      >
                        {phoneSaving ? <Loader2 className="w-3 h-3 animate-spin" /> : <Send className="w-3 h-3" />}
                        Add
                      </button>
                      <button
                        onClick={() => setAddingPhone(false)}
                        className="px-3 py-1.5 rounded-lg border border-border text-xs text-muted-foreground hover:bg-muted transition-colors"
                      >
                        Cancel
                      </button>
                    </div>
                  </div>
                </motion.div>
              )}
            </AnimatePresence>
          </motion.div>
        )}

        {/* ─── SECURITY TAB ─── */}
        {activeTab === "security" && (
          <motion.div
            key="security"
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
            className="space-y-4"
          >
            {/* Change Password */}
            <div className="rounded-xl border border-border p-5 bg-card/50 space-y-4">
              <div className="flex items-center gap-2">
                <Key className="w-4 h-4 text-primary" />
                <h3 className="text-sm font-semibold text-foreground">Change Password</h3>
              </div>

              {changingPassword ? (
                <div className="space-y-3">
                  <div className="relative">
                    <input
                      type={showCurrentPass ? "text" : "password"}
                      value={currentPassword}
                      onChange={(e) => setCurrentPassword(e.target.value)}
                      placeholder="Current Password"
                      className="w-full h-9 px-3 pr-9 rounded-lg bg-muted/40 border border-border text-sm text-foreground focus:outline-none focus:ring-1 focus:ring-primary/30 transition-all"
                    />
                    <button
                      type="button"
                      onClick={() => setShowCurrentPass(!showCurrentPass)}
                      className="absolute right-2.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground"
                    >
                      {showCurrentPass ? <EyeOff className="w-3.5 h-3.5" /> : <Eye className="w-3.5 h-3.5" />}
                    </button>
                  </div>
                  <div className="relative">
                    <input
                      type={showNewPass ? "text" : "password"}
                      value={newPassword}
                      onChange={(e) => setNewPassword(e.target.value)}
                      placeholder="New Password"
                      className="w-full h-9 px-3 pr-9 rounded-lg bg-muted/40 border border-border text-sm text-foreground focus:outline-none focus:ring-1 focus:ring-primary/30 transition-all"
                    />
                    <button
                      type="button"
                      onClick={() => setShowNewPass(!showNewPass)}
                      className="absolute right-2.5 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground"
                    >
                      {showNewPass ? <EyeOff className="w-3.5 h-3.5" /> : <Eye className="w-3.5 h-3.5" />}
                    </button>
                  </div>
                  <input
                    type="password"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    placeholder="Confirm New Password"
                    className="w-full h-9 px-3 rounded-lg bg-muted/40 border border-border text-sm text-foreground focus:outline-none focus:ring-1 focus:ring-primary/30 transition-all"
                  />
                  <div className="flex items-center gap-2">
                    <button
                      onClick={handleChangePassword}
                      disabled={passwordSaving}
                      className="flex items-center gap-2 px-4 py-2 rounded-lg bg-primary text-primary-foreground text-xs font-medium hover:bg-primary/90 disabled:opacity-50 transition-colors"
                    >
                      {passwordSaving ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : <Key className="w-3.5 h-3.5" />}
                      Update Password
                    </button>
                    <button
                      onClick={() => {
                        setChangingPassword(false);
                        setCurrentPassword("");
                        setNewPassword("");
                        setConfirmPassword("");
                      }}
                      className="px-4 py-2 rounded-lg border border-border text-xs text-muted-foreground hover:bg-muted transition-colors"
                    >
                      Cancel
                    </button>
                  </div>
                </div>
              ) : (
                <button
                  onClick={() => setChangingPassword(true)}
                  className="flex items-center gap-2 px-4 py-2 rounded-lg border border-border text-sm text-foreground hover:bg-muted transition-colors"
                >
                  <Key className="w-3.5 h-3.5" />
                  Change Password
                </button>
              )}
            </div>

            {/* Verification Status */}
            <div className="rounded-xl border border-border p-5 bg-card/50 space-y-3">
              <div className="flex items-center gap-2">
                <Shield className="w-4 h-4 text-primary" />
                <h3 className="text-sm font-semibold text-foreground">Verification Status</h3>
              </div>
              <div className="space-y-2">
                <div className="flex items-center justify-between py-2 px-3 rounded-lg bg-muted/30">
                  <div className="flex items-center gap-2 text-sm">
                    <Mail className="w-3.5 h-3.5 text-muted-foreground" />
                    <span>Email</span>
                  </div>
                  {account?.emailConfirmed ? (
                    <span className="flex items-center gap-1 text-emerald-500 text-xs font-medium">
                      <CheckCircle2 className="w-3 h-3" /> Verified
                    </span>
                  ) : (
                    <span className="flex items-center gap-1 text-amber-500 text-xs font-medium">
                      <AlertTriangle className="w-3 h-3" /> Not Verified
                    </span>
                  )}
                </div>
                <div className="flex items-center justify-between py-2 px-3 rounded-lg bg-muted/30">
                  <div className="flex items-center gap-2 text-sm">
                    <Phone className="w-3.5 h-3.5 text-muted-foreground" />
                    <span>Phone</span>
                  </div>
                  {account?.phoneNumberConfirmed ? (
                    <span className="flex items-center gap-1 text-emerald-500 text-xs font-medium">
                      <CheckCircle2 className="w-3 h-3" /> Verified
                    </span>
                  ) : (
                    <span className="flex items-center gap-1 text-muted-foreground text-xs font-medium">
                      {account?.phoneNumber ? "Pending" : "Not Set"}
                    </span>
                  )}
                </div>
              </div>
            </div>

            {/* Danger Zone */}
            <div className="rounded-xl border border-red-500/20 p-5 bg-red-500/5 space-y-3">
              <div className="flex items-center gap-2">
                <AlertTriangle className="w-4 h-4 text-red-500" />
                <h3 className="text-sm font-semibold text-red-500">Danger Zone</h3>
              </div>
              <p className="text-xs text-muted-foreground">
                Deleting your account is permanent. All data will be lost and cannot be recovered.
              </p>
              {deleteConfirm ? (
                <div className="flex items-center gap-2">
                  <button
                    onClick={handleDeleteAccount}
                    disabled={deleting}
                    className="flex items-center gap-2 px-4 py-2 rounded-lg bg-red-500 text-white text-xs font-medium hover:bg-red-600 disabled:opacity-50 transition-colors"
                  >
                    {deleting ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : <Trash2 className="w-3.5 h-3.5" />}
                    Yes, Delete My Account
                  </button>
                  <button
                    onClick={() => setDeleteConfirm(false)}
                    className="px-4 py-2 rounded-lg border border-border text-xs text-muted-foreground hover:bg-muted transition-colors"
                  >
                    Cancel
                  </button>
                </div>
              ) : (
                <button
                  onClick={() => setDeleteConfirm(true)}
                  className="flex items-center gap-2 px-4 py-2 rounded-lg border border-red-500/30 text-sm text-red-500 hover:bg-red-500/10 transition-colors"
                >
                  <Trash2 className="w-3.5 h-3.5" />
                  Delete Account
                </button>
              )}
            </div>
          </motion.div>
        )}

        {/* ─── SUBSCRIPTION TAB ─── */}
        {activeTab === "subscription" && (
          <motion.div
            key="subscription"
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
            className="space-y-4"
          >
            {/* Current Plan */}
            <div className="rounded-xl border border-border p-5 bg-gradient-to-br from-card to-card/60 space-y-4">
              <div className="flex items-center gap-2">
                <Crown className="w-4 h-4 text-primary" />
                <h3 className="text-sm font-semibold text-foreground">Current Plan</h3>
              </div>
              <div className="flex items-center gap-4">
                <div className="w-14 h-14 rounded-2xl bg-gradient-to-br from-primary/20 to-primary/5 border border-primary/10 flex items-center justify-center">
                  <Crown className="w-7 h-7 text-primary" />
                </div>
                <div>
                  <p className="text-lg font-bold text-foreground capitalize">
                    {subscription?.tier || "Free"} Plan
                  </p>
                  <p className="text-xs text-muted-foreground">
                    Status: <span className="text-emerald-500 font-medium">{subscription?.status || "Active"}</span>
                    {subscription?.currentPeriodEnd && (
                      <> · Renews {new Date(subscription.currentPeriodEnd).toLocaleDateString()}</>
                    )}
                  </p>
                </div>
              </div>
            </div>

            {/* Usage */}
            <div className="rounded-xl border border-border p-5 bg-card/50 space-y-4">
              <div className="flex items-center gap-2">
                <Activity className="w-4 h-4 text-primary" />
                <h3 className="text-sm font-semibold text-foreground">Usage</h3>
              </div>
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                {[
                  {
                    label: "Workspaces",
                    used: usage?.workspacesUsed ?? 0,
                    limit: usage?.workspacesLimit ?? 10,
                  },
                  {
                    label: "Integrations",
                    used: usage?.integrationsUsed ?? 0,
                    limit: usage?.integrationsLimit ?? 10,
                  },
                ].map((u) => (
                  <div key={u.label} className="space-y-2">
                    <div className="flex items-center justify-between text-xs">
                      <span className="text-muted-foreground">{u.label}</span>
                      <span className="text-foreground font-medium">
                        {u.used} / {u.limit === -1 || u.limit > 999 ? "∞" : u.limit}
                      </span>
                    </div>
                    <div className="h-2 bg-muted rounded-full overflow-hidden">
                      <motion.div
                        initial={{ width: 0 }}
                        animate={{
                          width: `${u.limit <= 0 || u.limit > 999 ? 10 : Math.min((u.used / u.limit) * 100, 100)}%`,
                        }}
                        transition={{ delay: 0.3, duration: 0.6, ease: "easeOut" }}
                        className="h-full bg-primary rounded-full"
                      />
                    </div>
                  </div>
                ))}
              </div>
            </div>

            {/* Upgrade CTA */}
            {(!subscription || subscription.tier?.toLowerCase() === "free") && (
              <div className="rounded-xl border border-primary/20 p-5 bg-gradient-to-br from-primary/5 to-primary/[0.02] space-y-3">
                <h3 className="text-sm font-semibold text-foreground">Upgrade to Pro</h3>
                <p className="text-xs text-muted-foreground">
                  Unlock unlimited workspaces, integrations, and premium AI features.
                </p>
                <motion.button
                  whileHover={{ scale: 1.02, boxShadow: "0 4px 20px -4px hsl(var(--primary) / 0.3)" }}
                  whileTap={{ scale: 0.98 }}
                  onClick={async () => {
                    try {
                      const res = await subscriptionApi.checkout({ tier: "Pro" });
                      if (res.data.isSuccess && res.data.data?.url) {
                        window.location.href = res.data.data.url;
                      }
                    } catch {
                      toast.error("Failed to start checkout");
                    }
                  }}
                  className="flex items-center gap-2 px-5 py-2.5 rounded-xl bg-gradient-to-r from-primary to-primary/80 text-primary-foreground text-sm font-medium shadow-md shadow-primary/20"
                >
                  <Crown className="w-4 h-4" />
                  Upgrade Now
                </motion.button>
              </div>
            )}
          </motion.div>
        )}
      </AnimatePresence>
    </motion.div>
  );
};

export default ProfilePanel;




