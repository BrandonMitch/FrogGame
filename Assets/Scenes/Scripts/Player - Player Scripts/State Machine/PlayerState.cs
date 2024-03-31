using UnityEngine;

public abstract class PlayerState
{
    protected Player player;
    protected PlayerStateMachine playerStateMachine;
    protected bool leftMouseDown    {get{return player.inputManager.LeftMouseDown;}}
    protected bool leftMouseUp      {get{return player.inputManager.LeftMouseUp;}}
    protected bool leftMouseButton  {get{return player.inputManager.LeftMouseButton;}}
    protected bool rightMouseDown   {get{return player.inputManager.RightMouseDown;}}
    protected bool rightMouseUp     {get{return player.inputManager.RightMouseUp;}}
    protected bool rightMouseButton {get{return player.inputManager.RightMouseButton;}}
    protected bool fKeyDown         {get{return player.inputManager.ReleaseKeyDown;}}

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
        /*// Check x/y movement, normalize vector
        Vector2 moveVec = Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),1.0f);
        return moveVec;*/
        return player.inputManager.CurrentMovementInputs;
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


    public virtual string[] PreviousStateData()
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

    public bool CheckIfPlayerWantsToRetractTongue()
    {
        //GetFKeyInputs();
        if (fKeyDown)
        {
            playerStateMachine.ChangeState(player.slowingState);
            player.tongueStateMachine.ChangeState(player.tongueRetractingState);
            return true;
        }
        return false;
    }

    protected void FindLeftMouseInputs()
    {
        /*
                // Trying to charge
                leftMouseButton = Input.GetButton("Fire1");
                // Attack is started
                leftMouseDown = Input.GetButtonDown("Fire1");
                // Attack is released
                leftMouseUp = Input.GetButtonUp("Fire1");
               */
    }
    protected void FindRightMouseInputs()
    {
        /*
                // Aiming the tongue
                rightMouseButton = Input.GetButton("Fire2");
                // Start aiming the tongue
                rightMouseDown = Input.GetButtonDown("Fire2");
                // Spitting out the tongue on release
                rightMouseUp = Input.GetButtonUp("Fire2");*/
    }
    protected void GetFKeyInputs()
    {
        /*        fKeyDown = (Input.GetKeyDown(KeyCode.F));
                if (fKeyDown) return;
                var a = Input.GetAxis("Retract");
                fKeyDown = (Mathf.Abs(a) > 0.01f);*/

    }
    bool debug = true;
    public bool LineCastEndOfTongueToRotationPoint()
    {
        TongueHitData hitData = player.tongueStateMachine.GetRotationPoint();
        Vector2 rotationPoint = hitData.getPos();
        Vector2 parentPosition = player.tongueStateMachine.GetParentTransformPosition(); 

        RaycastHit2D[] collisions = new RaycastHit2D[3];
        if (debug)
        {
            Debug.DrawLine(rotationPoint, parentPosition, Color.magenta);
        }
        Physics2D.Linecast(rotationPoint, parentPosition, player.tongueContactFilter, collisions);
        foreach (RaycastHit2D col in collisions)
        {
            Collider2D collision = col.collider;
            if (collision != null) // Check if the collision is not null
            {
                if (!collision.CompareTag("Player"))
                {

                    // *** Debugs ***
                    // Yellow is normal
                    // Red is collision point
                    ///Debug.Log("Collision Name: " + collision.name);
                    if (debug)
                    {
                        Tracer.DrawCircle(col.point, 0.03f, 10, Color.red);
                        Vector2 normVector = col.normal;
                        Debug.DrawLine(col.point, col.point + normVector, Color.yellow, 5f);
                    }
                    ///Debug.Break();
                    // -----------------

                    // Check if the collision can't be swung on
                    if (CantGoThroughTongueCheck(col))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public virtual bool CantGoThroughTongueCheck(RaycastHit2D col)
    {
        ICantGoThroughTongue cantGoThroughTongue = col.collider.GetComponent<ICantGoThroughTongue>();
        if (cantGoThroughTongue == null)
        {
            return false;
        }
        else
        {
            cantGoThroughTongue.OnTongueCollide(col);
            OnTongueCollisionIntersection();
            return true;
        }
    }
    public virtual bool CantBeSwungOnCheck(RaycastHit2D col)
    {
        ICantBeSwungOn cantBeSwungInterface = col.collider.GetComponent<ICantBeSwungOn>();
        if (cantBeSwungInterface == null)
        {
            return false;
        }
        else
        {
            cantBeSwungInterface.OnSwungOn(col);
            OnTongueSwungOnIntersection();
            return true;
        }
    }
    public virtual void OnTongueCollisionIntersection()
    {

    }
    public virtual void OnTongueSwungOnIntersection()
    {

    }
}
