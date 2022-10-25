namespace TopDownProteomics.Biochemistry;

/// <summary>Anything for which a glycan composition can be calculated.</summary>
public interface IHasGlycanComposition
{
    /// <summary>Gets the glycan composition.</summary>
    /// <returns></returns>
    IGlycanComposition GetGlycanComposition();
}