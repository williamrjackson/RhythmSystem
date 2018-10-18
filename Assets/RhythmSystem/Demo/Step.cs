using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Step : MonoBehaviour {
    [SerializeField]
    private int step = 0;
    [SerializeField]
    private int row = 0;
    private bool m_bIsOn = false;

    private Button m_thisButton;
    private ColorBlock m_colorBlock;
    private RowNotes m_rowNotes;
    private int m_noteIndex = 0;

    void Start ()
    {
        m_thisButton = GetComponent<Button>();
        m_colorBlock = m_thisButton.colors;
        m_rowNotes = FindObjectOfType<RowNotes>();

        // Subscribe to 16th notes
        RhythmTracker.instance.Subscribe(OnBeat, RhythmTracker.TriggerTiming.Sixteenths);
	}

    private void OnBeat(int beatIndex)
    {
        // If the step is enabled, play a note
        if (m_noteIndex == step && m_bIsOn)
        {
            m_rowNotes.PlayRow(row, step);
        }

        // increment the note index
        m_noteIndex = (m_noteIndex + 1) % 16;
    }

    // Called by the button click event.
    // Set state and button color
    public void StepClick()
    {
        if (m_bIsOn)
        {
            m_bIsOn = false;
            SetButtonColor(Color.white);
        }
        else
        {
            m_bIsOn = true;
            SetButtonColor(Color.black);
        }
    }

    // Set all button states to the same color, so it stays on for hover, click, etc.
    private void SetButtonColor(Color color)
    {
        m_colorBlock.highlightedColor = color;
        m_colorBlock.normalColor = color;
        m_colorBlock.pressedColor = color;
        m_colorBlock.disabledColor = color;
        m_thisButton.colors = m_colorBlock;
    }
}
