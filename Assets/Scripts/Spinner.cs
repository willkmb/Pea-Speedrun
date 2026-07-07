using UnityEngine;
using System.Collections;
public class Spinner : MonoBehaviour
{
    [SerializeField] float startSpeed = 1000f;
    [SerializeField] float deceleration = 300f;
    [SerializeField] string[] sectionNames = { "Red", "Blue", "Green", "Yellow" };

    public void Spin()
    {
        StartCoroutine(spin());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) StartCoroutine(spin());
    }

    IEnumerator spin()
    {
        float speed = startSpeed * Random.Range(0.85f, 1.15f);

        while (speed > 0f)
        {
            speed -= deceleration * Time.deltaTime;
            if (speed < 0f) speed = 0f;

            transform.eulerAngles += new Vector3(0, 0, speed * Time.deltaTime);
            yield return null;
        }

        float finalAngle = transform.eulerAngles.z % 360f;
        float sectionSize = 360f / sectionNames.Length;
        int landed = Mathf.FloorToInt(finalAngle / sectionSize);

        Debug.Log("Landed on: " + sectionNames[landed]);
    }
}