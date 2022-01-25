using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Infrastructure
{
	public interface ICoroutineRunner
	{
		Coroutine StartCoroutine(IEnumerator action);
		void StopCoroutine(Coroutine routine);
	}
}