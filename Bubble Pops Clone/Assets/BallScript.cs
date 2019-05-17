using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallScript : MonoBehaviour
{
    public Text textprefab;
    public SpriteRenderer SRenderer;
    public CircleCollider2D bColl;
    Text Numberdisplay;
    public bool isNumberObject = false;
    public int NumberValue = 2;
    public bool isfalling = false;

    private void Start()
    {
        Numberdisplay = Instantiate(textprefab, FindObjectOfType<Canvas>().transform,false);
        Numberdisplay.rectTransform.SetSiblingIndex(0);

    }
    private void Update()
    {
        Numberdisplay.transform.localScale = transform.localScale;
        if (isNumberObject == true)
        {
            Numberdisplay.enabled = true;
            Numberdisplay.transform.position = this.transform.position;
            Numberdisplay.text = NumberValue.ToString();

        }
        else
        {
            Numberdisplay.enabled = false;
        }
    }
    public void SetInvisble()
    {
        isNumberObject = false;
        SRenderer.enabled = false;
        bColl.enabled = false;

    }
    public void LabelEffect()
    {
        StartCoroutine(LabelEffect2());
    }
    IEnumerator LabelEffect2()
    {
        Text label = Instantiate(textprefab, Numberdisplay.transform);
        label.text = NumberValue.ToString();
        label.color = Color.gray;
        iTween.MoveBy(label.gameObject, iTween.Hash("amount", new Vector3(0, 0.4f, 0), "time", 0.6f));
        yield return new WaitForSeconds(0.6f);
        iTween.FadeTo(label.gameObject, iTween.Hash("alpha", 0.0f, "time", 0.1f));
        yield return new WaitForSeconds(0.1f);
        Destroy(label.gameObject);
    }
}
