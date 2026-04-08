using UnityEngine;
using UnityEngine.Pool;

namespace Core.ObjectPool
{
    public class PooledObject : MonoBehaviour
    {
        private IObjectPool<GameObject> _pool;

        public void SetPool(IObjectPool<GameObject> pool)
        {
            _pool = pool;
        }

        public virtual void ReturnToPool()
        {
            if (_pool != null)
                _pool.Release(gameObject);
            else
                Destroy(gameObject);
        }
    }
}