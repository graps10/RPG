using Managers;
using UnityEngine;

namespace Items_and_Inventory.Effects
{
    [CreateAssetMenu(fileName = "Ice and Fire effect", menuName = "Data/Item Effect/Ice and Fire", order = 0)]
    public class IceAndFireEffect : ItemEffect
    {
        private const int Third_Attack_Combo_Index = 2;
        private const float Return_To_Pool_Delay = 10f;
        [SerializeField] private GameObject iceAndFirePrefab;
        [SerializeField] private float xVelocity;

        public override void ExecuteEffect(Transform respawnPosition)
        {
            Player.Player player = PlayerManager.Instance.PlayerGameObject;

            bool thirdAttack = player.PrimaryAttack.ComboCounter == Third_Attack_Combo_Index;

            if (thirdAttack)
            {
                GameObject newIceAndFire = PoolManager.Instance.Spawn("fx", respawnPosition.position, 
                    player.transform.rotation, iceAndFirePrefab);
                
                newIceAndFire.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * player.FacingDir, 0);

                PoolManager.Instance.Return("fx", newIceAndFire, Return_To_Pool_Delay);
            }
        }
    }
}
