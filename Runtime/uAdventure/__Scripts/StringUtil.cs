using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringUtil {

    public static string RemoveFromEnd(this string s, string suffix)
    {
        if (s.EndsWith(suffix))
        {
            return s.Substring(0, s.Length - suffix.Length);
        }
        else
        {
            return s;
        }
    }
}
