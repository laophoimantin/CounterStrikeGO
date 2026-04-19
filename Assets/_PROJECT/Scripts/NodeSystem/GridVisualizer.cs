using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Create decal for the nodes and the lines connecting the nodes
/// </summary>
public class GridVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject _linePrefab;
    [SerializeField] private Transform _gridLinesContainer;
    [SerializeField] private float _drawSpeed = 0.5f;
    [SerializeField] private float _nodePadding = 0.5f;
    private float _width = 0.1f;
    
    public void DrawAllConnections()
    {
        foreach (Node node in NodeManager.Instance.AllNodes)
        {
            if (node.EastNode != null) SpawnAndAnimateFlatLine(node, node.EastNode); 
            if (node.NorthNode != null) SpawnAndAnimateFlatLine(node, node.NorthNode); 
        }
    }

    private void SpawnAndAnimateFlatLine(Node fromNode, Node toNode)
    {
        Vector3 flatPosA = new Vector3(fromNode.WorldPos.x, 0f, fromNode.WorldPos.z);
        Vector3 flatPosB = new Vector3(toNode.WorldPos.x, 0f, toNode.WorldPos.z);

        float flatDistance = Vector3.Distance(flatPosA, flatPosB);
        float lineLength = flatDistance - _nodePadding;

        Vector3 midPointXZ = (flatPosA + flatPosB) / 2f;

        GameObject lineObj = Instantiate(_linePrefab, midPointXZ, Quaternion.identity, _gridLinesContainer);

        DecalProjector decal = lineObj.GetComponent<DecalProjector>();
        if (decal == null)
        {
            return;
        }

        Vector3 dirToTarget = (flatPosB - flatPosA).normalized;
        lineObj.transform.rotation = Quaternion.LookRotation(Vector3.down, dirToTarget);

        float lowestY = Mathf.Min(fromNode.WorldPos.y, toNode.WorldPos.y);
        float calculatedDepth = (midPointXZ.y - lowestY) + 2f;
        float thickness = _width;

        Vector3 initialSize = new Vector3(thickness, 0f, calculatedDepth);
        Vector3 targetSize = new Vector3(thickness, lineLength, calculatedDepth);

        decal.size = initialSize;

        DOTween.To(() => decal.size, x => decal.size = x, targetSize, _drawSpeed).SetEase(Ease.InOutSine);
    }
}