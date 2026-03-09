using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health System Management")]
    [SerializeField] int maxHealth = 100; //Vida maxima del enemigo
    [SerializeField] int health = 100; //Vida actual del enemigo

    [Header("Feedback Configuration")]
    [SerializeField] Material damagedMat; //Material feedback de daño
    [SerializeField] GameObject deathVfx; //Efecto de particulas de muerte
    [SerializeField] MeshRenderer enemyRend; //Ref al componente que diuja los materiales del enemigo en pantalla
    Material baseMat; //Almacen del material base del enemigo

    private void Awake()
    {
        health = maxHealth; //Vida del enemigo se pone al maximo
        baseMat = enemyRend.material; //Referencia al material base
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            health = 0; //La vida no puede bajar de 0
            deathVfx.SetActive(true);
            deathVfx.transform.position = transform.position; //Se dice donde aparece el VFX
            gameObject.SetActive(false); //El enemigo muere/se apaga
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage; //Quitarle una cantidad determinada de vida al enemigo
        enemyRend.material = damagedMat; //Se cambia al material de feedback de daño
        Invoke(nameof(ResetEnemyMaterial), 0.1f); //Espera de tiempo que permite que se vea el parpadeo
    }

    void ResetEnemyMaterial()
    {
        //Devuelve el material del enemigo al original
        enemyRend.material = baseMat;
    }
}
