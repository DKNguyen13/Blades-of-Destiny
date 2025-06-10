using UnityEngine;
using UnityEngine.UI;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float cameraSpeed = 0.35f;
    [SerializeField] private Button next;
    [SerializeField] private Button prev;
    [SerializeField] private float distance = 18f;
    private float currentPosX;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        currentPosX = transform.position.x;
        next.onClick.AddListener(() => MoveBoard(true));
        prev.onClick.AddListener(() => MoveBoard(false));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = new Vector3(currentPosX, transform.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, cameraSpeed);
    }

    public void MoveBoard(bool check)
    {
        if(check == true)
        {
            currentPosX += distance;
        }
        else
        {
            if(currentPosX > distance)
            {
                currentPosX -= distance;
            }
        }
    }
}
