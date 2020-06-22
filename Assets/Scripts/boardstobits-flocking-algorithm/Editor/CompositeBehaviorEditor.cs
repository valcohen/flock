using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CompositeBehavior))]
public class CompositeBehaviorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //setup
        CompositeBehavior cb = (CompositeBehavior)target;

        //check for behaviors
        if (cb.behaviors == null || cb.behaviors.Length == 0)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("No behaviors in array!", MessageType.Warning);
            GUILayout.EndHorizontal();
        }
        else 
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Behaviors");
            GUILayout.Label("Weights");
            GUILayout.EndHorizontal();
            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < cb.behaviors.Length; i++)
            {
                GUILayout.BeginHorizontal();

                //GUILayout.Label( i.ToString());
                cb.behaviors[i] = (FlockBehavior)EditorGUI.ObjectField(
                    GUILayoutUtility.GetRect(100f, 20f),
                    cb.behaviors[i],
                    typeof(FlockBehavior),
                    false
                );

                cb.weights[i] = EditorGUI.FloatField(
                    GUILayoutUtility.GetRect(10f, 20f),
                    cb.weights[i]
                );

                GUILayout.EndHorizontal();
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(cb);
            }
        }


        GUILayout.BeginHorizontal();
        if (GUI.Button(GUILayoutUtility.GetRect(100f, 20f), "Add Behavior"))
        {
            AddBehavior(cb);
            EditorUtility.SetDirty(cb);
        }
        GUILayout.EndHorizontal();

        if (cb.behaviors != null && cb.behaviors.Length > 0)
        {
            GUILayout.BeginHorizontal();
            if (GUI.Button(GUILayoutUtility.GetRect(100f, 20f), "Remove Behavior"))
            {
                RemoveBehavior(cb);
                EditorUtility.SetDirty(cb);
            }
            GUILayout.EndHorizontal();
        }
    }

    void AddBehavior(CompositeBehavior cb)
    {
        int oldCount = (cb.behaviors != null) ? cb.behaviors.Length : 0;
        FlockBehavior[] newBehaviors = new FlockBehavior[oldCount + 1];
        float[] newWeights = new float[oldCount + 1];
        for (int i = 0; i < oldCount; i++)
        {
            newBehaviors[i] = cb.behaviors[i];
            newWeights[i] = cb.weights[i];
        }
        newWeights[oldCount] = 1f;
        cb.behaviors = newBehaviors;
        cb.weights = newWeights;
    }

    void RemoveBehavior(CompositeBehavior cb)
    {
        int oldCount = cb.behaviors.Length;
        if (oldCount == 1)
        {
            cb.behaviors = null;
            cb.weights = null;
            return;
        }
        FlockBehavior[] newBehaviors = new FlockBehavior[oldCount - 1];
        float[] newWeights = new float[oldCount - 1];
        for (int i = 0; i < oldCount - 1; i++)
        {
            newBehaviors[i] = cb.behaviors[i];
            newWeights[i] = cb.weights[i];
        }
        cb.behaviors = newBehaviors;
        cb.weights = newWeights;
    }
}
