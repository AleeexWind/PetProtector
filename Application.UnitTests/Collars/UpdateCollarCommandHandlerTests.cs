using Application.Collars.Commands.UpdateCollar;
using Application.Common.Interfaces;
using Application.UnitTests;
using Domain.Core.Enums;
using Infrastructure.Percistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.UnitTests.Collars;

/// <summary>
/// Regression tests for linking an unlinked collar to the current user.
/// </summary>
public class UpdateCollarCommandHandlerTests
{
    /// <summary>
    /// Linking an unlinked collar assigns the current user and marks the collar as linked.
    /// </summary>
    [Fact]
    public async Task UpdateCollar_links_unlinked_collar_to_current_user()
    {
        await using var db = AppDbContextFactory.Create();
        var (collarId, userId) = AppDbContextFactory.SeedUnlinkedCollar(db);
        var handler = new UpdateCollarCommandHandler(db, new TestExecutionContextAccessor(userId));

        var result = await handler.Handle(new UpdateCollarCommand { Id = collarId }, CancellationToken.None);

        var collar = await db.Collars.SingleAsync(c => c.Id == collarId);
        Assert.Equal(collarId, result);
        Assert.Equal(CollarStates.Linked, collar.State);
        Assert.Equal(userId, collar.UserId);
    }

    /// <summary>
    /// Saving through the domain-event dispatcher updates both the collar and questionnaire in one commit.
    /// </summary>
    [Fact]
    public async Task UpdateCollar_save_with_dispatcher_updates_collar_and_questionnaire()
    {
        var userId = Guid.NewGuid();
        await using var provider = AppDbContextFactory.CreateHost(userId);
        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var (collarId, _) = AppDbContextFactory.SeedUnlinkedCollar(db);

        var handler = new UpdateCollarCommandHandler(
            scope.ServiceProvider.GetRequiredService<IAppDbContext>(),
            scope.ServiceProvider.GetRequiredService<IExecutionContextAccessor>());

        await handler.Handle(new UpdateCollarCommand { Id = collarId }, CancellationToken.None);

        var collar = await db.Collars.AsNoTracking().SingleAsync(c => c.Id == collarId);
        var questionnaire = await db.Questionnaires.AsNoTracking().SingleAsync(q => q.Id == collarId);
        Assert.Equal(CollarStates.Linked, collar.State);
        Assert.Equal(userId, collar.UserId);
        Assert.Equal(QuestionnaireStates.Filling, questionnaire.State);
    }
}
