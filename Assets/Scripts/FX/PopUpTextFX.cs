using TMPro;
using UnityEngine;

public class PopUpTextFX : MonoBehaviour, IPooledObject
{
    private TextMeshPro myText;

    [SerializeField] private float speed;
    [SerializeField] private float disappearanceSpeed;
    [SerializeField] private float colorDisappearanceSpeed;
    [SerializeField] private float lifeTime;

    private float textTimer;
    private float defaultSpeed;

    void Awake() => myText = GetComponent<TextMeshPro>();

    public void OnSpawn()
    {
        defaultSpeed = speed;
        textTimer = lifeTime;
    }
    public void OnReturnToPool()
    {
        transform.position = Vector2.zero;
        speed = defaultSpeed;
        myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, 1);
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y + 1), speed * Time.deltaTime);
        textTimer -= Time.deltaTime;

        if (textTimer < 0)
        {
            float alpha = myText.color.a - colorDisappearanceSpeed * Time.deltaTime;

            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, alpha);

            if (myText.color.a < 50)
                speed = disappearanceSpeed;

            if (myText.color.a <= 0)
                PoolManager.instance.Return("text", gameObject);
        }
    }
}
