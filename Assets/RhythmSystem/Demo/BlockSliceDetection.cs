using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSliceDetection : MonoBehaviour {
    public GameObject mainChild;
    public GameObject sliceChild;
    public bool testSlice;
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
            Vector3 forcePoint = transform.InverseTransformPoint(new Vector3(0, .3f, 0));
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(1000, forcePoint, 10);
            }
        }
    }

    void Update()
    {
        if (testSlice)
        {
            testSlice = false;
            collider1Hit = true;
            RegisterCollision2();
        }
    }
}
