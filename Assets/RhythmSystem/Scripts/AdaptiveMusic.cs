using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptiveMusic : MonoBehaviour {
    [Range(0,1)]
    public float intensity = 0;
    public AdaptiveLoop[] loops;

    private float smoothTime = 25f;
    private float rangePerSection;

    void Start ()
    {
        rangePerSection = 1f / (loops.Length - 1);

        for (int i = 0; i < loops.Length; i++)
        {
            GameObject go = new GameObject();
            go.name = loops[i].clip.name;
            go.transform.parent = transform;
            loops[i].audioSrc = go.AddComponent<AudioSource>();
            loops[i].audioSrc.clip = loops[i].clip;
            loops[i].audioSrc.loop = true;
            if (i == 0)
            {
                loops[i].audioSrc.volume = 1f;
                loops[i].startDegree = -.01f;
                loops[i].maxDegree = 0f;
                loops[i].endDegree = rangePerSection;
            }
            else
            {
                loops[i].audioSrc.volume = 0f;
                loops[i].startDegree = loops[i - 1].maxDegree;
                loops[i].maxDegree = loops[i - 1].endDegree;
                loops[i].endDegree = loops[i].maxDegree + rangePerSection;
            }
            loops[i].audioSrc.Play();
        }
	}
	
	void Update ()
    {
        foreach (AdaptiveLoop al in loops) 
        {
            // default to silent
            float vol = 0;
            // If intensity is in range (or above on an additive loop)
            if (intensity >= al.startDegree && (intensity <= al.endDegree || al.additive))
            {
                // change default to 1, since there's a chance it won't meet either of the following conditions, suggesting 
                // that it's addititive
                vol = 1;
                if (intensity <= al.maxDegree)
                {
                    vol = Mathf.InverseLerp(al.startDegree, al.maxDegree, intensity);
                }
                else if (!al.additive)
                {
                    vol = Mathf.InverseLerp(al.endDegree, al.maxDegree, intensity);
                }
            }
            // Smooth to avoid jarring changes.
            al.audioSrc.volume = Mathf.SmoothDamp(al.audioSrc.volume, vol, ref al.smoothVel, smoothTime * Time.deltaTime);
        }
    }

    public int GetLoopCount()
    {
        return loops.Length;
    }

    public void SetIntensity(float newIntensity)
    {
        intensity = Mathf.Clamp01(newIntensity);
    }

    public bool SetLoopIntensity(int loopIndex)
    {
        if (loopIndex < 0 || loopIndex > loops.Length)
            return false;
        else
        {
            intensity = loops[loopIndex].maxDegree;
            return true;
        }
    }

    [System.Serializable]
    public class AdaptiveLoop
    {
        public AudioClip clip;
        public bool additive;
        [HideInInspector]
        public float startDegree;
        [HideInInspector]
        public float maxDegree;
        [HideInInspector]
        public float endDegree;
        [HideInInspector]
        public float smoothVel = 0;
        [HideInInspector]
        public AudioSource audioSrc;
    }
}
