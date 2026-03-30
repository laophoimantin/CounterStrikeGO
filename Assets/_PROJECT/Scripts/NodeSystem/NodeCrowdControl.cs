using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Node))]
public class NodeCrowdControl : MonoBehaviour
{
    [SerializeField] private float _crowdRadius = 0.35f;
    private Node _node;

    private void Awake()
    {
        _node = GetComponent<Node>();
    }

    private void OnEnable()
    {
        if (_node != null) _node.OnOccupancyChanged += RearrangeUnits;
    }

    private void OnDisable()
    {
        if (_node != null) _node.OnOccupancyChanged -= RearrangeUnits;
    }

    private void RearrangeUnits()
    {
        var activeUnits = _node.GetAllOccupants().Where(o => o.OccupiesSpace).ToList();
        int count = activeUnits.Count;
        
        if (count == 0) return;

        List<Vector3> slots = new List<Vector3>();

        if (count == 1)
        {
            slots.Add(Vector3.zero);
        }
        else
        {
            float angleStep = 360f / count;
            for (int i = 0; i < count; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * _crowdRadius;
                float z = Mathf.Sin(angle) * _crowdRadius;
                slots.Add(new Vector3(x, 0, z));
            }
        }

        List<GridOccupant> sortedUnits = activeUnits.OrderByDescending(u =>
            Vector3.SqrMagnitude(u.transform.position - transform.position)
        ).ToList();

        List<Vector3> availableSlots = new List<Vector3>(slots);

        foreach (GridOccupant unit in sortedUnits)
        {
            Vector3 bestSlot = Vector3.zero;
            float bestDist = float.MaxValue;
            int bestSlotIndex = -1;

            for (int i = 0; i < availableSlots.Count; i++)
            {
                Vector3 worldSlotPos = transform.TransformPoint(availableSlots[i]);
                float dist = Vector3.SqrMagnitude(unit.transform.position - worldSlotPos);

                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestSlot = availableSlots[i];
                    bestSlotIndex = i;
                }
            }

            if (bestSlotIndex != -1)
            {
                unit.SetVisualOffset(bestSlot);
                availableSlots.RemoveAt(bestSlotIndex);
            }
        }
    }

}