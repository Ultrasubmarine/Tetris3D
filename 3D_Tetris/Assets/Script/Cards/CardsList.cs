using System.Collections.Generic;
using UnityEngine;

namespace Script.Cards
{
    [CreateAssetMenu(fileName = "CardsList", menuName = "Cards", order = 0)]
    public class CardsList : ScriptableObject
    {
        public List<Sprite> cards;
    }
}