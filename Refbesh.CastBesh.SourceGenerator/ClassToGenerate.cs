using System.Collections.Immutable;

namespace Refbesh.CastBesh.SourceGenerator;

internal record ClassToGenerate(
    string Namespace,
    string ClassName,
    string FullClassName,
    ImmutableArray<string> CastableToTargets,
    ImmutableArray<string> CastableFromSources,
    bool IsPartial
);