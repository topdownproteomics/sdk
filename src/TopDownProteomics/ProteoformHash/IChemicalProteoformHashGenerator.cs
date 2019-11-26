using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.ProteoformHash
{
    interface IChemicalProteoformHashGenerator
    {
        IChemicalProteoformHash Generate(string proForma);
    }
}
