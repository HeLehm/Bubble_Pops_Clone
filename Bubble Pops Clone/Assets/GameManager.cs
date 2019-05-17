using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    List<Row> BallRows = new List<Row>();
    public GameObject ball;
    public LineRenderer theline;
    GameObject LastHitObject;
    GameObject Firingball;
    GameObject PreViewBall;
    Dictionary<int, Color> colorForValue = new Dictionary<int, Color>();
    List<GameObject> MergeList = new List<GameObject>();
    Vector2 WallHitPoint = new Vector2();
    bool WallHitActive = false;
    bool ActionsAllowed = true;
    public GameObject BallPar;
    int NumberOfShots = 0;
    public Text ScoreLabel;
    int Score = 0;


    // Start is called before the first frame update
    void Start()
    {
        colorForValue.Add(2, Color.magenta);
        colorForValue.Add(4, Color.red);
        colorForValue.Add(8, Color.yellow);
        colorForValue.Add(16, Color.yellow);
        colorForValue.Add(32, Color.green);
        colorForValue.Add(64, Color.cyan);
        colorForValue.Add(128, Color.blue);
        colorForValue.Add(256, Color.green);
        colorForValue.Add(512, Color.magenta);
        for (int i = 0; i < 10; i++)
        {
            var therow = CreateRow((i * 0.8f) + -2.0f, i == 0 || i == 2 || i == 4 || i == 6 || i == 8 || i == 10);
            BallRows.Add(therow);
        }
        PreViewBall = Instantiate(ball, new Vector3(-0.7f, -4, 0), Quaternion.identity);
        PreViewBall.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Firingball = Instantiate(ball, new Vector3(0, -4, 0), Quaternion.identity);
        var colliderball = Firingball.GetComponent<Collider2D>();
        colliderball.enabled = false;
        var colliderballP = PreViewBall.GetComponent<Collider2D>();
        colliderballP.enabled = false;
        PreViewBall.GetComponent<SpriteRenderer>().color = ColorForValue(PreViewBall.GetComponent<BallScript>().NumberValue);
        Firingball.GetComponent<SpriteRenderer>().color = ColorForValue(Firingball.GetComponent<BallScript>().NumberValue);
        // musste lashitobject irgendwas zuweisen
        LastHitObject = Firingball;
    }

    // Update is called once per frame
    void Update()
    {
        if (ActionsAllowed == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                theline.enabled = true;

            }
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 VerbVec = new Vector2(mousePos.x, mousePos.y - (-4));

                RaycastHit2D hit = Physics2D.Raycast(new Vector2(0, -4), VerbVec);

                if (hit.collider != null)
                {
                    theline.SetPosition(1, new Vector3(hit.point.x, hit.point.y, 0));

                    if (hit.collider.gameObject.tag == "Wall")
                    {
                        Vector2 VerbVec2 = new Vector2(VerbVec.x * -1, VerbVec.y);
                        int layerMask = 1 << 8;
                        layerMask = ~layerMask;
                        RaycastHit2D hit2 = Physics2D.Raycast(hit.point, VerbVec2, Mathf.Infinity, layerMask);

                        if (hit2.collider != null)
                        {
                            theline.enabled = true;
                            theline.SetPosition(2, new Vector3(hit2.point.x, hit2.point.y, 0));
                            var VerbindungsVector2 = new Vector3(hit2.point.x, hit2.point.y, 0) - hit2.collider.transform.position;
                            var angle = Mathf.Atan2(VerbindungsVector2.x, VerbindungsVector2.y);
                            WallHitPoint = hit.point;
                            WallHitActive = true;

                            var theobject = GetNeighborFromAngle(hit2.collider.gameObject, angle);

                            rayhitball(theobject);

                        }
                        else
                        {
                            theline.enabled = false;

                            LastHitObject.GetComponent<SpriteRenderer>().enabled = false;
                            //WallHitActive = false;

                        }
                    }
                    else
                    {
                        WallHitActive = false;
                        theline.enabled = true;
                        theline.SetPosition(2, new Vector3(hit.point.x, hit.point.y, 0));

                        var VerbindungsVector = new Vector3(hit.point.x, hit.point.y, 0) - hit.collider.transform.position;
                        var angle = Mathf.Atan2(VerbindungsVector.x, VerbindungsVector.y);

                        var theobject = GetNeighborFromAngle(hit.collider.gameObject, angle);

                        rayhitball(theobject);

                    }
                }
            }
            var ballscript = LastHitObject.GetComponent<BallScript>();
            if ((Input.GetMouseButtonUp(0)) && (LastHitObject != null) && (ballscript.isNumberObject == false))
            {
                ActionsAllowed = false;
                theline.enabled = false;

                if (ballscript.isNumberObject == false)
                {
                    NumberOfShots += 1;
                    ballscript.SRenderer.enabled = false;

                    // schussballball bewget sich
                   // Debug.Log(WallHitActive);
                    if (WallHitActive == false)
                    {
                        iTween.MoveTo(Firingball, iTween.Hash("position", LastHitObject.transform.position, "time", 0.3f, "easetype", iTween.EaseType.easeOutCubic));
                    }
                    else
                    {
                        iTween.MoveTo(Firingball, iTween.Hash("position", new Vector3(WallHitPoint.x, WallHitPoint.y, 0), "time", 0.1f, "easetype", iTween.EaseType.easeInOutSine));
                        iTween.MoveTo(Firingball, iTween.Hash("position", LastHitObject.transform.position, "time", 0.2f, "delay", 0.1f, "easetype", iTween.EaseType.easeInOutSine));
                    }

                    iTween.MoveTo(PreViewBall, iTween.Hash("position", new Vector3(0, -4.0f, 0), "time", 0.3f, "easetype", iTween.EaseType.easeOutCubic));

                    iTween.ScaleTo(PreViewBall, iTween.Hash("scale", new Vector3(0.75f, 0.75f, 0.75f), "time", 0.3f, "easetype", iTween.EaseType.easeInOutCubic));

                    //lasthitobject copy schussball
                    var fireballscript = Firingball.GetComponent<BallScript>();
                    ballscript.NumberValue = fireballscript.NumberValue;
                    //fabe anpassen

                    StartCoroutine(FiringballToOrigin(LastHitObject));
                }
            }
        }
    }


    RowCase GetBottomRowRowCase()
    {
        bool full = true;
        bool empty = true;
        foreach (var theball in BallRows[0].ballList.Values)
        {
            if (theball.GetComponent<BallScript>().isNumberObject == true)
            {
                empty = false;
            }
            else
            {
                full = false;
            }
        }
        if (full == true)
        {
            return RowCase.full;
        }
        if (empty == true)
        {
            return RowCase.empty;
        }
        return RowCase.half;
    }


    void MoveEverythingUp()
    {
        foreach (var arow in BallRows)
        {

            foreach (var theball in arow.ballList.Values)
            {
                iTween.MoveBy(theball, iTween.Hash("amount", new Vector3(0, 0.8f, 0), "time", 0.2f, "easetype", iTween.EaseType.easeInBack, "delay", ((float)BallRows.IndexOf(arow) - (float)BallRows.Count) /20));
                if (arow == BallRows[BallRows.Count - 1])
                {
                    theball.transform.position = new Vector2(theball.transform.position.x, theball.transform.position.y + (-0.8f * BallRows.Count));
                    var theballballscript = theball.GetComponent<BallScript>();
                    theballballscript.isNumberObject = false;
                    theballballscript.bColl.enabled = false;
                    var spritee = theballballscript.SRenderer;
                    spritee.enabled = false;
                }
            }
        }
        BallRows.Insert(0, BallRows[BallRows.Count - 1]);
        BallRows.RemoveAt(BallRows.Count - 1);
    }

    void moveEverythingDown()
    {
        foreach (var arow in BallRows)
        {
            foreach (var theball in arow.ballList.Values)
            {
                iTween.MoveBy(theball, iTween.Hash("amount", new Vector3(0, -0.8f, 0), "time", 0.2f, "easetype", iTween.EaseType.easeInBack,"delay",((float)BallRows.IndexOf(arow))/20 ));
                if (arow == BallRows[0])
                {
                   // Debug.Log("neuerow");
                    theball.transform.position = new Vector2(theball.transform.position.x, theball.transform.position.y + (0.8f * BallRows.Count));
                    var theballballscript = theball.GetComponent<BallScript>();
                    var random = Random.Range(1, 8);
                    float asFloat = (float)random;
                    theballballscript.NumberValue = (int)Mathf.Pow(2.0f, asFloat);
                    theballballscript.isNumberObject = true;
                    theballballscript.bColl.enabled = true;
                    var spritee = theballballscript.SRenderer;
                    spritee.enabled = true;
                    spritee.color = colorForValue[theballballscript.NumberValue];
                }
            }
        }
        BallRows.Add(BallRows[0]);
        BallRows.RemoveAt(0);
    }

    void getNeigborsWithSameValueAndAddtoMergeList(GameObject of)
    {
        var ofBallscript = of.GetComponent<BallScript>();
        foreach (var nei in GetNeighbors(of))
        {
            if (MergeList.Contains(nei) == false)
            {
                var ballscript = nei.GetComponent<BallScript>();
                if ((ballscript.isNumberObject == true) && (ballscript.NumberValue == ofBallscript.NumberValue))
                {
                    MergeList.Add(nei);
                    getNeigborsWithSameValueAndAddtoMergeList(nei);
                }
            }
        }
    }

    IEnumerator SetPositionBack(GameObject of,bool disableRigid)
    {
        var rigid = of.GetComponent<Rigidbody2D>();
        var ballscript = of.GetComponent<BallScript>();
        var time = 0.25f;
        if ((disableRigid == true) && (ballscript.isNumberObject == true))
        {
            if (rigid.gravityScale < 0.1)
            {
                Score += ballscript.NumberValue;
                ScoreLabel.text = "SCORE:" + Score.ToString();

                iTween.PunchScale(ScoreLabel.gameObject, iTween.Hash("amount", new Vector3(1.5f, 1.5f, 1.5f),"time",0.5f,"easetype",iTween.EaseType.easeOutQuad, "name", "scoretween"));
                //iTween.PunchRotation(ScoreLabel.gameObject, iTween.Hash("amount", Mathf.PI/5, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad,"name", "scoretween"));
            }
            iTween.FadeTo(of, iTween.Hash("alpha", 0.0f, "time", 0.4f));
            rigid.gravityScale = 2.5f;
            time = 0.4f;
            ballscript.isfalling = true;
        }
        yield return new WaitForSeconds(time);
        if (disableRigid == true)
        {
            iTween.FadeTo(of, iTween.Hash("alpha", 1.0f, "time", 0.05f));
            rigid.gravityScale = 0;
            rigid.velocity = new Vector2(0, 0);
            ballscript.isfalling = false;
        }
        ballscript.SetInvisble();
        var InitialPos = GetBelonginPosition2(of);
        of.transform.position = InitialPos;
    }

    int getClosestPowerof2(int Number)
    {
        var thelist = new List<int>();
        for (int i = 1; i < 13; i++)
        {
            thelist.Add((int)Mathf.Pow(2.0f, (float)i));
        }

        foreach (var nr in thelist)
        {
            if (nr == Number)
            {
                return Number;
            }
            else
            {
                if (Number < nr)
                {
                    return thelist[thelist.IndexOf(nr) - 1];
                }
            }
        }

        return 2048;
    }

    IEnumerator FiringballToOrigin(GameObject lasthit)
    {
        var lasthitzballscript = lasthit.GetComponent<BallScript>();
        var lasthitspriterenderer = lasthitzballscript.SRenderer;
        var lasthitcollider = lasthitzballscript.bColl;
        var fireballscript = Firingball.GetComponent<BallScript>();
        var fireballspriterenderer = fireballscript.SRenderer;

        yield return new WaitForSeconds(0.1f);


        lasthitzballscript.NumberValue = fireballscript.NumberValue;

        Handheld.Vibrate();

        foreach (var nei in GetNeighbors(lasthit))
        {
            var neiscript = nei.GetComponent<BallScript>();
            if ((neiscript.isNumberObject == true) && (neiscript.NumberValue != lasthitzballscript.NumberValue))
            {
               var verbvec = nei.transform.position - LastHitObject.transform.position;
                verbvec.Normalize();
                verbvec.Scale(new Vector3(0.3f, 0.3f, 0.3f));
                iTween.PunchPosition(nei, iTween.Hash("amount", verbvec, "time", 0.3f, "easetype", iTween.EaseType.easeOutSine, "delay", 0.15f));
            }
        }

        bool merged = false;

        void MergeProcess(GameObject of)
        {
            getNeigborsWithSameValueAndAddtoMergeList(of);
            if (MergeList.Count > 0)
            {
                merged = true;
                var lasthitballscript = of.GetComponent<BallScript>();
                var initialBallValue = lasthitballscript.NumberValue;

                foreach (var thing in MergeList)
                {
                    if (thing != of)
                    {
                        StartCoroutine(SetPositionBack(thing,false));
                        iTween.MoveTo(thing, iTween.Hash("position", of.transform.position, "time", 0.2f,"easetype",iTween.EaseType.easeOutQuad));
                        lasthitballscript.isNumberObject = true;
                        lasthitballscript.NumberValue = lasthitballscript.NumberValue + initialBallValue;
                        var par = Instantiate(BallPar, thing.transform);
                        par.GetComponent<ParticleSystem>().startColor = thing.GetComponent<SpriteRenderer>().color;
                    }
                }
                MergeList.Clear();
                lasthitballscript.NumberValue = getClosestPowerof2(lasthitballscript.NumberValue);

                MergeProcess(of);
            }
        }





        MergeProcess(lasthit);



        yield return new WaitForSeconds(0.25f);
        if (merged == true)
        {
            lasthitzballscript.LabelEffect();
        }




        /*
        if ((NumberOfShots > 2) && (BottomRowEmpty() == true))
        {
            moveEverythingDown();
            NumberOfShots = 0;
            StartCoroutine(SetEverythingToRightPos(1.0f));
        }
        else
        {
          
                StartCoroutine(SetEverythingToRightPos(0.1f));
            

        }
        */

        StartCoroutine(SetEverythingToRightPos());






        lasthitcollider.enabled = true;

        lasthitspriterenderer.color = ColorForValue(lasthitzballscript.NumberValue);
        lasthitspriterenderer.enabled = true;
        lasthitzballscript.isNumberObject = true;


        var previewballscrript = PreViewBall.GetComponent<BallScript>();
        var preveiwspriterendere = previewballscrript.SRenderer;

        fireballscript.NumberValue = previewballscrript.NumberValue;
        fireballspriterenderer.color = ColorForValue(fireballscript.NumberValue);

        PreViewBall.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        PreViewBall.transform.position = new Vector2(-0.7f, -4);
        iTween.ScaleTo(PreViewBall, iTween.Hash("scale", new Vector3(0.3f, 0.3f, 0.3f), "time", 0.1f, "easetype", iTween.EaseType.easeInOutCubic));


        //schussball wieder auf null
        Firingball.transform.position = new Vector2(0, -4);
        //schussball random wert

        var random = Random.Range(1, 8);
        float asFloat = (float)random;
        previewballscrript.NumberValue = (int)Mathf.Pow(2.0f, asFloat);
        //farbe anpassen
        preveiwspriterendere.color = ColorForValue(previewballscrript.NumberValue);




    }

    IEnumerator SetEverythingToRightPos()
    {
        yield return new WaitForSeconds(0.1f);

        var thecase = GetBottomRowRowCase();
        Debug.Log(thecase);

        if (thecase == RowCase.full)
        {
            MoveEverythingUp();
            NumberOfShots = 0;
            yield return new WaitForSeconds(0.8f);
        }
        if (thecase == RowCase.empty && NumberOfShots > 1)
        {
            moveEverythingDown();
            NumberOfShots = 0;
            yield return new WaitForSeconds(0.8f);
        }



        foreach (var arow in BallRows)
        {
            for (int i = 0; i < 6; i++)
            {
                arow.ballList[i].transform.position = GetBelonginPosition(BallRows.IndexOf(arow), i);
            }
        }
        for (int i = 0; i < BallRows.Count; i++)
        {
            var arow = BallRows[Mathf.Abs(i - BallRows.Count + 1)];
            for (int u = 0; u < 6; u++)
            {
                if (IsABallOnTop(arow.ballList[u]) == false)
                {
                    StartCoroutine(SetPositionBack(arow.ballList[u], true));
                }
            }
        }

        ActionsAllowed = true;
        if (Input.GetMouseButton(0) == true)
        {
            theline.enabled = true;
        }

    }

    void rayhitball(GameObject hitball)
    {
        if (hitball != null)
        {
            var hitballballscript = hitball.GetComponent<BallScript>();

            if ((LastHitObject != hitball) && (hitballballscript.isNumberObject == false))
            {
                var lasthitballscript = LastHitObject.GetComponent<BallScript>();
                if (lasthitballscript.isNumberObject == false)
                {
                    lasthitballscript.SRenderer.enabled = false;
                }

                var spritecompon = hitballballscript.SRenderer;
                spritecompon.enabled = true;
                hitball.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                iTween.ScaleTo(hitball, iTween.Hash("scale", new Vector3(0.75f, 0.75f, 0.75f), "time", 0.1f));
                spritecompon.color = Color.gray;
                LastHitObject = hitball;


            }

            var sprren = hitballballscript.SRenderer;
            if (sprren.enabled == false)
            {
                sprren.enabled = true;
                sprren.color = Color.gray;
            }
        }
    }

    Color ColorForValue(int value)
    {
        if (colorForValue.ContainsKey(value))
        {
            return colorForValue[value];
        }
        else
        {
            return Color.black;
        }

    }

    Vector2 GetBelonginPosition2(GameObject of)
    {
        var belongingrow = GetBelonginRow(of);
        int theKey = getKeyInRow(of, belongingrow);
        var theIndex = BallRows.IndexOf(belongingrow);
        return GetBelonginPosition(theIndex, theKey);
    }

    Vector2 GetBelonginPosition(int IndexInBallRows, int KeyInRow)
    {
        bool offset = BallRows[IndexInBallRows].offset;
        float offsetnr = 0;
        if (offset == true)
        {
            offsetnr = 0.4f;
        }
        return new Vector2((KeyInRow * 0.8f) + offsetnr - 2.2f, (IndexInBallRows * 0.8f) + -2.0f);
    }

    Row CreateRow(float atY, bool offset)
    {
        Row therow = new Row();
        therow.ballList = new Dictionary<int, GameObject>();
        for (int i = 0; i < 6; i++)
        {
            float offsetnr = 0;
            if (offset == true)
            {
                offsetnr = 0.4f;
            }
            therow.offset = offset;
            var theball = Instantiate(ball, new Vector3((i * 0.8f) + offsetnr - 2.2f, atY, 0), Quaternion.identity);
            var theballballscript = theball.GetComponent<BallScript>();
            var random = Random.Range(1, 8);
            float asFloat = (float)random;
            theballballscript.NumberValue = (int)Mathf.Pow(2.0f, asFloat);

            var sprite = theballballscript.SRenderer;
            sprite.color = colorForValue[theballballscript.NumberValue];
            therow.ballList.Add(i, theball);
            if (atY == 1.2f)
            {
                theballballscript.isNumberObject = false;
                sprite.enabled = false;
            }
            if (atY < 1.3f)
            {
                theballballscript.isNumberObject = false;
                sprite.enabled = false;
                theballballscript.bColl.enabled = false;
            }
        }
        return therow;
    }

    Row GetBelonginRow(GameObject of)
    {
        foreach (var aRow in BallRows)
        {
            if (aRow.ballList.ContainsValue(of))
            {
                return aRow;
            }
        }
        return null;
    }

    int getKeyInRow(GameObject of, Row belongingrow)
    {
        int theKey = new int();
        var Keys = belongingrow.ballList.Keys;
        foreach (var key in Keys)
        {
            if (belongingrow.ballList[key] == of)
            {
                theKey = key;
            }
        }
        return theKey;
    }

    GameObject GetNeighborFromAngle(GameObject of, float angle)
    {
        var belongingrow = GetBelonginRow(of);
        int theKey = getKeyInRow(of, belongingrow);
        var theIndex = BallRows.IndexOf(belongingrow);


        if ((angle >= 0) && (angle <= Mathf.PI / 2))
        {
            if (theKey + 1 < belongingrow.ballList.Count)
            {
                return belongingrow.ballList[theKey + 1];
            }
        }
        if ((angle < 0) && (angle >= -Mathf.PI / 2))
        {
            if (theKey - 1 >= 0)
            {
                return belongingrow.ballList[theKey - 1];
            }
        }
        if ((angle > Mathf.PI / 2) && (angle <= Mathf.PI))
        {
            if (theIndex - 1 >= 0)
            {
                if (belongingrow.offset == true)
                {
                    if (BallRows[theIndex - 1].ballList.Count > theKey + 1)
                    {
                        return BallRows[theIndex - 1].ballList[theKey + 1];
                    }
                }
                else
                {
                    return BallRows[theIndex - 1].ballList[theKey];
                }
            }
        }
        if ((angle < -Mathf.PI / 2) && (angle >= -Mathf.PI))
        {
            if (theIndex - 1 >= 0)
            {
                if (belongingrow.offset == true)
                {
                    return BallRows[theIndex - 1].ballList[theKey];
                }
                else
                {
                    if (0 <= theKey - 1)
                    {
                        return BallRows[theIndex - 1].ballList[theKey - 1];
                    }
                }

            }
        }
        return null;

    }

    bool IsABallOnTop(GameObject of)
    {
        var belongingrow = GetBelonginRow(of);
        int theKey = getKeyInRow(of, belongingrow);
        var theIndex = BallRows.IndexOf(belongingrow);
        if (theIndex + 1 < BallRows.Count)
        {
        var BS1 = BallRows[theIndex + 1].ballList[theKey].GetComponent<BallScript>();
        if ((BS1.isNumberObject == true) && (BS1.isfalling == false))
        {return true;}
            if (belongingrow.offset == true)
            {
                if (BallRows[theIndex + 1].ballList.Count > theKey + 1)
                {
                    var BS2 = BallRows[theIndex + 1].ballList[theKey + 1].GetComponent<BallScript>();
                    if ((BS2.isNumberObject == true) && (BS2.isfalling == false))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (theKey - 1 >= 0)
                {
                    var BS3 = BallRows[theIndex + 1].ballList[theKey - 1].GetComponent<BallScript>();
                    if ((BS3.isNumberObject == true) && (BS3.isfalling == false))
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            return true;
        }
        return false;
    }


    List<GameObject> GetNeighbors(GameObject of)
    {
        var thelist = new List<GameObject>();
        var belongingrow = GetBelonginRow(of);
        //get key of of in belongingrow
        int theKey = getKeyInRow(of, belongingrow);


        // check in own row
        if (belongingrow.ballList.Count > theKey + 1)
        {
            if (belongingrow.ballList[theKey + 1] != null)
            {
                thelist.Add(belongingrow.ballList[theKey + 1]);
            }
        }

        if (theKey - 1 >= 0)
        {
            if (belongingrow.ballList[theKey - 1] != null)
            {
                thelist.Add(belongingrow.ballList[theKey - 1]);
            }
        }




        //get Index of belonginrow 
        var theIndex = BallRows.IndexOf(belongingrow);

        //check in row above
        if (theIndex + 1 < BallRows.Count)
        {
            if (theKey >= 0 && theKey < BallRows[theIndex + 1].ballList.Count)
            {
                if (BallRows[theIndex + 1].ballList[theKey] != null)
                {
                    thelist.Add(BallRows[theIndex + 1].ballList[theKey]);
                }
            }

            if (belongingrow.offset == true)
            {
                if (BallRows[theIndex + 1].ballList.Count > theKey + 1)
                {
                    if (BallRows[theIndex + 1].ballList[theKey + 1] != null)
                    {
                        thelist.Add(BallRows[theIndex + 1].ballList[theKey + 1]);
                    }
                }

            }
            else
            {
                if (theKey - 1 >= 0)
                {
                    if (BallRows[theIndex + 1].ballList[theKey - 1] != null)
                    {
                        thelist.Add(BallRows[theIndex + 1].ballList[theKey - 1]);
                    }
                }
            }
        }


        //check in row beneat
        if (theIndex - 1 >= 0)
        {
            if (theKey >= 0 && theKey < BallRows[theIndex - 1].ballList.Count)
            {
                if (BallRows[theIndex - 1].ballList[theKey] != null)
                {
                    thelist.Add(BallRows[theIndex - 1].ballList[theKey]);
                }
            }

            if (belongingrow.offset == true)
            {
                if (BallRows[theIndex - 1].ballList.Count > theKey + 1)
                {
                    if (BallRows[theIndex - 1].ballList[theKey + 1] != null)
                    {
                        thelist.Add(BallRows[theIndex - 1].ballList[theKey + 1]);
                    }
                }

            }
            else
            {
                if (theKey - 1 >= 0)
                {
                    if (BallRows[theIndex - 1].ballList[theKey - 1] != null)
                    {
                        thelist.Add(BallRows[theIndex - 1].ballList[theKey - 1]);
                    }
                }
            }
        }
        return thelist;
    }
}

public class Row
{
    public Dictionary<int, GameObject> ballList;
    public bool offset;
}



enum RowCase
{
    full,
    empty,
    half
}
