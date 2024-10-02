// Copyright (c) 2023 Gary Riggins
// This code is licensed under the MIT License.
// See the LICENSE file for details.

using System;
using System.Collections;
using UnityEngine;

public static class MonoBehaviourExtension
{
    public static Coroutine<T> StartCoroutine<T>(this MonoBehaviour monoBehaviour, IEnumerator routine)
    {
        return new Coroutine<T>(monoBehaviour, routine);
    }
}

public class Coroutine<T>
{
    public Coroutine(MonoBehaviour monoBehaviour, IEnumerator routine)
    {
        this.coroutine = monoBehaviour.StartCoroutine(this.Initialize(routine));
    }

    public T Result
    {
        get
        {
            if (this.exception != null)
            {
                throw this.exception;
            }
            return this.result;
        }
    }

    public void Cancel()
    {
        this.isCancelled = true;
    }

    public static implicit operator Coroutine(Coroutine<T> generic)
    {
        return generic.coroutine;
    }

    private T result;

    private bool isCancelled;

    private Exception exception;
    private Coroutine coroutine;

    private IEnumerator Initialize(IEnumerator routine)
    {
        this.exception = new InvalidOperationException(
            "Attempt to access result of coroutine that has not yielded one."
        );
        while (!this.isCancelled)
        {
            try
            {
                if (!routine.MoveNext())
                {
                    yield break;
                }
            }
            catch (Exception e)
            {
                this.exception = e;
                yield break;
            }

            object current = routine.Current;
            if (current != null && current.GetType() == typeof(T))
            {
                this.result = (T)current;
                this.exception = null;
            }
            else
            {
                yield return routine.Current;
            }
        }

        this.exception = new OperationCanceledException();
        yield break;
    }
}
