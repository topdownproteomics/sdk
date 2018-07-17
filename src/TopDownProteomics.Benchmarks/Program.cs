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
            //var northwesternChemicalFormulas = new TestLibNamespace.Northwestern.IChemicalFormula[MaxRunValue];
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

                //TestLibNamespace.Northwestern.IChemicalFormula ff = new NorthwesternChemicalFormula(uwChemicalFormulas[i]);
                //northwesternChemicalFormulas[i] = ff;

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

            Console.WriteLine("Elapsed time for UofW Madison Isotopic Distribution Generation: " + stopwatch.Elapsed);

            // Time Northwestern OLD
            //var mercury7_OLD = new Mercury7_OLD();
            //stopwatch.Reset();
            //stopwatch.Start();
            //for (int i = 2; i < MaxRunValue; i++)
            //{
            //    MassSpectrometry.IIsotopicDistribution nice = mercury7_OLD.GenerateIsotopicDistribution(northwesternChemicalFormulas[i]);
            //}
            //stopwatch.Stop();

            //Console.WriteLine("Elapsed time for Northwestern OLD Isotopic Distribution Generation: " + stopwatch.Elapsed);

            // Time Northwestern NEW
            var mercury7 = new Mercury7();
            stopwatch.Reset();
            stopwatch.Start();
            for (int i = 2; i < MaxRunValue; i++)
            {
                MassSpectrometry.IIsotopicDistribution nice = mercury7.GenerateIsotopicDistribution(chemicalFormulas[i]);
            }
            stopwatch.Stop();

            Console.WriteLine("Elapsed time for Northwestern NEW Isotopic Distribution Generation: " + stopwatch.Elapsed);
        }
    }

    //internal class NorthwesternChemicalFormula : TestLibNamespace.Northwestern.IChemicalFormula
    //{
    //    #region Private Fields

    //    private static Dictionary<UWMadison.Chemistry.Element, TestLibNamespace.Northwestern.IElement> ElementDict = new Dictionary<UWMadison.Chemistry.Element, TestLibNamespace.Northwestern.IElement>
    //        {
    //            { PeriodicTable.GetElement("H"), new NorthwesternElement(PeriodicTable.GetElement("H")) },
    //            { PeriodicTable.GetElement("C"), new NorthwesternElement(PeriodicTable.GetElement("C")) },
    //            { PeriodicTable.GetElement("N"), new NorthwesternElement(PeriodicTable.GetElement("N")) },
    //            { PeriodicTable.GetElement("O"), new NorthwesternElement(PeriodicTable.GetElement("O")) },
    //            { PeriodicTable.GetElement("S"), new NorthwesternElement(PeriodicTable.GetElement("S")) }
    //        };

    //    private Dictionary<TestLibNamespace.Northwestern.IElement, int> elements;

    //    #endregion Private Fields

    //    #region Public Constructors

    //    public NorthwesternChemicalFormula(UWMadison.Chemistry.ChemicalFormula chemicalFormula)
    //    {
    //        elements = chemicalFormula.Elements.ToDictionary(b => ElementDict[b.Key], b => b.Value);
    //    }

    //    #endregion Public Constructors

    //    #region Public Properties

    //    public IEnumerable<IElementCount> ElementCounts => elements.Select(b => new NorthwesternElementCount(b.Key, b.Value));

    //    #endregion Public Properties

    //    #region Public Indexers

    //    public int this[TestLibNamespace.Northwestern.IElement element] => throw new NotImplementedException();

    //    #endregion Public Indexers

    //    #region Public Methods

    //    public TestLibNamespace.Northwestern.IChemicalFormula Add(TestLibNamespace.Northwestern.IChemicalFormula formula)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool Contains(TestLibNamespace.Northwestern.IElement element)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool Equals(TestLibNamespace.Northwestern.IChemicalFormula other)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public double GetMass(TestLibNamespace.Northwestern.MassType massType)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public TestLibNamespace.Northwestern.IChemicalFormula Multiply(int multiplier)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public TestLibNamespace.Northwestern.IChemicalFormula Subtract(TestLibNamespace.Northwestern.IChemicalFormula formula)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion Public Methods
    //}

    //internal class NorthwesternElement : TestLibNamespace.Northwestern.IElement
    //{
    //    #region Private Fields

    //    private IList<TestLibNamespace.Northwestern.IIsotope> isotopes;

    //    #endregion Private Fields

    //    #region Public Constructors

    //    public NorthwesternElement(UWMadison.Chemistry.Element element)
    //    {
    //        isotopes = element.Isotopes.Select(b => (TestLibNamespace.Northwestern.IIsotope)new NorthwesternIsotope(b)).ToList();
    //    }

    //    #endregion Public Constructors

    //    #region Public Properties

    //    public int AtomicNumber => throw new NotImplementedException();

    //    public string Symbol => throw new NotImplementedException();

    //    public IList<TestLibNamespace.Northwestern.IIsotope> Isotopes => isotopes;

    //    public string Name => throw new NotImplementedException();

    //    #endregion Public Properties

    //    #region Public Methods

    //    public double GetMass(TestLibNamespace.Northwestern.MassType massType)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion Public Methods
    //}

    //internal class NorthwesternIsotope : TestLibNamespace.Northwestern.IIsotope
    //{
    //    #region Private Fields

    //    private double atomicMass;
    //    private double abundance;

    //    #endregion Private Fields

    //    #region Public Constructors

    //    public NorthwesternIsotope(UWMadison.Chemistry.Isotope b)
    //    {
    //        atomicMass = b.AtomicMass;
    //        abundance = b.RelativeAbundance;
    //    }

    //    #endregion Public Constructors

    //    #region Public Properties

    //    public double AtomicMass => atomicMass;

    //    public double Abundance => abundance;

    //    #endregion Public Properties
    //}

    //internal class NorthwesternElementCount : IElementCount
    //{
    //    #region Private Fields

    //    private TestLibNamespace.Northwestern.IElement key;
    //    private int value;

    //    #endregion Private Fields

    //    #region Public Constructors

    //    public NorthwesternElementCount(TestLibNamespace.Northwestern.IElement key, int value)
    //    {
    //        this.key = key;
    //        this.value = value;
    //    }

    //    #endregion Public Constructors

    //    #region Public Properties

    //    public TestLibNamespace.Northwestern.IElement Element => key;

    //    public int Count => value;

    //    #endregion Public Properties
    //}
}