using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private float rate = 1;
    private bool start = false;
    TextMeshProUGUI questionMark;
    void OnNextScene(InputValue v)
    {
        if (SceneManager.GetActiveScene().name == "Win Screen")
        {
            questionMark =  GameObject.Find("question").GetComponent<TextMeshProUGUI>();
            start = true;
            StartCoroutine(HoldUpForReveal());
        }
        else
            SceneManager.LoadScene("Area1");
    }
    IEnumerator HoldUpForReveal()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("Instructions");
    }
    private void FixedUpdate()
    {
        if(start)
        {
            questionMark.color = Color.Lerp(Color.black, Color.white, rate);
            rate += 0.05f;
        }
    }
}
