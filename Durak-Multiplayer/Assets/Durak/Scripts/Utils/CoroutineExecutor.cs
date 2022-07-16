using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public static class CoroutineExecutor
{
    public static void DelayedAction(this MonoBehaviour mono, Action action, float delay)
    {
        mono.StartCoroutine(LaunchDelayedAction(action, delay));
    }

    public static async void DelayedAction(Action action, float delay)
    {
        await Task.Delay(TimeSpan.FromSeconds(delay));
        action?.Invoke();
    }

    public static void DelayedAction(this MonoBehaviour mono, Action action, int frames)
    {
        mono.StartCoroutine(LaunchDelayedAction(action, frames));
    }

    public static void DelayedAction(this MonoBehaviour mono, Action action, float delay, out IEnumerator routine)
    {
        routine = LaunchDelayedAction(action, delay);
        mono.StartCoroutine(routine);
    }

    public static void StopAction(this MonoBehaviour mono, IEnumerator routine)
    {
        mono.StopCoroutine(routine);
    }

    private static IEnumerator LaunchDelayedAction(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    private static IEnumerator LaunchDelayedAction(Action action, int frames)
    {
        for (int i = 0; i < frames; i++)
            yield return new WaitForEndOfFrame();
        action?.Invoke();
    }

    public static void StartCoroutine(this MonoBehaviour mono, IEnumerator routine)
    {
        mono.StartCoroutine(routine);
    }
}
