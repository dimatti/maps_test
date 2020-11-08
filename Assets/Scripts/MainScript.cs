using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InfinityCode;
using UnityEngine.UI;
using System.Globalization;
using System;
using InfinityCode.OnlineMapsExamples;

public class MainScript : MonoBehaviour
{
    [Serializable]
    public class StartPoint
    {
        public double lat;
        public double lon;
    }
    [Serializable]
    public class FinishPoint
    {
        public double lat;
        public double lon;
    }

    [Serializable]
    public class Coordinates
    {
        public double? lat;
        public double? lon;
    }

    [Serializable]
    public class Point
    {
        public string question;
        public string answer;
        public string tip;
        public bool is_answered_correctly;
        public bool is_help_used;
        public bool is_tip_used;
        public object time_completed;
        public bool is_checked;
        public Coordinates coordinates;
    }


    [Serializable]
    public class Status
    {
        public int id;
        public StartPoint start;
        public FinishPoint finish;
        public List<Point> points;
    }

    [Serializable]
    public class CheckResult
    {
        public int point;
        public bool result;
        public double lat;
        public double lon;
    }
    [Serializable]
    public class SPoint
    {
        public double f_az;
        public double b_az;
        public double distance;
    }
    [Serializable]
    public class DistanceInfo
    {
        public List<SPoint> points;
    }

    [Serializable]
    public class Question
    {
        public string question;
        public string tip;
    }
    [Serializable]
    public class CheckQuestion
    {
        public bool result;
    }

    public class ResultMiniGame
    {
        public bool First;
        public bool Second;
        public bool Third;
        public bool Fourth;
    }

    [SerializeField]
    private OnlineMaps map;
    [SerializeField]
    public Texture2D markerTexture1;
    [SerializeField]
    public Texture2D markerTexture2;
    [SerializeField]
    public Texture2D markerTexture3;
    [SerializeField]
    public Texture2D markerTexture4;
    [SerializeField]
    public Texture2D markerTextureStart;
    [SerializeField]
    public Texture2D markerTextureFinish;
    [SerializeField]
    public Texture2D markerTextureUser;


    [SerializeField]
    public InputField inputCluster;
    [SerializeField]
    public InputField inputCurrentGame;

    [SerializeField]
    public Text result;

    [SerializeField]
    public Text resultCurrent;

    [SerializeField]
    public Transform compass;

    [SerializeField]
    public Image image;

    [SerializeField]
    public GameObject questionPanel;

    [SerializeField]
    public Text question;

    [SerializeField]
    public Text tip;

    [SerializeField]
    public InputField answer;

    [SerializeField]
    public Text resultCurrentQuestion;

    private float _timer;
    private float _waitTime;

    private int cluster;
    private int currentGame;
    private int curreintPointIndex;
    private int curreintPointPreIndex;


    private OnlineMapsMarker markerStart;
    private OnlineMapsMarker markerFinish;
    private OnlineMapsMarker marker1;
    private OnlineMapsMarker marker2;
    private OnlineMapsMarker marker3;
    private OnlineMapsMarker marker4;
    private OnlineMapsMarker markerUser;
    private bool isGame;
    private OnlineMapsMarker[] markers;
    private int resultCount;

    private ResultMiniGame resultMiniGame;

    private float compassDiff;

    private bool isAnswerMode;

    private int currentPoint;


    // Start is called before the first frame update
    void Start()
    {
        _waitTime = 3f;
        marker1 = OnlineMapsMarkerManager.CreateItem(0, 0, markerTexture1, "My Marker 1");
        marker2 = OnlineMapsMarkerManager.CreateItem(0, 0, markerTexture2, "My Marker 2");
        marker3 = OnlineMapsMarkerManager.CreateItem(0, 0, markerTexture3, "My Marker 3");
        marker4 = OnlineMapsMarkerManager.CreateItem(0, 0, markerTexture4, "My Marker 4");
        markerUser = OnlineMapsMarkerManager.CreateItem(0, 0, markerTextureUser, "User");

        inputCluster.text = "0";
        inputCurrentGame.text = "0";
        markers = new OnlineMapsMarker[4] { marker1, marker2, marker3, marker4 };
        curreintPointIndex = 0;
        curreintPointPreIndex = 0;
        resultMiniGame = new ResultMiniGame();
        Input.compass.enabled = true;
        Input.location.Start();
        questionPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        markerUser.SetPosition(map.position.x, map.position.y);
        markerUser.rotationDegree = -compassDiff;
        //resultCurrent.text = string.Format("{0}", (int)(Input.compass.trueHeading/5)*5);
        //compass.eulerAngles = new Vector3(0, 0, Input.compass.trueHeading + compassDiff);
        _timer += Time.deltaTime;

        if (_timer >= _waitTime)
        {
            if (isGame && !isAnswerMode)
            {
                CheckInfo();
            }
            
            _timer = 0f;

        }
    }

    private void CheckInfo()
    {
        DistanceInfo di = GetDistances(currentGame, map.position.y, map.position.x);
        Debug.Log(di.points);
        SPoint p = di.points[curreintPointIndex];
        int max_len = di.points.Count;
        //result.text = string.Format("Point {0} back azimuth {1}; forward azimuth {2}; distance {3}", curreintPointIndex+1, p.b_az, p.f_az, p.distance);
        result.text = string.Format("Point {0}", curreintPointIndex + 1);
        compass.eulerAngles = new Vector3(0, 0, (float)p.f_az - 90);
        compassDiff = (float)p.f_az - 90;
        float d = 2000 - (float)p.distance;
        if (d < 0)
        {
            d = 0;
        }
        image.fillAmount = d/2000;
        image.color = Color.yellow;
        if (p.distance < 50f)
        {
            image.color = Color.red;
        }
        curreintPointPreIndex++;
        if (curreintPointPreIndex == 3)
        {
            curreintPointIndex++;
            if (curreintPointIndex == max_len)
            {
                curreintPointIndex = 0;
            }
            curreintPointPreIndex = 0;
        }
        

    }

    public void OnClick()
    {
        /*
        lon1 = float.Parse(inputLon1.text, CultureInfo.InvariantCulture.NumberFormat);
        lat1 = float.Parse(inputLat1.text, CultureInfo.InvariantCulture.NumberFormat);
        marker1.SetPosition(lon1, lat1);
        map.Redraw();


        lon2 = float.Parse(inputLon2.text, CultureInfo.InvariantCulture.NumberFormat);
        lat2 = float.Parse(inputLat3.text, CultureInfo.InvariantCulture.NumberFormat);
        marker2.SetPosition(lon2, lat2);
        map.Redraw();

        lon3 = float.Parse(inputLon3.text, CultureInfo.InvariantCulture.NumberFormat);
        lat3 = float.Parse(inputLat3.text, CultureInfo.InvariantCulture.NumberFormat);
        marker3.SetPosition(lon3, lat3);
        map.Redraw();
        */

        cluster = int.Parse(inputCluster.text);
        currentGame = int.Parse(inputCurrentGame.text);
        Status status;
        if (currentGame != 0) {
            status = GetStatus(currentGame);
            markerStart = OnlineMapsMarkerManager.CreateItem(status.start.lon, status.start.lat, markerTextureStart, "Start");
            markerFinish = OnlineMapsMarkerManager.CreateItem(status.finish.lon, status.finish.lat, markerTextureFinish, "Finish");
            isGame = true;
        }
    }

    public void Check()
    {
        if (isGame) {
            CheckResult resultCheck = Check(currentGame, map.position.y, map.position.x);
            if (resultCheck.point != -1)
            {
                resultCurrent.text = string.Format("Point {0} was found!", resultCheck.point+1);
                OnlineMapsMarker m = markers[resultCheck.point];
                m.SetPosition(resultCheck.lon, resultCheck.lat);
                map.Redraw();
                SetQuestion(resultCheck.point);
                if (resultCheck.point == 0)
                {
                    resultMiniGame.First = true;
                }
                if (resultCheck.point == 1)
                {
                    resultMiniGame.Second = true;
                }
                if (resultCheck.point == 2)
                {
                    resultMiniGame.Third = true;
                }
                if (resultCheck.point == 3)
                {
                    resultMiniGame.Fourth = true;
                }
                if (resultMiniGame.First && resultMiniGame.Second && resultMiniGame.Third && resultMiniGame.Fourth)
                {
                    isGame = false;
                    result.text = "You are a winner!";
                }
            }
        }
    }

    public void SetQuestion(int index)
    {
        isAnswerMode = true;
        questionPanel.SetActive(true);
        Debug.Log(index);
        Question result = GetQuestion(currentGame, index);
        question.text = result.question;
        tip.text = result.tip;
        currentPoint = index;
    }

    public void OnAnswerClick()
    {
        isAnswerMode = false;
        questionPanel.SetActive(false);
        CheckQuestion res = CheckAnswer(currentGame, currentPoint, answer.text);
        if (res.result)
        {
            resultCurrentQuestion.text = "You answered correctly";
        } else
        {
            resultCurrentQuestion.text = "You answered wrong";
        }
    }

    public Status GetStatus(int gameId)
    {
        var Data = new WWWForm();
        Data.AddField("id", gameId);
        var Query = new WWW("http://quizitor.pythonanywhere.com/mini_game/get_status/", Data);
        while (!Query.isDone) {

        }
        string status = Query.text;
        Status myDeserializedClass = JsonUtility.FromJson<Status>(status);
        return myDeserializedClass;
    }

    public CheckResult Check(int gameId, double lat, double lon)
    {
        var Data = new WWWForm();
        Data.AddField("id", gameId);
        Data.AddField("lat", lat.ToString(CultureInfo.InvariantCulture));
        Data.AddField("lon", lon.ToString(CultureInfo.InvariantCulture));
        var Query = new WWW("http://quizitor.pythonanywhere.com/mini_game/check/", Data);
        while (!Query.isDone)
        {

        }
        CheckResult myDeserializedClass = JsonUtility.FromJson<CheckResult>(Query.text);
        return myDeserializedClass;
    }

    public DistanceInfo GetDistances(int gameId, double lat, double lon)
    {
        var Data = new WWWForm();
        Data.AddField("id", gameId);
        Data.AddField("lat", lat.ToString(CultureInfo.InvariantCulture));
        Data.AddField("lon", lon.ToString(CultureInfo.InvariantCulture));
        var Query = new WWW("http://quizitor.pythonanywhere.com/mini_game/get_distances/", Data);
        while (!Query.isDone)
        {

        }
        string status = Query.text;
        Debug.Log(status);
        DistanceInfo myDeserializedClass = JsonUtility.FromJson<DistanceInfo>(status);
        return myDeserializedClass;
    }

    public Question GetQuestion(int gameId, int index)
    {
        var Data = new WWWForm();
        Data.AddField("id", gameId);
        Data.AddField("index", index);
        var Query = new WWW("http://quizitor.pythonanywhere.com/mini_game/get_question/", Data);
        while (!Query.isDone)
        {

        }
        string status = Query.text;
        Debug.Log(status);
        Question myDeserializedClass = JsonUtility.FromJson<Question>(status);
        return myDeserializedClass;
    }

    public CheckQuestion CheckAnswer(int gameId, int index, string answer)
    {
        var Data = new WWWForm();
        Data.AddField("id", gameId);
        Data.AddField("index", index);
        Data.AddField("answer", answer);
        var Query = new WWW("http://quizitor.pythonanywhere.com/mini_game/check_answer/", Data);
        while (!Query.isDone)
        {

        }
        string status = Query.text;
        Debug.Log(status);
        CheckQuestion myDeserializedClass = JsonUtility.FromJson<CheckQuestion>(status);
        return myDeserializedClass;
    }

}
