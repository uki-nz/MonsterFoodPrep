using UnityEngine;
using System.Collections;

public class LoadScene : MonoBehaviour {
        
    public void GoToLevel(string level)
    {
        Application.LoadLevel(level);
    }
}
