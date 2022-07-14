// FROM: https://forum.unity.com/threads/async-await-and-webgl-builds.472994/
using System;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Async awaitable UnityWebRequest <br/><br/>
/// Usage example: <br/><br/>
/// UnityWebRequest www = new UnityWebRequest(); <br/>
/// // do unitywebrequest setup here here... <br/>
/// await www.SendWebRequest(); <br/>
/// Debug.Log(req.downloadHandler.text); <br/>
/// </summary>
public struct UnityWebRequestAwaiter : INotifyCompletion
{
    private UnityWebRequestAsyncOperation asyncOp;
    private Action continuation;

    public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOp)
    {
        this.asyncOp = asyncOp;
        continuation = null;
    }

    public bool IsCompleted { get { return asyncOp.isDone; } }

    public void GetResult() { }

    public void OnCompleted(Action continuation)
    {
        this.continuation = continuation;
        asyncOp.completed += OnRequestCompleted;
    }

    private void OnRequestCompleted(AsyncOperation obj)
    {
        continuation?.Invoke();
    }
}

public static class ExtensionMethods
{
    public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
    {
        return new UnityWebRequestAwaiter(asyncOp);
    }
}