using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All the information describing an individual enemy
public class Genotype
{
    // physical information
    public Genes genes;

    // AI parameters
    public Memes memes;

    public Genotype(Genes g, Memes m)
    {
        this.genes = g;
        this.memes = m;
    }
}
