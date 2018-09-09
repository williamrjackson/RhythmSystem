using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSliceDetection : MonoBehaviour {
    public GameObject mainChild;
    public GameObject sliceChild;

    private bool collider1Hit = false;

    public void RegisterCollision1()
    {
        collider1Hit = true;
    }
    public void RegisterCollision2()
    {
        if (collider1Hit)
        {
            mainChild.SetActive(false);
            sliceChild.SetActive(true);
        }
    }
}
