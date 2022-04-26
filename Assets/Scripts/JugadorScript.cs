using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JugadorScript : MonoBehaviour
{
    #region Variables
    private Rigidbody rb;

    private float velocidadActual = 0;
    public float velocidadMaxima;
    public float velocidadBoost;
    private float velocidadReal;

    [Header("Ruedas")]
    public Transform ruedaDelanteraIzq;
    public Transform ruedaDelanteraDer;
    public Transform ruedaTraseraIzq;
    public Transform ruedaTraseraDer;


    public float direccion,tiempoDerrape;
    private float anguloGiro;
    public float anguloGiroMaximo = 25;

    bool derrapeIzquierda = false,derrapeDerecha=false;
    float fuerzaSalidaDerrape = 50000;
    public bool isDeslizando = false;
    private bool tocandoSuelo;


    public WheelCollider delanteraIzquierda, delanteraDerecha;
    public WheelCollider traseraIzquierda, traseraDerecha;
    #endregion
    #region Ciclo Unity
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Mover();
        DireccionRuedas();
        Direccion();
        AlineacionSuelo();
        Derrape();
    }

   


    #endregion
    #region Mis Metodos
    //ESTE METODO NOS PERMITE ACELERAR, FRENAR O ANDAR MARCHA ATRAS
    private void Mover() {
        velocidadReal = transform.InverseTransformDirection(rb.velocity).z;
        
        if (Input.GetAxis("Vertical") > 0)
        //if (Input.GetKey(KeyCode.Space))
        {
            velocidadActual = Mathf.Lerp(velocidadActual, velocidadMaxima, Time.deltaTime * 0.5f);
        }
        else if (Input.GetAxis("Vertical") < 0)
        //else if (Input.GetKey(KeyCode.F))
        {
            velocidadActual = Mathf.Lerp(velocidadActual, -velocidadMaxima / 1.75f, 1f * Time.deltaTime);
        }
        else 
        {
            velocidadActual = Mathf.Lerp(velocidadActual, 0,Time.deltaTime * 1.5f);
        }

        Vector3 velocidad = transform.forward * velocidadActual;
        velocidad.y = rb.velocity.y; //gravedad
        rb.velocity = velocidad;
    }

    //ESTE METODO NOS PERMITE DIRIGIR EL COCHE EN SI
    private void Direccion()
    {
        direccion = Input.GetAxisRaw("Horizontal");
        Vector3 direccionVector;

        float direccionSuma;

        if (derrapeIzquierda && !derrapeDerecha)
        {
            direccion = Input.GetAxis("Horizontal") < 0 ? -0.5f : 0f;
            transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, -20f, 0), 8f * Time.deltaTime);
            if (isDeslizando && tocandoSuelo) {
                rb.AddForce(transform.right * fuerzaSalidaDerrape * Time.deltaTime, ForceMode.Acceleration);
            }

        }
        else if (derrapeDerecha && !derrapeIzquierda)
        {
            direccion = Input.GetAxis("Horizontal") > 0 ? 0.5f : -0f;
            transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 20f, 0), 8f * Time.deltaTime);

            if (isDeslizando && tocandoSuelo)
            {
                rb.AddForce(transform.right * -fuerzaSalidaDerrape * Time.deltaTime, ForceMode.Acceleration);
            }
        }
        else {
            transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 0f, 0), 8f * Time.deltaTime);
        }
        direccionSuma = velocidadReal > 50 ? velocidadReal / 4 * direccion : direccionSuma = velocidadReal / 1.5f * direccion;

        direccionVector = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + direccionSuma, transform.eulerAngles.z);

        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, direccionVector, 3 * Time.deltaTime);

        anguloGiro = anguloGiroMaximo * direccion;
        delanteraIzquierda.steerAngle = anguloGiro;
        delanteraDerecha.steerAngle = anguloGiro;
    }

    //ESTE METODO NOS PERMITE REPRESENTAR LA DIRECCION DEL COCHE EN LAS RUEDAS
    private void DireccionRuedas()
    {
        UpdatePosicionRuedaIzquierda(delanteraIzquierda, ruedaDelanteraIzq);
        UpdatePosicionRuedaIzquierda(traseraIzquierda, ruedaTraseraIzq);
        UpdatePosicionRuedaDerecha(delanteraDerecha, ruedaDelanteraDer);
        UpdatePosicionRuedaDerecha(traseraDerecha, ruedaTraseraDer);

        /*if (velocidadActual > 50)
        {
            ruedaDelanteraIzq.Rotate(90 * Time.deltaTime * velocidadActual * 0.5f, 0, 0);
            ruedaDelanteraDer.Rotate(-90 * Time.deltaTime * velocidadActual * 0.5f, 0, 0);
            ruedaTraseraIzq.Rotate(90 * Time.deltaTime * velocidadActual * 0.5f, 0, 0);
            ruedaTraseraDer.Rotate(-90 * Time.deltaTime * velocidadActual * 0.5f, 0, 0);
        }
        else {
            ruedaDelanteraIzq.Rotate(90 * Time.deltaTime * velocidadReal * 0.5f, 0, 0);
            ruedaDelanteraDer.Rotate(-90 * Time.deltaTime * velocidadReal * 0.5f, 0, 0);
            ruedaTraseraIzq.Rotate(90 * Time.deltaTime * velocidadReal * 0.5f, 0, 0);
            ruedaTraseraDer.Rotate(-90 * Time.deltaTime * velocidadReal * 0.5f, 0, 0);
        } */
    }

    private void UpdatePosicionRuedaIzquierda(WheelCollider _collider, Transform _transform)
    {
        Vector3 pos = _transform.position;
        Quaternion rot = _transform.rotation;
        _collider.GetWorldPose(out pos, out rot);
        rot = rot * Quaternion.Euler(new Vector3(0, 0, 0));
        _transform.rotation = Quaternion.Lerp(_transform.rotation, rot, 8f * Time.deltaTime);
        _transform.position = pos;
        //_transform.rotation = rot;
    }

    private void UpdatePosicionRuedaDerecha(WheelCollider _collider, Transform _transform)
    {
        Vector3 pos = _transform.position;
        Quaternion rot = _transform.rotation;
        _collider.GetWorldPose(out pos, out rot);
        rot = rot * Quaternion.Euler(new Vector3(0, 180, 0));

        _transform.rotation = Quaternion.Lerp(_transform.rotation, rot, 8f * Time.deltaTime);
        _transform.position = pos;
        //_transform.rotation = rot;
    }

    //METODO PARA ALINEAR HORIZONTALMENTE EL COCHE CON EL SUELO
    private void AlineacionSuelo()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 0.75f))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 7.5f * Time.deltaTime);
            tocandoSuelo = true;
        }
        else {
            tocandoSuelo = false;
        }
    }
    private void Derrape()
    {
        if (Input.GetKeyDown(KeyCode.Space) && tocandoSuelo)
        {
            transform.GetChild(0).GetComponent<Animator>().SetTrigger("Salto");
            if (direccion > 0)
            {
                derrapeDerecha = true;
                derrapeIzquierda = false;
            }
            else if (direccion < 0)
            {
                derrapeDerecha = false;
                derrapeIzquierda = true;
            }
        }


        if (Input.GetKey(KeyCode.Space) && tocandoSuelo && velocidadActual > 20 && Input.GetAxis("Horizontal") != 0)
        {
            tiempoDerrape += Time.deltaTime;

        }

        if (!Input.GetKey(KeyCode.Space) || velocidadReal < 20)
        {
            derrapeIzquierda = false;
            derrapeDerecha = false;
            isDeslizando = false;



            //reset everything
            tiempoDerrape = 0;
            //stop particles
        }
    }
    #endregion
}
    