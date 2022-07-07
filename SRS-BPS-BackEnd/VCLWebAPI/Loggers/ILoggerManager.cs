using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace Services.Loggers
{
    public static class Logger
    {
        private static readonly string LOG_CONFIG_FILE = @"log4net.config";

        private static readonly log4net.ILog _log = GetLogger(typeof(Logger));

        public static ILog GetLogger(Type type)
        {
            return LogManager.GetLogger(type);
        }

        public static void Debug(object message)
        {
            SetLog4NetConfiguration();
            _log.Debug(message);
        }

        private static void SetLog4NetConfiguration()
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead(LOG_CONFIG_FILE));

            var repo = LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
        }
    }
    //public interface ILoggerManager
    //{
    //    void LogInformation(string message);
    //}

    //public class LoggerManager : ILoggerManager
    //{
    //    private readonly ILog _logger = LogManager.GetLogger(typeof(LoggerManager));

    //    public LoggerManager()
    //    {
    //        try
    //        {
    //            XmlDocument log4netConfig = new XmlDocument();

    //            using (var fs = File.OpenRead("log4net.config"))
    //            {
    //                log4netConfig.Load(fs);

    //                var repo = LogManager.CreateRepository(
    //                        Assembly.GetEntryAssembly(),
    //                        typeof(log4net.Repository.Hierarchy.Hierarchy));

    //                XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

    //                // The first log to be written 
    //                _logger.Info("Log System Initialized");
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.Error("Error", ex);
    //        }
    //    }

    //    // Logging functionality happens here
    //    public void LogInformation(string message)
    //    {
    //        _logger.Info(message);
    //    }

    //    public void LogDebug(string message)
    //    {
    //        _logger.Debug(message);
    //    }

    //    public void LogWarning(string message)
    //    {
    //        _logger.Warn(message);
    //    }

    //    public void LogError(string message)
    //    {
    //        _logger.Error(message);
    //    }

    //    public void LogFatal(string message)
    //    {
    //        _logger.Fatal(message);
    //    }

}
