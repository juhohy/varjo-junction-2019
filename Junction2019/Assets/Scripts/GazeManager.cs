using UnityEngine;
using Varjo;

public class GazeManager : MonoBehaviour
{
    /// Use mouse to fake gaze position
    public bool mouseGaze = false;

    /// Collider that the gaze ray is hitting
    public Collider gazeHitTarget { get; private set; }

    public static GazeManager instance
    {
        get
        {
            if (!_instance)
            {
                _instance = new GameObject("GazeManager", typeof(GazeManager)).GetComponent<GazeManager>();
                _instance.hideFlags = HideFlags.DontSave;
            }
            return _instance;
        }
    }

    private static GazeManager _instance;

    private void Start()
    {
        if (!mouseGaze && !VarjoPlugin.InitGaze())
        {
            Debug.LogWarning("Failed to initialize gaze, enabling mouse gaze");
            mouseGaze = true;
        }
    }

    private void Update()
    {
        var data = VarjoPlugin.GetGaze();
        if (!mouseGaze && data.status == VarjoPlugin.GazeStatus.INVALID)
        {
            gazeHitTarget = null;
            return;
        }


        var headTransform = VarjoManager.Instance.HeadTransform;
        Ray gazeRay;
        if (!mouseGaze)
        {
            var gazeLocalPos = new Vector3((float)data.gaze.position[0], (float)data.gaze.position[1], (float)data.gaze.position[2]);
            var gazeLocalDir = new Vector3((float)data.gaze.forward[0], (float)data.gaze.forward[1], (float)data.gaze.forward[2]);
            gazeRay = new Ray(
                headTransform.TransformPoint(gazeLocalPos),
                headTransform.TransformVector(gazeLocalDir)
            );
        }
        else
        {
            gazeRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        Color debugColor;
        RaycastHit gazeRayHit;
        if (Physics.Raycast(gazeRay, out gazeRayHit))
        {
            gazeHitTarget = gazeRayHit.collider;
            debugColor = Color.green;
        }
        else
        {
            gazeHitTarget = null;
            debugColor = Color.red;
        }
        Debug.DrawLine(gazeRay.origin, gazeRay.origin + 10.0f * gazeRay.direction, debugColor);
    }
}
