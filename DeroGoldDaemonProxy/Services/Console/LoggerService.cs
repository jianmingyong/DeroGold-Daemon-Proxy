using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DeroGoldDaemonProxy.Services.IO;
using TheDialgaTeam.DependencyInjection;

namespace DeroGoldDaemonProxy.Services.Console
{
    public sealed class LoggerService : IInitializableAsync, IErrorLogger, IDisposableAsync
    {
        private FilePathService FilePathService { get; }

        private StreamWriter StreamWriter { get; set; }

        private SemaphoreSlim StreamWriterLock { get; set; }

        public LoggerService(FilePathService filePathService)
        {
            FilePathService = filePathService;
        }

        public async Task InitializeAsync()
        {
            StreamWriter = new StreamWriter(new FileStream(FilePathService.ConsoleLogFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
            StreamWriterLock = new SemaphoreSlim(1, 1);

            await LogMessageAsync("==================================================").ConfigureAwait(false);
            await LogMessageAsync("DeroGold Daemon Proxy (.NET Core)").ConfigureAwait(false);
            await LogMessageAsync("==================================================").ConfigureAwait(false);
            await LogMessageAsync("Initializing Application...\n").ConfigureAwait(false);
        }

        public async Task LogErrorMessageAsync(Exception exception)
        {
            await LogMessageAsync(System.Console.Error, ConsoleColor.Red, exception.ToString()).ConfigureAwait(false);
        }

        public Task DisposeAsync()
        {
            StreamWriter?.Dispose();
            StreamWriterLock?.Dispose();

            return Task.CompletedTask;
        }

        public async Task LogMessageAsync(string message, ConsoleColor consoleColor = ConsoleColor.White)
        {
            await LogMessageAsync(System.Console.Out, consoleColor, message).ConfigureAwait(false);
        }

        private async Task LogMessageAsync(TextWriter writer, ConsoleColor consoleColor, string message)
        {
            if (message == null)
                return;

            await StreamWriterLock.WaitAsync().ConfigureAwait(false);

            try
            {
                System.Console.ForegroundColor = consoleColor;

                await writer.WriteLineAsync(message).ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);

                await StreamWriter.WriteLineAsync($"{DateTime.UtcNow:u} {message}").ConfigureAwait(false);
                await StreamWriter.FlushAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                await System.Console.Error.WriteLineAsync(ex.ToString()).ConfigureAwait(false);
            }
            finally
            {
                System.Console.ResetColor();
                StreamWriterLock.Release();
            }
        }
    }
}