///--------------------------------------------------------------------------------------------
/// Simple Car Damage system By Ciorbyn Studio https://www.youtube.com/c/CiorbynStudio
/// Tutorial link: https://youtu.be/l04cw7EChpI
/// -------------------------------------------------------------------------------------------
/// 
using Unity.Netcode;
using UnityEngine;

public class CarDamage : NetworkBehaviour
{
    public float maxMoveDelta = 1.0f; // maximum distance one vertice moves per explosion (in meters)
    public float maxCollisionStrength = 50.0f;
    public float YforceDamp = 0.1f; // 0.0 - 1.0
    public float demolutionRange = 0.5f;
    public float impactDirManipulator = 0.0f;
    public float damageFactor = 0.1f;
    public MeshFilter[] MeshList;

    private MeshFilter[] meshfilters;
    private float sqrDemRange;

    //Save Vertex Data
    private struct permaVertsColl
    {
        public Vector3[] permaVerts;
    }
    private permaVertsColl[] originalMeshData;
    int i;

    [SerializeField]
    private KartController kart;

    public void Start()
    {

        if (MeshList.Length > 0)
            meshfilters = MeshList;
        else
            meshfilters = GetComponentsInChildren<MeshFilter>();

        sqrDemRange = demolutionRange * demolutionRange;

        LoadOriginalMeshData();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) Repair();
    }

    void LoadOriginalMeshData()
    {
        originalMeshData = new permaVertsColl[meshfilters.Length];
        for (i = 0; i < meshfilters.Length; i++)
        {
            originalMeshData[i].permaVerts = meshfilters[i].mesh.vertices;
        }
    }

    void Repair()
    {
        for (int i = 0; i < meshfilters.Length; i++)
        {
            meshfilters[i].mesh.vertices = originalMeshData[i].permaVerts;
            meshfilters[i].mesh.RecalculateNormals();
            meshfilters[i].mesh.RecalculateBounds();
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        var parent = collision.gameObject.transform.parent;
        //Debug.Log(parent);

        bool isKart = true;

        // TODO: HACER QUE NO PUEDA CHOCAR CONTRA CIERTOS OBJETOS
        if (parent == null || !parent.CompareTag("Kart"))
        {
            isKart = false;
        }

        Vector3 colRelVel = collision.relativeVelocity;
        colRelVel.y *= YforceDamp;

        Vector3 colPointToMe = transform.position - collision.contacts[0].point;

        // Dot = angle to collision point, frontal = highest damage, strip = lowest damage
        float colStrength = colRelVel.magnitude * Vector3.Dot(collision.contacts[0].normal, colPointToMe.normalized) * damageFactor;

        //OnMeshForce(collision.contacts[0].point, Mathf.Clamp01(colStrength / maxCollisionStrength), isKart);

        DeformCarServerRpc(collision.contacts[0].point, Mathf.Clamp01(colStrength / maxCollisionStrength), isKart, kart.NetworkObjectId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeformCarServerRpc(Vector3 collisionPoint, float collisionForce, bool isKart, ulong kartId)
    {
        DeformCarClientRpc(collisionPoint, collisionForce, isKart, kartId);
    }

    // RPC para los clientes, aplican la deformación
    // La deformación quizás no es igual para los dos clientes, pero no importa (lo importante es que se aprecie el daño)
    [ClientRpc]
    public void DeformCarClientRpc(Vector3 collisionPoint, float collisionForce, bool isKart, ulong kartId)
    {
        if (kart.NetworkObjectId == kartId)
        {
            OnMeshForce(collisionPoint, collisionForce, isKart);
        }
    }

    public void OnMeshForce(Vector3 originPos, float force, bool isKart)
    {
        // force should be between 0.0 and 1.0
        force = Mathf.Clamp01(force);

        for (int j = 0; j < meshfilters.Length; ++j)
        {
            Vector3[] verts = meshfilters[j].mesh.vertices;

            for (int i = 0; i < verts.Length; ++i)
            {
                Vector3 scaledVert = Vector3.Scale(verts[i], transform.localScale);
                Vector3 vertWorldPos = meshfilters[j].transform.position + (meshfilters[j].transform.rotation * scaledVert);
                Vector3 originToMeDir = vertWorldPos - originPos;
                Vector3 flatVertToCenterDir = transform.position - vertWorldPos;
                flatVertToCenterDir.y = 0.0f;

                // 0.5 - 1 => 45° to 0°  / current vertice is nearer to exploPos than center of bounds

                float dist = 1; // Empieza en 0 para que luego si no es coche y no entra en el if, acabe siendo 0 la multiplicación de abajo

                if (!isKart)
                {
                    if (originToMeDir.sqrMagnitude < sqrDemRange) //dot > 0.8f )
                    {
                        dist = Mathf.Clamp01(originToMeDir.sqrMagnitude / sqrDemRange);
                    }
                }
                else
                {
                    dist = Mathf.Clamp01(sqrDemRange / originToMeDir.sqrMagnitude);
                }

                float moveDelta = force * (1.0f - dist) * maxMoveDelta;

                Vector3 moveDir = Vector3.Slerp(originToMeDir, flatVertToCenterDir, impactDirManipulator).normalized * moveDelta;

                verts[i] += Quaternion.Inverse(transform.rotation) * moveDir;

            }

            meshfilters[j].mesh.vertices = verts;
            meshfilters[j].mesh.RecalculateBounds();
        }
    }
}