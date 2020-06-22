using UnityEditor;
using UnityEngine;
//using UnityEditor.Build.Reporting;
using System;

public class BuildHelper : MonoBehaviour
{
    static void BuildServer()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/Not in current build/Server.unity" };
        buildPlayerOptions.locationPathName = "Builds/WindowsServerBuild/cwserver.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.EnableHeadlessMode;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        /*
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build result succeeded");
            Console.WriteLine("Build result succeeded");

        }
        else
        {
            Debug.Log("Build result: " + summary.result);
            Console.WriteLine("Build result: " + summary.result);
        }*/
    }

    static void BuildLinuxServer()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/Not in current build/Server.unity" };
        buildPlayerOptions.locationPathName = "Builds/LinuxServerBuild/cwserverl.x86_64";
        buildPlayerOptions.target = BuildTarget.StandaloneLinux64;
        buildPlayerOptions.options = BuildOptions.EnableHeadlessMode;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    static void BuildWindowsClient()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] 
        {
            "Assets/Scenes/" + ScenesEnum.MainMenu + ".unity",
            "Assets/Scenes/" + ScenesEnum.HotSeatGameMode + ".unity",
            "Assets/Scenes/" + ScenesEnum.MMDeckSelect + ".unity",
            "Assets/Scenes/" + ScenesEnum.DeckBuilder + ".unity",
        };
        buildPlayerOptions.locationPathName = "Builds/WindowsClientBuild/cwclient.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.ShowBuiltPlayer | BuildOptions.Development;
        BuildPipeline.BuildPlayer(buildPlayerOptions);

    }
}
