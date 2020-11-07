using System;
using UnityEngine;
public class CaveGenerationSettings : MonoBehaviour
{
    /*if (CaveGeneration.CaveNoise(x, y, z))
    {
        blockType = BlockType.Air;
    }*/
    private static CaveGenerationSettings m_instance = null;
    public FastNoise.FastNoise.FractalType m_fractalType = FastNoise.FastNoise.FractalType.Billow;
    public float m_fractalGain = 0.0f;
    public int m_fractalOctaves = 2;
    public float m_fractalLucanarity = 0.0f;
    public float m_frequency = 0.03f;
    public float m_airBlockTreshold = 0.01f;
    public static CaveGenerationSettings Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject singletonObject = GameObject.Find("TerrainGeneration");
                CaveGenerationSettings oldSingletonObject = singletonObject.GetComponent<CaveGenerationSettings>();
                if (oldSingletonObject != null) return m_instance;
                m_instance = singletonObject.AddComponent<CaveGenerationSettings>();
                //   DontDestroyOnLoad(singletonObject);
            }

            return m_instance;
        }
    }
}
public class CaveGeneration
{
    public static FastNoise.FastNoise caveNoise = new FastNoise.FastNoise(new System.Random().Next(1000, 9999));
    public static bool CaveNoise(float x, float y, float z)
    {
        if (y - 2 > WaterChunkObject.waterHeight) return false;
        /* CaveGenerationSettings caveGenerationSettings = CaveGenerationSettings.Instance;
         //SIMPLEX FRACTAL
         caveNoise.SetNoiseType(FastNoise.NoiseType.Cubic);
         caveNoise.SetFractalType(caveGenerationSettings.m_fractalType);
         caveNoise.SetFractalGain(caveGenerationSettings.m_fractalGain);
         caveNoise.SetFractalOctaves(caveGenerationSettings.m_fractalOctaves);
         caveNoise.SetFractalLacunarity(caveGenerationSettings.m_fractalLucanarity);
         caveNoise.SetFrequency(caveGenerationSettings.m_frequency);
         float simplex = caveNoise.GetPerlinFractal(x, y, z);*/
        //  return simplex > caveGenerationSettings.m_airBlockTreshold;
        return false;



    }
}