using TopDownProteomics.Chemistry;

namespace TopDownProteomics.Tests
{
    public class MockElementProvider : InMemoryElementProvider
    {
        public MockElementProvider() : base(GetMockElements()) { }

        private static IElement[] GetMockElements()
        {
            var elements = new IElement[128];
            elements[1] = new Element(1, "H", new[]
            {
                new Isotope(1.007825032, 0, 0.999885),
                new Isotope(2.014101778, 1, 0.000115)
            });
            elements[2] = new Element(2, "He", new[]
            {
                new Isotope(3.0160293191, 1, 1.34e-06),
                new Isotope(4.00260325415, 2, 0.99999866)
            });
            elements[5] = new Element(5, "B", new[]
            {
                new Isotope(10.01293695, 5, 0.199),
                new Isotope(11.00930536, 6, 0.801)
            });
            elements[6] = new Element(6, "C", new[]
            {
                new Isotope(12.0, 6, 0.9893),
                new Isotope(13.0033548378, 7, 0.0107)
            });
            elements[7] = new Element(7, "N", new[]
            {
                new Isotope(14.0030740052, 7, 0.99632),
                new Isotope(15.0001088984, 8, 0.00368)
            });
            elements[8] = new Element(8, "O", new[]
            {
                new Isotope(15.99491463, 8, 0.99757),
                new Isotope(16.9991312, 9, 0.00038),
                new Isotope(17.9991603, 10, 0.00205)
            });
            elements[15] = new Element(15, "P", new[]
            {
                new Isotope(30.97376163, 16, 1.0000)
            });
            elements[16] = new Element(16, "S", new[]
            {
                new Isotope(31.97207100, 16, 0.9499),
                new Isotope(32.97145876, 17, 0.0075),
                new Isotope(33.96786690, 18, 0.0425),
                new Isotope(35.96708076, 20, 0.0001)
            });
            elements[34] = new Element(34, "Se", new[]
            {
                new Isotope(73.9224764, 40, 0.0089),
                new Isotope(75.9192136, 42, 0.0937),
                new Isotope(76.9199140, 43, 0.0763),
                new Isotope(77.9173091, 44, 0.2377),
                new Isotope(79.9165213, 46, 0.4961),
                new Isotope(81.9166994, 48, 0.0873)
            });

            return elements;
        }
    }
}