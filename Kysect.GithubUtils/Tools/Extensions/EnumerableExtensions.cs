namespace Kysect.GithubUtils.Tools.Extensions;

public static class EnumerableExtensions
{
    public static (IReadOnlyCollection<T> Mathced, IReadOnlyCollection<T> NotMatched) SplitBy<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        List<T> first = new List<T>();
        List<T> second = new List<T>();
        foreach (T item in source)
        {
            if (predicate(item))
            {
                first.Add(item);
            }
            else
            {
                second.Add(item);
            }
        }
        return (first, second);
    }
}