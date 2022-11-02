namespace Core.Infrastructure
{
	public interface IUpdater
	{
		void AddUpdate(IUpdateTarget client);
		void RemoveUpdate(IUpdateTarget target);

		void AddFixedUpdate(IFixedUpdateTarget target);
		void RemoveFixedUpdate(IFixedUpdateTarget target);

	}

	public interface IUpdateTarget
	{
		void HandleUpdate(float dt);
	}

	public interface IFixedUpdateTarget
	{
		void HandleFixedUpdate(float dt);
	}
}