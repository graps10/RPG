using UnityEngine;

namespace Core.ObjectPool.Configs.FX
{
    [CreateAssetMenu(fileName = "PopupTextPoolConfig", menuName = "ObjectPool/Configs/FX/Popup Text")]
    public class PopupTextPoolConfig : BasePoolConfig
    {
        [Header("FX Specific Settings")]
        [SerializeField] private float speed;
        [SerializeField] private float disappearanceSpeed;
        [SerializeField] private float colorDisappearanceSpeed;
        [SerializeField] private float lifeTime;

        public float Speed => speed;
        public float DisappearanceSpeed => disappearanceSpeed;
        public float ColorDisappearanceSpeed => colorDisappearanceSpeed;
        public float LifeTime => lifeTime;
    }
}