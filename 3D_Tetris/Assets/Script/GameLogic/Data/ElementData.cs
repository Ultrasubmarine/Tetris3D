using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.GameLogic.TetrisElement
{
    public class ElementData: Singleton<ElementData> {
    public event Action onNewElementUpdate;
    public event Action onMergeElement;
    public Element newElement { get; private set; }

    public Func<Element> loader;
    public List<Element> mergerElements => _mergerElements;

    private List<Element> _mergerElements;
    
    private void Start()
    {
        _mergerElements = new List<Element>();
    }

    public void LoadNewElement()
    {
        if (!Equals(newElement, null))
            return;
        newElement = loader.Invoke();
        onNewElementUpdate?.Invoke();
    }

    public void MergeNewElement()
    {
        _mergerElements.Add(newElement);
        onMergeElement?.Invoke();
        newElement = null;
    }

    public void MergeElement(Element element)
    {
        _mergerElements.Add(element);
    }

    public void RemoveMergedElement(Element element)
    {
        _mergerElements.Remove(element);
    }

    public void RemoveAll()
    {
        _mergerElements.Clear();
        newElement = null;
    }
    }
}