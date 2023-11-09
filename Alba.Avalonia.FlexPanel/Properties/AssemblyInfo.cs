using System.Diagnostics.CodeAnalysis;
using Avalonia.Metadata;
using static Strings;

[assembly: XmlnsPrefix(UrnAlba, PrefixAlba)]
[assembly: XmlnsDefinition(UrnAlba, NsAlbaFlexPanel)]

[SuppressMessage("ReSharper", "CheckNamespace")]
file interface Strings
{
    const string UrnAlba = "urn:alba:avalonia";
    const string NsAlba = "Alba.Avalonia";
    const string NsAlbaFlexPanel = $"{NsAlba}.FlexPanel";
    const string PrefixAlba = "alba";
}