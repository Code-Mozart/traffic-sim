using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(XJunction))]
public class XJunctionEditor : Editor
{
    private XJunctionRules rules;
    private XJunction junction;


    public void OnEnable()
    {
        junction = (XJunction)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.LabelField("Traffic Rules");
        rules = (XJunctionRules)EditorGUILayout.EnumPopup(rules);
        junction.rules = FindRules(rules);
    }


    private IXJunctionRules FindRules(XJunctionRules rules)
    {
        switch(rules)
        {
            case XJunctionRules.FirstComeFirstServe:
                return new XJunctionRule_FirstComeFirstServe();
            default:
                throw new UnityException("Unsupported rules " + rules);
        }
    }
}

public enum XJunctionRules
{
    FirstComeFirstServe
}
