using System;
using System.Diagnostics;
using System.Linq;
using TopDownProteomics.Chemistry;
using TopDownProteomics.Tools;
using UsefulProteomicsDatabases;
using UWMadison.Chemistry;

namespace TopDownProteomics.Benchmarks
{
    internal class Program
    {
        private const int MaxRunValue = 1000;

        private static void Main(string[] args)
        {
            PeriodicTableLoader.Load("elements.dat");

            BenchmarkIsotopicEvelopeGeneration();
        }

        private static void BenchmarkIsotopicEvelopeGeneration()
        {
            // Generate the formulas
            var random = new Random(1);

            var uwChemicalFormulas = new UWMadison.Chemistry.ChemicalFormula[MaxRunValue];
            var chemicalFormulas = new IChemicalFormula[MaxRunValue];
            IElementProvider elementProvider = new MockElementProvider();

            for (int i = 2; i < MaxRunValue; i++)
            {
                var chemicalFormula = new UWMadison.Chemistry.ChemicalFormula();
                chemicalFormula.Add(PeriodicTable.GetElement("H"), (int)(random.Next(i) * 7.7583));
                chemicalFormula.Add(PeriodicTable.GetElement("C"), (int)(random.Next(i) * 4.9384));
                chemicalFormula.Add(PeriodicTable.GetElement("N"), (int)(random.Next(i) * 1.3577));
                chemicalFormula.Add(PeriodicTable.GetElement("O"), (int)(random.Next(i) * 1.4773));
                chemicalFormula.Add(PeriodicTable.GetElement("S"), (int)(random.Next(i) * 0.0417));

                //Console.WriteLine(chemicalFormula.Formula);

                uwChemicalFormulas[i] = chemicalFormula;

                var tdp_ff = new Chemistry.ChemicalFormula(chemicalFormula.Elements.Select(x => 
                    new EntityCardinality<IElement>(elementProvider.GetElement(x.Key.AtomicNumber), x.Value)));
                chemicalFormulas[i] = tdp_ff;
            }

            var stopwatch = new Stopwatch();

            // Time UW Madison
            stopwatch.Start();
            for (int i = 2; i < MaxRunValue; i++)
            {
                var nice = IsotopicDistribution.GetDistribution(uwChemicalFormulas[i], 0.2, 1E-26);
            }
            stopwatch.Stop();

            Console.WriteLine("Elapsed time for UofW Madison Original: " + stopwatch.Elapsed);

            // Time UW Madison (port)
            var fineGrain = new FineStructureIsotopicGenerator();
            stopwatch.Reset();
            stopwatch.Start();
            for (int i = 2; i < MaxRunValue; i++)
            {
                var nice = fineGrain.GenerateIsotopicDistribution(chemicalFormulas[i], 0.2, 1E-26);
            }
            stopwatch.Stop();

            Console.WriteLine("Elapsed time for UofW Madison Port: " + stopwatch.Elapsed);

            // Time Northwestern (port)
            var mercury7 = new Mercury7();
            stopwatch.Reset();
            stopwatch.Start();
            for (int i = 2; i < MaxRunValue; i++)
            {
                MassSpectrometry.IIsotopicDistribution nice = mercury7.GenerateIsotopicDistribution(chemicalFormulas[i]);
            }
            stopwatch.Stop();

            Console.WriteLine("Elapsed time for Northwestern Port: " + stopwatch.Elapsed);
        }
    }
}