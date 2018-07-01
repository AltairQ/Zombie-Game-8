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
}
