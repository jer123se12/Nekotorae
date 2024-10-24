using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatExpressionHandler : MonoBehaviour
{
    // Start is called before the first frame update
    Material expressionMat;
    public CatExpression expression;
    CatExpression currExpression;
    CatExpression priorDizzyExpression;
    bool isDizzy = false;

    void Start()
    {
        expressionMat = GetComponent<Renderer>().material;
        changeCatExpression(expression);
    }

    float getTextureOffset(CatExpression expression)
    {
        return ((float)expression / System.Enum.GetNames(typeof(CatExpression)).Length);
    }

    public void changeCatExpression(CatExpression expression)
    {
        expressionMat.mainTextureOffset = new Vector2(0.0f, -1.0f * getTextureOffset(expression));
        currExpression = expression;
    }

    public IEnumerator blink()
    {
        CatExpression prev=currExpression;
        changeCatExpression(CatExpression.Blink);
        yield return new WaitForSeconds(0.1f);

        // wait for the next fixed update

        // Hold the blink expression
        changeCatExpression(prev);
    }

    public void setDizzy()
    {
        if (!isDizzy)
        {
            // Cat is starting to get dizzy
            if (currExpression != CatExpression.Bocchi && currExpression != CatExpression.Confused)
            {
                // Store a expression that is not confusion
                priorDizzyExpression = currExpression;
            }
            
            if (Random.Range(1, 5) == 1)
            {
                changeCatExpression(CatExpression.Bocchi);
            }
            else
            {
                changeCatExpression(CatExpression.Confused);
            }
            isDizzy = true;
        } else
        {

        }
    }

    public void unsetDizzy()
    {
        if (isDizzy)
        {
            // Cat is stopping dizziness
            isDizzy = false;
            StartCoroutine(fadingDizziness());
        }
    }

    IEnumerator fadingDizziness()
    {
        yield return new WaitForSeconds(2f);
        print(priorDizzyExpression);
        changeCatExpression(priorDizzyExpression);
    }
}

public enum CatExpression
{
    Neutral,
    Blink,
    Derp,
    Smug,
    Shocked,
    Confused,
    Ashamed,
    Embarassed,
    Unsure,
    Disappointed,
    Laughing,
    Starry,
    Bocchi,
    Anya,
    Lost
}