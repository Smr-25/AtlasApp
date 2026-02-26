import React from 'react'

export default function WorkspaceCard({ workspace, onSetDefault, onDelete }: any) {
  return (
    <div className="border rounded p-4 shadow-sm bg-white">
      <div className="flex items-start justify-between">
        <div>
          <h3 className="text-lg font-medium">{workspace.Name}</h3>
          {workspace.Description && <p className="text-sm text-gray-500">{workspace.Description}</p>}
          <div className="mt-2 text-sm text-gray-600">{workspace.IsDefault ? <span className="text-green-600 font-medium">Default</span> : <span>Not default</span>}</div>
        </div>
        <div className="flex flex-col items-end space-y-2">
          {!workspace.IsDefault && <button className="btn btn-sm" onClick={onSetDefault}>Set default</button>}
          <button className="btn btn-danger btn-sm" onClick={onDelete}>Delete</button>
        </div>
      </div>
    </div>
  )
}

