using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class HandPostureUtils{

    private static TrainingUnit.Posture comparePosture;
    private static List<TrainingUnit> compareData;
    private static float postureHoldingThresh = 100;
    public static float getMinDistanceToPosture(TrainingUnit.Posture posture, AngleBasedHandModel hand)
    {
        if (comparePosture != posture || compareData == null)
        {
            comparePosture = posture;
            reload();
        }

        float result = float.PositiveInfinity;

        foreach (TrainingUnit tu in compareData)
        {
            if (result > hand.euclidianDistanceFingers(tu.hand))
                result = hand.euclidianDistanceFingers(tu.hand);
        }
        return result;
    }

    public static void reload()
    {
        compareData = PostureDataHandler.instance.getSublist(comparePosture);
    }

    public static bool isHolding(TrainingUnit.Posture posture, AngleBasedHandModel hand)
    {
        return (getMinDistanceToPosture(posture, hand)<=postureHoldingThresh);
    }
}
