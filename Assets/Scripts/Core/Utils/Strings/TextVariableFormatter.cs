using System.Reflection;
using System.Text;
using System;

public static class TextVariableFormatter
{
    private static readonly StringBuilder outputText = new StringBuilder(300);
    private static readonly StringBuilder currentWord = new StringBuilder(32);

    private static string wordTemp = string.Empty;

    public static string FormatText(string text, object classInstance)
    {
        outputText.Clear();
        currentWord.Clear();

        bool startedFormattedWord = false;

        foreach (char character in text)
        {
            if (character == '{')
            {
                startedFormattedWord = true;
            }
            else if (character == '}')
            {
                if (startedFormattedWord)
                {
                    wordTemp = currentWord.ToString();

                    object field = GetFieldValue(classInstance, wordTemp);
                    object property = GetPropertyValue(classInstance, wordTemp);

                    if (field != null)
                    {
                        outputText.Append(ValueToString(field));
                    }
                    else if (property != null)
                    {
                        outputText.Append(ValueToString(property));
                    }

                    currentWord.Clear();
                    startedFormattedWord = false;
                }
            }
            else if (character == ' ')
            {
                outputText.Append(currentWord);
                outputText.Append(' ');

                currentWord.Clear();
            }
            else
            {
                currentWord.Append(character);
            }
        }

        outputText.Append(currentWord);

        return outputText.ToString();
    }

    private static object GetFieldValue(object instance, string fieldName)
    {
        Type type = instance.GetType();
        FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

        if (fieldInfo != null)
        {
            return fieldInfo.GetValue(instance);
        }

        return null;
    }

    private static object GetPropertyValue(object instance, string propertyName)
    {
        Type type = instance.GetType();
        PropertyInfo propInfo = type.GetProperty(propertyName);

        if (propInfo != null)
        {
            return propInfo.GetValue(instance, null);
        }

        return null;
    }

    private static string ValueToString(object value)
    {
        if (value == null) return null;

        return value.ToString();
    }
}