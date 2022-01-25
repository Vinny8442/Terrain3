namespace Core
{
	public interface IDataInitable<in T> : IInitable
	{
		void Init(T data);
	}
}