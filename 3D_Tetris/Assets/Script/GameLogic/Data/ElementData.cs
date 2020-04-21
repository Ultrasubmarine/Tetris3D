using System;
using System.Collections.Generic;

namespace Script.GameLogic.TetrisElement
{
    public static class ElementData
    {
        public static event Action NewElementUpdate;
        public static Element NewElement { get; private set; }

        public static Func<Element> Loader;
        public static List<Element> MergerElements => _mergerElements;

        private static List<Element> _mergerElements;
        
        static ElementData()
        {
            _mergerElements = new List<Element>();
        }

        public static void LoadNewElement()
        {
            NewElement = Loader.Invoke();
            NewElementUpdate?.Invoke();
        }

        public static void MergeNewElement()
        {
            _mergerElements.Add(NewElement);
            NewElement = null;
        }

        public static void RemoveMergedElement(Element element)
        {
            _mergerElements.Remove(element);
        }

        public static void RemoveAll()
        {
            _mergerElements.Clear();
            NewElement = null;
        }
    }
}