// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-12-03 23-49-09  
//修改作者 : 杜鑫 
//修改时间 : 2021-12-03 23-49-09  
//版 本 : 0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityGameFramework.Runtime;

/// Author: Pim de Witte (pimdewitte.com) and contributors, https://github.com/PimDeWitte/UnityMainThreadDispatcher
/// <summary>
/// A thread-safe class which holds a queue with actions to execute on the next Update() method. It can be used to make calls to the main thread for
/// things such as UI Manipulation in Unity. It was developed for use in combination with the Firebase Unity plugin, which uses separate threads for event handling
/// </summary>
public class MainThreadDispatcherComponent : GameFrameworkComponent {

	private static readonly Queue<Action> _executionQueue = new Queue<Action>();

	public void Update() {
		lock(_executionQueue) {
			while (_executionQueue.Count > 0) {
				_executionQueue.Dequeue().Invoke();
			}
		}
	}

	/// <summary>
	/// Locks the queue and adds the IEnumerator to the queue
	/// </summary>
	/// <param name="action">IEnumerator function that will be executed from the main thread.</param>
	public void Enqueue(IEnumerator action) {
		lock (_executionQueue) {
			_executionQueue.Enqueue (() => {
				StartCoroutine (action);
			});
		}
	}

        /// <summary>
        /// Locks the queue and adds the Action to the queue
	/// </summary>
	/// <param name="action">function that will be executed from the main thread.</param>
	public void Enqueue(Action action)
	{
		Enqueue(ActionWrapper(action));
	}
	
	/// <summary>
	/// Locks the queue and adds the Action to the queue, returning a Task which is completed when the action completes
	/// </summary>
	/// <param name="action">function that will be executed from the main thread.</param>
	/// <returns>A Task that can be awaited until the action completes</returns>
	public Task EnqueueAsync(Action action)
	{
		var tcs = new TaskCompletionSource<bool>();

		void WrappedAction() {
			try 
			{
				action();
				tcs.TrySetResult(true);
			} catch (Exception ex) 
			{
				tcs.TrySetException(ex);
			}
		}

		Enqueue(ActionWrapper(WrappedAction));
		return tcs.Task;
	}

	
	IEnumerator ActionWrapper(Action a)
	{
		a();
		yield return null;
	}


	private static MainThreadDispatcherComponent _instance = null;

	public static bool Exists() {
		return _instance != null;
	}

	public static MainThreadDispatcherComponent Instance() {
		if (!Exists ()) {
			throw new Exception ("UnityMainThreadDispatcher could not find the UnityMainThreadDispatcher object. Please ensure you have added the MainThreadExecutor Prefab to your scene.");
		}
		return _instance;
	}


	void Awake() {
		if (_instance == null) {
			_instance = this;
		}
	}

	void OnDestroy() {
			_instance = null;
	}
}