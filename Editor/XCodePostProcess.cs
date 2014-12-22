using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public static class XCodePostProcess {
  [PostProcessBuild]
  public static void OnPostProcessBuild (BuildTarget target, string path) {
    Debug.Log(path);
    if (target == BuildTarget.iPhone) {
      UnityEditor.XCodeEditor.XCProject project = new UnityEditor.XCodeEditor.XCProject(path);

      string projModPath = System.IO.Path.Combine(Application.dataPath, "Editor");
      string[] files = System.IO.Directory.GetFiles(projModPath, "*.json", System.IO.SearchOption.AllDirectories);

      foreach (string file in files) {
        Debug.Log(file);
        project.ApplyMod(file);
      }

      // TODO is this needed always?
      project.AddOtherLDFlags("-ObjC");

      project.Save();
    }
  }
}
