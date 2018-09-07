using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class ClickTests : MonoBehaviour {
    public GameObject rightPrefab;
    public GameObject leftPrefab;
    public RhythmPattern pattern;
    public AnimationCurve curve;
    private int count = -1;

    private RhythmTracker.TriggerTiming triggerTiming;
    void Start ()
    {
        triggerTiming = pattern.timing;
        RhythmTracker.instance.Subscribe(Spawn, triggerTiming, true);
	}
    
    private void Spawn(int beatIndex)
    {
        count++;
        foreach(RhythmPatternEvent e in pattern.events)
        {
            if (e.hitIndex == count % pattern.steps && e.side != RhythmPatternEvent.Side.None)
                StartCoroutine(SpawnAndMoveAndDestroy(e));
        }
    }

    private IEnumerator SpawnAndMoveAndDestroy(RhythmPatternEvent e)
    {
        float x = Mathf.Lerp(-5, 5, e.position.x);
        float y = Mathf.Lerp(5, 0, e.position.y);
        GameObject instantiatePrefab = e.hand == RhythmPatternEvent.Hand.Right ? rightPrefab : leftPrefab;
        GameObject go = Instantiate(instantiatePrefab);
        go.transform.parent = transform;
        Vector3 targetPos = Vector3.zero  + Vector3.right * x;
        float offset = RhythmTracker.instance.GetOffset();
        float elapsedTime = 0;
        while (elapsedTime < offset)
        {
            float t = Mathf.InverseLerp(0, offset, elapsedTime);
            float inverseT = Mathf.InverseLerp(offset, 0, elapsedTime);
            Vector3 currentPos = targetPos + Vector3.up * y * curve.Evaluate(t) + 
                Vector3.forward * 40 * inverseT;
            go.transform.position = currentPos;
            elapsedTime += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(go);
    }

    private void OnDisable()
    {
        RhythmTracker.instance.Unsubscribe(Spawn, triggerTiming, true);
    }
}
