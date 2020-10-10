using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GetStatus()
    {
		StartCoroutine(POST());

	}

	public IEnumerator POST()
	{
		var Data = new WWWForm();
		Data.AddField("id", 1); 
		var Query = new WWW("http://quizitor.pythonanywhere.com/mini_game/get_status/", Data);
		yield return Query;
		if (Query.error != null)
		{
			Debug.Log("Server does not respond : " + Query.error);
		}
		else
		{
			Debug.Log(Query.text);
		}
		Query.Dispose();
	}
}
