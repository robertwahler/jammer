using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SDD;

namespace Jammer {

  /// <summary>
  /// Settings is a dictionary wrapper to manage saving strings as text files in
  /// a given folder.  A replacement for PlayerPrefs String storage.
  ///
  /// NOTE: This class will cache settings read from disk
  ///
  /// example: save application settings to application.txt as a single string, i.e. JSON format
  ///
  ///     Settings.SetString(SettingsKey.Application, jsonString);
  ///     Settings.Save();
  ///
  /// or legacy
  ///
  ///     Settings.SetString("application", jsonString);
  ///     Settings.Save();
  /// </summary>
  public static class Settings {

    /// <summary>
    /// Holds settings data
    /// </summary>
    public class Setting {

      /// <summary>
      /// Constructor
      /// </summary>
      public Setting(string value) {
        this.data = value;
      }

      public string data = "";
      public bool dirty = false;
    }

    /// <summary>
    /// The root data folder. Used by the entire application to read and write
    /// data, not just used by the Settings class.  This is an absolute path.
    /// </summary>
    #if UNITY_EDITOR
      // Each product has its own folder in tmp when running in the editor.
      // This allows audio and other debug settings to be separate from
      // standalone builds on the same development machine.
      // NOTE: The hard-coded forward slash should work on Windows, Linux, and OSX.
      public static string DataPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(),  "tmp/settings/" + Application.productName);
    #else
      // macOS
      //    ~/Library/Application Support/Salty\ Dog\ Digital/Fourtex\ Zen/
      //
      // Linux
      //   ~/.config/unity3d/Salty\ Dog\ Digital/Fourtex\ Zen/
      //
      // Windows
      //   ~/AppData/LocalLow/Salty\ Dog\ Digital/Fourtex\ Zen/
      public static string DataPath = Application.persistentDataPath;
    #endif

    /// <summary>
    /// Dictionary of setting objects
    /// </summary>
    public static readonly Dictionary<string, Setting> settings = new Dictionary<string, Setting>();

    public static bool ContainsKey(string key) {
      return settings.ContainsKey(key);
    }

    /// <summary>
    /// Set the string using the preferred SettingsKey
    /// </summary>
    public static void SetString(SettingsKey key, string value) {
      SetString(KeyToString(key), value);
    }

    /// <summary>
    /// Set the string using the legacy string key
    /// </summary>
    public static void SetString(string key, string value) {
      Log.Debug(string.Format("Settings.SetString(key: {0}, value: ...)", key));
      var setting = new Setting(value);
      setting.dirty = true;
      settings[key] = setting;
    }

    /// <summary>
    /// Get the string using the preferred SettingsKey
    /// </summary>
    public static string GetString(SettingsKey key) {
      return GetString(KeyToString(key));
    }

    /// <summary>
    /// Get the string using the legacy string key
    /// </summary>
    public static string GetString(string key) {
      if (settings.ContainsKey(key)) {
        Log.Debug(string.Format("Settings.GetString(key: {0}) returning cached value", key));
        return settings[key].data;
      }
      else {
        #if ((UNITY_WEBPLAYER || UNITY_WEBGL || UNITY_TVOS) && !UNITY_EDITOR)
        if (UnityEngine.PlayerPrefs.HasKey(key: key)) {
          return (UnityEngine.PlayerPrefs.GetString(key: key));
        }
        #else
        string filename = FilenameFromKey(key);
        if (System.IO.File.Exists(filename)) {
          Log.Debug(string.Format("Settings.GetString(key: {0}) filename={1}", key, filename ));
          return System.IO.File.ReadAllText(filename);
        }
        #endif
        else {
          return "";
        }
      }
    }

    public static string GetString(string key, string defaultValue) {
      if (settings.ContainsKey(key)) {
        return settings[key].data;
      }
      else {
        #if ((UNITY_WEBPLAYER || UNITY_WEBGL || UNITY_TVOS) && !UNITY_EDITOR)
        if (UnityEngine.PlayerPrefs.HasKey(key: key)) {
          return (UnityEngine.PlayerPrefs.GetString(key: key));
        }
        #else
        string filename = FilenameFromKey(key);
        if (System.IO.File.Exists(filename)) {
          Log.Debug(string.Format("Settings.GetString(key: {0}, defaultValue: {1}) filename={2}", key, defaultValue, filename ));
          return System.IO.File.ReadAllText(filename);
        }
        #endif
        else {
          Log.Debug(string.Format("Settings.GetString(key: {0}, defaultValue: {1}) no filename, returning defaultValue", key, defaultValue));
          var setting = new Setting(defaultValue);
          setting.dirty = true;
          settings[key] = setting;
          return settings[key].data;
        }
      }
    }

    /// <summary>
    /// Remove the setting from memory and delete the setting file using the
    /// preferred SettingsKey
    /// </summary>
    public static void Remove(SettingsKey key) {
      Log.Verbose(string.Format("Settings.Remove(key: {0})", key));
      Remove(KeyToString(key));
    }

    /// <summary>
    /// Remove the setting from memory and delete the setting file using the
    /// legacy string key
    /// </summary>
    public static void Remove(string key) {
      Log.Debug(string.Format("Settings.Remove(key: {0})", key));
      settings.Remove(key);

      #if ((UNITY_WEBPLAYER || UNITY_WEBGL || UNITY_TVOS) && !UNITY_EDITOR)
        Log.Debug(string.Format("Settings.Remove() using UnityEngine.PlayerPrefs key={0}", key));
        UnityEngine.PlayerPrefs.DeleteKey(key);
      #else
        string filename = FilenameFromKey(key);
        Log.Debug(string.Format("Settings.Remove() filename={0}", filename ));
        if (System.IO.File.Exists(filename)) {
          System.IO.File.Delete(filename);
        }
      #endif
    }

    /// <summary>
    /// Clear all settings from memory, but don't remove the setting files.
    /// </summary>
    public static void Clear() {
      Log.Debug(string.Format("Settings.Clear()"));
      settings.Clear();
    }

    public static void Save() {
      Log.Debug(string.Format("Settings.Save()"));

      foreach(string key in settings.Keys) {
        var setting = settings[key];
        if (setting.dirty) {
          string output = setting.data;
          if (output != null) {
            output = output.Trim();
          }

          #if ((UNITY_WEBPLAYER || UNITY_WEBGL || UNITY_TVOS) && !UNITY_EDITOR)
            Log.Debug(string.Format("Settings.Save() using UnityEngine.PlayerPrefs() key={0}", key));
            UnityEngine.PlayerPrefs.SetString(key, output);
            UnityEngine.PlayerPrefs.Save();
          #else
            string filename = FilenameFromKey(key);
            string folder = System.IO.Path.GetDirectoryName(filename);
            Log.Debug(string.Format("Settings.Save() filename={0}", filename ));

            if (!System.IO.Directory.Exists(folder)) {
              System.IO.Directory.CreateDirectory(folder);
            }
            System.IO.File.WriteAllText(filename, output);
          #endif

          setting.dirty = false;
        }
      }
    }

    /// <summary>
    /// Expand keys in the form "leaderboard.hiscore1" to "leaderboard/hiscore1"
    /// Returns a fullpath filename
    /// </summary>
    public static string FilenameFromKey(SettingsKey key, string extension=".txt") {
      return FilenameFromKey(KeyToString(key), extension);
    }

    /// <summary>
    /// Expand keys in the form "leaderboard.hiscore1" to "leaderboard/hiscore1"
    /// Returns a fullpath filename
    /// </summary>
    public static string FilenameFromKey(string key, string extension=".txt") {
      var paths = new List<string>();
      // add our static settings folder
      paths.Add(DataPath);

      List<string> keyParts = key.Split('.').ToList();
      for (int i = 0; i < keyParts.Count - 1; i++) {
        // add any parts before the last "." in the key
        paths.Add(keyParts[i]);
      }

      string expandedPath = paths.Aggregate(System.IO.Path.Combine);
      string expandedKey = keyParts.Last();
      string filename = System.IO.Path.Combine(expandedPath, expandedKey + extension );

      return filename;
    }

    /// <summary>
    /// </summary>
    public static string KeyToString(SettingsKey key) {
      return key.ToString().ToLower();
    }
  }
}
