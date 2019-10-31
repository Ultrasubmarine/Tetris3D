using System.Collections.Generic;

namespace Script.GameLogic.TetrisElement
{
    public static class ElementData
    {
        private static List<Element> _mergerElements;
        public static List<Element> MergerElement => _mergerElements;

        public static Element NewElement { get; set;}

        static ElementData()
        {
            _mergerElements = new List<Element>();    
        }
        
        public static void AddElement(Element element)
        {
            _mergerElements.Add(element);
        }
        
        public static void RemoveElement(Element element)
        {
            _mergerElements.Remove(element);
        }
    }
}