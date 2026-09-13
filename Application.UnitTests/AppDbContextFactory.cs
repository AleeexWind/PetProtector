using Application;
using Application.Common.Interfaces;
using Domain.Core.Entities;
using Domain.Core.Enums;
using Infrastructure.Percistance;
using Infrastructure.Percistance.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Application.UnitTests;

internal static class AppDbContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static ServiceProvider CreateHost(Guid userId)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddSingleton<IExecutionContextAccessor>(new TestExecutionContextAccessor(userId));
        services.AddScoped<ISaveChangesInterceptor, DomainEventDispatcher>();
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        return services.BuildServiceProvider();
    }

    public static (Guid CollarId, Guid UserId) SeedUnlinkedCollar(AppDbContext db)
    {
        var collarId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var entity = new Collar
        {
            Id = collarId,
            SecretKey = "secret-key",
            State = CollarStates.Unlinked,
            Questionnaire = new Questionnaire
            {
                Id = collarId,
                LinkQuestionnaire = Guid.NewGuid(),
                State = QuestionnaireStates.WaitingFilling
            }
        };

        db.Collars.Add(entity);
        db.SaveChanges();
        return (collarId, userId);
    }
}
