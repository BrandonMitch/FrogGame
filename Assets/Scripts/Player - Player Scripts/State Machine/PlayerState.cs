using UnityEngine;

public class PlayerState
{
    protected Player player;
    protected PlayerStateMachine playerStateMachine;
    static protected bool leftMouseDown = false;
    static protected bool leftMouseUp = false;
    static protected bool leftMouseButton = false;
    static protected bool rightMouseDown = false;
    static protected bool rightMouseUp = false;
    static protected bool rightMouseButton = false;

    static protected bool fKeyDown = false;
    public PlayerState(Player player, PlayerStateMachine playerStateMachine)
    {
        this.player = player;
        this.playerStateMachine = playerStateMachine;
    }
    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }

    public virtual void AnimationTriggerEvent(Player.AnimationTriggerType triggerType) 
    {

    }

    // TODO: IMPLEMENT
    public virtual float CalculateSpeed()
    {
        return 1.0f;
    }

    protected Vector2 GetCurrentMovementInputs()
    {
        // Check x/y movement, normalize vector
        Vector2 moveVec = Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),1.0f);
        return moveVec;
    }
    protected void SetMovementInputs(Vector2 moveVec)
    { 
        player.SetMovementInputs(moveVec,moveVec.magnitude);
    }
    protected void SetMovementInputs(Vector2 moveVec, float speed)
    {
        player.SetMovementInputs(moveVec, speed);
    }

    protected void SetLastMovementDirection(Vector2 LastMovementDirection)
    {
        player.SetLastMoveDirection(LastMovementDirection);
    }
    protected void FindLeftMouseInputs()
    {
        // Trying to charge
        leftMouseButton = Input.GetButton("Fire1");
        // Attack is started
        leftMouseDown = Input.GetButtonDown("Fire1");
        // Attack is released
        leftMouseUp = Input.GetButtonUp("Fire1");
       
    }
    protected void FindRightMouseInputs()
    {
        // Aiming the tongue
        rightMouseButton = Input.GetButton("Fire2");
        // Start aiming the tongue
        rightMouseDown = Input.GetButtonDown("Fire2");
        // Spitting out the tongue on release
        rightMouseUp = Input.GetButtonUp("Fire2");
    }

    public virtual string[] previousStateData()
    {
        string[] s = { "NULL" };
        return s;
    }

    /** Reads state data in search of a string.
    *  @return : If the term is found, it returns the index of the term and stores int an array.
    *  The position of the int[] array corresponds to the position of the term array
    *   parse( {hi,NULL,NULL,time} , {hi,time,DNE} ) would return {0,3,-1}
    */
    protected int[] ParsePreviousStateDataFor(string[] data, string[] terms)
    {
        int[] containsTermArray = new int[terms.Length];
        
        int termIndex = 0;
        foreach (string term in terms) // for each term, search through the data array
        {
            containsTermArray[termIndex] = -1; // Intialize to -1 indicating term not found

            int dataindex = 0;
            foreach (string lineOfData in data) // searching line by line
            {
                if ( (lineOfData != null) && (lineOfData != "NULL") && (lineOfData.Contains(term)))
                {
                    containsTermArray[termIndex] = dataindex; // store the index where the term is found
                    break; // exit the loop once the term is found
                }
                dataindex++;
            }
            termIndex++;
        }
        return containsTermArray;
    }
    /** TEST CASE 
    int[] result = ParsePreviousStateDataFor(new string[] { "hi", "NULL", "___ dog", "yolo", "NULL" }, new string[] { "hi", "dog", "yolo", "DNE" });

    int r2 = ParsePreviousStateDataFor(new string[] { "" }, "hi");
    string s = "result = {";
        foreach (int n in result)
        {
            s += n + ", ";
        }
    Debug.Log(s + "}");
    Debug.Log("result2= " + r2);
    */

    /** Reads state data in search of a string, usuually a variable name.
 *  @return : If the term is found, it returns the index of the term. If it is not found it returns -1
 *  parse( {NULL,hi,NULL} , hi ) returns 1
 */
    protected int ParsePreviousStateDataFor(string[] data, string term)
    {
        int dataIndex = 0;
        foreach (string lineOfData in data)
        {
            if ((lineOfData != null) && (lineOfData != "NULL") && (lineOfData.Contains(term)) ) {
                return dataIndex; 
            }
            dataIndex++;
        }
        return -1;
    }

    protected float parseDataForFloat(string input)
    {
        // Your parsing logic here
        string[] parts = input.Split('=');

        if (parts.Length >= 2)
        {
            string variable = parts[0].Trim();
            float nextFloat;

            if (float.TryParse(parts[1], out nextFloat))
            {
                return nextFloat;
            }
            else
            {
                Debug.LogError("Failed to parse the next float.");
            }
        }
        else
        {
            Debug.LogError("Invalid input string format.");
        }
        return 0;
    }

    public static void DrawCircle(Vector3 position, float radius, int segments, Color color)
    {
        // If either radius or number of segments are less or equal to 0, skip drawing
        if (radius <= 0.0f || segments <= 0)
        {
            return;
        }

        // Single segment of the circle covers (360 / number of segments) degrees
        float angleStep = (360.0f / segments);

        // Result is multiplied by Mathf.Deg2Rad constant which transforms degrees to radians
        // which are required by Unity's Mathf class trigonometry methods

        angleStep *= Mathf.Deg2Rad;

        // lineStart and lineEnd variables are declared outside of the following for loop
        Vector3 lineStart = Vector3.zero;
        Vector3 lineEnd = Vector3.zero;

        for (int i = 0; i < segments; i++)
        {
            // Line start is defined as starting angle of the current segment (i)
            lineStart.x = Mathf.Cos(angleStep * i);
            lineStart.y = Mathf.Sin(angleStep * i);

            // Line end is defined by the angle of the next segment (i+1)
            lineEnd.x = Mathf.Cos(angleStep * (i + 1));
            lineEnd.y = Mathf.Sin(angleStep * (i + 1));

            // Results are multiplied so they match the desired radius
            lineStart *= radius;
            lineEnd *= radius;

            // Results are offset by the desired position/origin 
            lineStart += position;
            lineEnd += position;

            // Points are connected using DrawLine method and using the passed color
            Debug.DrawLine(lineStart, lineEnd, color);
        }
    }

    protected void GetFKeyInputs()
    {
        fKeyDown = Input.GetKeyDown(KeyCode.F);
    }

}
