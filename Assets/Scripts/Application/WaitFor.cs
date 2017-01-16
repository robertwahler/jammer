using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SDD;

namespace Jammer {

  /// <summary>
  /// Stock Unity wait rountines allocate memory each invocation causing GC.
  /// These routines are cached globally.
  /// </summary>
  /// <remarks>
  /// original idea: http://forum.unity3d.com/threads/c-coroutine-waitforseconds-garbage-collection-tip.224878/
  /// </remarks>
  public static class WaitFor {

    private const int DICTIONARY_START_SIZE = 50;

    /// <summary>
    /// Cache the Second() methods using the float time as key
    /// </summary>
    private static Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>(DICTIONARY_START_SIZE);

    /// <summary>
    /// Replaces: yield return new WaitForEndOfFrame()
    ///
    /// Usage:
    ///
    /// yield return WaitFor.EndOfFrame;
    /// </summary>
    static WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
    public static WaitForEndOfFrame EndOfFrame {
      get{ return _endOfFrame;}
    }

    /// <summary>
    /// Replaces: yield return new WaitForFixedUpdate()
    ///
    /// Usage:
    ///
    /// yield return WaitFor.FixedUpdate;
    /// </summary>
    static WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();
    public static WaitForFixedUpdate FixedUpdate{
      get{ return _fixedUpdate; }
    }

    /// <summary>
    /// Cached waiting
    ///
    /// Usage:
    ///
    /// yield return WaitFor.Seconds(5.0f);
    /// </summary>
    public static WaitForSeconds Seconds(float seconds){
      if(!_timeInterval.ContainsKey(seconds)) {
        _timeInterval.Add(seconds, new WaitForSeconds(seconds));
        if (_timeInterval.Count > DICTIONARY_START_SIZE) {
          Log.Warning(string.Format("WaitFor.WaitForSeconds(seconds: {0}) allocated extra dictionary slots. Problem? If not, increase starting size", seconds));
        }
      }
      return _timeInterval[seconds];
    }

  }
}
