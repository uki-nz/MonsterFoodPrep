// Modified from Brad Key's script
using System.Collections.Generic;
using UnityEngine;

public enum Swipe { None, Up, Down, Left, Right };

public class SwipeManager : MonoBehaviour
{
    public float minSwipeLength = 200f;
    List<Vector2> swipePositions;
    List<Vector2> lastSwipePosition;
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;
    public RectTransform imageRectTransform;

    public static Swipe swipeDirection;

    void Start()
    {
        swipePositions = new List<Vector2>(100);
    }

    void Update()
    {
        DetectSwipe();
        if (swipeDirection != Swipe.None)
        {
            DrawSwipeLine();
        }
    }

    public void DetectSwipe()
    {
        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                print("TouchBegan");
                swipePositions.Clear();
                firstPressPos = new Vector2(t.position.x, t.position.y);
                swipePositions.Add(firstPressPos);
            }

            if (t.phase == TouchPhase.Moved)
            {
                swipePositions.Add(new Vector2(t.position.x, t.position.y));
            }

            if (t.phase == TouchPhase.Ended)
            {
                print("TouchEnded");
                secondPressPos = new Vector2(t.position.x, t.position.y);
                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
                swipePositions.Add(secondPressPos);

                // Make sure it was a legit swipe, not a tap
                if (currentSwipe.magnitude < minSwipeLength)
                {
                    swipeDirection = Swipe.None;
                    return;
                }

                currentSwipe.Normalize();

                // Swipe up
                if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f) {
                    swipeDirection = Swipe.Up;
                    print("SwipeUp");
                    // Swipe down
                } else if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f) {
                    swipeDirection = Swipe.Down;
                    print("SwipeDown");
                    // Swipe left
                } else if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f) {
                    swipeDirection = Swipe.Left;
                    print("SwipeLeft");
                    // Swipe right
                } else if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f) {
                    swipeDirection = Swipe.Right;
                    print("SwipeRight");
                }
            }
        }
        else
        {
            swipeDirection = Swipe.None;
        }
    }

    // Modified from DanSuperGP's script
    void DrawSwipeLine()
    {
        Vector3 differenceVector = secondPressPos - firstPressPos;
        float lineWidth = 10f;

        imageRectTransform.sizeDelta = new Vector2(differenceVector.magnitude, lineWidth);
        differenceVector.Normalize();
        imageRectTransform.pivot = new Vector2(0, 0.5f);
        imageRectTransform.position = firstPressPos;
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        imageRectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }    
}