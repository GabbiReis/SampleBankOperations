using System;
using System.IO;
using FluentAssertions;
using SampleBankOperations.Infrastructure.Logging;
using Xunit;

namespace SampleBankOperations.Tests.Infrastructure
{
    public class LoggerTests
    {
        [Fact]
        public void Log_ShouldWriteMessageWithTimestamp()
        {
            // Arrange
            var logger = new Logger();
            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            logger.Log("Teste de log");

            // Assert
            var result = output.ToString();
            result.Should().Contain("[LOG]");
            result.Should().Contain("Teste de log");
            result.Should().Contain(DateTime.Now.Year.ToString());

            // Cleanup
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }
    }
}
