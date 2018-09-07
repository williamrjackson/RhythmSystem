using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class RhythmPatternEditor : EditorWindow {
    public Object source;
    public int selected;
    [MenuItem("Window/Rhythm Pattern Editor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        RhythmPatternEditor window = (RhythmPatternEditor)GetWindow(typeof(RhythmPatternEditor));
        window.Show();
    }
    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        source = EditorGUILayout.ObjectField(source, typeof(RhythmPattern), false, GUILayout.MaxWidth(600));
        if (source)
        {
            RhythmPattern sourcePattern = (RhythmPattern)source;
            if (sourcePattern.events.Count != sourcePattern.steps * sourcePattern.rows)
                sourcePattern.InitializeList();
            RhythmTracker.TriggerTiming timing = sourcePattern.timing;
            sourcePattern.timing = (RhythmTracker.TriggerTiming)EditorGUILayout.EnumPopup(("Timing"), timing, GUILayout.MaxWidth(600));
            int offset = sourcePattern.offset;
            sourcePattern.offset = EditorGUILayout.IntField("Offset", offset, GUILayout.MaxWidth(600));
            string[] labels = new string[sourcePattern.steps * sourcePattern.rows];
            Texture2D[] textures = new Texture2D[sourcePattern.steps * sourcePattern.rows];
            GUIContent[] contents = new GUIContent[sourcePattern.steps * sourcePattern.rows];
            for (int i = 0; i < contents.Length; i++)
            {
                Color handColor;
                if (i > sourcePattern.steps - 1)
                {
                    sourcePattern.events[i].hand = RhythmPatternEvent.Hand.Right;
                    handColor = Color.red;
                }
                else
                {
                    sourcePattern.events[i].hand = RhythmPatternEvent.Hand.Left;
                    handColor = Color.blue;
                }
                labels[i] = (i % sourcePattern.steps + offset).ToString();
                textures[i] = new Texture2D(5, 10);
                Color[] colorArray = textures[i].GetPixels();
                Color stateColor;
                if (sourcePattern.events[i].side == RhythmPatternEvent.Side.None)
                {
                    stateColor = Color.gray;
                }
                else
                {
                    stateColor = handColor;
                }
                for (int j = 0; j < colorArray.Length; j++)
                {
                    colorArray[j] = stateColor;
                }
                textures[i].SetPixels(colorArray);

                contents[i] = new GUIContent(labels[i], textures[i]);
            }
            selected = GUILayout.SelectionGrid(selected, contents, sourcePattern.steps);

            RhythmPatternEvent patternEvent = sourcePattern.events[selected];
            int.TryParse(labels[selected], out patternEvent.hitIndex);
            float plotXSliderValue = patternEvent.position.x;
            float plotYSliderValue = patternEvent.position.y;
            RhythmPatternEvent.Side side = patternEvent.side;

            Texture2D XY = new Texture2D(200, 200);
            Color[] resetColorArray = XY.GetPixels();

            for (int i = 0; i < resetColorArray.Length; i++)
            {
                resetColorArray[i] = Color.black;
            }

            XY.SetPixels(resetColorArray);
            XY.Apply();
            EditorGUI.DrawPreviewTexture(new Rect(10, 110, 200, 200), XY);

            Texture2D pos = new Texture2D(10, 10);
            Color[] resetColorArrayPos = pos.GetPixels();

            for (int i = 0; i < resetColorArrayPos.Length; i++)
            {
                resetColorArrayPos[i] = Color.white;
            }

            pos.SetPixels(resetColorArrayPos);
            pos.Apply();
            plotXSliderValue = EditorGUI.Slider(new Rect(10, 320, 200, 20), plotXSliderValue, 0, 1);
            plotYSliderValue = EditorGUI.Slider(new Rect(10, 340, 200, 20), plotYSliderValue, 0, 1);
            EditorGUI.DrawPreviewTexture(new Rect(Mathf.RoundToInt(Mathf.Lerp(10, 200, plotXSliderValue)), Mathf.RoundToInt(Mathf.Lerp(110, 300, plotYSliderValue)), 10, 10), pos);

            patternEvent.position.x = plotXSliderValue;
            patternEvent.position.y = plotYSliderValue;
            GUILayout.BeginArea(new Rect(10, 370, 200, 20));
            patternEvent.side = (RhythmPatternEvent.Side)EditorGUILayout.EnumPopup(("Side"), side, GUILayout.Width(200));
            GUILayout.EndArea();
            EditorGUILayout.EndVertical();
        }
    }
}
