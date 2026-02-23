import React from 'react';

export default function SnippetEditor({ onClose, onCreate }: { onClose: () => void; onCreate: (payload: any) => void }) {
  const [title, setTitle] = React.useState('');
  const [language, setLanguage] = React.useState('javascript');
  const [code, setCode] = React.useState('');

  const submit = (e: React.FormEvent) => {
    e.preventDefault();
    onCreate({ title, language, code, tags: [] });
  };

  return (
    <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
      <div className="w-full max-w-2xl bg-white rounded p-6">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-semibold">Create snippet</h3>
          <button onClick={onClose} className="text-gray-600">Close</button>
        </div>
        <form onSubmit={submit}>
          <div className="grid grid-cols-1 gap-3">
            <input value={title} onChange={e => setTitle(e.target.value)} placeholder="Title" className="input input-bordered" />
            <select value={language} onChange={e => setLanguage(e.target.value)} className="select select-bordered">
              <option value="javascript">JavaScript</option>
              <option value="typescript">TypeScript</option>
              <option value="python">Python</option>
              <option value="csharp">C#</option>
            </select>
            <textarea value={code} onChange={e => setCode(e.target.value)} rows={10} className="textarea textarea-bordered" placeholder="Code"></textarea>
            <div className="flex justify-end gap-2">
              <button type="button" onClick={onClose} className="btn">Cancel</button>
              <button type="submit" className="btn btn-primary">Create</button>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
}

