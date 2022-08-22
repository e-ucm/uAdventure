using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

public static class UriHelper
{
    public static NameValueCollection DecodeQueryParameters(string query)
    {
        if (query == null)
            throw new ArgumentNullException("query");

        if (query.Length == 0)
            return new NameValueCollection();

        var dict =  query.TrimStart('?')
                        .Split(new[] { '&', ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(parameter => parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                        .GroupBy(parts => parts[0],
                                 parts => parts.Length > 2 ? string.Join("=", parts, 1, parts.Length - 1) : (parts.Length > 1 ? parts[1] : ""))
                        .ToDictionary(grouping => grouping.Key,
                                      grouping => string.Join(",", grouping));

        var nvc = new NameValueCollection();
        foreach(var kv in dict)
        {
            nvc.Add(kv.Key, kv.Value);
        }
        return nvc;
    }
}