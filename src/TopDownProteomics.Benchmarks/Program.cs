using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TopDownProteomics.Biochemistry;
using TopDownProteomics.Chemistry;
using TopDownProteomics.Tools;

namespace TopDownProteomics.Benchmarks
{
    internal class Program
    {
        private const int MaxRunValue = 2000;

        private static void Main(string[] args)
        {
            //BenchmarkIsotopicEnvelopeGeneration();
            BenchmarkChemicalFormulaAsKey();
        }

        private static void BenchmarkIsotopicEnvelopeGeneration()
        {
            // Generate the formulas
            var random = new Random(1);

            var chemicalFormulas = new ChemicalFormula[MaxRunValue];
            IElementProvider elementProvider = new MockElementProvider();

            for (int i = 2; i < MaxRunValue; i++)
            {
                var elements = new[]
                {
                    new EntityCardinality<IElement>(elementProvider.GetElement("H"), (int)(random.Next(i) * 7.7583)),
                    new EntityCardinality<IElement>(elementProvider.GetElement("C"), (int)(random.Next(i) * 4.9384)),
                    new EntityCardinality<IElement>(elementProvider.GetElement("N"), (int)(random.Next(i) * 1.3577)),
                    new EntityCardinality<IElement>(elementProvider.GetElement("O"), (int)(random.Next(i) * 1.4773)),
                    new EntityCardinality<IElement>(elementProvider.GetElement("S"), (int)(random.Next(i) * 0.0417)),
                };

                chemicalFormulas[i] = new ChemicalFormula(elements);
            }

            var stopwatch = new Stopwatch();

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

        private static void BenchmarkChemicalFormulaAsKey()
        {
            IElementProvider elementProvider = new MockElementProvider();
            IResidueProvider residueProvider = new IupacAminoAcidProvider(elementProvider);

            string sequence = "MLTELEKALNSIIDVYHKYSLIKGNFHAVYRDDLKKLLETECPQYIRKKGADVWFKELDINTDGAVNFQEFLILVIKMGVAAHKKSHEESHKE";
            var residues = sequence.Select(x => residueProvider.GetResidue(x));
            ChemicalFormula[] formulas = residues.Select(x => x.GetChemicalFormula()).ToArray();

            Dictionary<ChemicalFormula, bool> dictionary = new();
            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 9_000; i++)
            {
                for (int j = 0; j < formulas.Length; j++)
                {
                    if (!dictionary.ContainsKey(formulas[j]))
                        dictionary.Add(formulas[j], true);
                }
            }

            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
        }
    }
}