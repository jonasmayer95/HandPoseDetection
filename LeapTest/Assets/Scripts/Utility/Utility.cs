using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utility<T> {

    public static List<T> shuffleList(List<T> alpha )
    {
        for (int i = 0; i < alpha.Count; i++)
        {
            T temp = alpha[i];
            int randomIndex = Random.Range(i, alpha.Count);
            alpha[i] = alpha[randomIndex];
            alpha[randomIndex] = temp;
        }
        return alpha;
     }
}
