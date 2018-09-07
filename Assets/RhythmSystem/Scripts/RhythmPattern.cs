using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Rhythm Pattern")]
public class RhythmPattern : ScriptableObject
{
    public int rows = 2;
    public int steps = 16;
    public int offset = 0;
    public RhythmTracker.TriggerTiming timing = RhythmTracker.TriggerTiming.Sixteenths;
    public List<RhythmPatternEvent> events;

    public void InitializeList()
    {
        events = new List<RhythmPatternEvent>();
        for (int i = events.Count; i < rows * steps; i++)
        {
            events.Add(new RhythmPatternEvent());
        }
    }
}
