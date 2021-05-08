using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.GameLogic.TetrisElement
{
    public static class ElementData
    {
        public readonly static string NewElementTag = "newElementTag";
        
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
            
            // add TAG
            foreach (var block in newElement.blocks)
            {
                block.tag = NewElementTag;
            }
        }

        public static void MergeNewElement()
        {
            _mergerElements.Add(newElement);
            onMergeElement?.Invoke();
            
            // remove TAG
            foreach (var block in newElement.blocks)
            {
                block.tag = "none";
            }
            newElement = null;
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