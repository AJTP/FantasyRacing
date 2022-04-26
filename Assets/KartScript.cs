using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartScript : MonoBehaviour
{
	public void GetInput()
	{
		horizontalInput = Input.GetAxis("Horizontal");
		verticalInput = Input.GetAxis("Vertical");
	}

	private void Dirigir()
	{
		anguloGiro = anguloGiroMaximo * horizontalInput;
		delanteraIzquierda.steerAngle = anguloGiro;
		delanteraDerecha.steerAngle = anguloGiro;
	}

	private void Acelerar()
	{
		delanteraIzquierda.motorTorque = verticalInput * fuerzaMotor;
		delanteraDerecha.motorTorque = verticalInput * fuerzaMotor;
	}

	private void UpdatePosicionesRuedas()
	{
		UpdatePosicionRuedaIzquierda(delanteraIzquierda, delanteraIzquierdaT);
		UpdatePosicionRuedaIzquierda(traseraIzquierda, traseraIzquierdaT);
		UpdatePosicionRuedaDerecha(delanteraDerecha, delanteraDerechaT);
		UpdatePosicionRuedaDerecha(traseraDerecha, traseraDerechaT);
	}

	private void UpdatePosicionRuedaIzquierda(WheelCollider _collider, Transform _transform)
	{
		Vector3 _pos = _transform.position;
		Quaternion _quat = _transform.rotation;

		_collider.GetWorldPose(out _pos, out _quat);

		_transform.position = _pos;
		_transform.rotation = _quat;
	}

	private void UpdatePosicionRuedaDerecha(WheelCollider _collider, Transform _transform)
	{
		Vector3 pos = _transform.position;
		Quaternion rot = _transform.rotation;
		_collider.GetWorldPose(out pos, out rot);
		rot = rot * Quaternion.Euler(new Vector3(0, 180, 0));
		_transform.position = pos;
		_transform.rotation = rot;


	}



	private void FixedUpdate()
	{
		GetInput();
		Dirigir();
		Acelerar();
		UpdatePosicionesRuedas();
	}

	private float horizontalInput;
	private float verticalInput;
	private float anguloGiro;

	public WheelCollider delanteraIzquierda, delanteraDerecha;
	public WheelCollider traseraIzquierda, traseraDerecha;
	public Transform delanteraIzquierdaT, delanteraDerechaT;
	public Transform traseraIzquierdaT, traseraDerechaT;
	public float anguloGiroMaximo = 30;
	public float fuerzaMotor = 50;
}
