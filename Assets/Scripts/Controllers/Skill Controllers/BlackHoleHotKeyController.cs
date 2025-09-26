using TMPro;
using UnityEngine;

namespace Controllers.Skill_Controllers
{
    public class BlackHoleHotKeyController : MonoBehaviour
    {
        private static readonly Color hiddenColor = Color.clear;
        
        private SpriteRenderer _sr;
        private KeyCode _myHotKey;
        private TextMeshProUGUI _myText;

        private Transform _myEnemy;
        private BlackHoleSkillController _blackHole;

        public void SetupHotKey(KeyCode myNewHotKey, Transform myEnemy, BlackHoleSkillController myBlackHole)
        {
            _sr = GetComponent<SpriteRenderer>();
            _myText = GetComponentInChildren<TextMeshProUGUI>();

            _myHotKey = myNewHotKey;
            _myText.text = myNewHotKey.ToString();

            _myEnemy = myEnemy;
            _blackHole = myBlackHole;
        }

        private void Update() 
        {
            if(Input.GetKeyDown(_myHotKey))
            {
                _blackHole.AddEnemyToList(_myEnemy);

                _myText.color = hiddenColor;
                _sr.color = hiddenColor;
            }
        }
    }
}
