# TopDown SDK
The TopDown Software Development Kit (SDK) is the Consortium for Top-Down Proteomics' open-source software solution for common top-down proteomics tasks.

[![codecov](https://codecov.io/gh/topdownproteomics/sdk/branch/master/graph/badge.svg?token=T8Kf00TGxl)](https://codecov.io/gh/topdownproteomics/sdk)

## Nuget Installation
Install [topdown-sdk](https://www.nuget.org/packages/TopDownProteomics) from nuget.

## Usage
#### Basic syntax parsing
```csharp
var parser = new ProFormaParser();
var term = parser.ParseString("PRQ[info:test]TEOFORM");
```
#### Term validation and coverting to a proteoform group.
```csharp
// Initialize providers and modification lookup
var elementProvider = new ElementProvider();
var residueProvider = new IupacAminoAcidProvider(elementProvider);

// Pull in all RESID modifications
var residParser = new ResidXmlParser();
var modifications = parser.Parse("path to RESID XML");
var residLookup = ResidModificationLookup.CreateFromModifications(modifications, elementProvider);

// Create a simple term in code as an example
// SEQV[RESID:AA0038]ENCE
var term = new ProFormaTerm("SEQVENCE", null, null, new List<ProFormaTag>
{
    new ProFormaTag(3, new[] { new ProFormaDescriptor("RESID", "AA0038") })
});

// Validate and create proteoform group
var factory = new ProteoformGroupFactory(elementProvider, residueProvider);
var proteoform = factory.CreateProteoformGroup(term, residLookup);
```

## Credits
We would like to thank the Consortium and its members for their continued support.
