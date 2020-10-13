using UnityEngine;
using System.Collections;
using System;

namespace SimpleNetCore
{
    public class FixRandom
    {
        public int m_RandomSeed = 0;

        int m_randomA = 9301;
        int m_randomB = 49297;
        int m_randomC = 233280;

        public FixRandom(int seed)
        {
            SetFixRandomSeed(seed);
        }

        public void SetFixRandomSeed(int seed)
        {
            m_RandomSeed = seed;
        }

        public void SetFixRandomParm(int a, int b, int c)
        {
            m_randomA = a;
            m_randomB = b;
            m_randomC = c;
        }

        public int GetFixRandom()
        {
            m_RandomSeed = Math.Abs((m_RandomSeed * m_randomA + m_randomB) % m_randomC);

            return m_RandomSeed;
        }

        public int Range(int min, int max)
        {
            if (max <= min)
                return min;
            int random = GetFixRandom();
            int range = max - min;
            int res = (random % range) + min;
            return res;
        }
    }
}
