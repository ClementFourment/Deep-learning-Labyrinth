using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Manager : MonoBehaviour
{
    public GameObject botPrefab;


    private bool isTraining = false;
    public int populationSize = 32;

    private int generationNumber = 0;

    private List<NeuralNetwork> nets;
    private List<NeuralNetwork> newNets;
    private List<GameObject> botList = null;

    private int [] layers = new int[] {6, 5, 4, 2}; //Dimensions des reseaux de neurones

    private float fit = 0;
    public Material myMaterial;


    
    public float moy_acc;
    public float current_weight_1;
    Renderer rd;

    void Timer()
    {
        Debug.Log("Génération : " + generationNumber);
        isTraining = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (!isTraining)
        {
            if(generationNumber == 0)
            {
                InitCarNeuralNetworks();
                CreateBotBodies();
            }
            else
            {
                
                //bool reset = false;
                
                for (int i=0; i<populationSize; i++)
                {
                    NNController script = botList[i].GetComponent<NNController>();
                   
                    moy_acc += script.results[0];
                    
                    float fitness = script.fitness;
                    nets[i].SetFitness(fitness);
                    
                }
                Debug.Log(moy_acc/populationSize);
                
                nets.Sort();
                nets.Reverse();

                float maxFit = 0;
                fit = 0;
                for (int i=0; i<populationSize; i++)
                {
                    if(nets[i].GetFitness() > maxFit)
                    {
                        maxFit = nets[i].GetFitness();
                    }
                    fit += nets[i].GetFitness();
                }
                fit /= populationSize;
                Debug.Log("fitness : MOY : " + fit + " | MAX : " + maxFit + ", " + nets[0].GetFitness());

                List<NeuralNetwork> newNets = new List<NeuralNetwork>();
                

                
                for (int i=0; i < 8; i++)
                {
                    NeuralNetwork newNet = new NeuralNetwork(nets[i]);
                    newNets.Add(newNet);
                }
                for (int i=0; i < 8; i++)
                {
                    NeuralNetwork newNet = new NeuralNetwork(nets[i]);
                    newNet.Mutate(5f);
                    newNets.Add(newNet);
                }
                 for (int i=0; i < 8; i++)
                {
                    NeuralNetwork newNet = new NeuralNetwork(nets[i]);
                    newNet.Mutate(10f);
                    newNets.Add(newNet);
                }
                for (int i=0; i < 6; i++)
                {
                    NeuralNetwork newNet = new NeuralNetwork(nets[i]);
                    newNet.Mutate(30f);
                    newNets.Add(newNet);
                }
                for (int i=0; i < 2; i++)
                {
                    NeuralNetwork newNet = new NeuralNetwork(nets[i]);
                    newNet.Mutate(100f);
                    newNets.Add(newNet);
                }



                for (int i=0; i < populationSize; i++)
                {
                    nets[i] = newNets[i];
                }
                CreateBotBodies();
            }
            generationNumber++;
            //Invoke("Timer", timer);
            isTraining = true;
        }
        for (int i=0; i<populationSize; i++)
        {
            NNController script = botList[i].GetComponent<NNController>();
            float[] result;
            float vel = script.currentVelocity / script.maxDistance;
            float distForward = script.distForward / script.maxDistance;
            float distLeft = script.distLeft / script.maxDistance;
            float distRight = script.distRight / script.maxDistance;
            float distDiagLeft = script.distDiagLeft / script.maxDistance;
            float distDiagRight = script.distDiagRight / script.maxDistance;

            float[] tInput = new float[]{vel, distForward, distLeft, distRight, distDiagLeft, distDiagRight};
            result = nets[i].FeedForward(tInput);
            script.results = result;
        }
        int countBotDead = 0;
        for (int i=0; i<populationSize; i++)
        {
            NNController script = botList[i].GetComponent<NNController>();
            if(!script.active)
            {
                countBotDead++;
            }
            
        }
        if(countBotDead == populationSize)
        {
            Debug.Log("Génération : " + generationNumber);
            isTraining = false;
        }
        // if (Input.GetKeyDown("Space"))
        // {
        //     generationNumber++;
        //     isTraining = true;
        //     Timer();
        //     CreateBotBodies();
        // }
    }


    void InitCarNeuralNetworks()
    {
        nets = new List<NeuralNetwork>();
        for (int i=0; i<populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            nets.Add(net);
        }
    }


    private void CreateBotBodies()
    {
        if (botList != null)
        {
            for (int i=0; i<botList.Count; i++)
            {
                Destroy(botList[i]);
            }
        }
        botList = new List<GameObject>();

        // choisir la piste entre 0 et 2.
        int course = UnityEngine.Random.Range(0, 3);
        course = 1;
        Vector3 start = new Vector3(0f, 0f, 0f);
        if (course == 0) 
        {
            start = new Vector3(23f, 1f, -17f); 
            botPrefab.transform.rotation = new Quaternion(0f, 0f, 0f, 1);
        }
        else if (course == 1) 
        {
            start = new Vector3(23f, 1f, 18f);
            botPrefab.transform.rotation = new Quaternion(-0.75f, 0, 0, 0);
        }
        else if (course == 2) 
        {
            start = new Vector3(22f, 1f, 12f); 
            botPrefab.transform.rotation = new Quaternion(0, 45, 0, 1);
        }
        Debug.Log("course : " + course);
        Debug.Log(botPrefab.transform.rotation);
        for (int i=0; i<populationSize; i++)
        {
            //botPrefab.transform.rotation
            GameObject bot = Instantiate(botPrefab, start, botPrefab.transform.rotation);
            
            
            
        

            botList.Add(bot);
            //botList[i] = bot;
        }


        for (int i=0; i<populationSize; i++)
        {
            rd = botList[i].GetComponent<Renderer>();
            rd.enabled = true;

            
            if(i<populationSize/4)
            {
                Color newColor = new Color(255, 0, 0, 1.0f); // Rouge
                rd.material.color = newColor;
            }
            else if(i<2*populationSize/4)
            {
                Color newColor = new Color(0, 255, 255, 1.0f); // bleu clair
                rd.material.color = newColor;
            }
            else if(i<3*populationSize/4)
            {
                Color newColor = new Color(0, 0, 255, 1.0f); // bleu foncé
                rd.material.color = newColor;
            }
            else
            {
                Color newColor = new Color(255, 255, 255, 1.0f); // Blanc
                rd.material.color = newColor;
            }
            
        }
    }







}
