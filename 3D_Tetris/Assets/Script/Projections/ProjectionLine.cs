using System;
using UnityEngine;

namespace Script.Projections
{
    public class ProjectionLine : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _line;
        [SerializeField] Transform _mask;

        public void UpdatedLineScale( float y)
        {
            _line.size =  new Vector3(0.28f, y);
        }

        public void UpdatedBottomBound()
        {
            
        }
        public void IncrementHeight(float delta)
        {
            _line.size -= Vector2.up * delta; 
        }

        public void SetHeight( float height)
        {
            _line.size = new Vector2(_line.size.x, height);
        }

        public void SetMinBottomHeight( float height)
        {
            _mask.localScale = new Vector3( 1,height, 1);
        }

        public void SetLineColor(Color color, bool ignoreMask)
        {
            _line.color = color;
            if (ignoreMask)
            {
                _line.maskInteraction = SpriteMaskInteraction.None;
                _mask.gameObject.SetActive(false);
            }
        }
        
        private void OnDisable()
        {
            _line.color = new Color(1, 1, 1, 1);
            _line.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            _mask.gameObject.SetActive(true);
        }
    }
}