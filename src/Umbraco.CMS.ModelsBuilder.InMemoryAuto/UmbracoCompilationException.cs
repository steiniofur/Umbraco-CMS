using Microsoft.AspNetCore.Diagnostics;

namespace Umbraco.CMS.ModelsBuilder.InMemoryAuto;

internal class UmbracoCompilationException : Exception, ICompilationException
{
    public IEnumerable<CompilationFailure?>? CompilationFailures { get; init; }
}
