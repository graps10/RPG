using Core.ObjectPool.Configs.FX;
using Managers;
using UnityEngine;
using PoolManager = Core.ObjectPool.PoolManager;

namespace Items_and_Inventory.Effects
{
    [CreateAssetMenu(fileName = "Ice and Fire effect", menuName = "Data/Item Effect/Ice and Fire", order = 0)]
    public class IceAndFireEffect : ItemEffect
    {
        private const int Third_Attack_Combo_Index = 2;
        
        [SerializeField] private IceAndFirePoolConfig iceAndFireConfig;
        [SerializeField] private float xVelocity;

        public override void ExecuteEffect(Transform respawnPosition)
        {
            Player.Player player = PlayerManager.Instance.PlayerGameObject;

            bool thirdAttack = player.PrimaryAttack.ComboCounter == Third_Attack_Combo_Index;

            if (thirdAttack)
            {
                GameObject newIceAndFire = PoolManager.Instance
                    .Spawn(iceAndFireConfig.Prefab, respawnPosition.position, player.transform.rotation);
                
                newIceAndFire.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * player.FacingDir, 0);
                PoolManager.Instance.ReturnWithDelay(newIceAndFire, iceAndFireConfig.ReturnToPoolDelay);
            }
        }
    }
}
