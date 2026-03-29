using System.Collections;
using System.Collections.Generic;
using ChunkGeneration;
using ChunkGeneration.Configs;
using UnityEngine;

namespace UI_Elements.Background
{
    [System.Serializable]
    public struct ThemeBackground
    {
        public LocationTheme Theme;
        public GameObject BackgroundRoot; 
    }

    public class BackgroundManager : MonoBehaviour
    {
        [SerializeField] private List<ThemeBackground> themeBackgrounds;
        [SerializeField] private float fadeDuration = 2.5f;

        private GameObject _currentActiveBg;

        private void OnEnable() => ChunkGenerator.OnThemeChanged += HandleThemeChanged;

        private void OnDisable() => ChunkGenerator.OnThemeChanged -= HandleThemeChanged;

        private void Start()
        {
            foreach (var bg in themeBackgrounds)
            {
                if (bg.Theme == LocationTheme.Valley)
                {
                    _currentActiveBg = bg.BackgroundRoot;
                    SetAlpha(_currentActiveBg, 1f);
                    _currentActiveBg.SetActive(true);
                }
                else
                    bg.BackgroundRoot.SetActive(false);
            }
        }

        private void HandleThemeChanged(LocationTheme newTheme)
        {
            GameObject newBg = GetBackgroundRoot(newTheme);
            if (newBg == null || newBg == _currentActiveBg) return;

            StartCoroutine(CrossfadeBackgrounds(_currentActiveBg, newBg));
        }

        private IEnumerator CrossfadeBackgrounds(GameObject oldBg, GameObject newBg)
        {
            newBg.SetActive(true);
            SetAlpha(newBg, 0f);

            float elapsedTime = 0f;
            
            SpriteRenderer[] oldSprites = oldBg.GetComponentsInChildren<SpriteRenderer>();
            SpriteRenderer[] newSprites = newBg.GetComponentsInChildren<SpriteRenderer>();

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / fadeDuration;

                SetAlphaToGroup(oldSprites, Mathf.Lerp(1f, 0f, t));
                SetAlphaToGroup(newSprites, Mathf.Lerp(0f, 1f, t));

                yield return null;
            }

            oldBg.SetActive(false);
            _currentActiveBg = newBg;
        }

        private void SetAlphaToGroup(SpriteRenderer[] renderers, float alpha)
        {
            foreach (var sr in renderers)
            {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }
        }

        private void SetAlpha(GameObject root, float alpha)
        {
            SetAlphaToGroup(root.GetComponentsInChildren<SpriteRenderer>(), alpha);
        }

        private GameObject GetBackgroundRoot(LocationTheme theme)
        {
            foreach (var bg in themeBackgrounds)
                if (bg.Theme == theme) return bg.BackgroundRoot;
            
            return null;
        }
    }
}