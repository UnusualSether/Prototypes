
using System;
using System.Linq.Expressions;
using UnityEngine;



    [Serializable]
    public class Weapon
    {
        public virtual string name { get; set; }
        public bool debugisOn = false;

        public virtual int WeaponEffect(int numberOfBulletsUsed, int Damage, Zombie targetZombie)
        {
            if(debugisOn) Debug.Log("This weapon has no effect!");
            return Damage;
        }
    }

    public class Pistol : Weapon
    {
        public bool debugisOn = false;
        public override string name 
        { 
            get { return "Pistol"; }
            set { }
        }

        int tempo;

        public override int WeaponEffect(int numberOfBulletsUsed, int Damage, Zombie targetZombie)
        {
            int NewDamage;
            if (numberOfBulletsUsed > 3)
            {
                tempo = 0;
                NewDamage = Damage % 2;
                if(debugisOn) Debug.Log("Ruined tempo!");
                return NewDamage;
            }

            else
            {
                tempo += 1;
                NewDamage = Damage + tempo;
                if (debugisOn) Debug.Log($"You now have {tempo} tempo!");
                return NewDamage;
            }


        }

    }

    public class Shotgun: Weapon
    {
        public bool debugisOn = false;
         public override string name 
        { 
            get { return "Shotgun"; }
            set { }
        }

        int storedDamage;

        public override int WeaponEffect(int numberOfBulletsUsed, int Damage, Zombie targetZombie)
        {

            int prevStored = storedDamage;

            int debugStore = Damage - targetZombie.hp;

            if (Damage > targetZombie.hp)
            {
                 storedDamage += Damage - targetZombie.hp;
                
            }

            if (prevStored == 0)
            {
                if (debugisOn) Debug.Log($"Stored {debugStore} Damage!");
                return Damage;
            }

            else
            {
                int Applystored = storedDamage;
                storedDamage = 0;
                if (debugisOn) Debug.Log($"Applied {Applystored} stored damage to total!");
                return Damage + Applystored;
            }
           


            
            


        }
    }



