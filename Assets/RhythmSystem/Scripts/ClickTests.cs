using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class ClickTests : MonoBehaviour {
    public AudioClip oneShot;
    public TriggerTiming triggerTiming;
    public enum TriggerTiming
    {
        Thirtyseconds, Sixteenths, Eighths, Quarters, Halves, Wholes 
    };
    private AudioSource m_AudioSource;

    void Start () {
        // Subscribe to the desired trigger time.
        switch (triggerTiming)
        {
            case TriggerTiming.Thirtyseconds:
                {
                    RhythmTracker.instance.On32nd += Trigger;
                    break;
                }
            case TriggerTiming.Sixteenths:
                {
                    RhythmTracker.instance.On16th += Trigger;
                    break;
                }
            case TriggerTiming.Eighths:
                {
                    RhythmTracker.instance.On8th += Trigger;
                    break;
                }
            case TriggerTiming.Quarters:
                {
                    RhythmTracker.instance.OnQuarter += Trigger;
                    break;
                }
            case TriggerTiming.Halves:
                {
                    RhythmTracker.instance.OnHalf += Trigger;
                    break;
                }
            case TriggerTiming.Wholes:
                {
                    RhythmTracker.instance.OnWhole += Trigger;
                    break;
                }
        }
        m_AudioSource = GetComponent<AudioSource>();
	}
	
    private void Trigger()
    {
        m_AudioSource.PlayOneShot(oneShot);
    }
}
