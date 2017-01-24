using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using DG.Tweening;
using SDD;
using SDD.Events;

using Jammer.Scenes;

namespace Jammer.UI {

  /// <summary>
  /// Light weight menu manager fake singleton. Listens for menu events, maps
  /// them, handles loading and unload the menus as needed. Hang this off the
  /// GameManager.
  /// </summary>
  public class MenuManager : EventHandler {

    /// <summary>
    /// Main menu container. Set in IDE.
    /// </summary>
    public GameObject menuContainer;

    /// <summary>
    /// CanvasGroup convenience property. Lazy loaded. Cached.
    /// </summary>
    public CanvasGroup CanvasGroup {
      get {
        if (canvasGroup == null) {
          canvasGroup = menuContainer.GetComponent<CanvasGroup>();
        }
        return canvasGroup;
      }
    }
    private CanvasGroup canvasGroup;

    /// <summary>
    /// There should be just one manager. If not found return null
    /// </summary>
    public static MenuManager Instance {
      get {
        if (instance == null) {
          Log.Debug(string.Format("MenuManager.Instance.get looking for object"));
          instance = (MenuManager) GameObject.FindObjectOfType(typeof(MenuManager));
        }
        return instance;
      }
    }
    private static MenuManager instance = null;

    /// <summary>
    /// Dictionary of available menus, populated at runtime
    /// </summary>
    public Dictionary<MenuId, Menu> Menus { get; set; }

    /// <summary>
    /// The currently open menu. Defaults to MenuId.Main unless we are in play
    /// mode on this scene, then the loaded design time menu is current.
    /// </summary>
    public Menu CurrentMenu { get; private set; }

    /// <summary>
    /// Menu state
    /// </summary>
    public MenuState State { get { return GetState(); } protected set { SetState(value); }}
    private MenuState state = MenuState.Closed;

    /// <summary>
    /// The name of the currently loaded scene or nil none
    /// </summary>
    public string CurrentScene { get; set; }

    protected override void OnEnable() {
      Log.Debug(string.Format("MenuManager.OnEnable()"));
      base.OnEnable();

      // always turn off design time menus so we start from the initial know state of closed
      menuContainer.SetActive(false);

      Menus = new Dictionary<MenuId, Menu>();

      // collect all the child menus into a dictionary
      Menus.Clear();
      Menu[] menus = transform.GetComponentsInChildren<Menu>(includeInactive: true);
      foreach (Menu menu in menus) {
        if (!Menus.ContainsKey(menu.Id)) {
          Log.Verbose(string.Format("MenuManager.OnEnable() add menu={0} to Menus", menu));
          Menus[menu.Id] = menu;
          // make sure all design time active menus start inactive
          menu.gameObject.SetActive(false);
        }
        else {
          Log.Error(string.Format("MenuManager.OnEnable() menu={0} duplicate id assigned to menu gameObject", menu));
        }
      }
    }

    public override void SubscribeEvents() {
      Log.Debug(string.Format("MenuManager.SubscribeEvents()"));

      Events.AddListener<MenuCommandEvent>(OnMainMenuCommand);
      Events.AddListener<LoadSceneCommandEvent>(OnLoadSceneCommand);
      Events.AddListener<UnloadSceneCommandEvent>(OnUnloadSceneCommand);
    }

    public override void UnsubscribeEvents() {
      Log.Debug(string.Format("MenuManager.UnsubscribeEvents()"));

      Events.RemoveListener<MenuCommandEvent>(OnMainMenuCommand);
      Events.RemoveListener<LoadSceneCommandEvent>(OnLoadSceneCommand);
      Events.RemoveListener<UnloadSceneCommandEvent>(OnUnloadSceneCommand);
    }

    public void OnMainMenuCommand(MenuCommandEvent e) {
      if (!e.Handled) {
        Log.Debug(string.Format("MenuManager.OnMainMenuCommand({0})", e));

        OpenMenu(id: e.MenuId);

        // open or close depending on the request
        State = e.State;
      }
    }

    private void OpenMenu(MenuId id) {
      Log.Debug(string.Format("MenuManager.OpenMenu(id: {0})", id));

        Menu newMenu = null;
        if (Menus.ContainsKey(id)) {
          newMenu = Menus[id];
        }
        else {
          Log.Error(string.Format("MenuManager.OpenMenu({0}) unable to find requested menu", id));
          // stop the handler here
          return;
        }

        // TODO: swapping current menu should be a coroutine so it can be animated
        if (CurrentMenu != null) {
          Log.Verbose(string.Format("MenuManager.OpenMenu({0}) disabling CurrentMenu {1}", id, CurrentMenu), gameObject);
          CurrentMenu.gameObject.SetActive(false);
        }

        CurrentMenu = newMenu;
        Log.Verbose(string.Format("MenuManager.OpenMenu({0}) enable CurrentMenu {1}", id, CurrentMenu), gameObject);
        CurrentMenu.gameObject.SetActive(true);
    }


    /// <summary>
    /// Request to unload a scene
    /// </summary>
    private void OnUnloadSceneCommand(UnloadSceneCommandEvent e) {
      if (!e.Handled) {
        Log.Debug(string.Format("MenuManager.OnUnloadSceneCommand({0})", e));

        UnityEngine.SceneManagement.SceneManager.UnloadScene(sceneName: e.SceneName);

        // notify event
        Events.Raise(new UnloadSceneCommandEvent() { Handled=true, SceneName=e.SceneName });
      }
    }

    /// <summary>
    /// Request/Announce scene loading
    /// </summary>
    private void OnLoadSceneCommand(LoadSceneCommandEvent e) {
      if (!e.Handled) {
        Log.Debug(string.Format("MenuManager.OnLoadSceneCommand({0})", e));

        // turn off menus
        State = MenuState.Closed;

        if (CurrentScene == e.SceneName) {
          // TODO: Add e.Force to reload if needed
          Log.Debug(string.Format("MenuManager.OnLoadSceneCommand({0}) skipping, already loaded", e));
          return;
        }

        switch (e.Mode) {

          case LoadSceneMode.Additive:
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName: e.SceneName, mode: UnityEngine.SceneManagement.LoadSceneMode.Additive);
            break;

          default:
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName: e.SceneName, mode: UnityEngine.SceneManagement.LoadSceneMode.Single);
            break;
        }

        CurrentScene = e.SceneName;
      }
      else {
        Log.Verbose(string.Format("MenuManager.OnLoadSceneCommand({0}) scene loaded", e));
        // do nothing
      }
    }

    private MenuState GetState() {
      return state;
    }

    private void SetState(MenuState value) {
      Log.Verbose(string.Format("MenuManager.SetState(value: {0})", value));

      switch(value) {

        case MenuState.Closed:
          StartCoroutine(ToggleMenu(on: false));
          break;

        case MenuState.Open:
          StartCoroutine(ToggleMenu(on: true));
          break;

        case MenuState.Opening:
        case MenuState.Closing:
          // nothing to do
          break;

        default:
          Log.Error(string.Format("MenuManager.SetState(value: {0}) unhandled state", value));
          break;
      }

      // notify event
      Events.Raise(new MenuCommandEvent() { Handled=true, MenuId=CurrentMenu.Id, State=State });
    }

    private IEnumerator ToggleMenu(bool on) {
      Log.Verbose(string.Format("MenuManager.ToggleMenus(on: {0})", on));

      float duration = 0.5f;

      if (on) {
        state = MenuState.Opening;
        // alpha off immediately
        CanvasGroup.alpha = 0f;
        // enable container
        menuContainer.SetActive(true);
        // fade on over duration
        yield return CanvasGroup.DOFade(1f, duration: duration).WaitForCompletion();
        state = MenuState.Open;
      }
      else {
        state = MenuState.Closing;
        // fade off over duration,
        yield return CanvasGroup.DOFade(0f, duration: duration).WaitForCompletion();
        menuContainer.SetActive(false);
        state = MenuState.Closed;
      }
    }

    protected virtual void Update() {
      // only handle input if we are already open, ignore closed and transitioning states
      if (State == MenuState.Open) {
        HandleInput();
      }
    }

    protected virtual void HandleInput() {
      if (Input.GetKeyDown(KeyCode.Escape)) {
        Log.Debug(string.Format("MenuManager.HandleInput() KeyCode.Escape"));

        switch(CurrentMenu.Id) {

          case MenuId.Main:
            // close menus
            StartCoroutine(ToggleMenu(on: false));
            break;

          default:
            // back to main menu
            OpenMenu(id: MenuId.Main);
            break;
        }
      }
    }

  }
}
