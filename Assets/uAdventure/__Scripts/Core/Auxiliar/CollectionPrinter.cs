using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    public class CollectionPrinter
    {
        public static string PrintCollection<T>(IEnumerable<T> col)
        {
            string s = "\n";
            foreach (var item in col)
                s += item.ToString() + "\n";
            return s + "\n\n\n";
        }
    }
}
