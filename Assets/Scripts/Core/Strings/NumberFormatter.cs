using System.Text;

public static class NumberFormatter
{
    private static StringBuilder cappedText = new StringBuilder(16);
    private static StringBuilder commaText = new StringBuilder(16);
    private static StringBuilder text = new StringBuilder(16);

    private static float Hundred => 100f;
    private static float Thousand => 1000f;
    private static float Million => 1000000f;
    private static float Billion => 1000000000f;

    private const int DEFAULT_MAX_LENGTH = 3;

    public static string FormatNumber(float number)
    {
        return FormatNumber(number, DEFAULT_MAX_LENGTH);
    }

    public static string FormatNumber(float number, int maxLength)
    {
        text.Clear();

        if (number < Thousand)
        {
            text.Append(number);
            return CapLength(text, maxLength).ToString();
        }
        else if (number < Million)
        {
            text.Append(number / Thousand);
            return CapLength(text, maxLength).Append("k").ToString();
        }
        else if (number < Billion)
        {
            text.Append(number / Million);
            return CapLength(text, maxLength).Append("m").ToString();
        }
        else
        {
            text.Append(number / Billion);
            return CapLength(text, maxLength).Append("b").ToString();
        }
    }

    private static StringBuilder CapLength(StringBuilder stringBuilder, int maxLength)
    {
        cappedText.Clear();
        commaText.Clear();

        bool commaValue = false;
        bool commaValuesZero = true;

        int maxLoops = maxLength;

        for (int i = 0; i < stringBuilder.Length; i++)
        {
            if (i >= maxLoops) break;

            char character = stringBuilder[i];

            if (char.IsNumber(character))
            {
                if (!commaValue)
                {
                    cappedText.Append(character);
                }
                else
                {
                    commaText.Append(character);

                    if (character != '0')
                    {
                        commaValuesZero = false;
                    }
                }
            }
            else if (character == ',' || character == '.' && !commaValue)
            {
                maxLoops += 1;
                commaValue = true;
                commaText.Append(character);
            }
        }

        if (!commaValuesZero)
        {
            cappedText.Append(commaText);
        }

        return cappedText;
    }
}