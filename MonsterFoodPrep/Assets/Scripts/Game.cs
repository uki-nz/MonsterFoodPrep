using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
   public static Game game
    {
        get
        {
            return _game;
        }
    }
    private static Game _game;

    void Awake()
    {
        _game = this;
    }
}
