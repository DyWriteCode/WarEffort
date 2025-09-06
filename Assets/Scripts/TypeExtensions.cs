using System;
using System.Collections.Generic;
using System.Linq;

public static class TypeExtensions
{
    public static T[] Slice<T>(this T[] array, int start, int end)
    {
        // 处理负索引
        if (start < 0) start = array.Length + start;
        if (end < 0) end = array.Length + end;

        // 确保索引在有效范围内
        start = Math.Max(0, Math.Min(start, array.Length));
        end = Math.Max(0, Math.Min(end, array.Length));

        if (start >= end) return new T[0];

        int length = end - start;
        T[] result = new T[length];
        Array.Copy(array, start, result, 0, length);
        return result;
    }

    public static T[] Slice<T>(this T[] array, int start)
    {
        return array.Slice(start, array.Length);
    }

    public static T[] Slice<T>(this T[] array, Index startIndex, Index endIndex)
    {
        int start = startIndex.IsFromEnd ? array.Length - startIndex.Value : startIndex.Value;
        int end = endIndex.IsFromEnd ? array.Length - endIndex.Value : endIndex.Value;
        return array.Slice(start, end);
    }

    public static List<T> Slice<T>(this List<T> list, int start, int end)
    {
        // 处理负索引
        if (start < 0) start = list.Count + start;
        if (end < 0) end = list.Count + end;

        // 确保索引在有效范围内
        start = Math.Max(0, Math.Min(start, list.Count));
        end = Math.Max(0, Math.Min(end, list.Count));

        if (start >= end) return new List<T>();

        return list.GetRange(start, end - start);
    }

    public static List<T> Slice<T>(this List<T> list, int start)
    {
        return list.Slice(start, list.Count);
    }
}