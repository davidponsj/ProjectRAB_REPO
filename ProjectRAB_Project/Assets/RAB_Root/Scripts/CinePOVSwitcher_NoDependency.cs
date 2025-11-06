using System.Reflection;
using UnityEngine;

public class CinePOVSwitcher_NoDependency : MonoBehaviour
{
    [Header("Assign GameObjects of your Cinemachine virtual cameras")]
    public GameObject normalCamObject;
    public GameObject povCamObject;
    public Transform player;
    public LayerMask obstacleMask;
    public float detectDistance = 6f;
    public float detectHeight = 1.2f;
    public bool activateOnJump = true;

    // reflection cached
    PropertyInfo normalPriorityProp;
    PropertyInfo povPriorityProp;
    Component normalCamComponentWithProp;
    Component povCamComponentWithProp;

    void Start()
    {
        // Try to find a component on each GameObject that exposes "Priority" property (Cinemachine types do)
        if (normalCamObject != null)
        {
            normalCamComponentWithProp = FindComponentWithProperty(normalCamObject, "Priority");
            if (normalCamComponentWithProp != null)
                normalPriorityProp = normalCamComponentWithProp.GetType().GetProperty("Priority");
        }

        if (povCamObject != null)
        {
            povCamComponentWithProp = FindComponentWithProperty(povCamObject, "Priority");
            if (povCamComponentWithProp != null)
                povPriorityProp = povCamComponentWithProp.GetType().GetProperty("Priority");
        }
    }

    void Update()
    {
        if (player == null) return;

        Vector3 origin = player.position + Vector3.up * detectHeight;
        Vector3 dir = player.forward;

        bool obstacleAhead = Physics.Raycast(origin, dir, detectDistance, obstacleMask);
        bool jumpPressed = activateOnJump && Input.GetButton("Jump");
        bool usePOV = obstacleAhead || jumpPressed;

        if (usePOV)
            SetPOVActive();
        else
            SetNormalActive();

        Debug.DrawRay(origin, dir * detectDistance, obstacleAhead ? Color.red : Color.green);
    }

    void SetPOVActive()
    {
        // try setting Priority via reflection
        if (povPriorityProp != null && normalPriorityProp != null)
        {
            povPriorityProp.SetValue(povCamComponentWithProp, 20, null);
            normalPriorityProp.SetValue(normalCamComponentWithProp, 10, null);
            return;
        }

        // fallback: enable/disable gameobjects
        if (povCamObject != null) povCamObject.SetActive(true);
        if (normalCamObject != null) normalCamObject.SetActive(false);
    }

    void SetNormalActive()
    {
        if (povPriorityProp != null && normalPriorityProp != null)
        {
            povPriorityProp.SetValue(povCamComponentWithProp, 10, null);
            normalPriorityProp.SetValue(normalCamComponentWithProp, 20, null);
            return;
        }

        if (povCamObject != null) povCamObject.SetActive(false);
        if (normalCamObject != null) normalCamObject.SetActive(true);
    }

    Component FindComponentWithProperty(GameObject go, string propName)
    {
        var comps = go.GetComponents<Component>();
        foreach (var c in comps)
        {
            if (c == null) continue;
            var pi = c.GetType().GetProperty(propName);
            if (pi != null) return c;
        }
        return null;
    }
}
