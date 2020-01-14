using UnityEngine;

public class SetIntensityByDistance : MonoBehaviour
{
    public AdaptiveMusic adaptiveMusic;
    public Transform transformA;
    public Transform transformB;

    private float cachedDistance = 0f;
    private float minDistance = 3f;
    
    private void Start()
    {
        minDistance = Vector3.Distance(transformA.position, transformB.position);
        cachedDistance = minDistance;
    }
    void Update()
    {
        float currentDistance = Vector3.Distance(transformA.position, transformB.position);
        if (currentDistance != cachedDistance)
        {
            adaptiveMusic.SetIntensity(Mathf.InverseLerp(minDistance, 0f, currentDistance));
            cachedDistance = currentDistance;
        }
    }
}
