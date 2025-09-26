namespace Core.ObjectPool
{
    public interface IPooledObject
    {
        void OnReturnToPool();
    }
}