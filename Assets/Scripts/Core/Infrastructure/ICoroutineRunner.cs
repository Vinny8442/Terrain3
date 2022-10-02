using System.Collections;
using UnityEngine;

namespace Core.Infrastructure
{
	public interface ICoroutineRunner
	{
		Coroutine StartCoroutine(IEnumerator action);
		void StopCoroutine(Coroutine routine);
	}
}