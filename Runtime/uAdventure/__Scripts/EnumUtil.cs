using System;

public static class EnumUtil {
    
    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static T ToEnum<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static T ToEnum<T>(this string value, T defaultValue)
    {
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }

        T result;

        try { result = (T)Enum.Parse(typeof(T), value, true); }
        catch { result = defaultValue; }

        return  result;
    }
}
