using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Core.AsyncTask;
using ModestTree;
using UnityEngine;

namespace Jobs
{
	public class JobsTest : MonoBehaviour
	{
		[SerializeField] private Transform _transform;
		private float direction = 1;
		private CancellationTokenSource _token;

		private async Task Start()
		{
			Log("Start::begin");
			_token = new CancellationTokenSource();
			AsyncTask<int> task = new AsyncTask<int>();
			StartCoroutine(TaskCoroutine(task, _token.Token));
			int i = await task;
			float result =  await PrintResult(i);
			Log("Start::done");
		}

		private async IAsyncTask<float> PrintResult(int result)
		{
			Log($"RESULT: {result}");
			return 123.123f;
		}

		private IEnumerator TaskCoroutine(AsyncTask<int> task, CancellationToken token)
		{
			int result = 0;
			while (!token.IsCancellationRequested)
			{
				yield return null;
				result++;
			}
			task.Complete(result);
		}

		private void Update()
		{
			// Log("Update");
			if (Math.Abs(_transform.position.y) >= 1)
			{
				direction = -Math.Sign(_transform.position.y);
			}

			_transform.position += new Vector3(0, direction * Time.deltaTime, 0);

			if (Input.GetMouseButtonDown(0) && _token != null)
			{
				_token.Cancel();
				_token = null;
			}
		}

		private static void Log(string value)
		{
			Debug.Log($"[{Thread.CurrentThread.ManagedThreadId}]: {value}");
		}
	}
}