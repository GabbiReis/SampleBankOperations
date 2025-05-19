using Microsoft.Extensions.DependencyInjection;
using SampleBankOperations.App.Interfaces;
using SampleBankOperations.App.Services.Operations;
using SampleBankOperations.App.Services.UI;
using SampleBankOperations.Application.Interfaces;
using SampleBankOperations.Application.Services;
using SampleBankOperations.Core.Interfaces;
using SampleBankOperations.Infrastructure.Logging;
using SampleBankOperations.Infrastructure.Persistence;
using SampleBankOperations.Infrastructure.Persistence.Repositories;

var serviceProvider = new ServiceCollection()
    .AddSingleton<ILogger, Logger>()
    .AddSingleton<BankingDbContext>()
    .AddScoped<IAccountRepository, AccountRepository>()
    .AddScoped<IAccountService, AccountService>()
    .AddScoped<IBankOperations, BankOperations>()
    .AddTransient<UserInterface>()
    .BuildServiceProvider();

var userInterface = serviceProvider.GetService<UserInterface>();
userInterface?.Run();
