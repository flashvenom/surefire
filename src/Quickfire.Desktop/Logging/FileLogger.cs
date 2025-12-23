using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Quickfire.Desktop.Logging;

internal sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly string _filePath;
    private readonly LogLevel _minLevel;
    private readonly object _sync = new();
    private bool _disposed;

    public FileLoggerProvider(string filePath, LogLevel minLevel)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        _minLevel = minLevel;
    }

    public ILogger CreateLogger(string categoryName)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(FileLoggerProvider));
        }

        return new FileLogger(categoryName, _filePath, _minLevel, _sync);
    }

    public void Dispose()
    {
        _disposed = true;
    }
}

internal sealed class FileLogger : ILogger
{
    private readonly string _category;
    private readonly string _filePath;
    private readonly LogLevel _minLevel;
    private readonly object _sync;

    public FileLogger(string category, string filePath, LogLevel minLevel, object sync)
    {
        _category = category;
        _filePath = filePath;
        _minLevel = minLevel;
        _sync = sync;
    }

    public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= _minLevel;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel) || formatter is null)
        {
            return;
        }

        var message = formatter(state, exception);
        if (string.IsNullOrWhiteSpace(message) && exception is null)
        {
            return;
        }

        var line = $"{DateTimeOffset.Now:O} [{logLevel}] {_category}: {message}";
        if (exception is not null)
        {
            line += Environment.NewLine + exception;
        }

        lock (_sync)
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.AppendAllText(_filePath, line + Environment.NewLine);
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new();
        public void Dispose()
        {
        }
    }
}

public static class FileLoggerExtensions
{
    public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, string filePath, LogLevel minLevel = LogLevel.Information)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Services.AddSingleton<ILoggerProvider>(_ => new FileLoggerProvider(filePath, minLevel));
        return builder;
    }
}
