using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptiveMusic : MonoBehaviour {
    // Inspector fields
    [SerializeField]
    [Range(0,1)]
    private float m_Intensity = 0;
    [SerializeField]
    private AdaptiveLoop[] m_Loops;

    private float m_SmoothTime = 25f;
    private float m_RangePerSection;
    private float m_LastAppliedIntensity = -1;

    void Start ()
    {
        double audioStartTime = AudioSettings.dspTime + .5;
        // Requires more than one loop
        if (m_Loops.Length < 2)
        {
            Debug.LogError("AdaptiveMusic requires at least 2 audio loops. Disabling component");
            enabled = false;
            return;
        }

        // Determine the range each loop occupies
        m_RangePerSection = 1f / (m_Loops.Length - 1);

        // For each loop, set a position for fade in begin, max volume, and fade out complete.
        for (int i = 0; i < m_Loops.Length; i++)
        {
            // Create an AudioSource for this clip. Child it.
            GameObject go = new GameObject();
            go.name = m_Loops[i].clip.name;
            go.transform.parent = transform;
            m_Loops[i].audioSrc = go.AddComponent<AudioSource>();
            m_Loops[i].audioSrc.clip = m_Loops[i].clip;
            m_Loops[i].audioSrc.loop = true;
            m_Loops[i].audioSrc.playOnAwake = false;
            // First loop special handling... Start at volume 1, provide negative (out of range/inaccessible) fade in phase
            if (i == 0)
            {
                m_Loops[i].audioSrc.volume = 1f;
                m_Loops[i].fadeInBegin = -m_RangePerSection;
                m_Loops[i].maxVolPos = 0f;
                m_Loops[i].fadeOutEnd = m_RangePerSection;
            }
            // For each subsequent loop:
            else
            {
                m_Loops[i].audioSrc.volume = 0f;
                // Set the fade-in-start position to the previous max-vol/fade-out-start position
                m_Loops[i].fadeInBegin = m_Loops[i - 1].maxVolPos;
                // Set the max-vol position to the previous fade-out-end, resulting in a linear crossfade
                m_Loops[i].maxVolPos = m_Loops[i - 1].fadeOutEnd;
                // Add fade out range. Note: for the last loop this overflows to > 1, making in inaccessible (by design)
                m_Loops[i].fadeOutEnd = m_Loops[i].maxVolPos + m_RangePerSection;
            }
            // Start the loop (scheduled to ensure sample-accurate sync)
            m_Loops[i].audioSrc.PlayScheduled(audioStartTime);
        }
	}
	
	void Update ()
    {
        // If we've already calculated for the current intensity value, don't bother doing it again on this update.
        bool bRefreshTargetVols = (m_Intensity != m_LastAppliedIntensity);
        foreach (AdaptiveLoop al in m_Loops) 
        {
            if (bRefreshTargetVols)
            {
                // default to silent
                float vol = 0;
                // If intensity is in range (or above on an additive loop), we may need to manipulate it's volume. 
                // Otherwise, keep the value at 0, to silence it.
                if (m_Intensity >= al.fadeInBegin && (m_Intensity <= al.fadeOutEnd || al.additive))
                {
                    // change default to 1, since there's a chance it won't meet either of the following conditions, suggesting 
                    // that it's addititive
                    vol = 1;
                    // If intensity's in range, but below the max vol position, set the loops volume to the relative position between fade in begin, and max. 
                    if (m_Intensity <= al.maxVolPos)
                    {
                        vol = Mathf.InverseLerp(al.fadeInBegin, al.maxVolPos, m_Intensity);
                    }
                    // If intensity's in range, but above the max vol position, set the loops volume to the relative position between fade in begin, and max.
                    // Unless it's additive, in which case we don't fade it out. Keep vol=1; 
                    else if (!al.additive)
                    {
                        vol = Mathf.InverseLerp(al.fadeOutEnd, al.maxVolPos, m_Intensity);
                    }
                }
                m_LastAppliedIntensity = m_Intensity;
                al.targetVol = vol;
            }
            // Smooth to avoid jarring changes.
            al.audioSrc.volume = Mathf.SmoothDamp(al.audioSrc.volume, al.targetVol, ref al.smoothVel, m_SmoothTime * Time.deltaTime);
        }
    }



    // Get/Set intensity
    public void SetIntensity(float newIntensity)
    {
        m_Intensity = Mathf.Clamp01(newIntensity);
    }
    public float GetIntensity()
    {
        return m_Intensity;
    }

    public bool SetIntensityByLoopIndex(int loopIndex)
    {
        // Fail if requested index is out of range.
        if (loopIndex < 0 || loopIndex > m_Loops.Length)
            return false;
        else
        {
            // Set intensity value to the requested loops max volume position
            m_Intensity = m_Loops[loopIndex].maxVolPos;
            return true;
        }
    }

    // Return number of available loops, for use in combination with SetIntensityByLoopIndex
    public int GetLoopCount()
    {
        return m_Loops.Length;
    }

    [System.Serializable]
    public class AdaptiveLoop
    {
        public AudioClip clip;
        public bool additive;
        [HideInInspector]
        public float fadeInBegin;
        [HideInInspector]
        public float maxVolPos; // AKA fadeInEnd; AKA fadeOutBegin
        [HideInInspector]
        public float fadeOutEnd;
        [HideInInspector]
        public float smoothVel = 0;
        [HideInInspector]
        public float targetVol = 0;
        [HideInInspector]
        public AudioSource audioSrc;
    }
}
