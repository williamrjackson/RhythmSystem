using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHitReporter : MonoBehaviour {
    public BlockSliceDetection blockSliceDetector;
    public bool isFinalCollision;
    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<LightSaber>())
        {
            if (isFinalCollision)
            {
                blockSliceDetector.RegisterCollision2();
            }
            else
            {
                blockSliceDetector.RegisterCollision1();
            }
        }
    }
}
