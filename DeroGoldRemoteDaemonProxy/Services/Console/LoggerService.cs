using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DeroGoldRemoteDaemonProxy.DependencyInjection;
using DeroGoldRemoteDaemonProxy.Services.IO;

namespace DeroGoldRemoteDaemonProxy.Services.Console
{
    public sealed class LoggerService : IInitializable, IDisposable
    {
        private FilePathService FilePathService { get; }

        private StreamWriter StreamWriter { get; set; }

        private SemaphoreSlim StreamWriterLock { get; set; }

        private ConcurrentQueue<(TextWriter textWriter, ConsoleColor consoleColor, string message)> LoggerQueue { get; set; }

        private bool IsRunning { get; set; }

        public LoggerService(FilePathService filePathService)
        {
            FilePathService = filePathService;
        }

        public void Initialize()
        {
            StreamWriter = new StreamWriter(new FileStream(FilePathService.ConsoleLogFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
            StreamWriterLock = new SemaphoreSlim(1, 1);
            LoggerQueue = new ConcurrentQueue<(TextWriter textWriter, ConsoleColor consoleColor, string message)>();

            Task.Factory.StartNew(async () =>
            {
                IsRunning = true;

                do
                {
                    while (LoggerQueue.TryDequeue(out var logger))
                        await LogMessageAsync(logger.textWriter, logger.consoleColor, logger.message).ConfigureAwait(false);

                    await Task.Delay(1).ConfigureAwait(false);
                } while (IsRunning);

                IsRunning = false;
            }, TaskCreationOptions.LongRunning);
        }

        public void Dispose()
        {
            Task.Run(async () =>
            {
                while (IsRunning)
                    await Task.Delay(1).ConfigureAwait(false);
            }).Wait();

            StreamWriter?.Dispose();
            StreamWriterLock?.Dispose();
        }

        public void LogMessage(string message, ConsoleColor consoleColor = ConsoleColor.White)
        {
            LoggerQueue.Enqueue((System.Console.Out, consoleColor, message));
        }

        public void LogErrorMessage(Exception exception)
        {
            LoggerQueue.Enqueue((System.Console.Error, ConsoleColor.Red, exception.ToString()));
        }

        private async Task LogMessageAsync(TextWriter writer, ConsoleColor consoleColor, string message)
        {
            if (message == null)
                return;

            try
            {
                System.Console.ForegroundColor = consoleColor;

                await StreamWriterLock.WaitAsync().ConfigureAwait(false);

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