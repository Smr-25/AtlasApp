import React from 'react'

export default function IntegrationCard({ integration, onReconnect, onDelete, onConnect }: any) {
  return (
    <div className="border rounded p-4 shadow-sm bg-white">
      <div className="flex items-start justify-between">
        <div>
          <h3 className="text-lg font-medium">{integration.Name}</h3>
          <div className="text-sm text-gray-500">Provider: {integration.Provider}</div>
          <div className="text-sm mt-2">Status: <span className="font-medium">{integration.Status}</span></div>
        </div>
        <div className="flex flex-col items-end space-y-2">
          {integration.IsActive ? (
            <>
              <button className="btn btn-sm" onClick={onReconnect}>Reconnect</button>
              <button className="btn btn-danger btn-sm" onClick={onDelete}>Disconnect</button>
            </>
          ) : (
            <button className="btn btn-primary btn-sm" onClick={onConnect}>Complete setup</button>
          )}
        </div>
      </div>
    </div>
  )
}

