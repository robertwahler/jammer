using UnityEngine;
using UnityEngine.UI;
using SDD;
using SDD.Events;

using Colors.Events;

namespace Colors {

  /// <summary>
  /// Input handler
  /// </summary>
  public class InputHandler : MonoBehaviour {

    protected void Update() {

      if (Input.GetKeyDown(KeyCode.R)) {
        Log.Debug(string.Format("InputHandler.Update() KeyCode.R"));
        EventManager.Instance.Raise(new ButtonClickEvent(){ Kind=ButtonKind.Red });
      }
      else if (Input.GetKeyDown(KeyCode.G)) {
        Log.Debug(string.Format("InputHandler.Update() KeyCode.G"));
        EventManager.Instance.Raise(new ButtonClickEvent(){ Kind=ButtonKind.Green });
      }
      else if (Input.GetKeyDown(KeyCode.B)) {
        Log.Debug(string.Format("InputHandler.Update() KeyCode.B"));
        EventManager.Instance.Raise(new ButtonClickEvent(){ Kind=ButtonKind.Blue });
      }

    }

  }
}
