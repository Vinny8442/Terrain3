using System.Collections.Generic;
using Core.Infrastructure;
using UnityEngine;

namespace Application.Init
{
	public class UpdateService : MonoBehaviour, ICoroutineRunner
	{
		private readonly List<Client> _clients = new();
		private readonly List<Client> _removedClients = new();

		public interface Client
		{
			void OnUpdate();
		}

		public void Update()
		{
			foreach (Client client in _removedClients)
			{
				_clients.Remove(client);
			}

			_removedClients.Clear();
			foreach (Client client in _clients)
			{
				client.OnUpdate();
			}
		}

		public void Add(Client client)
		{
			if (!_clients.Contains(client))
			{
				_clients.Add(client);
			}

			_removedClients.Remove(client);
		}

		public void Remove(Client client)
		{
			if (_clients.Contains(client) && !_removedClients.Contains(client))
			{
				_removedClients.Add(client);
			}
		}
	}
}