using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(AudioSource))]

public class RhythmTracker : MonoBehaviour {
    public static RhythmTracker instance;

    [SerializeField]
    private int m_SubDivisions = 32;
    private AudioSource m_LoopAudioSource;
    private List<int> m_HitList;
    private int m_NextHitIndex = 0;

    public UnityAction On32nd;
    public UnityAction On16th;
    public UnityAction On8th;
    public UnityAction OnQuarter;
    public UnityAction OnHalf;
    public UnityAction OnWhole;

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
        // Get the audiosource
        m_LoopAudioSource = GetComponent<AudioSource>();
        // Make sure it's looping
        m_LoopAudioSource.loop = true;
        // If PlayOnAwake is enabled, hijack it so Play() actually happens on the first frame. After everything's initialized and all subscriptions in 
        // Start() and Awake() functions have been made. Otherwise, first beats will be intermittently skipped.
        if (m_LoopAudioSource.playOnAwake)
        {
            m_LoopAudioSource.Stop();
            StartCoroutine(PlayDelayedByFrame());
        }
        // Initialize list of target samples
        m_HitList = new List<int>();
        // Fill list with the the samples that represent target downbeats.
        for (int i = 0; i < m_SubDivisions; i++)
        {
            m_HitList.Add(m_LoopAudioSource.clip.samples / m_SubDivisions * i);
        }
    }

	void Update () {
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

    // Wait til everything is fully initialized before playback begins... We don't want samples running before we have a chance to see them.
    private IEnumerator PlayDelayedByFrame()
    {
        yield return new WaitForEndOfFrame();
        m_LoopAudioSource.Play();
    }
}
