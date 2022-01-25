using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
	public class SettingsStorage
	{
		private Dictionary<string, ScriptableObject> _storage = new Dictionary<string, ScriptableObject>();
		public T Load<T>(string name) where T : ScriptableObject
		{
			if (_storage.TryGetValue(name, out var cachedObject))
			{
				if (cachedObject is T typedObject)
				{
					return typedObject;
				}
				throw new InvalidCastException($"Object {name} is not of type {typeof(T)}");
			}
			T result =  Resources.Load<T>($"Data/{name}");
			if (result == null)
			{
				throw new InvalidOperationException($"Failed to load object {name}");
			}
			
			_storage[name] = result;
			return result;
		}

		public void Clear(string name)
		{
			if (_storage.ContainsKey(name))
			{
				_storage.Remove(name);
			}
		}
	}
}