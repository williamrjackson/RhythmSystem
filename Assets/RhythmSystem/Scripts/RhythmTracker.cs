using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RhythmTracker : MonoBehaviour {
    public static RhythmTracker instance;
    [SerializeField]
    private float m_InitialTempo = 120;
    [SerializeField]
    private float m_AdvancedNotificationOffset = 0;
    [SerializeField]
    private bool m_PlayOnAwake = true;
    [SerializeField]
    private AudioSource m_PlaybackAudioSource;
    private AudioSource m_LoopAudioSource;
    private AudioSource m_OffsetAudioSource;
    private int m_SubDivisions = 32;
    private List<int> m_HitList;
    private int m_NextHitIndex = 0;
    private int m_NextAdvancedHitIndex = 0;
    private bool m_IsPaused;

    public enum TriggerTiming { Thirtyseconds, Sixteenths, Eighths, Quarters, Halves, Wholes };

    public UnityAction On32nd;
    public UnityAction On16th;
    public UnityAction On8th;
    public UnityAction OnQuarter;
    public UnityAction OnHalf;
    public UnityAction OnWhole;

    public UnityAction OnAdvanced32nd;
    public UnityAction OnAdvanced16th;
    public UnityAction OnAdvanced8th;
    public UnityAction OnAdvancedQuarter;
    public UnityAction OnAdvancedHalf;
    public UnityAction OnAdvancedWhole;

    void Awake()
    {
        // Singleton behavior
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        // Set the main audio loop
        m_LoopAudioSource = new GameObject().AddComponent<AudioSource>();
        m_LoopAudioSource.gameObject.name = "MainTracking";
        m_LoopAudioSource.transform.parent = transform;
        m_LoopAudioSource.loop = true;
        m_LoopAudioSource.playOnAwake = false;
        m_LoopAudioSource.volume = 0;
        AudioClip silentClip = AudioClip.Create("100bpm-Silent", 423360, 1, 44100, false);
        m_LoopAudioSource.clip = silentClip;

        // If PlayOnAwake is enabled, queue it up so Play() actually happens on the first frame. After everything's initialized and all subscriptions in 
        // Start() and Awake() functions have been made. Otherwise, first beats will be intermittently skipped.
        if (m_PlayOnAwake)
        {
            StartCoroutine(PlayDelayedByFrame());
        }

        // If we're providing forewarnings on an offset, create a new child object with a
        // similarly set up audio source. This one will start right away, while the other will
        // start on after the offset time has passed.
        if (m_AdvancedNotificationOffset > 0)
        {
            m_OffsetAudioSource = new GameObject().AddComponent<AudioSource>();
            m_OffsetAudioSource.gameObject.name = "OffsetTracking";
            m_OffsetAudioSource.transform.parent = transform;
            m_OffsetAudioSource.loop = true;
            m_OffsetAudioSource.playOnAwake = false;
            m_OffsetAudioSource.clip = m_LoopAudioSource.clip;
            m_OffsetAudioSource.volume = 0;
        }
        // Initialize list of target samples
        m_HitList = new List<int>();
        // Fill list with the the samples that represent target downbeats.
        for (int i = 0; i < m_SubDivisions; i++)
        {
            m_HitList.Add(m_LoopAudioSource.clip.samples / m_SubDivisions * i);
        }
        SetTempo(m_InitialTempo);
    }

	void Update ()
    {
        CheckForHit();
        CheckForAdvancedHit();
    }

    private void CheckForHit()
    {
        if (m_LoopAudioSource.timeSamples > m_HitList[m_NextHitIndex])
        {
            // Special check to not fire on the condition where the hit we're waiting for is at 0, but we're greater than 0 only because
            // we're at the end of the loop.
            if (m_NextHitIndex == 0 && m_LoopAudioSource.timeSamples > m_HitList[1])
                return;
            // Call notifier
            Hit(m_NextHitIndex);
            // Increment hit index (looping back to 0 once out of range)
            m_NextHitIndex = (m_NextHitIndex + 1) % m_HitList.Count;
        }
    }
    private void CheckForAdvancedHit()
    {
        if (m_AdvancedNotificationOffset > 0 && m_OffsetAudioSource.timeSamples > m_HitList[m_NextAdvancedHitIndex])
        {
            if (m_NextAdvancedHitIndex == 0 && m_OffsetAudioSource.timeSamples > m_HitList[1])
                return;
            // Call notifier
            AdvancedHit(m_NextAdvancedHitIndex);
            // Increment hit index (looping back to 0 once out of range)
            m_NextAdvancedHitIndex = (m_NextAdvancedHitIndex + 1) % m_HitList.Count;
        }
    }

    // Alert subscribers that it's note time
    private void Hit(int index)
    {
        if (On32nd != null)
            On32nd();
        if (index % 2 == 0 && On16th != null)
            On16th();
        if (index % 4 == 0 && On8th != null)
            On8th();
        if (index % 8 == 0 && OnQuarter != null)
            OnQuarter();
        if (index % 16 == 0 && OnHalf != null)
            OnHalf();
        if (index == 0 && OnWhole != null)
            OnWhole();
    }
    private void AdvancedHit(int index)
    {
        if (OnAdvanced32nd != null)
            OnAdvanced32nd();
        if (index % 2 == 0 && OnAdvanced16th != null)
            OnAdvanced16th();
        if (index % 4 == 0 && OnAdvanced8th != null)
            OnAdvanced8th();
        if (index % 8 == 0 && OnAdvancedQuarter != null)
            OnAdvancedQuarter();
        if (index % 16 == 0 && OnAdvancedHalf != null)
            OnAdvancedHalf();
        if (index == 0 && OnAdvancedWhole != null)
            OnAdvancedWhole();
    }

    public float GetOffset()
    {
        return m_AdvancedNotificationOffset;
    }

    // Set and get tempo
    public float GetTempo()
    {
        return m_LoopAudioSource.pitch * 100;
    }
    public void SetTempo(float tempo)
    {
        m_LoopAudioSource.pitch = tempo / 100;
        if (m_OffsetAudioSource != null)
            m_OffsetAudioSource.pitch = tempo / 100;
        if (m_PlaybackAudioSource != null && m_PlaybackAudioSource.isPlaying)
            Debug.LogWarning("Warning: Changing the tempo of the RhythmSystem does NOT change the tempo of the playback audio clip.");
    }

    // Composite transport controls
    public void Play()
    {
        if (!m_IsPaused)
        {
            StartCoroutine(StartPlayback());
        }
        else
        {
            UnPause();
        }
    }
    public void Stop()
    {
        if (m_OffsetAudioSource != null && m_OffsetAudioSource.isPlaying)
            m_OffsetAudioSource.Stop();
        if (m_PlaybackAudioSource != null && m_PlaybackAudioSource.isPlaying)
            m_PlaybackAudioSource.Stop();
        if (m_LoopAudioSource.isPlaying)
            m_LoopAudioSource.Stop();
    }
    public bool Pause()
    {
        if (m_AdvancedNotificationOffset > 0 && !m_LoopAudioSource.isPlaying)
        {
            Debug.LogError("Cannot pause before offset is completed");
            return false;
        }
        if (m_OffsetAudioSource != null && m_OffsetAudioSource.isPlaying)
            m_OffsetAudioSource.Pause();
        if (m_PlaybackAudioSource != null && m_PlaybackAudioSource.isPlaying)
            m_PlaybackAudioSource.Pause();
        if (m_LoopAudioSource.isPlaying)
            m_LoopAudioSource.Pause();
        m_IsPaused = true;
        return true;
    }
    public void UnPause()
    {
        if (m_OffsetAudioSource != null && !m_OffsetAudioSource.isPlaying)
            m_OffsetAudioSource.UnPause();
        if (m_PlaybackAudioSource != null && !m_PlaybackAudioSource.isPlaying)
            m_PlaybackAudioSource.UnPause();
        if (!m_LoopAudioSource.isPlaying)
            m_LoopAudioSource.UnPause();
        m_IsPaused = false;
    }

    // Wait til everything is fully initialized before playback begins... We don't want samples running before we have a chance to see them.
    private IEnumerator PlayDelayedByFrame()
    {
        yield return new WaitForEndOfFrame();
        StartCoroutine(StartPlayback());
    }
    private IEnumerator StartPlayback()
    {
        // If an offset is desired, start the pre-roll audiosource's playback and delay main playback by that offset.
        if (m_OffsetAudioSource != null)
        {
            m_OffsetAudioSource.Play();
            yield return new WaitForSecondsRealtime(m_AdvancedNotificationOffset);
        }
        if (m_PlaybackAudioSource != null)
        {
            m_PlaybackAudioSource.Play();
        }
        // Play main loop audiosource
        m_LoopAudioSource.Play();
        yield return null;
    }


    // Public Subscription methods for convenience
    public void Subscribe(UnityAction subscriber, TriggerTiming triggerTiming, bool advanced = false)
    {
        if (advanced)
        {
            switch (triggerTiming)
            {
                case TriggerTiming.Thirtyseconds:
                {
                    OnAdvanced32nd += subscriber;
                    return;
                }
                case TriggerTiming.Sixteenths:
                {
                    OnAdvanced16th += subscriber;
                    return;
                }
                case TriggerTiming.Eighths:
                {
                    OnAdvanced8th += subscriber;
                    return;
                }
                case TriggerTiming.Quarters:
                {
                    OnAdvancedQuarter += subscriber;
                    return;
                }
                case TriggerTiming.Halves:
                {
                    OnAdvancedHalf += subscriber;
                    return;
                }
                case TriggerTiming.Wholes:
                {
                    OnAdvancedWhole += subscriber;
                    return;
                }
            }
        }
        else
        {
            switch (triggerTiming)
            {
                case TriggerTiming.Thirtyseconds:
                {
                    On32nd += subscriber;
                    return;
                }
                case TriggerTiming.Sixteenths:
                {
                    On16th += subscriber;
                    return;
                }
                case TriggerTiming.Eighths:
                {
                    On8th += subscriber;
                    return;
                }
                case TriggerTiming.Quarters:
                {
                    OnQuarter += subscriber;
                    return;
                }
                case TriggerTiming.Halves:
                {
                    OnHalf += subscriber;
                    return;
                }
                case TriggerTiming.Wholes:
                {
                    OnWhole += subscriber;
                    return;
                }
            }
        }
    }
    public void Unsubscribe(UnityAction subscriber, TriggerTiming triggerTiming, bool advanced = false)
    {
        if (advanced)
        {
            switch (triggerTiming)
            {
                case TriggerTiming.Thirtyseconds:
                {
                    OnAdvanced32nd -= subscriber;
                    return;
                }
                case TriggerTiming.Sixteenths:
                {
                    OnAdvanced16th -= subscriber;
                    return;
                }
                case TriggerTiming.Eighths:
                {
                    OnAdvanced8th -= subscriber;
                    return;
                }
                case TriggerTiming.Quarters:
                {
                    OnAdvancedQuarter -= subscriber;
                    return;
                }
                case TriggerTiming.Halves:
                {
                    OnAdvancedHalf -= subscriber;
                    return;
                }
                case TriggerTiming.Wholes:
                {
                    OnAdvancedWhole -= subscriber;
                    return;
                }
            }
        }
        else
        {
            switch (triggerTiming)
            {
                case TriggerTiming.Thirtyseconds:
                {
                    On32nd -= subscriber;
                    return;
                }
                case TriggerTiming.Sixteenths:
                {
                    On16th -= subscriber;
                    return;
                }
                case TriggerTiming.Eighths:
                {
                    On8th -= subscriber;
                    return;
                }
                case TriggerTiming.Quarters:
                {
                    OnQuarter -= subscriber;
                    return;
                }
                case TriggerTiming.Halves:
                {
                    OnHalf -= subscriber;
                    return;
                }
                case TriggerTiming.Wholes:
                {
                    OnWhole -= subscriber;
                    return;
                }
            }
        }
    }
}
