using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System;
using System.Threading.Tasks;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class Reset : MonoBehaviour
{
    public GameObject ball;
    int score=0;
    GameObject[] pins;

    public TextMeshProUGUI totalScore;

    public TextMeshProUGUI highestScore;

    public GameObject scoreParent;

    private int Tscore=0;

    private int numberOfPinsDown=0;

    string path;


    int k=1;
    Vector3[] positions;
    float rotationResetSpeed = 1.0f;
    // Start is called before the first frame update 
    void Start()
    {
        path = Application.persistentDataPath + "/SomethingRandom.txt";
        Debug.Log(path);
        pins=GameObject.FindGameObjectsWithTag("Pins"); 
        positions=new Vector3[pins.Length];
        for(int i=0;i<pins.Length;i++){
            positions[i]=pins[i].transform.position;
        } 
        ResetScoreCard(false);
        k++;
    }

    // Update is called once per frame
    void Update()
    {
        if(ball.transform.position.z>7f){
            CountPinsDown();
        }
    }

    void CountPinsDown()
    {
        for (int i = 0; i < pins.Length; i++)
        {
            float x = pins[i].transform.localEulerAngles.x;
            float z = pins[i].transform.localEulerAngles.z;
            if (((x >= 245 && x <= 275) || (x >= 65 && x <= 95) || (z >= 0 && z <= 30) || (z >= 165 && z <= 185)) && pins[i].activeSelf)
            {
                score++;
                numberOfPinsDown+=1;
                pins[i].SetActive(false);
            }
        }

        if(k!=0){
            TextMeshProUGUI currScoreCard = scoreParent.transform.Find(k.ToString()).GetComponent<TextMeshProUGUI>();
            currScoreCard.text = score.ToString();
        }
        
        Tscore += score;
        totalScore.text = Tscore.ToString();

        if(k%2==0){
            if(k==20){
                if(numberOfPinsDown!=10){
                    k++;
                    TextMeshProUGUI nextScoreCard = scoreParent.transform.Find(k.ToString()).GetComponent<TextMeshProUGUI>();
                    nextScoreCard.text = "/";
                    ResetScoreCard(true);
                }
                else
                    ResetPins();
            }
            else
                ResetPins();

        }

        else{
            if(k==21)
                ResetScoreCard(true);
            else
                ResetBall();
        }

        score = 0;
        k++;
    }

    void ResetPins()
    {
        for (int i = 0; i < pins.Length; i++)
        {
            pins[i].SetActive(true);
            pins[i].transform.position = positions[i];
            pins[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            pins[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            pins[i].transform.rotation = Quaternion.Slerp(pins[i].transform.rotation, Quaternion.Euler(0.0f,90.0f,-90.0f), Time.time * rotationResetSpeed);
            numberOfPinsDown=0;
            ResetBall();
        }
    }

    void ResetBall(){
        ball.transform.position = new Vector3(0.15f, 2.14f, -20.23f);
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        ball.transform.rotation = Quaternion.identity;
    }

    void ResetScoreCard(bool storeInFile){
        if(storeInFile){
            Debug.Log("before wait");
            StartCoroutine(ExampleCoroutine());
            Debug.Log("after wait");
            StreamWriter writer = File.AppendText(path);
            writer.WriteLine(Tscore.ToString());
            writer.Close();
            Debug.Log("written to file");
            SceneManager.LoadScene(0);
            Tscore=0;
        }
        
        
        totalScore.text = "0";
        for(int i=1;i<=21;i++){
            TextMeshProUGUI currScoreCard = scoreParent.transform.Find(i.ToString()).GetComponent<TextMeshProUGUI>();   
            currScoreCard.text = "0";
        }
        ResetPins();

        k=0;

        List<int> scores = new List<int>();

        try{
            StreamReader reader = File.OpenText(path);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                scores.Add(Int16.Parse(line));
                // Debug.Log(line);
            }
            int maximumScore=0;
            for(int i=scores.Count-1; i >= Math.Max(0,scores.Count-10) ; i--){
                maximumScore = Math.Max(maximumScore,scores[i]);
            }
            Debug.Log(maximumScore);
            highestScore.text = maximumScore.ToString();
        }
        catch(Exception e){
            return;
        }
    }

    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(5);
    }
}
