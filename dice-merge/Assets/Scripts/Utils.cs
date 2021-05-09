using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utils
{
    public int GetTotalNumberInArea(List<CubeEntity> list)
    {
        return list.Sum(a => a.Number);
    }

    public int GetMaxNumberInArea(List<CubeEntity> list)
    {
        return list.Max(a => a.Number);
    }

    public int GetPowerCountOfNumber(int number)
    {
        int count = 0;
        int calculatedNumber = number;
        while (calculatedNumber != 0)
        {
            calculatedNumber /= 2;

            if (calculatedNumber == 0)
                break;

            count++;
        }

        return count;
    }

    public int[] GenerateRandomPowers(int maxNumberInArea)
    {
        if (maxNumberInArea == 2)
            return new int[] { 1, 1 };

        if (maxNumberInArea >= 16)
            maxNumberInArea = 8;

        int maxPowerInRandom = GetPowerCountOfNumber(maxNumberInArea) + 1;
        int firstPower = Random.Range(1, maxPowerInRandom);
        int secondPower = Random.Range(1, maxPowerInRandom);

        while (secondPower == firstPower)
            secondPower = Random.Range(1, maxPowerInRandom);

        return new int[] { firstPower, secondPower };
    }

}
