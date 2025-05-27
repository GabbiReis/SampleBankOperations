using SampleBankOperations.Infrastructure.Logging;
using System;
using System.IO;
using Xunit;

public class LoggerTests
{
    [Fact]
    public void Log_WritesExpectedMessageToConsole()
    {
        // Arrange
        var logger = new Logger();
        var message = "Test message";

        using var sw = new StringWriter();
        Console.SetOut(sw);

        // Act
        logger.Log(message);

        // Assert
        var output = sw.ToString();
        Assert.Contains("[LOG]", output);
        Assert.Contains(message, output);

        Assert.Matches(@"\[LOG\] .+: Test message", output);
    }
}
