using System.Collections.Generic;
using System.Linq;
using TPMHelper.AfostoHelper.Algorithms.Helpers;

namespace TPMHelper.AfostoHelper.Algorithms
{
    public class GAPCGAlgorithm : IGAPCG
    {
        public List<List<T>> GAPCG<T>(List<List<T>> objects)
        {
            IEnumerable<List<T>> combos = new List<List<T>>() { new List<T>() };

            foreach (var inner in objects)
            {
                combos = combos.SelectMany(r => inner.Select(x =>
                {
                    var n = r.DeepClone();
                    if (x != null)
                    {
                        n.Add(x);
                    }
                    return n;

                }).ToList());
            }

            return combos.Where(c => c.Count > 0).ToList();
        }
    }
}
