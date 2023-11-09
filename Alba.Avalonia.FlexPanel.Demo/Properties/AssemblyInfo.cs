using System.Diagnostics.CodeAnalysis;
using Avalonia.Metadata;
using static Strings;

[assembly: XmlnsPrefix(UrnDemo, PrefixDemo)]
[assembly: XmlnsDefinition(UrnDemo, $"{NsDemo}.Demo")]
[assembly: XmlnsDefinition(UrnDemo, $"{NsDemo}.ViewModels")]
[assembly: XmlnsDefinition(UrnDemo, $"{NsDemo}.Converters")]

[SuppressMessage("ReSharper", "CheckNamespace")]
file interface Strings
{
    const string UrnDemo = "urn:alba:avalonia:demo";
    const string NsDemo = "Alba.Avalonia.FlexPanel.Demo";
    const string PrefixDemo = "demo";
}