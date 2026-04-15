using Core.ObjectPool;
using TMPro;
using UnityEngine;

namespace Controllers.Skill_Controllers
{
    public class BlackHoleHotKeyController : PooledObject
    {
        private static readonly Color hiddenColor = Color.clear;
        
        private SpriteRenderer _sr;
        private KeyCode _myHotKey;
        private TextMeshProUGUI _myText;

        private Transform _myEnemy;
        private BlackHoleSkillController _blackHole;
        
        private Color _defaultSrColor;
        private Color _defaultTextColor;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _myText = GetComponentInChildren<TextMeshProUGUI>();
            
            if (_sr != null) _defaultSrColor = _sr.color;
            if (_myText != null) _defaultTextColor = _myText.color;
        }
        
        public void SetupHotKey(KeyCode myNewHotKey, Transform myEnemy, BlackHoleSkillController myBlackHole)
        {
            _myHotKey = myNewHotKey;
            
            if (_myText != null)
                _myText.text = myNewHotKey.ToString();

            _myEnemy = myEnemy;
            _blackHole = myBlackHole;
        }
        
        public override void ReturnToPool()
        {
            if (_sr != null) _sr.color = _defaultSrColor;
            if (_myText != null) _myText.color = _defaultTextColor;
            
            _myEnemy = null;
            _blackHole = null;
            
            base.ReturnToPool();
        }

        private void Update() 
        {
            if(Input.GetKeyDown(_myHotKey))
            {
                _blackHole.AddEnemyToList(_myEnemy);

                if (_myText != null) _myText.color = hiddenColor;
                if (_sr != null) _sr.color = hiddenColor;
            }
        }
    }
}
