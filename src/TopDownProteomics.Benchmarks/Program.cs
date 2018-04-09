using UWMadison.Chemistry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TestLibNamespace.Northwestern;
using UsefulProteomicsDatabases;

namespace TopDownProteomics.Benchmarks
{
    internal class Program
    {
        #region Private Methods

        private static void Main(string[] args)
        {
            PeriodicTableLoader.Load("elements.dat");

            BenchmarkIsotopicEvelopeGeneration();
        }

        private static void BenchmarkIsotopicEvelopeGeneration()
        {
            // Generate the formulas

            Random random = new Random(1);

            ChemicalFormula[] chemicalFormulas = new ChemicalFormula[1000];
            IChemicalFormula[] northwesternChemicalFormulas = new IChemicalFormula[1000];

            for (int i = 2; i < 1000; i++)
            {
                ChemicalFormula chemicalFormula = new ChemicalFormula();
                chemicalFormula.Add(PeriodicTable.GetElement("H"), (int)(random.Next(i) * 7.7583));
                chemicalFormula.Add(PeriodicTable.GetElement("C"), (int)(random.Next(i) * 4.9384));
                chemicalFormula.Add(PeriodicTable.GetElement("N"), (int)(random.Next(i) * 1.3577));
                chemicalFormula.Add(PeriodicTable.GetElement("O"), (int)(random.Next(i) * 1.4773));
                chemicalFormula.Add(PeriodicTable.GetElement("S"), (int)(random.Next(i) * 0.0417));

                //Console.WriteLine(chemicalFormula.Formula);

                chemicalFormulas[i] = chemicalFormula;

                IChemicalFormula ff = new NorthwesternChemicalFormula(chemicalFormulas[i]);
                northwesternChemicalFormulas[i] = ff;
            }

            Stopwatch stopwatch = new Stopwatch();

            // Time UW Madison

            stopwatch.Start();
            for (int i = 2; i < 1000; i++)
            {
                var nice = UWMadison.Chemistry.IsotopicDistribution.GetDistribution(chemicalFormulas[i], 0.2, 1E-26);
            }
            stopwatch.Stop();

            Console.WriteLine("Elapsed time for UofW Madison Isotopic Distribution Generation: " + stopwatch.Elapsed);

            // Time Northwestern

            Mercury7 mercury7 = new Mercury7();
            stopwatch.Reset();
            stopwatch.Start();
            for (int i = 2; i < 1000; i++)
            {
                var nice = mercury7.GenerateIsotopicDistribution(northwesternChemicalFormulas[i]);
            }
            stopwatch.Stop();

            Console.WriteLine("Elapsed time for Northwestern Isotopic Distribution Generation: " + stopwatch.Elapsed);
        }

        #endregion Private Methods
    }

    internal class NorthwesternChemicalFormula : IChemicalFormula
    {
        #region Private Fields

        private static Dictionary<Element, IElement> ElementDict = new Dictionary<Element, IElement>
            {
                { PeriodicTable.GetElement("H"), new NorthwesternElement(PeriodicTable.GetElement("H")) },
                { PeriodicTable.GetElement("C"), new NorthwesternElement(PeriodicTable.GetElement("C")) },
                { PeriodicTable.GetElement("N"), new NorthwesternElement(PeriodicTable.GetElement("N")) },
                { PeriodicTable.GetElement("O"), new NorthwesternElement(PeriodicTable.GetElement("O")) },
                { PeriodicTable.GetElement("S"), new NorthwesternElement(PeriodicTable.GetElement("S")) }
            };

        private Dictionary<IElement, int> elements;

        #endregion Private Fields

        #region Public Constructors

        public NorthwesternChemicalFormula(ChemicalFormula chemicalFormula)
        {
            elements = chemicalFormula.Elements.ToDictionary(b => ElementDict[b.Key], b => b.Value);
        }

        #endregion Public Constructors

        #region Public Properties

        public IEnumerable<IElementCount> ElementCounts => elements.Select(b => new NorthwesternElementCount(b.Key, b.Value));

        #endregion Public Properties

        #region Public Indexers

        public int this[IElement element] => throw new NotImplementedException();

        #endregion Public Indexers

        #region Public Methods

        public IChemicalFormula Add(IChemicalFormula formula)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IElement element)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IChemicalFormula other)
        {
            throw new NotImplementedException();
        }

        public double GetMass(MassType massType)
        {
            throw new NotImplementedException();
        }

        public IChemicalFormula Multiply(int multiplier)
        {
            throw new NotImplementedException();
        }

        public IChemicalFormula Subtract(IChemicalFormula formula)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }

    internal class NorthwesternElement : IElement
    {
        #region Private Fields

        private IList<IIsotope> isotopes;

        #endregion Private Fields

        #region Public Constructors

        public NorthwesternElement(Element element)
        {
            isotopes = element.Isotopes.Select(b => (IIsotope)new NorthwesternIsotope(b)).ToList();
        }

        #endregion Public Constructors

        #region Public Properties

        public int AtomicNumber => throw new NotImplementedException();

        public string Symbol => throw new NotImplementedException();

        public IList<IIsotope> Isotopes => isotopes;

        public string Name => throw new NotImplementedException();

        #endregion Public Properties

        #region Public Methods

        public double GetMass(MassType massType)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }

    internal class NorthwesternIsotope : IIsotope
    {
        #region Private Fields

        private double atomicMass;
        private double abundance;

        #endregion Private Fields

        #region Public Constructors

        public NorthwesternIsotope(Isotope b)
        {
            atomicMass = b.AtomicMass;
            abundance = b.RelativeAbundance;
        }

        #endregion Public Constructors

        #region Public Properties

        public double AtomicMass => atomicMass;

        public double Abundance => abundance;

        #endregion Public Properties
    }

    internal class NorthwesternElementCount : IElementCount
    {
        #region Private Fields

        private IElement key;
        private int value;

        #endregion Private Fields

        #region Public Constructors

        public NorthwesternElementCount(IElement key, int value)
        {
            this.key = key;
            this.value = value;
        }

        #endregion Public Constructors

        #region Public Properties

        public IElement Element => key;

        public int Count => value;

        #endregion Public Properties
    }
}