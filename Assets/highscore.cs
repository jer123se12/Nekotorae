using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine.SceneManagement;
using UnityEngine;

public class highscore : MonoBehaviour
{
    // Start is called before the first frame update
    public string path="/highscores.json";
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
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
    public void updatescore(){
        scoreboard scores=JsonUtility.FromJson<scoreboard>(System.IO.File.ReadAllText(Application.persistentDataPath+path));
        displayScoreBoard(scores);
    }
    public void restart()
    {
        
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
