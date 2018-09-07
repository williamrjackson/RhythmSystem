using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class RhythmPatternEvent
{
    public Side side = Side.None;
    public Hand hand = Hand.Left;
    public int hitIndex = -1;
    public Vector2 position = new Vector2(.5f, .5f);

    public enum Side
    { None, Left, Right, Top, Bottom, Front, Any };
    public enum Hand
    { Left, Right }
}

