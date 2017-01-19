using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using DG.Tweening;
using SDD;
using SDD.Events;

namespace Jammer {

  /// <summary>
  /// Contains basic App/Game logic that belongs in all games.
  /// </summary>
  public class GameManager : Singleton<GameManager> {

    /// <summary>
    /// Static constructor. Not inherited and only executed once for generics.
    /// </summary>
    static GameManager() {
      // set globals, defines here
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
    private LogLevels logLevels = (LogLevels.Debug | LogLevels.Info | LogLevels.Warning | LogLevels.Error);

    /// <summary>
    /// ApplicationManager.Awake() should be the first Monobehaviour Awake() to fire.
    /// Initialize happens on Awake only for new Singletons.
    /// </summary>
    protected override void Initialize() {
      Log.Verbose(string.Format("GameManager.Initialize() ID={0}", GetInstanceID()));
      base.Initialize();

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

      // load the data, defaults to empty string
      string json = Settings.GetString(SettingsKey.Application);
      if (json != "") {
        DeserializeSettings(json);
      }

      // Production builds always disable most settings regardless of IDE
      // setting, desktop can override on CLI for most thing except console
      // server just two options allowed by default. You can turn on logging
      // in a production build by tapping the dog's head 5 times.
      LogLevels = (LogLevels.Info | LogLevels.Warning | LogLevels.Error);
      #if SDD_LOG_DEBUG
        LogLevels |= LogLevels.Debug;
      #endif
      #if SDD_LOG_VERBOSE
        LogLevels |= LogLevels.Verbose;
      #endif

      #if SDD_CONSOLE
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
      State = GameState.Started;
    }

    protected virtual void OnDisable(){
      Log.Verbose(string.Format("ApplicationManager.OnDisable()"));

      UnsubscribeEvents();
    }

    protected virtual void SubscribeEvents() {
      Log.Verbose(string.Format("ApplicationManager.SubscribeEvents()"));

    }

    protected virtual void UnsubscribeEvents() {
      Log.Verbose(string.Format("ApplicationManager.UnsubscribeEvents()"));

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
      Log.Verbose(string.Format("ApplicationManager.OnApplicationPause(pauseStatus: {0})", pauseStatus));

    }

    protected void OnApplicationFocus(bool focusStatus) {
      Log.Verbose(string.Format("ApplicationManager.OnApplicationFocus(focusStatus: {0})", focusStatus));

      Focused = focusStatus;

      if (UnityEngine.EventSystems.EventSystem.current != null) {
        // toggle navigation events
        UnityEngine.EventSystems.EventSystem.current.sendNavigationEvents = Focused;
      }
    }

    public void SaveSettings() {
      Log.Debug(string.Format("ApplicationManager.SaveSettings()"));

      string json = SerializeSettings();
      Settings.SetString(SettingsKey.Application, json);
      Settings.Save();
    }

    /// <summary>
    /// Serialize to a dictionary of strings. Return a JSON string.
    /// </summary>
    public string SerializeSettings(bool prettyPrint = true) {
      Log.Debug(string.Format("ApplicationManager.SerializeSettings()"));

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
      Log.Debug(string.Format("ApplicationManager.DeserializeSettings({0}) this={1}", json,  this));

      bool result = false;

      try {
        if (string.IsNullOrEmpty(json)) {
          throw new System.InvalidOperationException("json string is empty or null");
        }

        ApplicationSettings = JsonConvert.DeserializeObject<ApplicationSettings>(json);
        result = true;
      }
      catch (System.Exception e) {
        Log.Error(string.Format("ApplicationManager.DeserializeSettings() failed with {0}", e.ToString()));
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

      Log.Debug(string.Format("Product.Load() loading asset at {0}", path));
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

        // user initiated new game, set in Game.Reset()
        case GameState.New:
          break;

        // After new and after resets and loading
        case GameState.Ready:
          break;

        // Game over
        case GameState.GameOver:
          break;

      }

      // publish state change event
      EventManager.Instance.Raise(new GameStateEvent(){ Game=this, State=state });
    }

    private void Update() {
      if (!Focused) return;

      HandleInput();
    }

    private void HandleInput() {
      // global input handler
    }

  }
}

