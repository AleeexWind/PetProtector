using Application.Collars.EventHandlers;
using Application.UnitTests;
using Domain.Core.Events;
using Domain.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.UnitTests.Collars;

/// <summary>
/// Regression tests for moving a questionnaire to Filling after a collar is linked.
/// </summary>
public class CollarUpdatedEventHandlerTests
{
    /// <summary>
    /// Handling CollarUpdatedEvent moves a WaitingFilling questionnaire to Filling without a nested save.
    /// </summary>
    [Fact]
    public async Task CollarUpdated_moves_questionnaire_from_WaitingFilling_to_Filling()
    {
        await using var db = AppDbContextFactory.Create();
        var (collarId, _) = AppDbContextFactory.SeedUnlinkedCollar(db);
        var handler = new CollarUpdatedEventHandler(db);

        await handler.Handle(new CollarUpdatedEvent(collarId), CancellationToken.None);

        var questionnaire = await db.Questionnaires.SingleAsync(q => q.Id == collarId);
        Assert.Equal(QuestionnaireStates.Filling, questionnaire.State);
        Assert.Equal(1, await db.SaveChangesAsync());
    }
}
