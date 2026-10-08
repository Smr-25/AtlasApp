using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Atlas.Application;
using Atlas.Infrastructure;

if (args.Contains("--self-test")) { await SelfTest.Run(); return; }
using var cancellation = new CancellationTokenSource();
using var signal = PosixSignalRegistration.Create(PosixSignal.SIGTERM, context => { context.Cancel = true; cancellation.Cancel(); });
void Emit(ToolEvent value) => Console.WriteLine(JsonSerializer.Serialize(value, ToolJson.Options));
try
{
    var input = new StringBuilder();
    var buffer = new char[8192];
    int count;
    while ((count = await Console.In.ReadAsync(buffer, cancellation.Token)) > 0)
    {
        input.Append(buffer, 0, count);
        if (input.Length > 16_000_000) throw new ArgumentException("Request exceeds 16 MB.");
    }
    var request = JsonSerializer.Deserialize<ToolRequest>(input.ToString(), ToolJson.Options) ?? throw new ArgumentException("Empty request.");
    var result = await new ToolRunner().Run(request, Emit, cancellation.Token);
    Emit(new("result", result.Text, 1, result.Files, result.Rows));
}
catch (OperationCanceledException) { Emit(new("cancelled", "Operation stopped. Completed outputs are preserved.")); }
catch (Exception ex) { Emit(new("error", ex.Message)); }
