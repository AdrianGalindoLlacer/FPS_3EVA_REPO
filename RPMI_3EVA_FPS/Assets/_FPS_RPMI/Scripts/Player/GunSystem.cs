using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunSystem : MonoBehaviour
{

    #region General Variables
    [Header("General References")]
    [SerializeField] Camera fpsCam; //Ref si disparamos desde el centro de la camara
    [SerializeField] Transform shootPoint; //Ref si disparamos desde la punta del cañon del arma
    [SerializeField] LayerMask impactLayer; //Layer con la que interactua el Raycast
    RaycastHit hit; //Almacena la informacion de los objetos con los que el Raycast puede chocar

    [Header("Weapon Parameters")]
    [SerializeField] int damage = 10; //Daño del arma por bala
    [SerializeField] float range = 100f; //Distancia maxima de disparo (si es escopeta es menos si es sniper mas, etc)
    [SerializeField] float spread = 0; //Dispersion del disparo/ radio de dispersion (para escopetas, etc)
    [SerializeField] float shootingCooldown = 0.2f; //Tiempo entre disparos
    [SerializeField] float reloadTime = 1.5f; //Tiempo de recarga en segundos
    [SerializeField] bool allowButtonHold = false; //Si el disparo se hace por click (false) o se puede mantener pulsado (true)

    [Header("Bullet Management")]
    [SerializeField] int amoSize = 30; //Cantidad maxima de balas por cargador
    [SerializeField] int bulletsPerTap = 1; //Cantidad de balas disparadas por dis paro (sie es escopeta pueden ser varias)
    [SerializeField]int bulletsLeft; //Cantidad de balas que quedan dentro del cargador

    [Header("Feedback References")]
    [SerializeField] GameObject impactEffect; //Ref al VFX de impacto de bala

    [Header("Dev - Gun State Bools")]
    [SerializeField] bool shooting; //Indica si estamos disparando
    [SerializeField] bool canShoot; //Indica si podemos disparar en X momento del juego
    [SerializeField] bool reloading; //Indica si estamos en proceso de recarga

    #endregion

    private void Awake()
    {
        bulletsLeft = amoSize; //Al iniciar partida se llena el cargador
        canShoot = true; //Al iniciar partida se podra disparar
    }


    // Update is called once per frame
    void Update()
    {
        if (canShoot && shooting && !reloading && bulletsLeft > 0) //Si nos permite disparar la programacion && el input && no estamos recargando && y nos quedan balas
         
        {
            StartCoroutine(ShootRoutine());
        }
    }

    IEnumerator ShootRoutine()
    {
        canShoot = false; //Primra capa de seguridad que evita que hayan muchisimos disparos por frame y vaya mal el juego
        if (!allowButtonHold) shooting = false; //Configuracion del disparo por tap
        for (int i = 0; i < bulletsPerTap; i++)
        {
            if (bulletsLeft <= 0) break; //Segunda prevencion de errores para que no puedas disparar sin tener balas pero si haga sonido de sin balas
            Shoot(); //Disparo en si = Raycast que permite daño
            bulletsLeft--; //Quita una bala del cargador actual
        }

        yield return new WaitForSeconds(shootingCooldown); //Llama a la espera entre disparos
        canShoot = true; //Se puede volver a disparar
    }

    void Shoot()
    {
        //EL METODO MAS IMPORTANTE
        //SE DEFINE DISPARO POR RAYCAST -> UTILIZABLE PARA CUALQUIER MECANICA

        //Almacenar la direccion del disparo y modificarla en caso de haber dispersion
        Vector3 direction = fpsCam.transform.forward; //Para que el disparo siempre vaya hacia delante

        //Añadir dispersion/spread aleatoria segun el valor de spread
        direction.x += Random.Range(-spread, spread); //Al poner -s spread y spread hace que la dispersion pueda i hacia los dos lados y no solo a un lado
        direction.y += Random.Range(-spread, spread); //Dispersion hacia arriba y abajo

        //DECLARACION DEL RAYCAS
        //Physics.Raycast(Origen del rayo, direccion, almacen de la info del inpacto, longitud del rayo, layer con la que impacta el rayo)
        if (Physics.Raycast(fpsCam.transform.position, direction, out hit, range, impactLayer)) //el raycast sale desde la posicion de la camara en la direccion que se ha dicho antes (siempre hacia delante), que golpea (hit), su distancia maxima que puede recorrer (range) y la capa a la que afecta (impactLayer)
        {
            //AQUI SE PUEDEN CODEAR TODOS LOS EVENTOS QUE QUIERO PARA INTERACCION
            Debug.Log(hit.collider.name);
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                enemyHealth.TakeDamage(damage);
            }
        }
    }   

    IEnumerator ReloadRoutine()
    {
        reloading = true; //Se activa modo recarga (Para que no se pueda recargar varias veces a la vez)
        //Animacion de recarga si hay
        yield return new WaitForSeconds(reloadTime);
        bulletsLeft = amoSize; //Ser recarga a nivel de datos
        reloading = false;
    }

    void Reload()
    {
        if (bulletsLeft < amoSize && !reloading)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    #region Gun Input Methods

    public void OnShoot(InputAction.CallbackContext context)
    {
        //El sistema de inpput debe comprobar si el disparo es por tap o por mantener
        if (allowButtonHold)
        {
            //Modo mantener ON
            shooting = context.ReadValueAsButton(); //shooting es true mientras se pulsa el boton
        }
        else
        {
            //Modo tap ON
            if (context.performed) shooting = true;
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed) Reload();
    }

    #endregion




    #region Apuntes

    #endregion

}
