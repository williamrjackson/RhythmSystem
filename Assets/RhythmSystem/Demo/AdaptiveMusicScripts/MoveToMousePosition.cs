using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToMousePosition : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 1f)]
    private float chaseDrag = .5f;
    private Vector3 mousePos;
    private Vector3 vel;
    private float zPos;
    private Vector3 targetPos;
    private void Start()
    {
        targetPos = transform.position;
        zPos = transform.position.z;
    }
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        targetPos.z = zPos;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref vel, chaseDrag, 105f);
    }
}
