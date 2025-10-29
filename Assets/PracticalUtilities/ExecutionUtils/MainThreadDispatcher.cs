using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace PracticalUtilities.ExecutionUtils
{
    public static class MainThreadDispatcher
    {
        public static void InvokeOnMainThread(this Action action, PlayerLoopTiming timing = PlayerLoopTiming.Update)
        {
            UniTask.Post(action, timing);
        }

        public static void InvokeOnMainThread<T>(this Action<T> action, T param
            , PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            
            UniTask.SwitchToMainThread(timing, cancellationToken);
            action?.Invoke(param);
        }

        public static void InvokeOnMainThread<T1, T2>(this Action<T1, T2> action, T1 param1, T2 param2
            , PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            
            UniTask.SwitchToMainThread(timing, cancellationToken);
            action?.Invoke(param1, param2);
        }

        public static void InvokeOnMainThread<T1, T2, T3>(this Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3
            , PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            
            UniTask.SwitchToMainThread(timing, cancellationToken);
            action?.Invoke(param1, param2, param3);
        }

        public static void InvokeOnMainThread<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 param1, T2 param2,
            T3 param3
            , T4 param4, PlayerLoopTiming timing = PlayerLoopTiming.Update,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            
            UniTask.SwitchToMainThread(timing, cancellationToken);
            action?.Invoke(param1, param2, param3, param4);
        }

        public static void InvokeOnMainThread<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 param1,
            T2 param2,
            T3 param3, T4 param4, T5 param5, PlayerLoopTiming timing = PlayerLoopTiming.Update,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            
            UniTask.SwitchToMainThread(timing, cancellationToken);
            action?.Invoke(param1, param2, param3, param4, param5);
        }

        public static void InvokeOnMainThread<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 param1
            , T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, PlayerLoopTiming timing = PlayerLoopTiming.Update
            , CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            
            UniTask.SwitchToMainThread(timing, cancellationToken);
            action?.Invoke(param1, param2, param3, param4, param5, param6);
        }

        public static void ToMainThread(Action action, PlayerLoopTiming timing = PlayerLoopTiming.Update)
        {
            UniTask.Post(action, timing);
        }

        public static void ToMainThread<T>(Action<T> action, T param
            , PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            
            UniTask.SwitchToMainThread(timing, cancellationToken);
            action?.Invoke(param);
        }

        public static void ToMainThread<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2
            , PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            
            UniTask.SwitchToMainThread(timing, cancellationToken);
            action?.Invoke(param1, param2);
        }

        public static void ToMainThread<T1, T2, T3>(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3
            , PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            
            UniTask.SwitchToMainThread(timing, cancellationToken);
            action?.Invoke(param1, param2, param3);
        }

        public static void ToMainThread<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 param1, T2 param2
            , T3 param3, T4 param4, PlayerLoopTiming timing = PlayerLoopTiming.Update,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            
            UniTask.SwitchToMainThread(timing, cancellationToken);
            action?.Invoke(param1, param2, param3, param4);
        }

        public static void ToMainThread<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 param1
            , T2 param2, T3 param3, T4 param4, T5 param5, PlayerLoopTiming timing = PlayerLoopTiming.Update
            , CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            
            UniTask.SwitchToMainThread(timing, cancellationToken);
            action?.Invoke(param1, param2, param3, param4, param5);
        }

        public static void ToMainThread<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 param1
            , T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, PlayerLoopTiming timing = PlayerLoopTiming.Update
            , CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            
            UniTask.SwitchToMainThread(timing, cancellationToken);
            action?.Invoke(param1, param2, param3, param4, param5, param6);
        }
    }
}
