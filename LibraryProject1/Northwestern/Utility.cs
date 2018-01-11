using System;
using System.Collections.Generic;
using System.Linq;

namespace TestLibNamespace.Northwestern
{
    public static class Utility
    {
        /// <summary>
        /// Returns a subsequence of a sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">A sequence of values.</param>
        /// <param name="start">Inclusive start index (zero-based). </param>
        /// <param name="end">Inclusive end index (zero-based).</param>
        /// <returns>The subsequence.</returns>
        public static IEnumerable<T> SubSequence<T>(this IEnumerable<T> source, int start, int end)
        {
            return source.Skip(start).Take(end - start + 1);
        }

        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <typeparam name="TOut">The type of the value returned by selector.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An IEnumerable whose elements are the result of invoking the transform function on each element of source.</returns>
        public static TOut[] Select<T, TOut>(this IList<T> source, Func<T, TOut> selector)
        {
            TOut[] result = new TOut[source.Count];

            for (int i = 0; i < source.Count; i++)
            {
                result[i] = selector(source[i]);
            }

            return result;
        }

        private const double Proton = 1.007276466;

        /// <summary>
        /// Converts the m/z to mass.
        /// </summary>
        /// <param name="mz">The m/z.</param>
        /// <param name="charge">The charge.</param>
        /// <param name="positiveCharge">if set to <c>true</c> [positive charge].</param>
        /// <returns></returns>
        public static double ConvertMzToMass(double mz, int charge, bool positiveCharge = true)
        {
            if (positiveCharge)
            {
                return charge * (mz - Proton);
            }

            return charge * (mz + Proton);
        }

        /// <summary>
        /// Converts the mass to m/z.
        /// </summary>
        /// <param name="mass">The mass.</param>
        /// <param name="charge">The charge.</param>
        /// <param name="positiveCharge">if set to <c>true</c> [positive charge].</param>
        /// <returns></returns>
        public static double ConvertMassToMz(double mass, int charge, bool positiveCharge = true)
        {
            if (positiveCharge)
            {
                return mass / charge + Proton;
            }

            return mass / charge - Proton;
        }
    }
}