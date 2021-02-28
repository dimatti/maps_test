using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InfinityCode;
using UnityEngine.UI;
using System.Globalization;
using System;
using InfinityCode.OnlineMapsExamples;
using TMPro;

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
        public double lat;
        public double lon;
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
    public class RequestSimpleResult
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
    public Text resultGeneral;

    [SerializeField]
    public Transform compass;

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

    [SerializeField]
    public TMP_Text timeText;

    [SerializeField]
    public Texture2D markerDinstance1;
    [SerializeField]
    public Texture2D markerDinstance2;
    [SerializeField]
    public Texture2D markerDinstance3;
    [SerializeField]
    public Texture2D markerDinstance4;
    [SerializeField]
    public Texture2D markerDinstance5;
    [SerializeField]
    public Texture2D markerDinstance6;
    [SerializeField]
    public Texture2D markerDinstance7;
    [SerializeField]
    public Texture2D markerDinstance8;
    [SerializeField]
    public Texture2D markerDinstance9;
    [SerializeField]
    public Texture2D markerDinstance10;
    [SerializeField]
    public Texture2D markerDinstance11;
    [SerializeField]
    public Texture2D markerDinstance12;
    [SerializeField]
    public Texture2D markerDinstance13;
    [SerializeField]
    public Texture2D markerDinstance14;
    [SerializeField]
    public Texture2D markerDinstance15;
    [SerializeField]
    public Texture2D markerDinstance16;

    [SerializeField]
    public Sprite numb1;
    [SerializeField]
    public Sprite numb2;
    [SerializeField]
    public Sprite numb3;
    [SerializeField]
    public Sprite numb4;

    [SerializeField]
    public Image pointNumberIndicator;

    private float _timer;
    private float _waitTime;

    private float _timerShlik;
    private float _waitTimeShlik;

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

    private List<Point> statusPoints;

    private DateTime startTime;

    private int max_len = 4; //TODO FIX

    private bool isCheckingNow = false;


    // Start is called before the first frame update
    void Start()
    {
        currentGame = DataHolder.GameNumber;
        Status status;
        statusPoints = new List<Point>();
        if (currentGame != 0)
        {
            status = GetStatus(currentGame);
            markerStart = OnlineMapsMarkerManager.CreateItem(status.start.lon, status.start.lat, markerTextureStart, "Start");
            markerFinish = OnlineMapsMarkerManager.CreateItem(status.finish.lon, status.finish.lat, markerTextureFinish, "Finish");
            isGame = true;
            statusPoints = status.points;
        }
        _waitTime = 5f;
        _waitTimeShlik = 15f;
        marker1 = OnlineMapsMarkerManager.CreateItem(0, 0, markerTexture1, "My Marker 1");
        marker2 = OnlineMapsMarkerManager.CreateItem(0, 0, markerTexture2, "My Marker 2");
        marker3 = OnlineMapsMarkerManager.CreateItem(0, 0, markerTexture3, "My Marker 3");
        marker4 = OnlineMapsMarkerManager.CreateItem(0, 0, markerTexture4, "My Marker 4");
        markerUser = OnlineMapsMarkerManager.CreateItem(0, 0, markerTextureUser, "User");
        markerUser.align = OnlineMapsAlign.Center;
        markers = new OnlineMapsMarker[4] { marker1, marker2, marker3, marker4 };
        curreintPointIndex = 0;
        curreintPointPreIndex = 0;
        resultMiniGame = new ResultMiniGame();
        Input.compass.enabled = true;
        Input.location.Start();
        questionPanel.SetActive(false);
        startTime = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        var timeDiff = DateTime.Now - startTime;
        timeText.text = String.Format("{0}:{1}:{2}", timeDiff.Hours.ToString("D2"), timeDiff.Minutes.ToString("D2"), timeDiff.Seconds.ToString("D2"));
        //Convert.ToInt64(timeDiff.TotalSeconds).ToString();
        markerUser.SetPosition(map.position.x, map.position.y);
        markerUser.rotationDegree = -compassDiff;
        _timer += Time.deltaTime;
        _timerShlik += Time.deltaTime;

        if (_timerShlik > _waitTimeShlik)
        {
            if (isGame && !isAnswerMode)
            {
                NextPoint();
            }
            _timerShlik = 0f;
        }

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
        if (isCheckingNow)
        {
            return;
        }
        isCheckingNow = true;
        DistanceInfo di = GetDistances(currentGame, map.position.y, map.position.x);
        SPoint p = di.points[curreintPointIndex];
        max_len = di.points.Count;
        //result.text = string.Format("Point {0}", curreintPointIndex + 1);
        compassDiff = (float)p.f_az - 90;
        float d = 1500 - (float)p.distance;
        if (d < 0)
        {
            d = 0;
        }
        var isFinish = p.distance < 50f;
        markerUser.texture = TextureSelection(d / 1500, isFinish);
        if (isFinish)
        {
            if ((curreintPointIndex == 0 & !resultMiniGame.First) | (curreintPointIndex == 1 & !resultMiniGame.Second) | (curreintPointIndex == 2 & !resultMiniGame.Third) | (curreintPointIndex == 3 && !resultMiniGame.Fourth)) {
                Check();
            }

        }
        isCheckingNow = false;
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
            statusPoints = status.points;
        }
    }

    public void Check()
    {
        //RequestSimpleResult result;
        //result = UpdateInfo(currentGame, currentPoint, true, false, true, true);

        if (isGame) {
            CheckResult resultCheck = Check(currentGame, map.position.y, map.position.x);
            if (resultCheck.point != -1)
            {
                resultGeneral.text = string.Format("Point {0} was found!", curreintPointIndex + 1);
                OnlineMapsMarker m = markers[resultCheck.point];
                m.SetPosition(resultCheck.lon, resultCheck.lat);
                map.Redraw();
                Debug.Log("R");
                Debug.Log(resultCheck.point);
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
                    resultGeneral.text = "You are a winner!";
                }
            }
        }
    }

    public void SetQuestion(int index)
    {
        isAnswerMode = true;
        questionPanel.SetActive(true);
        Question result = GetQuestion(currentGame, index);
        question.text = result.question;
        tip.text = result.tip;
        tip.enabled = false;
        currentPoint = index;
    }

    public void OnAnswerClick()
    {
        isAnswerMode = false;
        questionPanel.SetActive(false);
        RequestSimpleResult res = CheckAnswer(currentGame, currentPoint, answer.text);
        if (res.result)
        {
            resultGeneral.text = "You answered correctly";
        } else
        {
            resultGeneral.text = "You answered wrong";
        }
    }

    public void OnShowTipClick()
    {
        tip.enabled = true;
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

    public RequestSimpleResult CheckAnswer(int gameId, int index, string answer)
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
        RequestSimpleResult myDeserializedClass = JsonUtility.FromJson<RequestSimpleResult>(status);
        return myDeserializedClass;
    }

    public RequestSimpleResult UpdateInfo(int gameId, int index, bool is_checked, bool is_answered_correctly, bool is_tip_used, bool is_help_used)
    {
        var Data = new WWWForm();
        Data.AddField("id", gameId);
        Data.AddField("index", index);
        Data.AddField("is_checked", BoolToInt(is_checked));
        Data.AddField("is_answered_correctly", BoolToInt(is_answered_correctly));
        Data.AddField("is_tip_used", BoolToInt(is_tip_used));
        Data.AddField("is_help_used", BoolToInt(is_help_used));
        var Query = new WWW("http://quizitor.pythonanywhere.com/mini_game/update_info/", Data);
        while (!Query.isDone)
        {

        }
        string status = Query.text;
        Debug.Log(status);
        RequestSimpleResult myDeserializedClass = JsonUtility.FromJson<RequestSimpleResult>(status);
        return myDeserializedClass;
    }

    private int BoolToInt(bool b)
    {
        if (b)
        {
            return 1;
        }
        return 0;
    }

    static double DegreeBearing(
    double lon1, double lat1,
    double lon2, double lat2)
    {
        var dLon = ToRad(lon2 - lon1);
        var dPhi = Math.Log(
            Math.Tan(ToRad(lat2) / 2 + Math.PI / 4) / Math.Tan(ToRad(lat1) / 2 + Math.PI / 4));
        if (Math.Abs(dLon) > Math.PI)
            dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);
        return ToBearing(Math.Atan2(dLon, dPhi));
    }

    public static double ToRad(double degrees)
    {
        return degrees * (Math.PI / 180);
    }

    public static double ToDegrees(double radians)
    {
        return radians * 180 / Math.PI;
    }

    public static double ToBearing(double radians)
    {
        // convert radians to degrees (as bearing: 0...360)
        return (ToDegrees(radians) + 360) % 360;
    }

    public Texture2D TextureSelection(double distance, bool isFinish)
    {
        if (isFinish)
        {
            return markerDinstance16;
        }
        if (distance < 0.0625) {
            return markerDinstance1;
        }
        if (distance < 2 * 0.0625)
        {
            return markerDinstance2;
        }
        if (distance < 3 * 0.0625)
        {
            return markerDinstance3;
        }
        if (distance < 4 * 0.0625)
        {
            return markerDinstance4;
        }
        if (distance < 5 * 0.0625)
        {
            return markerDinstance5;
        }
        if (distance < 6 * 0.0625)
        {
            return markerDinstance6;
        }
        if (distance < 7 * 0.0625)
        {
            return markerDinstance7;
        }
        if (distance < 8 * 0.0625)
        {
            return markerDinstance8;
        }
        if (distance < 9 * 0.0625)
        {
            return markerDinstance9;
        }
        if (distance < 10 * 0.0625)
        {
            return markerDinstance10;
        }
        if (distance < 11 * 0.0625)
        {
            return markerDinstance11;
        }
        if (distance < 12 * 0.0625)
        {
            return markerDinstance12;
        }
        if (distance < 13 * 0.0625)
        {
            return markerDinstance13;
        }
        if (distance < 14 * 0.0625)
        {
            return markerDinstance14;
        }
        return markerDinstance15;
    }

    public void PrevPoint()
    {
        STARTPREV:
        curreintPointIndex--;
        if (!isValidIndex(curreintPointIndex))
        {
            goto STARTPREV;
        }
        if (curreintPointIndex < 0)
        {
            curreintPointIndex = max_len - 1;
        }
        setPointNumberIndicator(curreintPointIndex);
        CheckInfo();
    }

    public void NextPoint()
    {
    STARTNEXT:
        curreintPointIndex++;
        if (!isValidIndex(curreintPointIndex))
        {
            goto STARTNEXT;
        }
        if (curreintPointIndex >= max_len)
        {
            curreintPointIndex = 0;
        }
        setPointNumberIndicator(curreintPointIndex);
        CheckInfo();
    }

    private bool isValidIndex(int index)
    {
        if ((index == 0 & resultMiniGame.First) | (index == 1 & resultMiniGame.Second) | (index == 2 & resultMiniGame.Third) | (index == 3 && resultMiniGame.Fourth))
        {
            return false;
        }
        return true;
    }

    private void setPointNumberIndicator(int n)
    {
        if (n == 0)
        {
            pointNumberIndicator.sprite = numb1;
        } 
        else if (n==1)
        {
            pointNumberIndicator.sprite = numb2;
        }
        else if (n == 2)
        {
            pointNumberIndicator.sprite = numb3;
        } else
        {
            pointNumberIndicator.sprite = numb4;
        }
    }
}
