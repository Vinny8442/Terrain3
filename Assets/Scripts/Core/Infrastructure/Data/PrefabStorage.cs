using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Core
{
	public class PrefabStorage
	{
		private readonly Dictionary<string, GameObject> _storage = new Dictionary<string, GameObject>();
		private readonly DiContainer _container;

		public PrefabStorage(DiContainer container)
		{
			_container = container;
		}
		public GameObject Load(string name) 
		{
			if (_storage.TryGetValue(name, out var cachedObject))
			{
				return cachedObject;
			}
			GameObject result = Resources.Load<GameObject>($"Prefabs/{name}"); 
			if (result == null)
			{
				throw new InvalidOperationException($"Failed to load prefab {name}");
			}
			
			_storage[name] = result;
			return result;
		}

		public T InstantiateAs<T>(string name, Transform parent) where T : MonoBehaviour, IInitable
		{
			var prefab = Load(name);
			var gameObject =  GameObject.Instantiate(prefab, parent);
			var component = gameObject.GetComponent<T>();
			var injectableComponents = gameObject.GetComponentsInChildren<IInjectable>();
			foreach (IInjectable injectable in injectableComponents)
			{
				_container.Inject(injectable);
			}
			
			component.Init(); 

			return component;
		}

		public T InstantiateAs<T, TData>(string name, TData data, Transform parent) where T : MonoBehaviour, IDataInitable<TData>
		{
			T result = InstantiateAs<T>(name, parent);
			result.Init(data);
			return result;
		}
	}
}