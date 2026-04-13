using System;
using Enemies.Base;

namespace Core.Interfaces
{
    public interface IMinionSpawner
    {
        public event Action<Enemy> OnMinionSpawned;
    }
}