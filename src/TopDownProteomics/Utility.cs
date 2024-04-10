using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TopDownProteomics
{
    /// <summary>
    /// Northwestern Utility functions.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Lazy creates the collection if needed and adds the item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static void LazyCreateAndAdd<T>(ref ICollection<T>? list, T item)
        {
            if (list == null)
                list = new List<T> { item };
            else
                list.Add(item);
        }

        /// <summary>
        /// Lazy creates the collection if needed and adds the item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static void LazyCreateAndAdd<T>(ref ICollection<T>? list, IEnumerable<T> items)
        {
            if (list == null)
            {
                list = new List<T>(items);
            }
            else
            {
                foreach (var item in items)
                    list.Add(item);
            }
        }

        /// <summary>
        /// Finds the first element that maximizes a given function.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The first element that maximizes the function.</returns>
        public static T FirstWithMax<T>(this IEnumerable<T> source, Func<T, double> selector)
        {
            using (IEnumerator<T> iterator = source.GetEnumerator())
            {
                if (!iterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence has no elements.");
                }

                T withMax = iterator.Current;
                double max = selector(withMax);

                while (iterator.MoveNext())
                {
                    double val = selector(iterator.Current);

                    if (val > max)
                    {
                        max = val;
                        withMax = iterator.Current;
                    }
                }

                return withMax;
            }
        }

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

        /// <summary>The mass of a proton</summary>
        public const double Proton = 1.007276466;

        /// <summary>
        /// Converts the m/z to mass.
        /// </summary>
        /// <param name="mz">The m/z.</param>
        /// <param name="charge">The charge.</param>
        /// <param name="positiveCharge">if set to <c>true</c> [positive charge].</param>
        /// <returns></returns>
        [Obsolete("Use ConvertMzToMass(double, int, double) instead.")]
        public static double ConvertMzToMass(double mz, int charge, bool positiveCharge)
        {
            Debug.Assert(charge > 0, "Charge must be greater than 0.");

            if (positiveCharge)
            {
                return charge * (mz - Proton);
            }

            return charge * (mz + Proton);
        }

        /// <summary>
        /// Converts the m/z to mass.
        /// </summary>
        /// <param name="mz">The m/z.</param>
        /// <param name="charge">The charge.</param>
        /// <param name="chargeCarrier">The charge carrier. Default value is Proton.</param>
        /// <returns>The mass.</returns>
        public static double ConvertMzToMass(double mz, int charge, double chargeCarrier = Proton)
        {
            Debug.Assert(chargeCarrier > 0, "Charge carrier must be greater than 0.");

            if (charge > 0)
                return charge * (mz - chargeCarrier);

            return -charge * (mz + chargeCarrier);
        }

        /// <summary>
        /// Converts the mass to m/z.
        /// </summary>
        /// <param name="mass">The mass.</param>
        /// <param name="charge">The charge.</param>
        /// <param name="positiveCharge">if set to <c>true</c> [positive charge].</param>
        /// <returns></returns>
        [Obsolete("Use ConvertMassToMz(double, int, double) instead.")]
        public static double ConvertMassToMz(double mass, int charge, bool positiveCharge)
        {
            Debug.Assert(charge > 0, "Charge must be greater than 0.");

            if (positiveCharge)
                return mass / charge + Proton;

            return mass / charge - Proton;
        }

        /// <summary>
        /// Converts the mass to m/z.
        /// </summary>
        /// <param name="mass">The mass.</param>
        /// <param name="charge">The charge.</param>
        /// <param name="chargeCarrier">The charge carrier. Default value is Proton.</param>
        /// <returns></returns>
        public static double ConvertMassToMz(double mass, int charge, double chargeCarrier = Proton)
        {
            Debug.Assert(chargeCarrier > 0, "Charge carrier must be greater than 0.");

            if (charge > 0)
                return mass / charge + chargeCarrier;

            return mass / -charge - chargeCarrier;
        }

#if !NETSTANDARD2_1
        /// <summary>
        /// Split implementation that takes a single char.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="separator"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string[] Split(this string input, char separator, StringSplitOptions options)
        {
            return input.Split(new[] { separator }, options);
        }

        /// <summary>
        /// Split implementation that takes a single string.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="separator"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string[] Split(this string input, string separator, StringSplitOptions options)
        {
            return input.Split(new[] { separator }, options);
        }
#endif
    }
}