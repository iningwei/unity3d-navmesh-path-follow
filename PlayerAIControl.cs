/*
 *by:iningwei
 * Unity3D 4.7.2f1
 */

using UnityEngine;
/// <summary>
/// navmesh寻路
/// </summary>
public class PlayerAIControl : MonoBehaviour
{
    public GameObject desObj;
    public bool allowDrawPath = true;
    public Color lineRenderColor = Color.green;
    NavMeshAgent navMeshAgent;
    Vector3 targetPos;
    GameObject desObjClone;
    Vector3[] pathCorners;
    int countOfCorner;
    GameObject lineRenderObj;
    LineRenderer lineRenderer;
    Material lineRenderMaterial;
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;//禁用自带的旋转，或者通过设置AngularSpeed为0
        if (allowDrawPath)
        {
            lineRenderObj = new GameObject();
            lineRenderObj.name = "pathRenderer";
            lineRenderer = lineRenderObj.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = true;
            lineRenderer.SetWidth(0.02f, 0.02f);
            lineRenderer.SetColors(lineRenderColor, lineRenderColor);
            lineRenderMaterial = new Material(Shader.Find("Particles/Additive"));
            lineRenderer.material = lineRenderMaterial;
            lineRenderObj.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, 1000, 1 << LayerMask.NameToLayer("Walkable")))
            {
                targetPos = raycastHit.point;

                if (desObj != null)
                {
                    if (desObjClone != null)
                    {
                        Destroy(desObjClone);
                        desObjClone = (GameObject)Instantiate(desObj, targetPos, Quaternion.identity);
                    }
                    else
                    {
                        desObjClone = (GameObject)Instantiate(desObj, targetPos, Quaternion.identity);
                    }
                }
                navMeshAgent.destination = targetPos;
            }
        }


        pathCorners = navMeshAgent.path.corners;
        countOfCorner = pathCorners.Length;//静止状态下都会有一个corner，即到达终点后停止也具有一个corner
        if (countOfCorner > 1)
        {
            lineRenderObj.SetActive(true);
            _drawLinePath(pathCorners);
        }
        else
        {
            lineRenderObj.SetActive(false);
        }
    }


    void FixedUpdate()
    {
        //控制旋转
        if (navMeshAgent.hasPath && navMeshAgent.path.corners.Length > 1)
        {
            //经过测试验证：corneres[0]为当前位置，corners[1]为下一个拐点的位置
            Vector3 dir = navMeshAgent.path.corners[1] - navMeshAgent.transform.position;
            if (dir != Vector3.zero)
            {
                transform.forward = Vector3.Slerp(transform.forward, dir, 200f * Time.fixedDeltaTime);
            }
        }
    }
    void _drawLinePath(Vector3[] pathCorners)
    {
        lineRenderer.SetVertexCount(countOfCorner);//设定linerender具有的顶点数，否则默认的是2个顶点。

        for (int i = 0; i < countOfCorner; i++)
        {
            lineRenderer.SetPosition(i, pathCorners[i]);
        }
    }
}