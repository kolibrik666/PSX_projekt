using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class SpotlightController : MonoBehaviour
{
    [SerializeField] SetSpotlight _setSpotlight;
    [SerializeField] GameObject _spotlightMoveStand;
    [SerializeField] SlidingWallAnimation _slidingAnim;
    [SerializeField] float _rotationSpeed = 15.0f;
    [SerializeField] float _limitX = 32f;
    //[SerializeField] float _limitY = 40f;

    float _rotationThreshold = 0.9999f;
    bool _puzzleCompleted = false;
    bool _firstRun = true;
    int _multiplier = 1;
    
    public bool PuzzleCompleted => _puzzleCompleted;
    private void OnEnable()
    {
        if (!_firstRun) return;
        if (transform.forward.z > 0.5f) _multiplier *= -1;
        //Debug.Log(transform.forward.z);
        _firstRun = false;
    }
    bool IsTransformRotatedTowards(Vector3 direction)
    {
        float dotProduct = Vector3.Dot(transform.forward * _multiplier, direction.normalized);        
        return dotProduct >= _rotationThreshold;
    }

    void Update() // po prejdeni  uriètej rhanici sa vypne a prejde znova na movenemnt !!!SPAVIT REMAKE NA DOTWEEN DoLookAt
    {
        if (PuzzleCompleted)
        {

            _setSpotlight.ChangeControl(true);
            return;
        }
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
            _puzzleCompleted = true;
            _slidingAnim.OpenDoor();
            Debug.Log("Transform is rotated towards the saved direction!");
        }

    }
}
