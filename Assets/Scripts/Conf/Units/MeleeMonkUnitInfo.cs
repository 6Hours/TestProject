namespace Conf
{
    public class MeleeMonkUnitInfo : UnitInfo
    {
        public int AbilityShieldValue;
        public int AbilityShieldAbsorbingPercent;
        public int AbilityShieldExplosionDamage;
        public int AbilityShieldExplosionHeal;
        public int HealDamagePercent;

        public MeleeMonkUnitInfo()
        {
            MaxHealth = 1800;
            MaxMana = 100;
            Speed = 3;
            Damage = 5;
            ManaRegen = 5;
            AttackDistance = 2;
            AbilityShieldValue = 100;
            AbilityShieldAbsorbingPercent = 50;
            AbilityShieldExplosionDamage = 250;
            AbilityShieldExplosionHeal = 100;
            HealDamagePercent = 10;
            StunFreeDamage = 100;
        }
    }
}
