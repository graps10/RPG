using Core;
using Managers;
using UnityEngine;

namespace Components.Level
{
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private string id;
        
        private Animator _anim;
        private bool _activationStatus;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<Player.Player>() != null)
                ActivateCheckpoint();
        }

        public void ActivateCheckpoint()
        {
            if (!_activationStatus)
                AudioManager.Instance.PlaySFX(3, transform);

            _activationStatus = true;
            _anim.SetBool(AnimatorHashes.ActiveCheckpoint, true);
        }

        [ContextMenu("Generate checkpoint id")]
        private void GenerateID()
        {
            id = System.Guid.NewGuid().ToString();
        }
    }
}
