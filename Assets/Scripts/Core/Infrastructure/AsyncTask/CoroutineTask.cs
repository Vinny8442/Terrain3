using System;
using System.Collections;
using System.Threading;
using Core.Infrastructure;
using UnityEngine;

namespace Core.AsyncTask
{
	public class CoroutineTask : AsyncTask 
	{
		private Coroutine _coroutine;
		private readonly ICoroutineRunner _runner;

		public CoroutineTask(ICoroutineRunner runner, IEnumerator enumerator, CancellationToken token)
		{
			_runner = runner;
			_coroutine = runner.StartCoroutine(Run(enumerator));
			token.Register(Cancel);
		}

		private IEnumerator Run(IEnumerator enumerator)
		{
			yield return enumerator;
			_coroutine = null;
			Complete();
		}

		private void Cancel()
		{
			if (_coroutine != null)
			{
				_runner.StopCoroutine(_coroutine);
				_coroutine = null;
				Fail(new OperationCanceledException("Coroutine was cancelled"));
			}
		}
		
	}
}