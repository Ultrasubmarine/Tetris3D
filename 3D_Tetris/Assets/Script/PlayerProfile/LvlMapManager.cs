using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Script.PlayerProfile
{
    public class LvlMapManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _lvlObjects;
        [SerializeField] private ScrollRect _scroller;
        [SerializeField] private RectTransform _mapTransform;

        private void Start()
        {
           UpdateMap();
        }

        public void UpdateMap()
        {
            int lvl = PlayerSaveProfile.instance._lvl;
            for (int i = 0; i < _lvlObjects.Count; i++)
            {
                _lvlObjects[i].SetActive( i<=lvl);
                CenterToLvlBtn(_lvlObjects[i].GetComponent<RectTransform>());
            }
            
            _scroller.ScrollToCeneter(_lvlObjects[lvl < _lvlObjects.Count? lvl: _lvlObjects.Count-1].GetComponent<RectTransform>() );
        }
        
        public void UnlockAll()
        {
            for (int i = 0; i < _lvlObjects.Count; i++)
            {
                _lvlObjects[i].SetActive(true);
            }
        }
        
        public void CenterToItem(RectTransform obj)
        {
            float normalizePosition = _mapTransform.anchorMin.y - obj.anchoredPosition.y;
            normalizePosition += (float)obj.transform.GetSiblingIndex() / (float)_scroller.content.transform.childCount;
            normalizePosition /= 1000f;
            normalizePosition = Mathf.Clamp01(1 - normalizePosition);
            _scroller.verticalNormalizedPosition = normalizePosition;
            Debug.Log(normalizePosition);
        }

        private float CenterToLvlBtn(RectTransform btn)
        {
            var koff = 1 / _mapTransform.rect.size.y;
            var normalizeBtnPos = btn.anchoredPosition.y * koff;

            var contentNormalizeSize = 0;//= _scroller.GetComponent<RectTransform>().rect.size.y * koff;
            
            
            Debug.Log( " n pos: " + (normalizeBtnPos - contentNormalizeSize/2));
            return normalizeBtnPos - contentNormalizeSize/2;
        }
    }

    public static class UIScrollExtensions
    {
        // Shared array used to receive result of RectTransform.GetWorldCorners
        static Vector3[] corners = new Vector3[4];
        /// <summary>
        /// Transform the bounds of the current rect transform to the space of another transform.
        /// </summary>
        /// <param name="source">The rect to transform</param>
        /// <param name="target">The target space to transform to</param>
        /// <returns>The transformed bounds</returns>
        public static Bounds TransformBoundsTo(this RectTransform source, Transform target)
        {
            // Based on code in ScrollRect's internal GetBounds and InternalGetBounds methods
            var bounds = new Bounds();
            if (source != null) {
                source.GetWorldCorners(corners);

                var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

                var matrix = target.worldToLocalMatrix;
                for (int j = 0; j < 4; j++) {
                    Vector3 v = matrix.MultiplyPoint3x4(corners[j]);
                    vMin = Vector3.Min(v, vMin);
                    vMax = Vector3.Max(v, vMax);
                }

                bounds = new Bounds(vMin, Vector3.zero);
                bounds.Encapsulate(vMax);
            }
            return bounds;
        }

        /// <summary>
        /// Normalize a distance to be used in verticalNormalizedPosition or horizontalNormalizedPosition.
        /// </summary>
        /// <param name="axis">Scroll axis, 0 = horizontal, 1 = vertical</param>
        /// <param name="distance">The distance in the scroll rect's view's coordiante space</param>
        /// <returns>The normalized scoll distance</returns>
        public static float NormalizeScrollDistance(this ScrollRect scrollRect, int axis, float distance)
        {
            // Based on code in ScrollRect's internal SetNormalizedPosition method
            var viewport = scrollRect.viewport;
            var viewRect = viewport != null ? viewport : scrollRect.GetComponent<RectTransform>();
            var viewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);

            var content = scrollRect.content;
            var contentBounds = content != null ? content.TransformBoundsTo(viewRect) : new Bounds();

            var hiddenLength = contentBounds.size[axis] - viewBounds.size[axis];
            return distance / hiddenLength;
        }

        public static void ScrollToCeneter(this ScrollRect scrollRect, RectTransform target)
        {
            // The scroll rect's view's space is used to calculate scroll position
            var view = scrollRect.viewport != null ? scrollRect.viewport : scrollRect.GetComponent<RectTransform>();

            // Calcualte the scroll offset in the view's space
            var viewRect = view.rect;
            var elementBounds = target.TransformBoundsTo(view);
            var offset = viewRect.center.y - elementBounds.center.y;

            // Normalize and apply the calculated offset
            var scrollPos = scrollRect.verticalNormalizedPosition - scrollRect.NormalizeScrollDistance(1, offset);
            scrollRect.verticalNormalizedPosition = Mathf.Clamp(scrollPos, 0f, 1f);
        }
    }
}