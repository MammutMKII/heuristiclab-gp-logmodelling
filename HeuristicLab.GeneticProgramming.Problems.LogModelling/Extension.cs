using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.LogModelling
{
  public static class Extension
  {
    public static T Identity<T>(T x) => x;

    public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> source)
    {
      return PermuteRec(source);
    }

    private static IEnumerable<IEnumerable<T>> PermuteRec<T>(IEnumerable<T> source)
    {
      var count = source.Count();
      if(count == 1)
        yield return new List<T> { source.Single() };

      for(int i = 0; i < source.Count(); i++)
      {
        var item = source.ElementAt(i);
        var withoutItem = source.Take(i).Concat(source.Skip(i + 1));
        var permutationsOfSource = PermuteRec(withoutItem).Select(subPermutation => subPermutation.Prepend(item));

        foreach(var permutation in permutationsOfSource)
          yield return permutation;
      }
    }
  }
}
