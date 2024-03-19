using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneListNameSpace;

public static class Loader 
{
    private class LoadingMonoBehavior : MonoBehaviour { };

    // Deligate that returns void
    private static Action onLoaderCallBack;
    private static AsyncOperation loadingAsyncOperation;

    public static void Load(Scenes scene)
    {
        /* 
         * onLoaderCallBack = () => { SceneManager.LoadScene(scene.ToString()); };
         * This line assigns an anonymous function to the onLoaderCallBack delegate.
         * The => syntax denotes a lambda expression, defining an inline function without explicitly declaring a separate method. 
         * Inside the lambda expression, SceneManager.LoadScene(scene.ToString()) is called,
         * which loads the specified scene.
         */

        // Set the loader callback action to load the target scene (just defining an inline function)
        onLoaderCallBack = () => {
            GameObject loadingGameObject = new GameObject("Loading Game Object");
            // LoadSceneAsync will load the target screne async
            // supposed to keep the game running while the next scene is loading
            // need a new game object to run a coroutine
            loadingGameObject.AddComponent<LoadingMonoBehavior>().StartCoroutine(LoadSceneAsync(scene));
        }; 
        // Load the loading Scene
        SceneManager.LoadScene(Scenes.LoadingScene.ToString());
    }
    public static IEnumerator LoadSceneAsync(Scenes scene)
    {
        yield return null; // wait one frame
        loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());
        while (!loadingAsyncOperation.isDone)
        {
            yield return null; // wait one frame
        }
    }
    public static float GetLoadingProgress()
    {
        if(loadingAsyncOperation != null)
        {
            return loadingAsyncOperation.progress;
        }
        else
        {
            return 1f;
        }
    }
    // Called after the screen refreshes
    public static void LoaderCallback()
    {
        // Triggered after the first update which lets the screen refresh
        // Exeecute the loader callback action which will load the target scene
        if (onLoaderCallBack != null)
        {
            onLoaderCallBack(); // this runs the inline function we made
            onLoaderCallBack = null;
        }
    }
}
