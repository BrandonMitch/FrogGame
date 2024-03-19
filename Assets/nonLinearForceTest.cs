using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nonLinearForceTest : MonoBehaviour
{
    Rigidbody2D RB;

    float t0;
    [Header("W = length of smooth step in fixed frames")]
    [SerializeField] float W = 100; // Ten Frames
    [SerializeField] float E = 100;
    float w = 0;
    float e = 0;
    float ep;
    float wp;
    float range;
    float k; // Forcing Constant
    [Header("VEL = desired velocity")]
    [SerializeField] float VEL = 1; // Desired Velocity

    bool firstEntry = true;

    float m;
    [SerializeField] GameObject rotObj;
    Transform rotTransform;
    Vector3 rotationPoint;
    float r0;
    Vector3 ihat = Vector3.zero;
    Vector3 jhat = Vector3.zero;
    void Start()
    {
        rotTransform = rotObj.GetComponent<Transform>();
        w = W * Time.fixedDeltaTime;
        e = E * Time.fixedDeltaTime;
        range = e + w;
        ep = e / range;
        wp = 1 - ep;
        RB = gameObject.GetComponent<Rigidbody2D>();
        m = RB.mass;
        k = 2 * m * VEL / (w+e); // Derived from mathematica. Usually forcing constant is VEL*m/ dt, but sine smooth step integrates to 0.5, we have to multiply by 2.
/*        DebugGUI.SetGraphProperties("t", "time(0,1):", 0, 1, 6, new Color(0, 1, 0), false);
        DebugGUI.SetGraphProperties("fm", "forceMagnitude:", 0, 2, 7, new Color(1, 0, 0), true);
        DebugGUI.SetGraphProperties("v", "velocity calculated:", 0, 3, 8, new Color(0, 0, 1), false);
        DebugGUI.SetGraphProperties("va", "velocity actual:", 0, 3, 8, new Color(0, 0.5f, 1), false);*/

        r0 = ((Vector2)rotTransform.position - RB.position).magnitude;
    }
    private void FixedUpdate()
    {
        if (firstEntry)
        {
            t0 = Time.time;
/*            Debug.Log("k=" + k);
            Debug.Log("w=" + w);
            Debug.Log("t0" + t0);*/
            firstEntry = false;
        }
        rotationPoint = rotTransform.position;

        //float t = calculatetime();
        // float fm = smoothForce(t);
        // float v = calculateCurrentVelocity(t,0); // Used for applying correct centripetal force

        float t = calculateTimeWithEase();
        float fm = ForceTotal(t);
        float v = calculateCurrentVelocityWithEase(t);

        Updateihat(rotationPoint);
   

/*        DebugGUI.Graph("t", t);
        DebugGUI.Graph("fm", fm);
        DebugGUI.Graph("v", v);
        DebugGUI.Graph("va", RB.velocity.magnitude);*/
        RB.AddForce(ihat * fm); // add non linear acceleration
        //float error = Mathf.Abs(RB.velocity.magnitude - v);

        float vActual = RB.velocity.magnitude;
        //Debug.Log("Actual Velocity:" + vActual + " | Calculated Velocity:" + v + "| Error: " + error);

        float centripetalForceM = calculateNonLinearCentripetalForce(vActual, v, r0);
        ApplyNonLinearCentripetalForce(rotationPoint, RB.position, centripetalForceM, r0, t, jhat);// add non linear centripetal force
        //lastFrameTime = Time.time;
        Tracer(lastPos, RB.position);
        lastPos = RB.position;
    }
    private float smoothStep(float x) // 3x^2 - 2x^3 clampedbetween 0<->1
    {
        if (x >= 1)
        {
            return 1;
        }
        else if( x <= 0)
        {
            return 0;
        }
        else
        {
            return (3 * Mathf.Pow(x, 2)) - (2 * Mathf.Pow(x, 3));
        }
    }
    private float smoothForce(float t)
    {

        return k * (1 - smoothStep(t));
    }

    private float smoothForceAvg(float t0, float t) // 4th version
    { 
        if (t <= 0) { return 0; }
        if (t >= 1) { return 0; }
        t *= w;
        float w3 = Mathf.Pow(w, 3);
        float t2 = Mathf.Pow(t, 2);
        float t3 = Mathf.Pow(t, 3);
        float t02 = Mathf.Pow(t0, 2);
        float t03 = Mathf.Pow(t0, 3);

        float fm = (k / (2 * w3))*(t3 + t2*t0 + t*t02 + t03 - 2*(t2 + t*t0 + t02)*w + 2*w3);
        Debug.Log("Frame(" + frame + "), t=" + t + ", Force Magnitude:" + fm);
        frame++;
        return fm;
    }

    private float calculateCurrentVelocity(float t)
    {
        t = t * w;
        // vf = 2*(t^4 - 2*t^3*w + 2*t*w^3)/ w^4
        float velocity = 2 * (Mathf.Pow(t, 4) - (2 * Mathf.Pow(t, 3) * w) + (2 * t * Mathf.Pow(w, 3) ))/Mathf.Pow(w,4); // basically the result of integrating the smooth step function with window w
        return velocity;
    }
    private float calculateCurrentVelocity(float t,int i) //version 2
    {
        t = t * w;
        // vf = 2*(t^4 - 2*t^3*w + 2*t*w^3)/ w^4
        float velocity = k * (Mathf.Pow(t, 4) - (2 * Mathf.Pow(t, 3) * w) + (2 * t * Mathf.Pow(w, 3))) / (Mathf.Pow(w, 3)*2*m); // basically the result of integrating the smooth step function with window w
        return velocity;
    }
    private float calculatetime()
    {

        float t = (Time.time - t0) / w;
        t = Mathf.Clamp(t, 0, 1);
        if (t == 1)
        {
            //Debug.Log("Time 1 reach @t=" + Time.time);
        }
        return t;
    }

    // Define your force curve using time and force values

    /*private readonly Vector2[] forceCurve = new Vector2[]
    {
        new Vector2(0.02f, 19.81f),
        new Vector2(0.04f, 18.75f),
        new Vector2(0.06f, 16.85f),
        new Vector2(0.08f, 14.35f),
        new Vector2(0.1f, 11.49f),
        new Vector2(0.12f, 8.51f),
        new Vector2(0.14f, 5.65f),
        new Vector2(0.16f, 3.15f),
        new Vector2(0.18f, 1.25f),
        new Vector2(0.2f, 0.19f)
    };
    int FRAME = 0;
    private float smoothforce(float t)
    {
        if (t <= 0) { return 0; }
        if (t >= 1) { return 0; }
        float f = forceCurve[FRAME].y;
        frame++;
        Debug.Log("Frame(" + frame + "), t=" + t + ", Force Magnitude:" + f);
        return f;
    }*/
    /*private float smoothForceAvg(float t0, float t) // This is basically the definite integral of k*(1-smoothstep) over (0, t0, t1)
{
    if (t == 0) { return k; }
    if (t >= 1) { return 0; }
    float w3 = Mathf.Pow(w, 3);
    float t2 = Mathf.Pow(t, 2);
    float t3 = Mathf.Pow(t, 3);
    float t02 = Mathf.Pow(t0, 2);
    float t03 = Mathf.Pow(t0, 3);

    float fm = k * (t3 + (t2 * t0) + (t * t02) + (t03) - (2 * (t2 + t * t0 + t02) * w) + (2 * w3) ) / (2 * w3);
    return fm;
}*/
    /*
    private float smoothForceAvg(float t0, float t) // This is basically the definite integral of k*(1-smoothstep) over (0, t0, t1)
    {
        if (t <= 0) { return 0; }
        if (t >= 1) { return 0; }
        float w3 = Mathf.Pow(w, 3);
        float t2 = Mathf.Pow(t, 2);
        float t3 = Mathf.Pow(t, 3);
        float t02 = Mathf.Pow(t0, 2);
        float t03 = Mathf.Pow(t0, 3);

        float fm = k * (2 + t3 + t2*(t0-2) + t*t0*(t0-2) + t02*(t0-2) )/(2*(t-t0));
        Debug.Log("Frame(" + frame + "), t=" + t + ", Force Magnitude:" + fm);
        frame++;
        return fm;
    }
    */
    int frame = 0;
    /*private float smoothForceAvg(float t0, float t) // This is basically the definite integral of k*(1-smoothstep) over (0, t0, t1)
    {
        frame++;
        if (t <= 0) { return 0; }
        if (t >= 1) { return 0; }

        float t2 = Mathf.Pow(t, 2);
        float t3 = Mathf.Pow(t, 3);
        float t02 = Mathf.Pow(t0, 2);
        
        float fm = (1/2) * k * (2 + t3 + t2 * (t0 - 2) + t * t0 * (t0 - 2) + t02 * (t0 - 2) );
        Debug.Log("Frame(" + frame + "), t=" + t + ", Force Magnitude:" + fm);
        return fm;
    }*/
    Vector2 lastPos = Vector2.zero;
    private void Tracer(Vector2 lastPosition, Vector2 currentPosition)
    {
        if (lastPosition != Vector2.zero)
        {
            Debug.DrawLine(currentPosition, lastPosition, Color.red, 5f);
        }
    }
    
    public float calculateNonLinearCentripetalForce(float currentVelocityMag, float calculatedVelocityMag, float radiusInital)
    {
        float v;
        // Fc = m v^2 / r

        // Use calclauted velocity by default, but if the error is too high we need to not use it
        float errorInVelocityCal = Mathf.Abs(currentVelocityMag - calculatedVelocityMag);
        if (errorInVelocityCal > 0.1f)
        {
            v = currentVelocityMag;
        }
        else
        {
            v = calculatedVelocityMag;
        }

        float fc = m * Mathf.Pow(v, 2) / r0;
        return fc;
    }
    public void ApplyNonLinearCentripetalForce(Vector2 rotationPoint, Vector2 currentPos, float forceMag , float radiusInital, float t, Vector3 jhat)
    {
        // Jhat should be removed from method header
        Debug.DrawLine(RB.position, (Vector3)RB.position + this.jhat,Color.green);
        RB.AddForce(jhat * forceMag);
    }

    private void Updateihat(Vector2 rotationPoint)
    {
        // We have make sure this works for analog also it needs to be rotated to the reference frame relative to the tongue direction
        // EOT = j hat
        // right of this vector is i hat, which is EOTx{0,1,0}; 
        jhat = rotationPoint - RB.position;
        jhat.z = 0;
        jhat.Normalize();

        Vector3 khat = Vector3.forward;
        ihat = Vector3.Cross(jhat, khat); // gets the vector perpendicular to the tongue direction
        Debug.DrawLine(RB.position, (Vector3)RB.position + ihat, Color.blue);
    }
    
    private float fEaseIn(float t)
    {
        if (t > ep) return 0;
        if (ep == 0) return 0;

        float tscaled = t / ep;
        //Debug.Log("tscaled" + tscaled);
        Mathf.Clamp(tscaled, 0, 1);
        float f = k * smoothStep(tscaled);
        return f;
    }
    private float FEaseOut(float t)
    {
        if (t <= ep) return 0;
        float tscaled = (t - ep) / wp;
        float f = k * (1 - smoothStep(tscaled));
        return f;
    }

    private float calculateTimeWithEase()
    {
        float t = (Time.time - t0) / range;
        t = Mathf.Clamp(t, 0, 1);
        if (t == 1)
        {
            //Debug.Log("Time 1 reach @t=" + Time.time);
        }
        return t;
    }
    private float ForceTotal(float t)
    {
        if (t >= 1) { return 0; }
        return fEaseIn(t) + FEaseOut(t);
    }

    private float calculateCurrentVelocityWithEase(float t)
    {
        if (t == 0) { return 0; }
        float e_t = e - t;
        float e_t3 = Mathf.Pow(e_t, 3);
        float e_t4 = Mathf.Pow(e_t, 4);
        float w3 = Mathf.Pow(w, 3);

        float v = k * (e_t4 + 2 * e_t3 * w - (e - 2 * t) * w3) / (2 * m * w3);
        return v;
    }

}
