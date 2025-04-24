using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNController : MonoBehaviour
{
    private Transform transf;
    

    private Vector3 movDir = Vector3.zero;  //vecteur de deplacement
    private float rotSpeed = 400f;          //vecteur de roation
    private float speed = 0.1f;             //facteur de vitesse
    private float initialVelocity = 0.1f;   //vitesse initiale
    private float finalVelocity = 5f;       //vitesse max
    public float currentVelocity = 0.1f;   //vitesse actuelle
    private float accelerationRate = 0.2f;  //taux d'acceleration
   // private float decelerationRate = 0.2f;  //taux de deceleration

    //rayons de detection
    public float maxDistance = 30f;
    public float distForward = 0f;
    public float distLeft = 0f;
    public float distRight = 0f;
    public float distDiagLeft = 0f;
    public float distDiagRight = 0f;

    //fitness
    public float fitness = 0f;
    private Vector3 lastPosition;          //pour calculer la distance parcourue
    public float distanceTraveled;
    public float[] results;               //resultats du feedforward du neuralnetwork
    public bool active = true;            //permet de rendre inactif une fois un mur touche


    private Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(active)
        {
            CharacterController controller = gameObject.GetComponent<CharacterController>();
            
            if(results.Length != 0)
            {
                currentVelocity += (accelerationRate * Time.deltaTime) * results[0];
                currentVelocity = Mathf.Clamp(currentVelocity, initialVelocity, finalVelocity); //remet la vitesse entre les limites de vitesse

                movDir = new Vector3(0, 0, currentVelocity);
                movDir *= speed;
                movDir = transform.TransformDirection(movDir);

                controller.Move(movDir);
                transform.Rotate(0, results[1] * rotSpeed * Time.deltaTime, 0);
            }
            InteractRaycast();

            //gestion du fitness
            distanceTraveled += Vector3.Distance(transform.position, lastPosition);
            lastPosition = transform.position;
            fitness = distanceTraveled;     //augmente le fitness en fonction de la distance parcourue
            if (fitness > 0)
                fitness -= 0.01f;                       
        
            //Debug.Log("fitness : " + fitness);
            //Debug.Log(currentVelocity);
        }
    }

    //trigger collision
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Mur")
        {
            active = false;
        }
        if (other.gameObject.tag == "Checkpoint")
        {
            fitness += 5f;
        }
    }


    //rayons de detection
    void InteractRaycast() 
    {
        transf = GetComponent<Transform>();
        Vector3 playerPosition = transform.position;

        //direction des rayons
        Vector3 forwardDirection = transf.forward;
        Vector3 leftDirection = transf.right * (-1);
        Vector3 rightDirection = transf.right;
        Vector3 diagLeft = transf.TransformDirection(new Vector3(maxDistance/5, 0f, maxDistance/5));
        Vector3 diagRight = transf.TransformDirection(new Vector3(-maxDistance/5, 0f, maxDistance/5));

        //rayons
        Ray frontRay = new Ray(playerPosition, forwardDirection);
        Ray leftRay = new Ray(playerPosition, leftDirection);
        Ray rightRay = new Ray(playerPosition, rightDirection);
        Ray diagLeftRay = new Ray(playerPosition, diagLeft);
        Ray diagRightRay = new Ray(playerPosition, diagRight);

        //Collisions des rayons 
        RaycastHit hit;

        if (Physics.Raycast(frontRay, out hit, maxDistance) && hit.transform.tag == "Mur")
        {
            distForward = hit.distance;
        }
        if (Physics.Raycast(leftRay, out hit, maxDistance) && hit.transform.tag == "Mur")
        {
            distLeft = hit.distance;
        }
        if (Physics.Raycast(rightRay, out hit, maxDistance) && hit.transform.tag == "Mur")
        {
            distRight = hit.distance;
        }
        if (Physics.Raycast(diagLeftRay, out hit, maxDistance) && hit.transform.tag == "Mur")
        {
            distDiagLeft = hit.distance;
        }
        if (Physics.Raycast(diagRightRay, out hit, maxDistance) && hit.transform.tag == "Mur")
        {
            distDiagRight = hit.distance;
        }

        //Afficher les rayons

        Debug.DrawRay(transform.position, forwardDirection * maxDistance, Color.green);
        Debug.DrawRay(transform.position, leftDirection * maxDistance, Color.green);
        Debug.DrawRay(transform.position, rightDirection * maxDistance, Color.green);
        Debug.DrawRay(transform.position, diagLeft * maxDistance, Color.green);
        Debug.DrawRay(transform.position, diagRight * maxDistance, Color.green);
    }
}
