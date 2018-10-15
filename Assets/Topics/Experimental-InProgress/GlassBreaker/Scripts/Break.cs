using UnityEngine;
using System.Collections;

public class Break : MonoBehaviour
{
    public Transform brokenObject;
    public float magnitudeCol, radius, power, upwards;

    private bool m_ScreenBroken = false;

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.relativeVelocity.magnitude > magnitudeCol)
    //    {
    //        Destroy(gameObject);
    //        Instantiate(brokenObject, transform.position, transform.rotation);
    //        brokenObject.localScale = transform.localScale;
    //        Vector3 explosionPos = transform.position;
    //        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

    //        foreach (Collider hit in colliders)
    //        {
    //            if (hit.GetComponent<Rigidbody>())
    //            {

    //                hit.GetComponent<Rigidbody>().AddExplosionForce(power * collision.relativeVelocity.magnitude, explosionPos, radius, upwards);
    //            }
    //        }
    //    }
    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (!m_ScreenBroken)
            {
                BreakScreen();
            }
        }
    }

    public void BreakScreen()
    {

        Destroy(gameObject);
        Instantiate(brokenObject, transform.position, transform.rotation);
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


    }
}
