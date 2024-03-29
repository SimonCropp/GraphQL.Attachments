﻿using System.Text.RegularExpressions;

static class Compress
{
    public static string Query(string query)
    {
        query = Regex.Replace(query, @"\s+", " ");
        return Regex.Replace(query, @"\s*(\[|\]|\{|\}|\(|\)|:|\,)\s*", "$1");
    }
}