using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jammer.UI {

  /// <summary>
  /// Used to identify the GameObject menu containers at runtime
  /// </summary>
  public class Menu : BaseBehaviour {

    public MenuId Id;

    /// <summary>
    /// Return a string
    /// </summary>
    public override string ToString(){
      return string.Format("{0}, Id {1}", base.ToString(), Id);
    }
  }
}
