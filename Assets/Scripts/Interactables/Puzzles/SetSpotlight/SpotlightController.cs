using UnityEngine;

public class SpotlightController : MonoBehaviour
{
    [SerializeField] SetSpotlight _setSpotlight;
    [SerializeField] GameObject _spotlightMoveStand;
    [SerializeField] SlidingWallAnimation _slidingAnim;
    [SerializeField] float _rotationSpeed = 15.0f;
    [SerializeField] float _limitX = 32f;
    [SerializeField] float _limitY = 40f;

    bool _completed = false;
    bool IsTransformRotatedTowards(Vector3 direction)
    {
        float dotProduct = Vector3.Dot(transform.forward*-1, direction.normalized);
        float rotationThreshold = 0.999f;
        return dotProduct >= rotationThreshold;
    }

    void Update() // po prejdeni  uriètej rhanici sa vypne a prejde znova na movenemnt !!!SPAVIT REMAKE NA DOTWEEN DoLookAt
    {
        if (_completed) return;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 rotation = new Vector3(verticalInput, horizontalInput, 0) * _rotationSpeed * Time.deltaTime;

        Vector3 clampedRotation = new Vector3(
            Mathf.Clamp(Mathf.DeltaAngle(0, transform.rotation.eulerAngles.x + rotation.x), -_limitX, _limitX),
            transform.rotation.eulerAngles.y + rotation.y,
            0
        );

        transform.rotation = Quaternion.Euler(clampedRotation);

        Vector3 clampedStandRotation = new Vector3(
           0,
           transform.rotation.eulerAngles.y + rotation.y,
           0
        );

        _spotlightMoveStand.transform.rotation = Quaternion.Euler(clampedStandRotation);

        if (IsTransformRotatedTowards(_setSpotlight.SavedRandomDirection))
        {
            _completed = true;
            _slidingAnim.OpenDoor();
            Debug.Log("Transform is rotated towards the saved direction!");
        }

    }
}
