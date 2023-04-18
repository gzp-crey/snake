using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// Keep track of the path of a single snake part.
public class SnakePathManager : MonoBehaviour
{
    public class Marker
    {
        public Vector3 position;
        public Quaternion rotation;
        public GameObject debug;

        public Marker(Vector3 position, Quaternion rotation, GameObject debug)
        {
            this.position = position;
            this.rotation = rotation;
            this.debug = debug;
        }
    }

    List<Marker> markers = new List<Marker>();
    public int Segment = 0;
    public float Length = 1;

    void FixedUpdate()
    {
        Add();
    }

    // Add to the marker the current position of the part
    void Add()
    {
        GameObject debug = null;
        var marker = new Marker(transform.position, transform.rotation, debug);

        markers.Add(marker);
        if (markers.Count > 500)
        {
            Debug.LogError("Markers are leaking");
        }
    }

    /// Remove the marker at the tail of the path. If path is empty, null is returned.
    public Marker TakeTail()
    {
        if (markers.Count < 1)
            return null;

        var marker = markers[0];
        markers.RemoveAt(0);
        return marker;
    }

    /// Clamp the path to have at most the given length and return the first (closes to head) removed marker. If path is shorter, null is returned.
    public Marker TrimAtLength(float maxLength)
    {
        if (markers.Count < 1)
            return null;

        // find clipping position from the start, note the tail of the path is at 0
        float len = 0;
        int i = markers.Count - 1;
        while (i > 0 && len < maxLength)
        {
            var d = (markers[i - 1].position - markers[i].position).magnitude;
            len += d;
            i -= 1;
        }

        if (i > 0)
        {
            // remove unwanted section
            var marker = markers[i - 1];
            markers.RemoveRange(0, i);
            return marker;
        }
        else
        {
            return null;
        }
    }

    /// Clamp the path to have at most the given number of markers and return the first (closes to head) removed marker. If path is shorter, null is returned.
    public Marker TrimAtCount(int count)
    {
        Assert.IsTrue(count > 0);
        var countToRemove = markers.Count - count;
        if (countToRemove > 0)
        {
            // remove unwanted section
            var marker = markers[countToRemove - 1];
            markers.RemoveRange(0, countToRemove);
            return marker;
        }
        else
        {
            return null;
        }
    }

    void OnDrawGizmos()
    {
        Marker prev = null;
        switch (Segment % 4)
        {
            case 0: Gizmos.color = Color.yellow; break;
            case 1: Gizmos.color = Color.red; break;
            case 2: Gizmos.color = Color.green; break;
            default: Gizmos.color = Color.blue; break;
        };

        foreach (var marker in markers)
        {
            if (prev != null)
            {
                Gizmos.DrawSphere(prev.position, 0.1f);
                Gizmos.DrawLine(prev.position, marker.position);
            }
            prev = marker;
        }

        if (prev != null)
        {
            Gizmos.DrawSphere(prev.position, 0.1f);
            Gizmos.DrawLine(prev.position, transform.position);
        }
    }
}
