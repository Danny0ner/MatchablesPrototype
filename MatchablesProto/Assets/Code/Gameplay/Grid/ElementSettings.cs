using System;
using UnityEngine;

public enum ElementType
{
    Blue = 1,
    Green = 2,
    Orange = 3,
    Pink = 4,
    Turquoise = 5,
    Yellow = 6
}

[Serializable]
public class ElementColor
{
    public ElementType Type;
    public Sprite Sprite;
}

[CreateAssetMenu(menuName = "Scriptables/Game/Element Settings")]
public class ElementSettings : ScriptableObject
{
    [SerializeField] ElementColor[] _elementColors;

    public Sprite GetElementSprite(ElementType type)
    {
        for (int i = 0; i < _elementColors.Length; i++)
        {
            if (type == _elementColors[i].Type)
                return _elementColors[i].Sprite;
        }

        Debug.LogError($"Element Type = {type} is not configured in element settings");
        return null;
    }
}
