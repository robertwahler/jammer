using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters;
using System.Linq;

using SDD;

namespace Jammer.Console {

  /// <summary>
  /// Override of DebugLogConsole to allow easy mods and hooks in one place
  /// </summary>
  public class ConsoleManager : DebugLogManager {

    /// <summary>
    /// The hotkey to show and hide the console window.
    /// </summary>
    public KeyCode toggleKey = KeyCode.BackQuote;

    /// <summary>
    /// State machine
    /// </summary>
    public ConsoleState State { get { return GetState(); } set { SetState(value); }}
    protected ConsoleState state = ConsoleState.Inactive;

    protected virtual ConsoleState GetState() {
      return state;
    }

    protected virtual void SetState(ConsoleState value) {
      Log.Debug(string.Format("ConsoleState.SetState(value: {0})", value));

      if (state != value) {
        state = value;
      }
    }

    public void Start() {
      Log.Debug(string.Format("ConsoleState.Start()"));

      // TODO: Use C# attributes to define commands instead of hard-wiring here
      DebugLogConsole.AddCommandInstance(command: "SetMuteAudio", methodName: "SetMuteAudio", instance: AudioManager.Instance);
    }

    public void Update() {
      HandleInput();
    }

    /// <summary>
    /// Main input. Called each frame from update.
    /// </summary>
    protected virtual void HandleInput() {

      if (Input.GetKeyDown(toggleKey)) {
        Log.Debug(string.Format("ConsoleState.HandleInput() State {0}, toggleKey {1} pressed", State, toggleKey));

        // clear input of hotkey, is there a better way to eat the hotkey?
        commandInputField.text = Regex.Replace(commandInputField.text, "`", "");

        switch(State) {

          case ConsoleState.Inactive:
            OnSetVisible();
            // focus input
            commandInputField.Select();
            commandInputField.ActivateInputField();
            // kludge, fix this
            logWindowTR.anchoredPosition = Vector3.zero;
            State = ConsoleState.Active;
            break;

          case ConsoleState.Hidden:
            OnSetVisible();
            // focus input
            commandInputField.Select();
            commandInputField.ActivateInputField();
            // kludge, fix this
            logWindowTR.anchoredPosition = Vector3.zero;
            State = ConsoleState.Active;
            break;

          case ConsoleState.Active:
            OnSetInvisible();
            State = ConsoleState.Hidden;
            break;

          case ConsoleState.Minimized:
            OnSetVisible();
            // focus input
            commandInputField.Select();
            commandInputField.ActivateInputField();
            // kludge, fix this
            logWindowTR.anchoredPosition = Vector3.zero;
            State = ConsoleState.Active;
            break;

        }
      }
    }

    //protected virtual void

  }
}

