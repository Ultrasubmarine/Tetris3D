using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.PlayerProfile
{
    public class LvlMapManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _lvlObjects;

        private void Start()
        {
            UpdateMap();
        }

        private void UpdateMap()
        {
            int lvl = PlayerSaveProfile.instance._lvl;
            for (int i = 0; i < _lvlObjects.Count; i++)
            {
                _lvlObjects[i].SetActive( i<=lvl);
            }
        }
    }
}