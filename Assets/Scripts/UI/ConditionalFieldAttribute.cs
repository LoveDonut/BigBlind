using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ComparisonType
{
    Equals,
    NotEquals,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual
}

public class ConditionalFieldAttribute : PropertyAttribute
{
    public string conditionalFieldName;
    public ComparisonType comparisonType;
    public object comparisonValue;

    public ConditionalFieldAttribute(string conditionalFieldName)
    {
        this.conditionalFieldName = conditionalFieldName;
        this.comparisonType = ComparisonType.Equals;
        this.comparisonValue = true;
    }

    public ConditionalFieldAttribute(string conditionalFieldName, object comparisonValue, ComparisonType comparisonType = ComparisonType.Equals)
    {
        this.conditionalFieldName = conditionalFieldName;
        this.comparisonType = comparisonType;
        this.comparisonValue = comparisonValue;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
public class ConditionalFieldAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalFieldAttribute condAttr = (ConditionalFieldAttribute)attribute;
        bool enabled = GetConditionalFieldResult(property, condAttr);

        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
        if (enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
        GUI.enabled = wasEnabled;
    }

    private bool GetConditionalFieldResult(SerializedProperty property, ConditionalFieldAttribute condAttr)
    {
        SerializedProperty conditionalFieldProperty = property.serializedObject.FindProperty(condAttr.conditionalFieldName);

        if (conditionalFieldProperty == null)
            return true;

        switch (conditionalFieldProperty.propertyType)
        {
            case SerializedPropertyType.Boolean:
                return CompareValues(conditionalFieldProperty.boolValue, Convert.ToBoolean(condAttr.comparisonValue), condAttr.comparisonType);
            case SerializedPropertyType.Enum:
                return CompareValues(conditionalFieldProperty.enumValueIndex, Convert.ToInt32(condAttr.comparisonValue), condAttr.comparisonType);
            case SerializedPropertyType.Integer:
                return CompareValues(conditionalFieldProperty.intValue, Convert.ToInt32(condAttr.comparisonValue), condAttr.comparisonType);
            case SerializedPropertyType.Float:
                return CompareValues(conditionalFieldProperty.floatValue, Convert.ToSingle(condAttr.comparisonValue), condAttr.comparisonType);
            case SerializedPropertyType.String:
                return CompareValues(conditionalFieldProperty.stringValue, condAttr.comparisonValue.ToString(), condAttr.comparisonType);
            default:
                return true;
        }
    }

    private bool CompareValues<T>(T value1, T value2, ComparisonType comparisonType) where T : IComparable
    {
        int compareResult = value1.CompareTo(value2);
        switch (comparisonType)
        {
            case ComparisonType.Equals: return compareResult == 0;
            case ComparisonType.NotEquals: return compareResult != 0;
            case ComparisonType.GreaterThan: return compareResult > 0;
            case ComparisonType.LessThan: return compareResult < 0;
            case ComparisonType.GreaterThanOrEqual: return compareResult >= 0;
            case ComparisonType.LessThanOrEqual: return compareResult <= 0;
            default: return true;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalFieldAttribute condAttr = (ConditionalFieldAttribute)attribute;
        bool enabled = GetConditionalFieldResult(property, condAttr);

        if (enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
#endif