using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
 
[InitializeOnLoad]
public abstract class ManagedObject : ScriptableObject
{
    public abstract void OnBegin();
    public abstract void OnEnd();
 
#if UNITY_EDITOR
    
    [MenuItem("Tools/Persistent Folder")]
    private static void OpenPersistentFolder()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }

    protected void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayStateChange;
    }
 
    protected void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayStateChange;
    }
 
    void OnPlayStateChange(PlayModeStateChange state)
    {
        if(state == PlayModeStateChange.EnteredPlayMode)
        {
            OnBegin();
        }
        else if(state == PlayModeStateChange.ExitingPlayMode)
        {
            OnEnd();
        }
    }
#else
    protected void OnEnable()
    {
        OnBegin();
    }

    protected void OnDisable()
    {
        OnEnd();
    }
#endif
}