using System;

using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
[System.Serializable]
public class scoreboard{
    public List<row> rows=new List<row>(); 
}
[System.Serializable]
public class row{
    public string name;
    public float score;
}
public class endpanel : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject finalScore;
    scoreboard originalScoreBoard;
    public GameObject hs;
    public scoreboard newboard;
    List<TextMeshProUGUI> initialsT=new List<TextMeshProUGUI>();
    public int[] initials = new int[3];
    public float fs;
    int[] prei=new int[3];
    public string filePath="/highscores.json";
    public String letters="ABCEDFGHIJKLMNOPQRSTUVWXYZ";
    public void displayScoreBoard(scoreboard scores){
        for (int i=0;i<6;i++){
            GameObject row=gameObject.GetNamedChild("r"+i.ToString());
            TextMeshProUGUI name=row.GetNamedChild("n").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI score=row.GetNamedChild("s").GetComponent<TextMeshProUGUI>();
            if(i<scores.rows.Count){
                name.SetText(scores.rows[i].name);
                score.SetText(scores.rows[i].score.ToString("F2"));
            }else{
                name.SetText("");
                score.SetText("");
            }
        }

    }
    void changeI(int pos, int amt){
        newboard=JsonUtility.FromJson<scoreboard>(JsonUtility.ToJson(originalScoreBoard));
        int letter=this.initials[pos];
        letter+=amt;
        if (letter<0){
            letter+=26;
        }else if (letter>25){
            letter-=26;
        }
        this.initials[pos]=letter%26;
        string initia="";
        for(int i=0;i<initialsT.Count;i++){
            initia+=letters[initials[i]].ToString();
        }
        print(initia);
        row r= new row();
        r.name=initia;
        r.score=fs;
        newboard.rows.Add(r);
        newboard.rows=(newboard.rows.OrderBy(w=>w.score).Reverse()).ToList();
        displayScoreBoard(newboard);
    }
    void Start()
    {
        hs.SetActive(false);
        filePath=Application.persistentDataPath+filePath;
        initials[0]=0;
        initials[1]=0;
        initials[2]=0;
        finalScore=gameObject.GetNamedChild("FinalScore");
        GameObject ini=gameObject.GetNamedChild("Initial");
        for(int i=1; i<4; i++){
            TextMeshProUGUI te=ini.GetNamedChild("n"+i.ToString()).GetComponent<TextMeshProUGUI>();
            initialsT.Add(te);
        }
        for(int i=0; i<3; i++){
            Button t=ini.GetNamedChild("n"+(i+1).ToString()+"bu").GetComponent<Button>();
            int tempi=i;
            UnityAction ex=()=>changeI(tempi, 1);
            
            t.onClick.AddListener(ex);
        }
        for(int i=0; i<3; i++){
            Button t=ini.GetNamedChild("n"+(i+1).ToString()+"bd").GetComponent<Button>();
            int tempi=i;
            t.onClick.AddListener(()=>changeI(tempi, -1));
        }
        if (System.IO.File.Exists(filePath)){
            originalScoreBoard=JsonUtility.FromJson<scoreboard>(System.IO.File.ReadAllText(filePath));
        }else{
        originalScoreBoard=new scoreboard();
        }
        changeI(0,0);
    }

    // Update is called once per frame
    void Update()
    {
        finalScore.GetComponent<TextMeshProUGUI>().text=fs.ToString("F2");
        for(int i=0;i<initialsT.Count;i++){
            initialsT[i].text=letters[initials[i]].ToString();
        }
    }
    public void save(){
        System.IO.File.WriteAllText(filePath, JsonUtility.ToJson(newboard));
    }
    
    public void next(){
        string initia="";
        for(int i=0;i<initialsT.Count;i++){
            initia+=letters[initials[i]].ToString();
        }
        print(initia);
        row r= new row();
        r.name=initia;
        r.score=fs;
        scoreboard highscores=new scoreboard();
        if (System.IO.File.Exists(filePath)){
            highscores=JsonUtility.FromJson<scoreboard>(System.IO.File.ReadAllText(filePath));
        }
        highscores.rows.Add(r);
        highscores.rows=(highscores.rows.OrderBy(w=>w.score).Reverse()).ToList();
        System.IO.File.WriteAllText(filePath, JsonUtility.ToJson(highscores));
        hs.SetActive(true);
        hs.GetComponent<highscore>().updatescore();
        gameObject.SetActive(false);

    }
}
