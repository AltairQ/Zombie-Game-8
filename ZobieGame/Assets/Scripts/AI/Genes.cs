
// Bundle containing physical parameters and also ID
public class Genes
{
    // DEDUPLICATED
    // GD global ID
    // public int Id;
    
    // Total (max) HP
    public float G_health;

    // Max speed
    public float G_speed;

    // Strength (influences damage)
    public float G_strength;

    // Melee attack range
    public float G_melee_range;

    // Armour (blocks a fraction of damage)
    public float G_armor;

    public float GetPhysArmor()
    {
        return G_armor;
    }

    public float GetPhysSize()
    {
        return G_health;
    }

    public float GetPhysDamage()
    {
        return G_strength;
    }

    public float GetPhysSpeed()
    {
        return G_speed;
    }

    public float GetPhysMeleeRange()
    {
        return G_melee_range;
    }

    public float GetArmor()
    {
        return G_armor;
    }

    public override string ToString()
    {
        return string.Format("HP:{0}, SP:{1}, STR:{2}, RNG:{3}", G_health, G_speed, G_strength, G_melee_range);
    }

}
