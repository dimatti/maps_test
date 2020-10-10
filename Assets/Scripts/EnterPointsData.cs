using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EnterPointsData : MonoBehaviour
{
    public void OnClick()
    {
        StartCoroutine(StartProcess());
    }

    private IEnumerator StartProcess()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
