using UnityEngine;
using System.Collections;

public class Break : MonoBehaviour
{
    public Transform brokenObject;
    public float magnitudeCol, radius, power, upwards;

    public void BreakScreen()
    {
        //Deactivate the screen
        //gameObject.SetActive(false);


        var brokenPieces = Instantiate(brokenObject, transform.position, transform.rotation);
        brokenObject.localScale = transform.localScale;
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

        foreach (Collider hit in colliders)
        {
            if (hit.GetComponent<Rigidbody>())
            {
                hit.GetComponent<Rigidbody>().AddExplosionForce(power, explosionPos, radius, upwards);
                //hit.GetComponent<Rigidbody>().AddExplosionForce(power*collision.relativeVelocity.magnitude, explosionPos, radius, upwards);
            }
        }

        Destroy(brokenPieces.gameObject, 5.0f);

    }

    public void ResetScreen()
    {
        //Make the original screen visible again
        //gameObject.SetActive(true);
    }

  
}
