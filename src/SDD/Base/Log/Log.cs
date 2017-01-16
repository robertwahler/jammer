using UnityEngine;

namespace SDD {

  /// <summary>
  /// UnityEngine.Debug.Log wrapper
  /// </summary>
  /// <remarks>
  /// Keep this in a separate assembly DLL so that double clicking a log entry
  /// in Unity will skip over these wrapper methods
  /// </remarks>
  public class Log {

    /// <summary>
    /// Log level bitfield. Defaults announcing everything.
    /// </summary>
    public static LogLevels LogLevels = (LogLevels.Verbose | LogLevels.Debug | LogLevels.Warning | LogLevels.Error);

    /// <summary>
    /// Verbose logging
    /// </summary>
    /// <remarks>
    /// Will be stripped out by the compiler unless "SDD_DEBUG" is defined
    /// </remarks>
    [System.Diagnostics.Conditional("SDD_DEBUG")]
    public static void Verbose(object message) {
      if ((LogLevels & LogLevels.Verbose) == LogLevels.Verbose) {
        if (Application.platform == RuntimePlatform.Android) {
          message = "[Log] " + message;
        }
        UnityEngine.Debug.Log(message);
      }
    }

    /// <summary>
    /// Debug logging
    /// </summary>
    /// <remarks>
    /// Will be stripped out by the compiler unless "SDD_DEBUG" is defined
    /// </remarks>
    [System.Diagnostics.Conditional("SDD_DEBUG")]
    public static void Debug(object message) {
      if ((LogLevels & LogLevels.Debug) == LogLevels.Debug) {
        if (Application.platform == RuntimePlatform.Android) {
          message = "[Log] " + message;
        }
        UnityEngine.Debug.Log(message);
      }
    }

    /// <summary>
    /// User informational logging
    /// </summary>
    public static void Info(object message) {
      if ((LogLevels & LogLevels.Info) == LogLevels.Info) {
        if (Application.platform == RuntimePlatform.Android) {
          message = "[Info] " + message;
        }
        UnityEngine.Debug.Log(message);
      }
    }

    /// <summary>
    /// Warning logging
    /// </summary>
    public static void Warning(object message) {
      if ((LogLevels & LogLevels.Warning) == LogLevels.Warning) {
        if (Application.platform == RuntimePlatform.Android) {
          message = "[Warning] " + message;
        }
        UnityEngine.Debug.LogWarning(message);
      }
    }

    /// <summary>
    /// Error logging
    /// </summary>
    public static void Error(object message) {
      if ((LogLevels & LogLevels.Error) == LogLevels.Error) {
        if (Application.platform == RuntimePlatform.Android) {
          message = "[Error] " + message;
        }
        UnityEngine.Debug.LogError(message);
      }
    }

    /// <summary>
    /// Verbose logging with object context
    /// </summary>
    /// <remarks>
    /// Will be stripped out by the compiler unless "SDD_DEBUG" is defined
    /// </remarks>
    [System.Diagnostics.Conditional("SDD_DEBUG")]
    public static void Verbose(object message, Object context) {
      if ((LogLevels & LogLevels.Verbose) == LogLevels.Verbose) {
        if (Application.platform == RuntimePlatform.Android) {
          message = "[Log] " + message;
        }
        UnityEngine.Debug.Log(message, context);
      }
    }

    /// <summary>
    /// Debug logging with object context
    /// </summary>
    /// <remarks>
    /// Will be stripped out by the compiler unless "SDD_DEBUG" is defined
    /// </remarks>
    [System.Diagnostics.Conditional("SDD_DEBUG")]
    public static void Debug(object message, Object context) {
      if ((LogLevels & LogLevels.Debug) == LogLevels.Debug) {
        if (Application.platform == RuntimePlatform.Android) {
          message = "[Log] " + message;
        }
        UnityEngine.Debug.Log(message, context);
      }
    }

    /// <summary>
    /// Info logging with object context
    /// </summary>
    public static void Info(object message, Object context) {
      if ((LogLevels & LogLevels.Info) == LogLevels.Info) {
        if (Application.platform == RuntimePlatform.Android) {
          message = "[Info] " + message;
        }
        UnityEngine.Debug.Log(message, context);
      }
    }


    /// <summary>
    /// Warning logging with object context
    /// </summary>
    public static void Warning(object message, Object context) {
      if ((LogLevels & LogLevels.Warning) == LogLevels.Warning) {
        if (Application.platform == RuntimePlatform.Android) {
          message = "[Warning] " + message;
        }
        UnityEngine.Debug.LogWarning(message, context);
      }
    }

    /// <summary>
    /// Error logging with object context
    /// </summary>
    public static void Error(object message, Object context) {
      if ((LogLevels & LogLevels.Error) == LogLevels.Error) {
        if (Application.platform == RuntimePlatform.Android) {
          message = "[Error] " + message;
        }
        UnityEngine.Debug.LogError(message, context);
      }
    }

  }
}
