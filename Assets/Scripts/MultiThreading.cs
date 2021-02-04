using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class MultiThreading : MonoBehaviour
{
    void Start()
    {
        var heavyMethod = Observable.Start(() =>
        {
            // heavy method...
            Debug.Log($"Start ThreadId:{Thread.CurrentThread.ManagedThreadId}");
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
            return 10;
        });

        var heavyMethod2 = Observable.Start(() =>
        {
            // heavy method...
            Debug.Log($"Start ThreadId:{Thread.CurrentThread.ManagedThreadId}");
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));
            return 10;
        });

        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(xs =>
            {
                Observable.WhenAll(heavyMethod, heavyMethod2)
                .ObserveOnMainThread()
                .Subscribe(x =>
                {
                    Debug.Log($"Start ThreadId:{Thread.CurrentThread.ManagedThreadId}");
                    Debug.Log(x[0] + ":" + x[1]);
                });
            });

       
            

        /*
        Observable.WhenAll(heavyMethod, heavyMethod2)
            .ObserveOnMainThread() // return to main thread
            .Subscribe(xs =>
            {
                Debug.Log($"Start ThreadId:{Thread.CurrentThread.ManagedThreadId}");
                Debug.Log(xs[0] + ":" + xs[1]);
            });
        */
    }

}
