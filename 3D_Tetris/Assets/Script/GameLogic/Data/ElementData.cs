using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.GameLogic.TetrisElement
{
    public static class ElementData
    {
        public static event Action onNewElementUpdate;
        public static event Action onMergeElement;
        public static Element newElement { get; private set; }

        public static Func<Element> loader;
        public static List<Element> mergerElements => _mergerElements;

        private static List<Element> _mergerElements;
        
        static ElementData()
        {
            _mergerElements = new List<Element>();
        }

        public static void LoadNewElement()
        {
            newElement = loader.Invoke();
            onNewElementUpdate?.Invoke();
        }

        public static void MergeNewElement()
        {
            _mergerElements.Add(newElement);
            onMergeElement?.Invoke();
            newElement = null;
        }

        public static void MergeElement(Element element)
        {
            _mergerElements.Add(element);
        }
        public static void RemoveMergedElement(Element element)
        {
            _mergerElements.Remove(element);
        }

        public static void RemoveAll()
        {
            _mergerElements.Clear();
            newElement = null;
        }
    }
}