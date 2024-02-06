using System;
using Unity.Jobs;

public class JobCompleter
{
    public readonly Func<JobHandle> Schedule;
    public readonly Action OnComplete;
    
    public JobCompleter(Func<JobHandle> schedule, Action onComplete)
    {
        Schedule = schedule;
        OnComplete = onComplete;
    }
}