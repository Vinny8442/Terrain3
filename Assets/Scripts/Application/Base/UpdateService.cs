using System;
using System.Collections.Generic;
using Core.Infrastructure;
using UnityEngine;

namespace Application.Init
{
	public class UpdateService : MonoBehaviour, ICoroutineRunner, IUpdater
	{
		private TargetsCollection<IUpdateTarget> _update;
		private TargetsCollection<IFixedUpdateTarget> _fixedUpdate;

		public void Update()
		{
			_update?.Update(Time.deltaTime);
		}

		public void FixedUpdate()
		{
			_fixedUpdate?.Update(Time.fixedDeltaTime);
		}

		public void AddUpdate(IUpdateTarget client)
		{
			_update ??= new TargetsCollection<IUpdateTarget>((target, dt) => target.HandleUpdate(dt));
			_update.Add(client);
		}

		public void RemoveUpdate(IUpdateTarget target)
		{
			_update?.Remove(target);
		}

		public void AddFixedUpdate(IFixedUpdateTarget target)
		{
			_fixedUpdate ??= new TargetsCollection<IFixedUpdateTarget>((target, dt) => target.HandleFixedUpdate(dt));
			_fixedUpdate.Add(target);
		}

		public void RemoveFixedUpdate(IFixedUpdateTarget target)
		{
			_fixedUpdate?.Remove(target);
		}

		private class TargetsCollection<T>
		{
			public TargetsCollection(Action<T, float> callDelegate)
			{
				_callDelegate = callDelegate;
			}

			private readonly List<T> _clients = new();
			private readonly List<T> _removedClients = new();
			private readonly Action<T, float> _callDelegate;

			public void Add(T client)
			{
				if (!_clients.Contains(client))
				{
					_clients.Add(client);
				}

				_removedClients.Remove(client);
			}

			public void Remove(T target)
			{
				if (_clients.Contains(target) && !_removedClients.Contains(target))
				{
					_removedClients.Add(target);
				}
			}

			public void Update(float dt)
			{
				foreach (T client in _removedClients)
				{
					_clients.Remove(client);
				}

				_removedClients.Clear();
				
				foreach (T client in _clients)
				{
					_callDelegate(client, dt);
				}
			}
		}
	}
}