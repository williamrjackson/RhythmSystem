using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RowNotes : MonoBehaviour {
    public InputField tempo;
    public Slider swing;
    public Double buffer = 0.1;
    public AudioClip[] rowClips;
    private List<AudioSource> audioSources = new List<AudioSource>();
    void Start()
    {
        gameObject.AddComponent<AudioSource>();
        // Create an output audio source for each row
        for (int i = 0; i < rowClips.Length; i++)
        {
            GameObject go = new GameObject();
            go.name = rowClips[i].name + " Playback";
            go.transform.parent = transform;
            AudioSource audSrc = go.AddComponent<AudioSource>();
            audSrc.clip = rowClips[i];
            audSrc.playOnAwake = false;
            audioSources.Add(audSrc);
        }
        tempo.text = RhythmTracker.instance.GetTempo().ToString();
        tempo.onEndEdit.AddListener(delegate
        {
            TempoChange(tempo);
        }) ;
        
    }
    private void TempoChange(InputField input)
    {
        float newTempo;
        float.TryParse(input.text, out newTempo);
        RhythmTracker.instance.SetTempo(newTempo);
    }

    public void PlayRow(int row, int step)
    {
        // Pass it on to the swing coroutine
        StartCoroutine(PlaySwung(row, step));
    }
    
    private IEnumerator PlaySwung(int row, int step)
    {
        // If it's an even beat, delay based on the swing amount
        if (step % 2 != 0)
            yield return new WaitForSeconds(Mathf.Lerp(0, 15 / RhythmTracker.instance.GetTempo(), swing.value));

        if (audioSources[row].isPlaying)
            audioSources[row].Stop();

        audioSources[row].Play();
    }
}
