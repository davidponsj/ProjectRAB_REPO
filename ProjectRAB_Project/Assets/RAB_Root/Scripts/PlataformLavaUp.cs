using UnityEngine;
using System.Collections;

public class PlataformaSubeBaja : MonoBehaviour
{
    [Header("Configuración de movimiento")]
    public float altura = 3f; //Qué tan alto sube en eye Y
    public float tiempoMovimiento = 2f; //Cuanto tarda en subir o bajar
    public float tiempoQuietoArriba = 3f; //Tiempo que espera arriba antes de bajar
    public float tiempoQuietoAbajo = 3f;  //Tiempo que espera abajo antes de subir

    private Vector3 posicionInicial;
    private Vector3 posicionArriba;
    private bool subiendo = true;

    void Start()
    {
        posicionInicial = transform.position;
        posicionArriba = posicionInicial + Vector3.up * altura;

        StartCoroutine(MoverPlataforma());
    }

    IEnumerator MoverPlataforma()
    {
        while (true)
        {
            // Determinar posiciones objetivo
            Vector3 inicio = subiendo ? posicionInicial : posicionArriba;
            Vector3 destino = subiendo ? posicionArriba : posicionInicial;

            float tiempo = 0f;

            // Movimiento suave (lerp)
            while (tiempo < tiempoMovimiento)
            {
                tiempo += Time.deltaTime;
                float t = tiempo / tiempoMovimiento;
                transform.position = Vector3.Lerp(inicio, destino, t);
                yield return null;
            }

            // Asegurar posicion exacta al final del movimiento
            transform.position = destino;

            // Espera según la posición actual
            if (subiendo)
                yield return new WaitForSeconds(tiempoQuietoArriba);
            else
                yield return new WaitForSeconds(tiempoQuietoAbajo);

            // Cambiar direccion
            subiendo = !subiendo;
        }
    }
}
