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
        rules = ToEnum(junction.rules);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.LabelField("Traffic Rules");
        var rulesBefore = rules;
        rules = (XJunctionRules)EditorGUILayout.EnumPopup(rules);

        if (rules != rulesBefore)
        {
            junction.rules = FindRules(rules);
            Debug.Log("Set rules of junction '" + junction + "' to '" + junction.rules + "'");
        }
    }


    private IXJunctionRules FindRules(XJunctionRules rules)
    {
        switch (rules)
        {
            case XJunctionRules.None:
                return null;
            case XJunctionRules.FirstComeFirstServe:
                return new XJunctionRule_FirstComeFirstServe();
            default:
                throw new UnityException("Unsupported rules " + rules);
        }
    }

    private XJunctionRules ToEnum(IXJunctionRules rules)
    {
        if (rules == null)
        {
            return XJunctionRules.None;
        }
        else if (rules.GetType() == typeof(XJunctionRule_FirstComeFirstServe))
        {
            return XJunctionRules.FirstComeFirstServe;
        }
        else
        {
            throw new UnityException("Unsupported rules " + rules);
        }
    }
}

public enum XJunctionRules
{
    None,
    FirstComeFirstServe
}
