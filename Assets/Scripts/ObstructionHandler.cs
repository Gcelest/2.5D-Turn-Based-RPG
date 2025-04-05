using UnityEngine;

public class CameraObstructionCircle : MonoBehaviour
{
    public Transform player;
    public LayerMask obstructionMask;
    public float revealRadius = 2f;

    private Material currentObstructMaterial;
    private Renderer currentRenderer;

    void Update()
    {
        Vector3 dirToPlayer = player.position - transform.position;
        float distToPlayer = dirToPlayer.magnitude;

        if (Physics.Raycast(transform.position, dirToPlayer, out RaycastHit hit, distToPlayer, obstructionMask))
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                if (rend != currentRenderer)
                {
                    ClearPrevious();
                    currentRenderer = rend;
                    currentObstructMaterial = rend.material;
                }

                if (currentObstructMaterial != null)
                {
                    Vector3 playerWorldPos = player.position;
                    currentObstructMaterial.SetVector("_RevealPosition", new Vector4(playerWorldPos.x, playerWorldPos.y, playerWorldPos.z, 0));
                    currentObstructMaterial.SetFloat("_RevealRadius", revealRadius);
                    currentObstructMaterial.SetFloat("_FadeAlpha", 0.3f);
                }
            }
        }
        else
        {
            ClearPrevious();
        }
    }

    private void ClearPrevious()
    {
        if (currentObstructMaterial != null)
        {
            currentObstructMaterial.SetFloat("_FadeAlpha", 1f);
        }

        currentObstructMaterial = null;
        currentRenderer = null;
    }
}
