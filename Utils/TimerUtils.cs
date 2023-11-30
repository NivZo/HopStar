using System;
using System.Threading.Tasks;

public static class TimerUtils
{
    public static async void DelayAction(float seconds, Action action)
    {
        await Task.Delay((int)(seconds * 1000));
        action();
    }
}