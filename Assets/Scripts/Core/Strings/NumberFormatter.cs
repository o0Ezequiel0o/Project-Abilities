using System.Text;

public static class NumberFormatter
{
    private static StringBuilder cappedText = new StringBuilder(16);
    private static StringBuilder text = new StringBuilder(16);

    private static float Hundred => 100f;
    private static float Thousand => 1000f;
    private static float Million => 1000000f;
    private static float Billion => 1000000000f;

    private static int maxLength = 3;

    public static string FormatNumber(float number)
    {
        text.Clear();

        if (number < Thousand)
        {
            text.Append(number);
            return CapLength(text).ToString();
        }
        else if (number < Million)
        {
            text.Append(number / Thousand);
            return CapLength(text).Append("k").ToString();
        }
        else
        {
            text.Append(number / Million);
            return CapLength(text).Append("m").ToString();
        }
    }

    private static StringBuilder CapLength(StringBuilder stringBuilder)
    {
        cappedText.Clear();

        int maxLoops = maxLength;

        for (int i = 0; i < stringBuilder.Length; i++)
        {
            if (i >= maxLoops) break;

            char character = stringBuilder[i];

            cappedText.Append(character);

            if (character == ',' || character == '.')
            {
                maxLoops += 1;
            }
        }

        return cappedText;
    }
}