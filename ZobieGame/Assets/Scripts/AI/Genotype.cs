using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All the information describing an individual enemy
public class Genotype
{
    // species (population id)
    public string species ="";

    // Unique enemy ID (used by Game Director)
    public int Id;

    // Level of the enemy (todo)
    public int Lvl;

    // physical information
    public Genes genes;

    // AI parameters
    public Memes memes;

    public Genotype(Genes g, Memes m)
    {
        this.genes = g;
        this.memes = m;
    }

    public Genotype(Genes g, Memes m, string spcs) : this(g, m)
    {
        species = System.String.Copy(spcs);
    }

    public Color GetColor()
    {
        int htmp = 0;

        for(int i = 0; i < species.Length; i++)
        {
            htmp += (species[i] - 'a') * System.Math.Max(1, 21 - 10 * i);
            htmp %= 359;
        }

        return Color.HSVToRGB(htmp / 358.0f, 0.6f, 0.7f);
    }

    public float GetValue()
    {
        // TODO find a better formula
        return genes.G_health * genes.G_armor * (genes.G_speed / 2.0f);
    }
}
