using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class ClickTests : MonoBehaviour {
    public GameObject instantiatePrefab;
    public AudioClip oneShot;
    public RhythmTracker.TriggerTiming triggerTiming;
    public AnimationCurve curve;
    public UnityEngine.UI.Text countText;
    private int count = 0;

    private AudioSource m_AudioSource;

    void Start ()
    {        
        RhythmTracker.instance.Subscribe(Spawn, triggerTiming, true);
        RhythmTracker.instance.Subscribe(Trigger, triggerTiming, false);

        m_AudioSource = GetComponent<AudioSource>();
	}
    
    private void Spawn(int beatIndex)
    {
        StartCoroutine(SpawnAndMoveAndDestroy());
    }

    private void Trigger(int beatIndex)
    {
        m_AudioSource.PlayOneShot(oneShot);

        countText.text = (count + 1).ToString();
        count = (count + 1) % 4;
    }

    private IEnumerator SpawnAndMoveAndDestroy()
    {
        GameObject go = Instantiate(instantiatePrefab);
        float rightOffset = UnityEngine.Random.Range(-4, 4);
        Vector3 targetPos = Vector3.zero + Vector3.forward * -4 + Vector3.right * rightOffset + Vector3.up * -1f;
        go.transform.parent = transform;
        float offset = RhythmTracker.instance.GetOffset();
        float elapsedTime = 0;
        while (elapsedTime < offset)
        {
            float t = Mathf.InverseLerp(0, offset, elapsedTime);
            float inverseT = Mathf.InverseLerp(offset, 0, elapsedTime);
            Vector3 currentPos = targetPos + Vector3.up * 5 * curve.Evaluate(t) + 
                Vector3.forward * 20 * inverseT;
            go.transform.position = currentPos;
            elapsedTime += Time.unscaledDeltaTime;
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
