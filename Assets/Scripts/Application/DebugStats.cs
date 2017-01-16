using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SDD;
using SDD.Events;

namespace Jammer {

  public class DebugStats : EventHandler {

    /// <summary>
    /// Text component for stat display. Set in IDE.
    /// </summary>
    public Text labelStats;

    private int fps;
    private int gameObjectCount;
    private string stats;
    private float memory = 0f;
    private int eventDelegates = 0;

    public override void SubscribeEvents() {
      // future use
    }

    public override void UnsubscribeEvents() {
      // future use
    }

    public IEnumerator Start() {
      Log.Debug(string.Format("DebugStats.Start()"));

      // wait for game to start, only needed when this scene is run from the IDE
      //while (Game == null) yield return null;
      //while (Game.State == GameState.New) yield return null;

      #if SDD_DEBUG_OVERLAY
        gameObjectCount = 0;
        fps = 0;
        stats = "";
        labelStats.text = stats;

        // skip the first frame
        yield return null;

        StartCoroutine(CalculateFPS());
        StartCoroutine(CalculateStats());
        StartCoroutine(UpdateDisplay());
      #else
        enabled = false;
      #endif
    }

    IEnumerator CalculateFPS() {
      float count = 0;

      while (true) {
        if (Time.timeScale == 1) {
          yield return WaitFor.Seconds(0.1f);
          count = (1 / Time.deltaTime);
          fps = (int) Mathf.Round(count);
        }
        else {
          fps = -1;
        }
        yield return WaitFor.Seconds(0.5f);
      }
    }

    // Calculate more cpu intensive stats as a slower rate than FPS
    IEnumerator CalculateStats() {
      GameObject[] gameObjects;

      while (true) {
        gameObjects = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        gameObjectCount = gameObjects.Length;

        // convert to MB (1024x1024)
        memory = System.GC.GetTotalMemory(false) / 1048576f;
        eventDelegates = EventManager.Instance.DelegateLookupCount;

        yield return WaitFor.Seconds(1f);
      }
    }


    IEnumerator UpdateDisplay() {
      while (true) {
        stats = string.Format("FPS:{0, -3} MB:{1, -3} GO:{2, -4} {3}x{4} ({5}x{6})  ED:{7} {8} {9}",
                fps.ToString(),
                string.Format("{0:f0}", memory),
                gameObjectCount,
                UnityEngine.Screen.width,
                UnityEngine.Screen.height,
                UnityEngine.Screen.currentResolution.width,
                UnityEngine.Screen.currentResolution.height,
                eventDelegates,
                TimeScale(),
                FetchKey()
              );
        labelStats.text = stats;
        yield return WaitFor.Seconds(0.5f);
      }
    }

    /// <summary>
    /// Return timeScale debug string but only if timeScale isn't at its default
    /// </summary>
    private string TimeScale() {
      if (Time.timeScale == 1.0f) {
        return "";
      }
      else {
        return string.Format("TS:{0:0.##}", Time.timeScale);
      }
    }

    private string FetchKey() {
      foreach (KeyCode keyCode in (KeyCode[]) System.Enum.GetValues(typeof(KeyCode)))  {
        if(Input.GetKey(keyCode)) {
          return (keyCode.ToString());
        }
      }

      return "";
    }
  }
}

