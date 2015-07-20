using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountdownPlayer
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            foreach (var o in self)
                action(o);
        }
    }
}
