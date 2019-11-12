using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneUIMgr : Singleton<SceneUIMgr>
{
    public string scenePrefabPath = "SceneUI/{0}";
    public enum Scene { NONE, GAMESTART, GAMEOVER };

    private Scene curScene = Scene.NONE;
    private GameObject sceneUI = null;

    // ========================================================== GET/ SET ==========================================================
    public bool IsShowing() { return curScene != Scene.NONE; }

    // ========================================================== UNITY FUNC ==========================================================
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
    }

    // ========================================================== PUBLIC FUNC ==========================================================
    public void ShowScene(Scene _scene)
    {
        TopPanelMgr.Instance.SetActive(false);

        GameObject prefScene = Resources.Load<GameObject>(scenePrefabPath.Replace("{0}", _scene.ToString().ToLower()));
        if (prefScene)
        {
            curScene = _scene;
            sceneUI = Instantiate(prefScene, transform);
        }
    }

    public void HideScene(Scene _scene)
    {
        TopPanelMgr.Instance.SetActive(true);

        if (curScene == _scene && sceneUI)
        {
            curScene = Scene.NONE;
            Destroy(sceneUI);
        }
    }
}
