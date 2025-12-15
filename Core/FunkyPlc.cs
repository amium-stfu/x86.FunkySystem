using FunkySystem;
using FunkySystem.Core;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public abstract class FunkyPlcBase : IAsyncDisposable
{
    protected FunkyDevice Device { get; private set; } = null!;

    // Thread-sichere Settings
    public ConcurrentDictionary<object, string> Settings { get; } = new();

    private readonly object _lifecycleLock = new();

    private CancellationTokenSource? _cts;
    private Task? _runTask;
    private Task? _idleTask;

    private int _index;
    public int Index => Volatile.Read(ref _index);

    private long _startTicks;
    private long _endTicks;
    private long _stepStartTicks;

    public DateTime StartTimeUtc => new DateTime(Interlocked.Read(ref _startTicks), DateTimeKind.Utc);
    public DateTime EndTimeUtc => new DateTime(Interlocked.Read(ref _endTicks), DateTimeKind.Utc);
    public DateTime StepStartUtc => new DateTime(Interlocked.Read(ref _stepStartTicks), DateTimeKind.Utc);

    // Interner Token (nicht als Parameter zwingend)
    protected CancellationToken AbortToken => _cts?.Token ?? CancellationToken.None;
    protected bool AbortRequested => _cts?.IsCancellationRequested == true;

    public bool IsRunning
    {
        get
        {
            var cts = _cts;
            return cts != null && !cts.IsCancellationRequested;
        }
    }

    protected FunkyPlcBase() { }

    // Wird vom Host (deinem System) vor Start gesetzt
    public void AttachDevice(FunkyDevice device)
    {
        Device = device ?? throw new ArgumentNullException(nameof(device));
    }

    // ---------- Public Lifecycle ----------
    public void Start()
    {
        lock (_lifecycleLock)
        {
            if (Device == null) throw new InvalidOperationException("AttachDevice() muss vor Start() aufgerufen werden.");
            if (IsRunning) return;

            // Initialisieren (Anwendercode)
            Initialize();

            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            Device.State = State.Running;
            SetStartTimeUtc(DateTime.UtcNow);
            SetEndTimeUtc(DateTime.UtcNow);

            // Optional: Idle parallel (kann später entfernt werden, falls nicht benötigt)
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
                if (Device != null) Device.State = State.Ready;
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

        SetEndTimeUtc(DateTime.UtcNow);
        Device.State = State.Ready;
    }

    public void Abort(string reason = "User")
    {
        lock (_lifecycleLock)
        {
            if (_cts == null || _cts.IsCancellationRequested) return;
            _cts.Cancel();
        }

        Device.SetAbortedState("Aborted: " + reason);
        OnAbort(reason);
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync().ConfigureAwait(false);
    }

    // ---------- Core Loops ----------
    private async Task RunLoopAsync(CancellationToken token)
    {
        try
        {
            Device.State = State.Running;
            Volatile.Write(ref _index, 0);
            SetStepStartUtc(DateTime.UtcNow);

            // Anwendercode
            await Run().ConfigureAwait(false);

            // Finalize nur, wenn nicht abgebrochen
            if (!token.IsCancellationRequested)
            {
                Device.State = State.Done;
                await FinalizeAsync().ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // normal bei Stop/Abort
        }
        catch (Exception ex)
        {
            Device.SetAlertState("RunError " + ex.Message);
        }
        finally
        {
            SetEndTimeUtc(DateTime.UtcNow);
            lock (_lifecycleLock) { _cts?.Cancel(); }
        }
    }

    private async Task IdleLoopAsync(CancellationToken token)
    {
        try
        {
            await IdleAsync().ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // normal
        }
        catch (Exception ex)
        {
            Device.SetAlertState("IdleError " + ex.Message);
        }
    }

    // ---------- Anwender-Helfer ----------
    // Abbruchfähiges Delay ohne Token-Parameter
    protected Task DelayAsync(int milliseconds) => Task.Delay(milliseconds, AbortToken);

    protected void NextStep(string subStatus)
    {
        Device.SubStatus = subStatus;
        Interlocked.Increment(ref _index);
        SetStepStartUtc(DateTime.UtcNow);
    }

    // ---------- Internals ----------
    private static async Task SafeAwait(Task? t)
    {
        if (t == null) return;
        try { await t.ConfigureAwait(false); }
        catch { }
    }

    private void SetStartTimeUtc(DateTime dtUtc)
        => Interlocked.Exchange(ref _startTicks, dtUtc.ToUniversalTime().Ticks);

    private void SetEndTimeUtc(DateTime dtUtc)
        => Interlocked.Exchange(ref _endTicks, dtUtc.ToUniversalTime().Ticks);

    private void SetStepStartUtc(DateTime dtUtc)
        => Interlocked.Exchange(ref _stepStartTicks, dtUtc.ToUniversalTime().Ticks);

    // ---------- Extension Points (nur diese 4 sind „Pflicht“/relevant) ----------
    public virtual void Initialize() { }

    // Anwender muss nur Run implementieren
    public abstract Task Run();

    // Optional
    public virtual Task FinalizeAsync() => Task.CompletedTask;

    // Optional
    public virtual void OnAbort(string reason) { }

    // Optional (falls du Idle behalten willst)
    protected virtual Task IdleAsync() => Task.CompletedTask;
}
