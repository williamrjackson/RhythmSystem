using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserLine : MonoBehaviour {
    [SerializeField]
    private Color outerColor = Color.red;
    [SerializeField]
    private Color innerColor = Color.white;
    public float width = .5f;
    [Range(0,100)]
    public float centerGlow = 25;
    [Range(0,1)]
    public float pulseWidth = 0f;
    [Range(0, 5)]
    public float pulseLength = 0f;
    public Vector3[] positions = { Vector3.zero, new Vector3( 0, 0, 10 ) };
    public Material innerFadeMaterial;
    public Material outerFadeMaterial;
    public bool useWorldSpace = true;
    LineRenderer m_ColorLine;
    LineRenderer m_WhiteLine;
    bool m_IsVisible = true;
    float sourceAlpha = 1f;
    float goalAlpha;
    float lastColorChangeTime;
	// Use this for initialization
	void Awake () {
        foreach (LineRenderer lr in transform.GetComponentsInChildren<LineRenderer>())
        {
            Destroy(lr.gameObject);
        }
        GameObject colorGO = new GameObject("ColorLine");
        colorGO.transform.parent = transform;            
        colorGO.transform.localPosition = Vector3.zero;
        m_ColorLine = colorGO.AddComponent<LineRenderer>();
        m_ColorLine.numCapVertices = 9;

        GameObject whiteGO = new GameObject("WhiteLine");
        whiteGO.transform.parent = transform;
        whiteGO.transform.localPosition = Vector3.zero;
        m_WhiteLine = whiteGO.AddComponent<LineRenderer>();
        m_WhiteLine.numCapVertices = 9;

        if (outerFadeMaterial != null)
        {
            m_ColorLine.material = outerFadeMaterial;
        }
        else
        {
            Debug.LogError("Outer Fade Material is Missing.");
        }
        if (innerFadeMaterial != null)
        {
            m_WhiteLine.material = innerFadeMaterial;
        }
        else
        {
            Debug.LogError("Inner Fade Material is Missing.");
        }

        SetPositions( positions );
    }

    void Update () {
        if (useWorldSpace != m_ColorLine.useWorldSpace) m_ColorLine.useWorldSpace = useWorldSpace;
        if (useWorldSpace != m_WhiteLine.useWorldSpace) m_WhiteLine.useWorldSpace = useWorldSpace;

        m_ColorLine.enabled = m_IsVisible;
        m_WhiteLine.enabled = m_IsVisible;

        m_ColorLine.startWidth = width;
        m_ColorLine.endWidth = width;

        m_WhiteLine.startWidth = width / (100 / centerGlow);
        m_WhiteLine.endWidth = width / (100 / centerGlow);
        Color appliedColor = outerColor;
        if (pulseLength > 0 && pulseWidth > 0)
        {
            if (goalAlpha > sourceAlpha)
            {
                sourceAlpha = 1f - pulseWidth;
            }
            else
            {
                goalAlpha = 1f - pulseWidth;
            }
            float percentage = (Time.unscaledTime - lastColorChangeTime) / pulseLength;
            percentage = Mathf.Clamp01(percentage);
            appliedColor.a = Mathf.Lerp(sourceAlpha, goalAlpha, percentage);
            if (percentage == 1f)
            {
                lastColorChangeTime = Time.unscaledTime;

                // Switch alpha fade direction
                float temp = sourceAlpha;
                sourceAlpha = goalAlpha;
                goalAlpha = temp;
            }
        }

        m_ColorLine.startColor = appliedColor;
        m_ColorLine.endColor = appliedColor;
        m_WhiteLine.startColor = innerColor;
        m_WhiteLine.endColor = innerColor;

    }

    public void SetPositions(Vector3[] newPositions)
    {
        positions = newPositions;
        m_ColorLine.positionCount = positions.Length;
        m_ColorLine.SetPositions( positions );
        m_WhiteLine.positionCount = positions.Length;
        m_WhiteLine.SetPositions( positions );
    }

    public void SetPosition(int index, Vector3 newPosition)
    {
        if (index > positions.Length) Debug.Log("Line Index Out of Range.");
        positions[index] = newPosition;
        SetPositions(positions);
    }

    public Vector3 GetPosition(int index)
    {
        return positions[index];
    }

    public void SetColor(Color newColor)
    {
        outerColor = newColor;
    }

    public int numPositions
    {
        get
        {
            return positions.Length;
        }
        set
        {
            positions = new Vector3[value];
        }
    }

    public bool Visible
    {
        get
        {   
            return m_IsVisible;
        }
        set
        {
            m_IsVisible = value;
        }
    }
}
