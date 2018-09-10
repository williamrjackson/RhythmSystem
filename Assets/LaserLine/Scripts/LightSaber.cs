using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSaber : MonoBehaviour {
    public float length = 3;
    public float speed = .15f;
    public Transform followTransform;
    public LaserLine line;
    public Transform tip;
    public ParticleSystem particles;
    private bool m_AllowSaber;
    private CapsuleCollider m_Collider;
    
    void Start()
    {
        m_Collider = GetComponent<CapsuleCollider>();
        m_AllowSaber = true;
        SaberOn();
    }

    void LateUpdate () {
        transform.position = followTransform.position;
        transform.rotation = followTransform.rotation;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, tip.position);
    }

    public void SaberOn()
    {
        if (m_AllowSaber)
        { 
            line.Visible = true;
            EnableCollider(true);
            StopAllCoroutines();
            StartCoroutine(OpenSaber());
        }
    }

    public void SaberOff()
    {
        EnableCollider(false);
        StartCoroutine(CloseSaber());
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
            return;
        particles.transform.position = col.contacts[0].point;
        particles.Play();
    }

    private void EnableCollider(bool enable)
    {
        m_Collider.enabled = enable;
        m_Collider.direction = 2;
        m_Collider.radius = line.width;
        m_Collider.height = length;
        m_Collider.center = new Vector3(0, 0, length / 2);
    }

    private void SaberRetractComplete()
    {
        line.Visible = false;
    }

    private IEnumerator OpenSaber()
    {
        float elapsedTime = 0;
        tip.localPosition = Vector3.zero;
        while (elapsedTime < speed)
        {
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.unscaledDeltaTime;
            tip.transform.localPosition = Vector3.zero + tip.forward * length * Mathf.Lerp(0, length, Mathf.InverseLerp(0, speed, elapsedTime));
        }
        tip.transform.localPosition = Vector3.zero + tip.forward * length;
    }
    private IEnumerator CloseSaber()
    {
        float elapsedTime = 0;
        tip.localPosition = Vector3.zero + tip.forward * length;
        while (elapsedTime < speed)
        {
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.unscaledDeltaTime;
            tip.transform.localPosition = Vector3.zero + tip.forward * length * Mathf.Lerp(length, 0, Mathf.InverseLerp(0, speed, elapsedTime));
        }
        tip.transform.localPosition = Vector3.zero;
    }

}
