using Core;
using UnityEngine;

namespace UI_Elements
{
    public class FadeScreen : MonoBehaviour
    {
        private Animator _anim;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        public void FadeOut() => _anim.SetTrigger(AnimatorHashes.FadeOut);
        public void FadeIn() => _anim.SetTrigger(AnimatorHashes.FadeIn);
    }
}
