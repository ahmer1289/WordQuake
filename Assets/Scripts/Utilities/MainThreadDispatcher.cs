using System;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> executionQueue = new Queue<Action>(); // Thread-safe queue of actions

    public static MainThreadDispatcher Instance;

    void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

    public void Run(Action action)
    {
        if (Instance == null)
        {
            // Create a persistent GameObject if it doesn't exist
            var go = new GameObject("MainThreadDispatcher");
            Instance = go.AddComponent<MainThreadDispatcher>();
            DontDestroyOnLoad(go);
        }

        // Queue up the action in the lock block to prevent null references. This makes sure that action a get's called before b. 
        lock (executionQueue)
        {
            executionQueue.Enqueue(action);
        }
    }

    void Update()
    {
        lock (executionQueue)
        {
            // Run all queued actions on the main thread (update runs in the unity main thread)
            while (executionQueue.Count > 0)
            {
                var action = executionQueue.Dequeue(); //removes the action from the queue so it's only done once in one frame
                action?.Invoke(); // carries out the action 
            }
        }
    }
}
