using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
#if UNITY_EDITOR
        Debug.Log("Editor only log");
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }
}
