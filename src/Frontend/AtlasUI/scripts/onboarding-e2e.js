

const fetch = require('node-fetch');
const BASE = process.env.ATLAS_API_BASE || 'http://localhost:5075';

async function run() {
  try {
    // 1) Register
    const suffix = Math.random().toString(36).slice(2, 8);
    const email = `e2e+${suffix}@example.test`;
    const user = {
      fullName: 'E2E Test',
      userName: `e2e_${suffix}`,
      email,
      phoneNumber: null,
      password: 'Test1234!',
      confirmPassword: 'Test1234!',
      phoneVerificationChannel: 1
    };
    console.log('Registering', email);
    const regRes = await fetch(`${BASE}/api/accounts/register`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(user) });
    const regJson = await regRes.json().catch(() => null);
    console.log('Register status', regRes.status, regJson);

    // 2) Login
    const loginBody = { email, userName: null, password: user.password };
    const loginRes = await fetch(`${BASE}/api/accounts/login`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(loginBody) });
    const loginJson = await loginRes.json();
    if (!loginRes.ok || !(loginJson?.data?.accessToken)) {
      console.error('Login failed', loginRes.status, loginJson);
      return;
    }
    const token = loginJson.data.accessToken;
    console.log('Login success, token length:', token.length);

    // 3) Get profession question
    const pqRes = await fetch(`${BASE}/api/onboarding/profession-question`, { headers: { Authorization: `Bearer ${token}` } });
    const pqJson = await pqRes.json();
    console.log('Profession question status', pqRes.status, pqJson?.data ?? pqJson);
    const pq = pqJson?.data ?? pqJson;
    if (!pq || !pq.options || pq.options.length === 0) {
      console.error('No profession options available');
      return;
    }

    // 4) choose first option
    const firstOpt = pq.options[0];
    // Map to profession value if provided or use text heuristic
    const profession = firstOpt.value ?? 1;

    // 5) Get questions for profession
    const qsRes = await fetch(`${BASE}/api/onboarding/questions?profession=${profession}`, { headers: { Authorization: `Bearer ${token}` } });
    const qsJson = await qsRes.json();
    const qs = qsJson?.data ?? qsJson;
    console.log('Questions fetched:', Array.isArray(qs) ? qs.length : 'unknown');

    // Build answers payload: pick first option of each question
    const answers = [];
    if (Array.isArray(qs)) {
      for (const q of qs) {
        if (!q.options || !q.options.length) continue;
        // skip profession question if it's in the list
        if (q.id === pq.id) continue;
        answers.push({ questionId: q.id, optionId: q.options[0].id });
      }
    }

    // include profession selection
    answers.unshift({ questionId: pq.id, optionId: firstOpt.id });

    // 6) Complete onboarding
    const completeBody = { profession: profession, jobTitle: 'E2E Tester', answers };
    const completeRes = await fetch(`${BASE}/api/onboarding/complete`, { method: 'POST', headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` }, body: JSON.stringify(completeBody) });
    const completeJson = await completeRes.json().catch(() => null);
    console.log('Complete status', completeRes.status, completeJson);

    if (completeRes.ok) console.log('Onboarding complete');
    else console.error('Onboarding failed');

  } catch (err) {
    console.error('E2E script error', err);
  }
}

run();

