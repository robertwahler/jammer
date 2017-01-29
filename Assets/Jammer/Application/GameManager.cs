using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using DG.Tweening;

using Jammer.Events;
using Jammer.Scenes;
using Jammer.UI;

namespace Jammer {

  /// <summary>
  /// Contains basic App/Game logic that belongs in all games.
  /// </summary>
  public class GameManager : Singleton<GameManager> {

    /// <summary>
    /// Static constructor. Not inherited and only executed once for generics.
    /// initialize here even before the Initialize() method
    /// </summary>
    static GameManager() {
      // default static settings
      Log.LogLevels = LogLevels.Info | LogLevels.Warning | LogLevels.Error;
    }

    /// <summary>
    /// Application wide settings. These are serialized to application.txt in JSON format.
    /// </summary>
    public ApplicationSettings ApplicationSettings { get; set; }

    /// <summary>
    /// Focused when application is focused
    /// </summary>
    public bool Focused { get; set; }

    /// <summary>
    /// Game finite state machine
    /// </summary>
    public GameState State { get { return GetState(); } set { SetState(value); }}
    private GameState state = GameState.New;

    /// <summary>
    /// Log granularity
    /// </summary>
    public LogLevels LogLevels { get { return GetLogLevels(); } set { SetLogLevels(value); }}
    private LogLevels logLevels = Log.LogLevels;

    /// <summary>
    /// Initialize happens on Awake for new Singletons
    /// </summary>
    protected override void Initialize() {
      Log.Verbose(string.Format("GameManager.Initialize() ID={0}", GetInstanceID()));
      base.Initialize();

      // signal loading state
      State = GameState.Loading;

      // App starts focused
      Focused = true;

      // create settings at their defaults
      ApplicationSettings = new ApplicationSettings();

      //
      // DOTween
      //
      // initialize with the preferences set in DOTween's Utility Panel
      DOTween.Init();

      //
      // Newtonsoft.Json default serialization settings
      //
      // http://www.newtonsoft.com/json/help/html/SerializationSettings.htm
      JsonConvert.DefaultSettings = (() => new JsonSerializerSettings {

        // add '$type' only if needed.
        // NOTE: Can't use Binder in NET35 || NET20
        TypeNameHandling = TypeNameHandling.Auto,

        // don't write properties at default values, use
        // [System.ComponentModel.DefaultValue(x)] to mark defaults that cannot
        // be directly determined naturally, i.e. int defaults to 0.
        DefaultValueHandling = DefaultValueHandling.Ignore,

        Converters = { new StringEnumConverter { CamelCaseText = false }},

        // The default is to write all keys in camelCase. This can be overridden at
        // the field/property level using attributes
        ContractResolver = new CamelCasePropertyNamesContractResolver(),

        // pretty print
        Formatting = Formatting.Indented,
      });

      // read command line options
      if (ApplicationHelper.IsDesktop()) {
        string[] args = System.Environment.GetCommandLineArgs();
        string commandLine = string.Join(" ", args);

        foreach (string arg in args) {
          // store all settings the given folder
          if (Regex.IsMatch(arg, @"--settings-folder")) {
            // This handles quoted paths with spaces but leaves the quotes in place
            Regex regex = new Regex(@"--settings-folder\s+(?<path>[\""].+?[\""]|[^ ]+)");
            Match match = regex.Match(commandLine);
            if (match.Success) {
              string path = match.Groups["path"].Value;
              if (!Path.IsPathRooted(path)) {
                // convert to absolute path
                path = Path.Combine(Directory.GetCurrentDirectory(), path);
              }
              if (Directory.Exists(path)) {
                Log.Debug(string.Format("GameManager.Initialize() Settings.DataPath: {0}", path));
                Settings.DataPath = path;
              }
              else {
                Log.Error(string.Format("GameManager.Initialize() requested settings folder {0} does not exist", path));
              }
            }
          }
        }
      }

      // load the data, defaults to empty string
      string json = Settings.GetString(SettingsKey.Application);
      if (json != "") {
        DeserializeSettings(json);
      }

      #if JAMER_LOG_DEBUG
        LogLevels |= LogLevels.Debug;
      #endif

      #if JAMMER_LOG_VERBOSE
        LogLevels |= LogLevels.Verbose;
      #endif

      #if JAMMER_CONSOLE
        // log debug console from resources
        CreatePrefab(name: "Console");
      #endif

    }

    protected void OnEnable() {
      Log.Verbose(string.Format("GameManager.OnEnable() this={0}", this));

      if (MenuManager.Instance == null) {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName: ApplicationConstants.UIScene, mode: UnityEngine.SceneManagement.LoadSceneMode.Additive);
      }

      SubscribeEvents();

      // signal game started
      State = GameState.Started;
    }

    protected virtual void OnDisable(){
      Log.Verbose(string.Format("GameManager.OnDisable()"));

      UnsubscribeEvents();
    }

    protected virtual void SubscribeEvents() {
      Log.Verbose(string.Format("GameManager.SubscribeEvents()"));

      Events.AddListener<LoadSceneCommandEvent>(OnLoadSceneCommand);
    }

    protected virtual void UnsubscribeEvents() {
      Log.Verbose(string.Format("GameManager.UnsubscribeEvents()"));

      Events.RemoveListener<LoadSceneCommandEvent>(OnLoadSceneCommand);
    }

    /// <summary>
    /// Request/Announce scene loading
    /// </summary>
    void OnLoadSceneCommand(LoadSceneCommandEvent e) {
      if (e.Handled) {
        Log.Debug(string.Format("GameManager.OnLoadSceneCommand({0})", e));

        if (e.SceneName == ApplicationConstants.MainScene) {
          State = GameState.Ready;
        }
      }
    }

    // Event order:
    //
    //   App initially starts:
    //   OnApplicationFocus(true) is called
    //   App is soft closed:
    //   OnApplicationFocus(false) is called
    //   OnApplicationPause(true) is called
    //   App is brought forward after soft closing:
    //   OnApplicationPause(false) is called
    //   OnApplicationFocus(true) is called
    protected void OnApplicationPause(bool pauseStatus) {
      Log.Verbose(string.Format("GameManager.OnApplicationPause(pauseStatus: {0})", pauseStatus));
    }

    protected void OnApplicationFocus(bool focusStatus) {
      //Log.Verbose(string.Format("GameManager.OnApplicationFocus(focusStatus: {0})", focusStatus));

      Focused = focusStatus;

      if (UnityEngine.EventSystems.EventSystem.current != null) {
        // toggle navigation events
        UnityEngine.EventSystems.EventSystem.current.sendNavigationEvents = Focused;
      }
    }

    public void SaveSettings() {
      Log.Debug(string.Format("GameManager.SaveSettings()"));

      string json = SerializeSettings();
      Settings.SetString(SettingsKey.Application, json);
      Settings.Save();
    }

    /// <summary>
    /// Serialize to a dictionary of strings. Return a JSON string.
    /// </summary>
    public string SerializeSettings(bool prettyPrint = true) {
      Log.Debug(string.Format("GameManager.SerializeSettings()"));

      Formatting formatting = Formatting.None;
      if (prettyPrint) {
        formatting = Formatting.Indented;
      }
      return JsonConvert.SerializeObject(value: ApplicationSettings, formatting: formatting);
    }

    /// <summary>
    /// Deserialize the application settings from a JSON string as a dictionary
    /// of strings.
    /// </summary>
    public bool DeserializeSettings(string json) {
      Log.Debug(string.Format("GameManager.DeserializeSettings({0}) this={1}", json,  this));

      bool result = false;

      try {
        if (string.IsNullOrEmpty(json)) {
          throw new System.InvalidOperationException("json string is empty or null");
        }

        ApplicationSettings = JsonConvert.DeserializeObject<ApplicationSettings>(json);
        result = true;
      }
      catch (System.Exception e) {
        Log.Error(string.Format("GameManager.DeserializeSettings() failed with {0}", e.ToString()));
      }

      return result;
    }

    /// <summary>
    /// Create a prefab by name, loading from Resources. Returns null if the prefab can't be loaded.
    /// </summary>
    public GameObject CreatePrefab(string name, GameObject parent = null, string folder = null) {
      string path;
      GameObject prefab;

      // must use POSIX style slashes
      if (string.IsNullOrEmpty(folder)) {
        path = name;
      }
      else {
        path = folder + "/" + name;
      }

      Log.Debug(string.Format("Product.Load() loading asset {0}", path));
      // this is the "live" prefab, not an instance clone that lives in the scene
      prefab = (GameObject) Resources.Load(path);

      if (prefab != null) {
        // an instance clone that lives in the scene
        prefab = (GameObject) UnityEngine.Object.Instantiate(prefab);
        // remove the "(clone)" from the name
        prefab.name = name;
        // assign parent if not given so the the instance will be cleaned up with the scene
        if (parent == null) {
          // this gameObject
          parent = gameObject;
        }
        prefab.transform.SetParent(parent.transform, worldPositionStays: true);
      }

      return prefab;
    }

    private LogLevels GetLogLevels() {
      return logLevels;
    }

    private void SetLogLevels(LogLevels value) {
      logLevels = value;
      Log.LogLevels = logLevels;
    }

    private GameState GetState() {
      return state;
    }

    private void SetState(GameState value) {
      Log.Verbose(string.Format("GameManager.SetState(value: {0})", value));

      state = value;

      switch(state) {

        // TODO: add handler for user initiated new game
        case GameState.New:
          break;

        // TODO: add handler
        case GameState.Ready:
          break;

      }

      // publish state change event
      Events.Raise(new GameStateEvent(){ Game=this, State=state });
    }

    private void Update() {
      if (!Focused) return;

      // game manager only handles input if the menus are closed
      if (MenuManager.Instance.State == MenuState.Closed) {
        HandleInput();
      }
    }

    /// <summary>
    /// Global input handler
    /// </summary>
    private void HandleInput() {

      if (Input.GetKeyDown(KeyCode.Escape)) {
        Log.Verbose(string.Format("GameManager.HandleInput() KeyCode.Escape, opening main menu"));
        Events.Raise(new MenuCommandEvent(){ Handled=false, MenuId=MenuId.Main, State=MenuState.Open });
      }
    }

  }
}

