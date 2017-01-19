using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SDD;

namespace Jammer.UI {

  /// <summary>
  /// Assign this behaviour to automatically set the version to the prefab Version.txt
  /// </summary>
  public class VersionText : BaseText {

    void OnEnable() {
      Log.Debug(string.Format("VersionText.OnEnable() ID={0}", GetInstanceID()));
      Text.text = string.Format("Version {0} {1}", ApplicationHelper.Version.ToString(), ApplicationHelper.Version.BuildFlags);
    }

  }
}
