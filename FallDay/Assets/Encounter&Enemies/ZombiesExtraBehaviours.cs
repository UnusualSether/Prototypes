using UnityEngine;

public class ExtraBehaviourComponent : MonoBehaviour
{

}

public class AttackBehaviourComponent : ExtraBehaviourComponent
{
    public virtual void OnAttack(Zombie this_zombie, GameHandler handler)
    {

    }
}

public class ExplodeComponent : AttackBehaviourComponent
{
    public override void OnAttack(Zombie this_zombie, GameHandler handler)
    {
        Explode(this_zombie, handler);
    }

    void Explode(Zombie exploder, GameHandler handler)
    {
        handler.CallPlayerDamage(exploder.damage * 3);

        handler._KillZombie(exploder);
    }
}
