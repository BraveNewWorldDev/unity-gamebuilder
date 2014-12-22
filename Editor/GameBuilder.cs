using UnityEditor;
using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

class GameBuilder {
  static List<string> knownTargets = new List<string> {
    "osx",
    "windows",
    "linux",
    "web",
    "ios",
    "android"
  };

  static List<string> scenes = new List<string>();
  static List<string> targets = new List<string>();
  static List<string> excludedTargets = new List<string>();

  //static BuildOptions options = BuildOptions.Development | BuildOptions.WebPlayerOfflineDeployment;
  static BuildOptions options = BuildOptions.WebPlayerOfflineDeployment;
  static BuildTarget CurrentBuildTarget = BuildTarget.WebPlayer;
  static string CurrentBuildPath = null;

  static string ProjectName { get {
    string ret = System.Environment.GetEnvironmentVariable("UNITY_BUILD_NAME");

    if (string.IsNullOrEmpty(ret)) {
      throw new Exception("Please set `UNITY_BUILD_NAME`");
    }

    return ret;
  } }

  static void Build () {
    //setKeystorePasswords();
    buildSceneList();
    buildTargetList();

    foreach (string targetName in targets) {
      if (!knownTargets.Contains(targetName)
          || excludedTargets.Contains(targetName)) {
        continue;
      }

      System.Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
      System.Console.WriteLine(string.Format("Starting build for {0}", targetName));

      switch (targetName) {
        case "osx":
          runBuildFor(BuildTarget.StandaloneOSXUniversal, "osx/{0}.app");
          runCommand("zip", "-q -r -9 \"../{0}-osx.zip\" \"{0}.app\"", new List<string> {
              "wd", "build/osx"
              });

          Directory.Delete("build/osx", true);
          tryRunAfterCommand("UNITY_AFTER_OSX");
          break;

        case "windows":
          runBuildFor(BuildTarget.StandaloneWindows, "windows/{0}/{0}.exe");
          runCommand("zip", "-q -r -9 \"../{0}-windows.zip\" \"{0}\"", new List<string> {
              "wd", "build/windows"
              });

          Directory.Delete("build/windows", true);
          tryRunAfterCommand("UNITY_AFTER_WINDOWS");
          break;

        case "linux":
          runBuildFor(BuildTarget.StandaloneLinuxUniversal, "linux/{0}/{0}");
          runCommand("tar", "czf \"../{0}-linux.tar.gz\" \"{0}\"", new List<string> {
              "wd", "build/linux",
              "GZIP", "-9"
              });

          Directory.Delete("build/linux", true);
          tryRunAfterCommand("UNITY_AFTER_LINUX");
          break;

        case "web":
          runBuildFor(BuildTarget.WebPlayer, "web/game");
          tryRunAfterCommand("UNITY_AFTER_WEB");
          break;

        case "ios":
          runBuildFor(BuildTarget.iPhone, "xcode");
          tryRunAfterCommand("UNITY_AFTER_IOS");
          break;

        case "android":
          runBuildFor(BuildTarget.Android, "{0}.apk");
          tryRunAfterCommand("UNITY_AFTER_ANDROID");
          break;
      }

      System.Console.WriteLine(string.Format("Finished build for {0}", targetName));
      System.Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
    }
  }

  static void runBuildFor (BuildTarget buildTarget, string buildPath) {
    CurrentBuildTarget = buildTarget;
    CurrentBuildPath = Directory.GetCurrentDirectory()
      + string.Format("/build/" + buildPath, ProjectName);

    Directory.CreateDirectory(Path.GetDirectoryName(CurrentBuildPath));

    string error = BuildPipeline.BuildPlayer(scenes.ToArray(),
        CurrentBuildPath, CurrentBuildTarget, options);

    if (!string.IsNullOrEmpty(error)) {
      throw new Exception(error);
    }

  }

  static void tryRunAfterCommand (string envVariableName) {
    string afterCommand = System.Environment.GetEnvironmentVariable(envVariableName);

    if (string.IsNullOrEmpty(afterCommand)) {
      return;
    }

    runCommand("bash", afterCommand, new List<string> {
        "wd", Directory.GetCurrentDirectory()
        });
  }

  static Process runCommand (string command, string args = "",
      List<string> envPairs = null) {

    Process runner = new Process();
    runner.StartInfo.FileName = command;
    runner.StartInfo.Arguments = string.Format(args, ProjectName);
    runner.StartInfo.UseShellExecute = false;

    if (envPairs == null) {
      envPairs = new List<string>();
    }

    for (int i = 0; i < envPairs.Count; i += 2) {
      string key = envPairs[i];
      string value = envPairs[i + 1];

      if (key == "wd") {
        runner.StartInfo.WorkingDirectory = value;
        continue;
      }

      runner.StartInfo.EnvironmentVariables[key] = value;
    }

    runner.Start();
    runner.WaitForExit();

    return runner;
  }

  //static void setKeystorePasswords () {
    //string keyaliasPass = System.Environment.GetEnvironmentVariable("UNITY_KEYALIAS_PASSWORD");
    //string keystorePass = System.Environment.GetEnvironmentVariable("UNITY_KEYSTORE_PASSWORD");

    //if (string.IsNullOrEmpty(keyaliasPass)
        //|| string.IsNullOrEmpty(keystorePass)) {
      //return;
    //}

    //PlayerSettings.keyaliasPass = keyaliasPass;
    //PlayerSettings.keystorePass = keystorePass;
  //}

  static void buildTargetList () {
    string envRequestedTargets = System.Environment.GetEnvironmentVariable("UNITY_BUILD_TARGETS");
    string envExcludedTargets = System.Environment.GetEnvironmentVariable("UNITY_BUILD_EXCLUDE");

    if (string.IsNullOrEmpty(envRequestedTargets)) {
      envRequestedTargets = "all";
    }

    if (envRequestedTargets == "all") {
      envRequestedTargets = string.Join(" ", knownTargets.ToArray());
    }

    if (!string.IsNullOrEmpty(envExcludedTargets)) {
      excludedTargets = new List<string>(envExcludedTargets
        .Replace("\"", "")
        .Replace("\r", " ")
        .Replace("\n", " ")
        .Split(null));
    }

    targets = new List<string>(envRequestedTargets
        .Replace("\"", "")
        .Replace("\r", " ")
        .Replace("\n", " ")
        .Split(null));
  }

  static void buildSceneList () {
    foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
      if (scene.enabled) {
        scenes.Add(scene.path);
      }
    }
  }
}
