using System;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.CompilerServices;

//Reference http://www.stevevermeulen.com/index.php/2017/09/using-async-await-in-unity3d-2017/

public static class TaskExtensions
{
	/// <summary>
	/// Blocks while condition is true or timeout occurs.
	/// </summary>
	/// <param name="condition">The condition that will perpetuate the block.</param>
	/// <param name="frequency">The frequency at which the condition will be check, in milliseconds.</param>
	/// <param name="timeout">Timeout in milliseconds.</param>
	/// <exception cref="TimeoutException"></exception>
	/// <returns></returns>
	public static async Task WaitWhile(Func<bool> condition, int frequency = 25, int timeout = -1)
	{
		var waitTask = Task.Run(async () =>
		{
			while (condition()) await Task.Delay(frequency);
		});

		if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
			throw new TimeoutException();
	}

	/// <summary>
	/// Blocks until condition is true or timeout occurs.
	/// </summary>
	/// <param name="condition">The break condition.</param>
	/// <param name="frequency">The frequency at which the condition will be checked.</param>
	/// <param name="timeout">The timeout in milliseconds.</param>
	/// <returns></returns>
	public static async Task WaitUntil(Func<bool> condition, int frequency = 25, int timeout = -1)
	{
		var waitTask = Task.Run(async () =>
		{
			while (!condition()) await Task.Delay(frequency);
		});

		if (waitTask != await Task.WhenAny(waitTask,
				Task.Delay(timeout)))
			throw new TimeoutException();
	}

	public static IEnumerator AsIEnumerator(this Task task)
	{
		if (task == null)
			yield break;
		while (!task.IsCompleted)
		{
			yield return null;
		}

		if (task.IsFaulted)
		{
			throw task.Exception;
		}
	}

	public static TaskAwaiter GetAwaiter(this TimeSpan timeSpan)
	{
		return Task.Delay(timeSpan).GetAwaiter();
	}
}