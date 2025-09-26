namespace Core.ObjectPool
{
    public interface ISpawnedPooledObject : IPooledObject
    {
        void OnSpawn();
    }
}