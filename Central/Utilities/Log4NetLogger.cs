using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using log4net;
using Microsoft.Practices.Prism.Logging;

namespace Central.Utilities
{
  [Export(typeof(ILoggerFacade))]
  internal class Log4NetLogger : ILoggerFacade
  {
    private readonly ILog _logger = LogManager.GetLogger(typeof(Log4NetLogger));

    #region ILoggerFacade Members

    public void Log(string message, Category category, Priority priority)
    {
      switch (category)
      {
        case Category.Debug:
          _logger.Debug(message);
          break;
        case Category.Warn:
          _logger.Warn(message);
          break;
        case Category.Exception:
          _logger.Error(message);
          break;
        case Category.Info:
          _logger.Info(message);
          break;
      }
    }
    #endregion ILoggerFacade Members
  }
}