using System;

public class Event
{
    Action handler;

    public void AddListener(Action cb) => handler += cb;

    public void RemoveListener(Action cb) => handler -= cb;

    public void Invoke() => handler?.Invoke();
}

public class Event<T>
{
    Action<T> handler;

    public void AddListener(Action<T> cb) => handler += cb;

    public void RemoveListener(Action<T> cb) => handler -= cb;

    public void Invoke(T arg1) => handler?.Invoke(arg1);
}

public class Event<T1, T2>
{
    Action<T1, T2> handler;

    public void AddListener(Action<T1, T2> cb) => handler += cb;

    public void RemoveListener(Action<T1, T2> cb) => handler -= cb;

    public void Invoke(T1 arg1, T2 arg2) => handler?.Invoke(arg1, arg2);
}

public class Event<T1, T2, T3>
{
    Action<T1, T2, T3> handler;

    public void AddListener(Action<T1, T2, T3> cb) => handler += cb;

    public void RemoveListener(Action<T1, T2, T3> cb) => handler -= cb;

    public void Invoke(T1 arg1, T2 arg2, T3 arg3) => handler?.Invoke(arg1, arg2, arg3);
}
