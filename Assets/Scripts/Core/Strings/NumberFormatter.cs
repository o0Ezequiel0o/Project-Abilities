public static class NumberFormatter
{
    public static string FormatNumber(float number)
    {
        if (number >= 999f)
            return (number / 1000f).ToString() + "k";

        return number.ToString();
    }
}