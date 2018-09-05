# RhythmSystem

Basic notification-based rhythm system for Unity.

Add the RhythmSystem component to a GameObject and subscribe from other scripts:

```C#
public class Test : MonoBehaviour 
{
    private void Start()
    {
        RhythmTracker.instance.SetTempo(140);
        RhythmTracker.instance.Subscribe(Trigger, RhythmTracker.TriggerTiming.Eighths);
    }

    // This will run on every eighth note at 140bpm;
    private void Trigger()
    {
        GetComponent<AudioSource>().Play();
    }
}
```

You can also set on offset, to get advance notifications of beats, in addition to the beats themselves.