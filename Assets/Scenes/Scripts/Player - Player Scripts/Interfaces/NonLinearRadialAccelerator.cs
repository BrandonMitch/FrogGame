using UnityEngine;

public class NonLinearRadialAccelerator 
{
    private float e;    // ease in time
    private float E;    // ease in frames
    private float ep;   // ease in percent
    private float w;    // ease out time
    private float W;    // ease out frames
    private float wp;   // ease out percent
    private float range; // total time applying force

    private float VEL;  // desired velocity
    private float k;    // forcing constant
    private float m;    // mass of the character

    private float t0;
    private Vector3 lastPosition = Vector3.zero;
    public bool DebugOn = true;
    private Vector2 ihat = Vector2.zero;
    private Vector2 jhat = Vector2.zero;

    int dampingCounter;
    public NonLinearRadialAccelerator(float E, float W, float VEL, float m)
    {
        this.E = E;
        this.W = W;
        this.VEL = VEL;
        this.m = m;
        intitalize();
    }
    public void intitalize() {
        e = (int)E * Time.deltaTime;
        w = (int)W * Time.deltaTime;
        range = e + w;
        ep = e / range;
        wp = 1 - ep;
        k = 2 * m * VEL / range;
        dampingCounter = 0;
        // Save t0;
        t0 = Time.time;
    }
    public void intitalize(float E, float W, float VEL, float m)
    {
        this.E = E;
        this.W = W;
        this.VEL = VEL;
        this.m = m;
        intitalize();
    }

    /*  Applies a smooth ease in ease out radial acceleration
        @param ihat - Tangental Direction
        @param jhat - Radial Direction
        @param RB - the rigidbody you want to apply the force to
        @param r0 - the inital radius of the object you are orbiting
     */    
    public void FixedUpdateCall(Vector3 ihat, Vector3 jhat, Rigidbody2D RB, float r0)
    {
        if(RB == null) { Debug.LogError("RB is null"); return; }
        float t = CalculateTime(); // t goes from 0 -> 1 based on the length of range (range = ease in time + ease out time)
        float fm = CalculateForceTotal(t);
        if (fm > 0.01)
        {
            ApplyNonLinearTangentialForce(RB, fm, ihat);
        }

        ClampRBVelocity(RB);

        float cp = CalculateNonLinearCentripetalForce(RB.velocity.magnitude, r0);

        ApplyCentripetalForce(RB, cp, jhat);
        if (DebugOn)
        {
            Tracer.Trace(lastPosition, RB.position, Color.red, 5f);
            Tracer.DrawTangentalandCentripetal(RB.position, ihat, jhat, Color.blue, Color.green);
            //Debug.DrawLine(RB.position, (Vector3)RB.position + ihat, Color.blue);
            //Debug.DrawLine(RB.position, (Vector3)RB.position + jhat, Color.green);
            lastPosition = RB.position;
        }
    }
    public void FixedUpdateCall(Vector2 rotationPoint, Rigidbody2D RB, float r0)
    {
        if (RB == null) { Debug.LogError("RB is null"); return; }
        Updateihatjhat(rotationPoint, RB.position);
        FixedUpdateCall(ihat, jhat, RB, r0);
    }
    private void Updateihatjhat(Vector2 rotationPoint, Vector2 position)
    {
        // We have make sure this works for analog also it needs to be rotated to the reference frame relative to the tongue direction
        // EOT = j hat
        // right of this vector is i hat, which is EOTx{0,1,0}; 
        jhat = rotationPoint - position;
        jhat.Normalize();

        Vector3 khat = Vector3.forward;
        ihat = Vector3.Cross(jhat, khat); // gets the vector perpendicular to the tongue direction
    }

    private float CalculateTime()
    {
        float t = (Time.time - t0) / range;
        t = Mathf.Clamp(t, 0, 1);
        return t;
    }
    private float CalculateForceTotal(float t)
    {
       if (t >= 1) { return 0; }
        return EaseInForce(t) + EaseOutForce(t);
    }
    private float smoothStep(float x) // 3x^2 - 2x^3 clampedbetween 0<->1
    {
        if (x >= 1)
        {
            return 1;
        }
        else if (x <= 0)
        {
            return 0;
        }
        else
        {
            return (3 * Mathf.Pow(x, 2)) - (2 * Mathf.Pow(x, 3));
        }
    }
    private float EaseInForce(float t)
    {
        if (t > ep) return 0;
        if (ep == 0) return 0;

        float tscaled = t / ep;
        Mathf.Clamp(tscaled, 0, 1); 
        float f = k * smoothStep(tscaled);
        return f;
    }
    private float EaseOutForce(float t)
    {
        if (t <= ep) return 0;
        float tscaled = (t - ep) / wp;
        Mathf.Clamp(tscaled, 0, 1); 
        float f = k * (1 - smoothStep(tscaled));
        return f;
    }
    private void ApplyNonLinearTangentialForce(Rigidbody2D RB, float fm,Vector3 ihat)
    {
        RB.AddForce(ihat * fm);
    }
    private float CalculateNonLinearCentripetalForce(float currentVelocity, float radiusInital)
    {
        float fc = m * Mathf.Pow(currentVelocity, 2) / radiusInital;
        return fc;
    }
    private void ApplyCentripetalForce(Rigidbody2D RB,  float forceMagnitude, Vector3 jhat)
    { 
        RB.AddForce(jhat * forceMagnitude);
    }

    private void ClampRBVelocity(Rigidbody2D RB)
    {
        RB.velocity = Vector2.ClampMagnitude(RB.velocity, VEL * 1.5f);
    }

    private void IncrementDampingCounter()
    {
        dampingCounter++;
    }
}
