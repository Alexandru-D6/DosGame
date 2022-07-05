using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum CardType
{
    C0,
    C1,
    C2,
    C3,
    C4,
    C5,
    C6,
    C7,
    C8,
    C9,
    CBlock,
    CRevers,
    Cplus2,
    CChangeColor,
    Cplus4
}

[System.Serializable]
public enum CardColor
{
    Blue,
    Green,
    Red,
    Yellow,
    Black
}

[System.Serializable]
public enum Layers
{
    Default,
    TransparentFX,
    IgnoreRayCast,
    Water,
    UI,
    CenterDeck,
    PlayerCard,
    MiddleCard,
    ColorSelector
}
public static class Enumerations
{
    public static string getString(this CardType me) {
        switch(me) {
            case CardType.C0: return "C0";
            case CardType.C1: return "C1";
            case CardType.C2: return "C2";
            case CardType.C3: return "C3";
            case CardType.C4: return "C4";
            case CardType.C5: return "C5";
            case CardType.C6: return "C6";
            case CardType.C7: return "C7";
            case CardType.C8: return "C8";
            case CardType.C9: return "C9";
            case CardType.CBlock: return "CBlock";
            case CardType.CChangeColor: return "CChangeColor";
            case CardType.CRevers: return "CRevers";
            case CardType.Cplus4: return "Cplus4";
            case CardType.Cplus2: return "Cplus2";
            default: return "Default";
        }
    }

    public static int getIndex(this CardType me) {
        switch (me) {
            case CardType.C0: return 0;
            case CardType.C1: return 1;
            case CardType.C2: return 2;
            case CardType.C3: return 3;
            case CardType.C4: return 4;
            case CardType.C5: return 5;
            case CardType.C6: return 6;
            case CardType.C7: return 7;
            case CardType.C8: return 8;
            case CardType.C9: return 9;
            case CardType.CBlock: return 10;
            case CardType.CChangeColor: return 11;
            case CardType.CRevers: return 12;
            case CardType.Cplus4: return 13;
            case CardType.Cplus2: return 14;
            default: return -1;
        }
    }

    /// <summary>
    /// Get random Type of Card [0,14]
    /// </summary>
    /// <param name="minInclusive"></param>
    /// <param name="maxInclusive"></param>
    /// <returns></returns>
    public static CardType getRandomType(int minInclusive, int maxInclusive) {
        return (CardType)Random.Range(minInclusive, maxInclusive);
    }

    public static CardType getTypeByIndex(int index) {
        return (CardType)index;
    }

    public static string getString(this CardColor me) {
        switch (me) {
            case CardColor.Blue: return "Blue";
            case CardColor.Green: return "Green";
            case CardColor.Red: return "Red";
            case CardColor.Yellow: return "Yellow";
            case CardColor.Black: return "Black";
            default: return "Default";
        }
    }

    public static int getIndex(this CardColor me) {
        switch (me) {
            case CardColor.Blue: return 0;
            case CardColor.Green: return 1;
            case CardColor.Red: return 2;
            case CardColor.Yellow: return 3;
            case CardColor.Black: return 4;
            default: return -1;
        }
    }

    /// <summary>
    /// Get random Color of Card [0,4]
    /// </summary>
    /// <param name="minInclusive"> </param>
    /// <param name="maxInclusive"> </param>
    /// <returns></returns>
    public static CardColor getRandomColor(int minInclusive, int maxInclusive) {
        return (CardColor)Random.Range(minInclusive, maxInclusive);
    }

    public static CardColor getColorByIndex(int index) {
        return (CardColor)index;
    }

    public static string getString(this Layers me) {
        switch (me) {
            case Layers.Default: return "Default";
            case Layers.TransparentFX: return "TransparentFX";
            case Layers.IgnoreRayCast: return "Ignore Raycast";
            case Layers.Water: return "Water";
            case Layers.UI: return "UI";
            case Layers.CenterDeck: return "Center Deck";
            case Layers.PlayerCard: return "Player Card";
            case Layers.MiddleCard: return "Middle Card";
            case Layers.ColorSelector: return "Color Selector";
            default: return "Default";
        }
    }
}
