using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour {

    public string scene;

    public void ChangeToScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }

    public void SetScene(string new_scene)
    {
        scene = new_scene;
    }
}
