using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.LowDb.UnitTests.Fakes;

[ExcludeFromCodeCoverage]
internal class FakeJSRuntime : IJSRuntime
{
    public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>
        (string identifier, object?[]? args)
    {
        throw new NotImplementedException();
    }

    public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>
        (string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        throw new NotImplementedException();
    }
}
