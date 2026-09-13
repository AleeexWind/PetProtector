using Application.Common.Interfaces;

namespace Application.UnitTests;

internal sealed class TestExecutionContextAccessor(Guid userId) : IExecutionContextAccessor
{
    public Guid UserId { get; } = userId;

    public string BaseUrl => "https://localhost";
}
