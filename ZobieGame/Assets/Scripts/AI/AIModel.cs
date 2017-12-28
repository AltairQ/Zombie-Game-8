
// Interface to perform external actions with the enemy
public interface IAIActions
{
    // Try to perform a melee attack towards the player
    // true  - the action has/will succeed
    // false - the action has/will fail
    bool AttackMeleePlayer();
    
    // Go towards player (using pathfinding)
    // true  - player is reachable
    // false - player is unreachable
    bool GoToPlayer();
}

// Interface to query the state of an individual enemy
public interface IAIState
{
    // Return the Game Director ID for this enemy
    int GetID();
    
    // Current health of the enemy
    float CurrentHealth();

    // Is the enemy alive
    bool IsAlive();

    // Distance to the player in a straight line
    float EuclidDistanceToPlayer();

    // Is the melee attack ready (no cooldown, no stuns)
    bool MeleeReady();

    // Is the player close enough to be hit with a melee attack
    bool PlayerInMeleeRange();
}
