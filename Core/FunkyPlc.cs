using FunkySystem.Core;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public abstract class FunkyPlcBase : IAsyncDisposable
{
    // Untyped device (host always can set this)
    protected FunkyDevice DeviceUntyped { get; private set; } = null!;

    private readonly object _lifecycleLock = new();
    private CancellationTokenSource? _cts;
    private Task? _runTask;
    private Task? _idleTask;

    protected CancellationToken AbortToken => _cts?.Token ?? CancellationToken.None;
    protected bool AbortRequested => _cts?.IsCancellationRequested == true;

    // Host attaches the device instance (untyped)
    public void AttachDevice(FunkyDevice device)
        => DeviceUntyped = device ?? throw new ArgumentNullException(nameof(device));

    public bool IsRunning => _cts is { IsCancellationRequested: false };

    public void Start()
    {
        lock (_lifecycleLock)
        {
            if (DeviceUntyped == null) throw new InvalidOperationException("AttachDevice() must be called before Start().");
            if (IsRunning) return;

            Initialize();

            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            DeviceUntyped.State = State.Running;

            _idleTask = Task.Run(() => IdleLoopAsync(AbortToken), AbortToken);
            _runTask = Task.Run(() => RunLoopAsync(AbortToken), AbortToken);
        }
    }

    public async Task StopAsync()
    {
        Task? run;
        Task? idle;

        lock (_lifecycleLock)
        {
            if (_cts == null)
            {
                DeviceUntyped.State = State.Ready;
                return;
            }

            _cts.Cancel();
            run = _runTask;
            idle = _idleTask;
        }

        await SafeAwait(idle).ConfigureAwait(false);
        await SafeAwait(run).ConfigureAwait(false);

        lock (_lifecycleLock)
        {
            _cts?.Dispose();
            _cts = null;
            _runTask = null;
            _idleTask = null;
        }

        DeviceUntyped.State = State.Ready;
    }

    public void Abort(string reason = "User")
    {
        lock (_lifecycleLock)
        {
            if (_cts == null || _cts.IsCancellationRequested) return;
            _cts.Cancel();
        }

        DeviceUntyped.SetAbortedState("Aborted: " + reason);
        OnAbort(reason);
    }

    public async ValueTask DisposeAsync() => await StopAsync().ConfigureAwait(false);

    private async Task RunLoopAsync(CancellationToken token)
    {
        try
        {
            await Run().ConfigureAwait(false);

            if (!token.IsCancellationRequested)
            {
                DeviceUntyped.State = State.Done;
                await FinalizeAsync().ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // expected on Stop/Abort
        }
        catch (Exception ex)
        {
            DeviceUntyped.SetAlertState("RunError " + ex.Message);
        }
        finally
        {
            lock (_lifecycleLock) { _cts?.Cancel(); }
        }
    }

    private async Task IdleLoopAsync(CancellationToken token)
    {
        try { await IdleAsync().ConfigureAwait(false); }
        catch (OperationCanceledException) { }
        catch (Exception ex) { DeviceUntyped.SetAlertState("IdleError " + ex.Message); }
    }

    protected Task DelayAsync(int milliseconds) => Task.Delay(milliseconds, AbortToken);

    private static async Task SafeAwait(Task? t)
    {
        if (t == null) return;
        try { await t.ConfigureAwait(false); }
        catch { }
    }

    // Extension points
    public virtual void Initialize() { }
    public abstract Task Run();
    public virtual Task FinalizeAsync() => Task.CompletedTask;
    public virtual void OnAbort(string reason) { }
    protected virtual Task IdleAsync() => Task.CompletedTask;
}

// ------------------------------
// Generic derived class (script-facing)
// ------------------------------
public abstract class FunkyPlcBase<TDevice> : FunkyPlcBase
    where TDevice : FunkyDevice
{
    // Typed view on DeviceUntyped
    protected TDevice Device => (TDevice)DeviceUntyped;

    // Optional: strongly-typed attach to get an early, clear error
    public void AttachDevice(TDevice device) => base.AttachDevice(device);
}
