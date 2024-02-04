using System;
using System.Collections.Generic;

namespace DTSchema;

public class CyclicTopoSortException : Exception
{
    public object Item;
    public CyclicTopoSortException(object item, string itemRepr) : base("Cyclic dependency detected: " + itemRepr)
    {
        Item = item;
    }
}

public static class Utils
{

    public static List<T> TopoSort<T>(List<T> source, Func<T, IEnumerable<T>> getDeps,
        bool ignoreCyclic = false, IEqualityComparer<T>? compare = null) where T : notnull
    {
        // Naive implementation of topological sort is enough for our use case.

        var result = new List<T>();
        HashSet<T> visited = compare == null ? new() : new(compare);

        // Temp is used to detect cycles
        HashSet<T> temp = compare == null ? new() : new(compare);

        foreach (var item in source)
        {
            _TopoSortVisit(item, getDeps, visited, temp, result, ignoreCyclic);
        }

        return result;

    }

    private static void _TopoSortVisit<T>(T item, Func<T, IEnumerable<T>> getDeps, HashSet<T> visited, HashSet<T> temp, List<T> result, bool ignoreCyclic) where T : notnull
    {
        if (temp.Contains(item))
        {
            if (ignoreCyclic)
                return;
            throw new CyclicTopoSortException((object)item, item.ToString() ?? (typeof(T).Name + " instance"));
        }

        if (!visited.Contains(item))
        {
            temp.Add(item);
            foreach (var dep in getDeps(item))
            {
                _TopoSortVisit(dep, getDeps, visited, temp, result, ignoreCyclic);
            }
            temp.Remove(item);
            visited.Add(item);
            result.Add(item);
        }
    }
}