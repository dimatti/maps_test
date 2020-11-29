using UnityEngine;

public class DataHolder
{
    private static int gameNumber;

    public static int GameNumber
    {
        get
        {
            return gameNumber;
        }
        set
        {
            gameNumber = value;
        }
    }
}
