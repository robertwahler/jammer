using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Jammer.Extensions;

namespace Jammer.Editor {

  public static class LogLevelsEditor {

    /// <summary>
    /// Disable menus unless running because these loglevels are not saved.
    /// LogLevels are intialized at runtime using compiler defines.
    /// </summary>
    [MenuItem("Window/Jammer/Runtime/LogLevels/None", true)]
    [MenuItem("Window/Jammer/Runtime/LogLevels/Error", true)]
    [MenuItem("Window/Jammer/Runtime/LogLevels/Warning", true)]
    [MenuItem("Window/Jammer/Runtime/LogLevels/Info", true)]
    [MenuItem("Window/Jammer/Runtime/LogLevels/Debug", true)]
    [MenuItem("Window/Jammer/Runtime/LogLevels/Verbose", true)]
    public static bool LogLevelsEnable() {
      return EditorApplication.isPlaying;
    }

    [MenuItem("Window/Jammer/Runtime/LogLevels/None", false, Shortcuts.MENU_BUILDER_LOGLEVELS + 10)]
    public static void LogLevelsNone() {
      Log.LogLevels = 0;
    }

    [MenuItem("Window/Jammer/Runtime/LogLevels/Error", false, Shortcuts.MENU_BUILDER_LOGLEVELS + 20)]
    public static void LogLevelsError() {
      Log.LogLevels = LogLevels.Error;
    }

    [MenuItem("Window/Jammer/Runtime/LogLevels/Warning", false, Shortcuts.MENU_BUILDER_LOGLEVELS + 30)]
    public static void LogLevelsWarning() {
      Log.LogLevels = LogLevels.Error | LogLevels.Warning;
    }

    [MenuItem("Window/Jammer/Runtime/LogLevels/Info", false, Shortcuts.MENU_BUILDER_LOGLEVELS + 40)]
    public static void LogLevelsInfo() {
      Log.LogLevels = LogLevels.Error | LogLevels.Warning | LogLevels.Info;
    }

    [MenuItem("Window/Jammer/Runtime/LogLevels/Debug", false, Shortcuts.MENU_BUILDER_LOGLEVELS + 50)]
    public static void LogLevelsDebug() {
      Log.LogLevels = LogLevels.Error | LogLevels.Warning | LogLevels.Info | LogLevels.Debug;
    }

    [MenuItem("Window/Jammer/Runtime/LogLevels/Verbose", false, Shortcuts.MENU_BUILDER_LOGLEVELS + 60)]
    public static void LogLevelsVerbose() {
      Log.LogLevels = LogLevels.Error | LogLevels.Warning | LogLevels.Info | LogLevels.Debug | LogLevels.Verbose;
    }

  }
}
