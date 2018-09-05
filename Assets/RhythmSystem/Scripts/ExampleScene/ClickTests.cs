using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class ClickTests : MonoBehaviour {
    public GameObject instantiatePrefab;
    public AudioClip oneShot;
    public RhythmTracker.TriggerTiming triggerTiming;
    public UnityEngine.UI.Text countText;
    private int count = 0;

    private AudioSource m_AudioSource;

    void Start ()
    {        
        RhythmTracker.instance.Subscribe(Spawn, RhythmTracker.TriggerTiming.Eighths, true);
        RhythmTracker.instance.Subscribe(Trigger, triggerTiming, false);

        m_AudioSource = GetComponent<AudioSource>();
	}
    
    private void Spawn()
    {
        StartCoroutine(SpawnAndMoveAndDestroy());
    }

    private void Trigger()
    {
        m_AudioSource.PlayOneShot(oneShot);

        countText.text = (count + 1).ToString();
        count = (count + 1) % 4;
    }

    private IEnumerator SpawnAndMoveAndDestroy()
    {
        GameObject go = Instantiate(instantiatePrefab);
        go.transform.position = Vector3.zero + Vector3.up * 5;
        go.transform.parent = transform;
        float offset = RhythmTracker.instance.GetOffset();
        float elapsedTime = 0;
        while (elapsedTime < offset)
        {
            elapsedTime += Time.unscaledDeltaTime;
            go.transform.position = Vector3.zero + Vector3.up * Mathf.Lerp(5, 0, Mathf.InverseLerp(0, offset, elapsedTime));
            yield return new WaitForEndOfFrame();
        }
        Destroy(go);
    }

    private void OnDisable()
    {
        RhythmTracker.instance.Unsubscribe(Spawn, triggerTiming, true);
        RhythmTracker.instance.Unsubscribe(Trigger, triggerTiming, false);
    }
}
