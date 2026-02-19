// Lightweight mock API that intercepts fetch when window.__ATLAS_MOCK__ === true
// It supports minimal endpoints used by UI so you can view pages without backend.

type ResBody = { success?: boolean; isSuccess?: boolean; data?: any; message?: string | null; errors?: any };

function ok(data: any): ResBody { return { success: true, data }; }
function err(msg: string): ResBody { return { success: false, message: msg }; }

export function enableMocking(baseUrl = 'http://localhost:5075') {
  (window as any).__ATLAS_MOCK__ = true;
  const orig = window.fetch.bind(window);
  window.fetch = async (input: RequestInfo, init?: RequestInit) => {
    const url = typeof input === 'string' ? input : input instanceof Request ? input.url : String(input);
    const rel = url.startsWith(baseUrl) ? url.substring(baseUrl.length) : url;

    // simple router
    if (rel.startsWith('/api/accounts/login')) {
      const body = init?.body ? JSON.parse(String(init.body)) : {};
      if ((body.email === 'demo@atlas.app' || body.userName === 'demo') && body.password === 'DemoPass1!') {
        return new Response(JSON.stringify(ok({ accessToken: 'mock-access', refreshToken: 'mock-refresh', accessTokenExpiration: new Date(Date.now()+3600*1000).toISOString(), refreshTokenExpiration: new Date(Date.now()+7*24*3600*1000).toISOString(), userId: 'mock-user', userName: 'demo', email: 'demo@atlas.app', fullName: 'Demo User' })), { status: 200, headers: { 'Content-Type': 'application/json' } });
      }
      return new Response(JSON.stringify(err('Invalid credentials')), { status: 401, headers: { 'Content-Type': 'application/json' } });
    }

    if (rel.startsWith('/api/onboarding/profession-question')) {
      const sample = { id: '11111111-1111-1111-1111-111111111111', text: 'What is your profession?', order: 1, isMultiSelect: false, options: [ { id: 'opt-dev', text: 'Developer', value: 1 }, { id: 'opt-des', text: 'Designer', value: 2 }, { id: 'opt-devops', text: 'DevOps Engineer', value: 3 } ] };
      return new Response(JSON.stringify(ok(sample)), { status: 200, headers: { 'Content-Type': 'application/json' } });
    }

    if (rel.startsWith('/api/onboarding/questions')) {
      const params = new URL('http://x' + rel).searchParams;
      const prof = Number(params.get('profession') ?? 0);
      const general = [ { id: 'q1', text: 'What are your main goals for using Atlas?', order: 2, isMultiSelect: true, options: [ { id: 'opt1', text: 'Improve productivity' }, { id: 'opt2', text: 'Collaborate with team' } ] } ];
      const devQuestions = [ { id: 'q2', text: 'Which programming languages do you primarily work with?', order: 3, isMultiSelect: true, options: [ { id: 'opt-js', text: 'JavaScript / TypeScript' }, { id: 'opt-py', text: 'Python' } ] } ];
      const questions = prof === 1 ? [...general, ...devQuestions] : general;
      return new Response(JSON.stringify(questions), { status: 200, headers: { 'Content-Type': 'application/json' } });
    }

    if (rel.startsWith('/api/workspaces')) {
      if (init?.method === 'GET' || !init?.method) {
        const ws = [ { id: 'w1', name: 'Frontend', isDefault: true, createdAt: new Date().toISOString(), integrations: [ { id: 'g', name: 'GitHub', enabled: true, logoUrl: null } ] } ];
        return new Response(JSON.stringify(ok(ws)), { status: 200, headers: { 'Content-Type': 'application/json' } });
      }
    }

    // fallback to network
    return orig(input, init);
  };
}
