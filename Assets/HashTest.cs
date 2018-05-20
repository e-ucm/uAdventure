using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using System.Runtime.InteropServices;

public static class HashTest {

    [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
    static extern int memcmp(byte[] b1, byte[] b2, long count);

    [MenuItem("uAdventure/TestHash")]
    public static void UnHash()
    {
        byte[] hash;
        string lastDigits = string.Empty;

        byte[] data = "73-26-9f-69-d4-15-51-7d-86-1f-dc-3a-a2-4d-06-f6-28-92-af-e8".Split('-').Select(b => Convert.ToByte(b, 16)).ToArray();
        using (var sha1 = new SHA1Managed())
        {
            EditorUtility.DisplayCancelableProgressBar("DeHashing", "Starting! ", 0f);
            for (int i = 0; i < 1679615; i++)
            {
                lastDigits = ToA9Fill(i, 4);
                if (i % 1000 == 0 && EditorUtility.DisplayCancelableProgressBar("DeHashing", "Dehashing... " + lastDigits, (float)i / 1679615))
                { 
                    EditorUtility.ClearProgressBar();
                    break;
                }
                hash = sha1.ComputeHash(Encoding.UTF8.GetBytes("F4-E8VC-VH3H-JACY-D9VQ-" + lastDigits));
                if (memcmp(hash, data, hash.Length) == 0)
                {
                    EditorUtility.DisplayDialog("code", "Found! " + ToA9Fill(i, 4), "OK");
                }
            }
        }
        EditorUtility.ClearProgressBar();
    }

    private static string ToA9Fill(int i, int v)
    {
        var a9 = ToA9(i);
        while(a9.Length < v)
        {
            a9 = "0" + a9;
        }
        return a9;
    }

    // 01234 56789 ABCDE FGHIJ KLMNO PQRST UVWXY Z
    private static string ToA9(int number)
    {
        if (number < 36)
        {
            switch (number)
            {
                default:
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    return number.ToString();
                case 10: return "A";
                case 11: return "B";
                case 12: return "C";
                case 13: return "D";
                case 14: return "E";
                case 15: return "F";
                case 16: return "G";
                case 17: return "H";
                case 18: return "I";
                case 19: return "J";
                case 20: return "K";
                case 21: return "L";
                case 22: return "M";
                case 23: return "N";
                case 24: return "O";
                case 25: return "P";
                case 26: return "Q";
                case 27: return "R";
                case 28: return "S";
                case 29: return "T";
                case 30: return "U";
                case 31: return "V";
                case 32: return "W";
                case 33: return "X";
                case 34: return "Y";
                case 35: return "Z";
            }
        }
        else
        {
            return ToA9(number / 36) + ToA9(number % 36);
        }
    }
}
