using UnityEngine;
using System.Collections;
using UnityEditor.Callbacks;
using UnityEditor;
using System.IO;
using System;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class SKBuildPostprocessor : IPreprocessBuildWithReport
{
    /// <summary>
    /// Returns the player build processor callback order.
    /// </summary>
    public int callbackOrder
    {
        get { return 1; }
    }

    public void OnPreprocessBuild(BuildReport report)
    {
#if UNITY_2019_2_OR_NEWER
        Debug.Log("Set EditorUserBuildSettings.androidCreateSymbolsZip : true");
        //EditorUserBuildSettings.androidCreateSymbolsZip = true;
#endif
    }

    [PostProcessBuildAttribute()]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        Debug.Log("OnPostprocessBuild : " + pathToBuiltProject + "   " + target);
#if UNITY_2019_2_OR_NEWER
        if (/*EditorUserBuildSettings.androidCreateSymbolsZip == */true)
        {
            Debug.Log("use EditorUserBuildSettings.androidCreateSymbolsZip to create symbols zip");
        }
        else
#endif
        {
            if (target == BuildTarget.Android)
                PostProcessAndroidBuild(pathToBuiltProject);
        }
    }

    public static void PostProcessAndroidBuild(string pathToBuiltProject)
    {
        UnityEditor.ScriptingImplementation backend = UnityEditor.PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android);

        if (backend == UnityEditor.ScriptingImplementation.IL2CPP)
        {
            CopyAndroidIL2CPPSymbols(pathToBuiltProject, PlayerSettings.Android.targetArchitectures);
        }
    }

    public static void CopyAndroidIL2CPPSymbols(string pathToBuiltProject, AndroidArchitecture targetArchi)
    {
        string buildName = Path.GetFileNameWithoutExtension(pathToBuiltProject);
        string symbolsDir = Path.GetDirectoryName(pathToBuiltProject);
        symbolsDir = symbolsDir + "/" + buildName + "_IL2CPPSymbols/";
        Debug.Log("CopyAndroidIL2CPPSymbols : " + symbolsDir + "   " + targetArchi.ToString());

        if(Directory.Exists(symbolsDir))
            Directory.Delete(symbolsDir, true);
        CreateDir(symbolsDir);

        if((targetArchi & AndroidArchitecture.ARMv7) > 0)
        {
            CopyARMV7Symbols(symbolsDir);
        }
        if ((targetArchi & AndroidArchitecture.ARM64) > 0)
        {
            CopyARMV8Symbols(symbolsDir);
        }
    }


    const string symbolpath = "/../Temp/StagingArea/symbols/";
    static readonly string[] symbolsName = { "libil2cpp.sym.so", "libil2cpp.dbg.so", "libunity.sym.so" };
    private static void CopyARMV7Symbols(string symbolsDir)
    {
        string v7Path = "armeabi-v7a/";
        CreateDir(symbolsDir + v7Path);
        for (int i = 0; i < symbolsName.Length; ++i)
        {
            string sourcefileARM = Application.dataPath + symbolpath + v7Path + symbolsName[i];
            Debug.Log("CopyARMV7Symbols : " + sourcefileARM);
            File.Copy(sourcefileARM, symbolsDir + "/" + v7Path + "/" + symbolsName[i]);
        }
    }

    private static void CopyARMV8Symbols(string symbolsDir)
    {
        string v8Path = "arm64-v8a/";
        CreateDir(symbolsDir + v8Path);
        for (int i = 0; i < symbolsName.Length; ++i)
        {
            string sourcefileARM = Application.dataPath + symbolpath + v8Path + symbolsName[i];
            Debug.Log("CopyARMV8Symbols : " + sourcefileARM);
            File.Copy(sourcefileARM, symbolsDir + "/" + v8Path + "/" + symbolsName[i]);
        }
    }

    public static void CreateDir(string path)
    {
        if (Directory.Exists(path))
            return;

        Directory.CreateDirectory(path);
    }
}