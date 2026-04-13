using Stateless;

namespace BugPro;

public class Bug
{
    public enum State { NewDefect, Triage, Fix, Closed, Reopened }
    public enum Trigger { StartTriage, NoTime, NeedSeparateSolution, OtherProduct, NeedMoreInfo, FixDone, NotDefect, DoNotFix, Duplicate, NotReproducible, VerifyOk, VerifyFailed, Reopen }

    private readonly StateMachine<State, Trigger> _tracker;
    private State _state;

    public Bug()
    {
        _state = State.NewDefect;
        _tracker = new StateMachine<State, Trigger>(() => _state, s => _state = s);

        ConfigureWorkflow();
    }

    private void ConfigureWorkflow()
    {
        _tracker.Configure(State.NewDefect)
            .Permit(Trigger.StartTriage, State.Triage);

        _tracker.Configure(State.Triage)
            .Permit(Trigger.NoTime, State.Fix)
            .Permit(Trigger.NeedSeparateSolution, State.Fix)
            .Permit(Trigger.OtherProduct, State.Closed)
            .Ignore(Trigger.NeedMoreInfo)
            .Ignore(Trigger.StartTriage)
            .Ignore(Trigger.Reopen);

        _tracker.Configure(State.Fix)
            .Permit(Trigger.FixDone, State.Closed)
            .Permit(Trigger.NotDefect, State.Closed)
            .Permit(Trigger.DoNotFix, State.Closed)
            .Permit(Trigger.Duplicate, State.Closed)
            .Permit(Trigger.NotReproducible, State.Closed)
            .Permit(Trigger.Reopen, State.Reopened)
            // Добавим обработку, чтобы не падало
            .Ignore(Trigger.VerifyOk)
            .Ignore(Trigger.VerifyFailed);

        _tracker.Configure(State.Closed)
            .Permit(Trigger.Reopen, State.Reopened)
            .Ignore(Trigger.FixDone)
            .Ignore(Trigger.NotDefect)
            .Ignore(Trigger.DoNotFix)
            .Ignore(Trigger.Duplicate)
            .Ignore(Trigger.NotReproducible);

        _tracker.Configure(State.Reopened)
            .Permit(Trigger.StartTriage, State.Triage)
            .Permit(Trigger.FixDone, State.Closed)
            .Ignore(Trigger.Reopen);
    }

    public State CurrentState => _state;

    // Методы теперь выглядят иначе
    public void StartTriage() => _tracker.Fire(Trigger.StartTriage);
    public void NoTime() => _tracker.Fire(Trigger.NoTime);
    public void NeedSeparateSolution() => _tracker.Fire(Trigger.NeedSeparateSolution);
    public void OtherProduct() => _tracker.Fire(Trigger.OtherProduct);
    public void NeedMoreInfo() => _tracker.Fire(Trigger.NeedMoreInfo);
    public void FixDone() => _tracker.Fire(Trigger.FixDone);
    public void NotDefect() => _tracker.Fire(Trigger.NotDefect);
    public void DoNotFix() => _tracker.Fire(Trigger.DoNotFix);
    public void Duplicate() => _tracker.Fire(Trigger.Duplicate);
    public void NotReproducible() => _tracker.Fire(Trigger.NotReproducible);
    public void VerifyOk() => _tracker.Fire(Trigger.VerifyOk);
    public void VerifyFailed() => _tracker.Fire(Trigger.VerifyFailed);
    public void Reopen() => _tracker.Fire(Trigger.Reopen);
}

public static class Program
{
    public static void Main()
    {
        var myBug = new Bug();
        Console.WriteLine($"[System] Initial bug status: {myBug.CurrentState}");

        myBug.StartTriage();
        myBug.NeedSeparateSolution();
        myBug.FixDone();

        Console.WriteLine($"[System] Final status after fix: {myBug.CurrentState}");
        
        myBug.Reopen();
        Console.WriteLine($"[System] Status after reopening: {myBug.CurrentState}");
    }
}