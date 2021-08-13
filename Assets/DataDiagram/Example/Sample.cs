using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour {

    List<GameObject> lineList = new List<GameObject>(); //Average, SD, Min, Max, Curr;

    private DD_DataDiagram m_DataDiagram;
    //private RectTransform DDrect;

    private bool m_IsContinueInput = false;
    private float m_Input = 0f;
    private float h = 0;

    private float maxima = 0f;
    private float minima = 0f;
    private float deviation = 0f;
    private float currPosition = 0f;
     
    void AddALine() {

        if (null == m_DataDiagram)
            return;

        Color color = Color.HSVToRGB((h += 0.1f) > 1 ? (h - 1) : h, 0.8f, 0.8f);
        GameObject line = m_DataDiagram.AddLine(color.ToString(), color);
        if (null != line)
            lineList.Add(line);
    }

    void AddALine(Color lineColor)
    {

        if (null == m_DataDiagram)
            return;

        Color color = lineColor;
        GameObject line = m_DataDiagram.AddLine(color.ToString(), color);
        if (null != line)
            lineList.Add(line);
    }


    // Use this for initialization
    void Start () {

        GameObject dd = GameObject.Find("DataDiagram");
        if(null == dd) {
            Debug.LogWarning("can not find a gameobject of DataDiagram");
            return;
        }
        m_DataDiagram = dd.GetComponent<DD_DataDiagram>();

        m_DataDiagram.PreDestroyLineEvent += (s, e) => { lineList.Remove(e.line); };

        //AddALine();
    }

    // Update is called once per frame
    void Update () {

    }

    private void FixedUpdate() {

        m_Input += Time.deltaTime;
        ContinueInput(m_Input);
    }

    private void ContinueInput(float f) {
        maxima = StaticVariable.Maxima;
        minima = StaticVariable.Minima;
        deviation = StaticVariable.SD;
        currPosition = StaticVariable.PositionY;

        if (null == m_DataDiagram)
            return;

        if (false == m_IsContinueInput)
            return;

        float d = 0f;
        //foreach (GameObject l in lineList) {
        //m_DataDiagram.InputPoint(l, new Vector2(0.1f, (Mathf.Sin(f + d) + 1f) * 2f));
        //    d += 1f;
        //}

        ///////////////////Average, SD, Min, Max, Curr;
        AddALine(Color.white);
        AddALine(Color.green);
        AddALine(Color.grey);
        AddALine(Color.blue);
        AddALine(Color.red);

        float normalizer = maxima == 0 ? 0 : 22f / maxima;

        //float normalizer = 1f;

        m_DataDiagram.InputPoint(lineList[0], new Vector2(Time.deltaTime, 0f));
        m_DataDiagram.InputPoint(lineList[1], new Vector2(Time.deltaTime, deviation * normalizer));
        m_DataDiagram.InputPoint(lineList[2], new Vector2(Time.deltaTime, minima * normalizer));
        m_DataDiagram.InputPoint(lineList[3], new Vector2(Time.deltaTime, maxima * normalizer));
        m_DataDiagram.InputPoint(lineList[4], new Vector2(Time.deltaTime, currPosition * normalizer));
    }

    public void onButton() {

        if (null == m_DataDiagram)
            return;

        foreach (GameObject l in lineList) {
            m_DataDiagram.InputPoint(l, new Vector2(1, Random.value * 4f));
        }
    }

    public void OnAddLine() {
       // AddALine();
    }

    public void OnRectChange() {

        if (null == m_DataDiagram)
            return;

        //Rect rect = new Rect(Random.value * Screen.width, Random.value * Screen.height, Random.value * Screen.width / 2, Random.value * Screen.height / 2);

        Rect rect = new Rect(10, 120, Screen.width - 100, Screen.height - 100);

        m_DataDiagram.rect = rect;
    }

    public void OnContinueInput() {

        m_IsContinueInput = !m_IsContinueInput;

    }

}
