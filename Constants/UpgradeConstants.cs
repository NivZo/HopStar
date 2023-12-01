public static class UpgradeConstants
{
    public enum PortalUpgrade
    {
        AttackSpeed,
        AttackDamage,
        AddBullets,
        BulletSpeed,
        FlyingSpeed,
        TurningSpeed,
        AimAccuracy,
        PiercingBullets,
        MaxHealth,
        // Shield,
        // CriticalChance,
        // CriticalDamage,
        ScrapValue,
        PickupRange,
        // HealFromScrap,
    }

    public static string Description(this PortalUpgrade upgrade)
    {
        switch (upgrade)
        {
            case PortalUpgrade.AttackSpeed:
                return "Increase attack speed by 30%";
            case PortalUpgrade.AttackDamage:
                return "Increase attack damage by 30%";
            case PortalUpgrade.AddBullets:
                return "Increase bullet count by 2\nDecrease attack damage by 30%";
            case PortalUpgrade.BulletSpeed:
                return "Increase bullet speed by 30%";
            case PortalUpgrade.FlyingSpeed:
                return "Increase flying speed by 30%";
            case PortalUpgrade.TurningSpeed:
                return "Increase turning speed by 30%";
            case PortalUpgrade.AimAccuracy:
                return "Increase aim accuracy by 30%";
            case PortalUpgrade.PiercingBullets:
                return "Bullets pierce 1 additional target";
            case PortalUpgrade.MaxHealth:
                return "Increase max health by 30%";
            // case PortalUpgrade.Shield:
            //     return "Increase shield by 1";
            // case PortalUpgrade.CriticalChance:
            //     return "Increase critical change by 20%";
            // case PortalUpgrade.CriticalDamage:
            //     return "Increase critical damage by 50%";
            case PortalUpgrade.ScrapValue:
                return "Increase scrap gained by 40%";
            case PortalUpgrade.PickupRange:
                return "Increase scrap pickup range by 25%";
            // case PortalUpgrade.HealFromScrap:
            //     return "Heal 1 health per 50 scrap picked up";
            default:
                throw new System.Exception($"Bad upgrade received - {upgrade}");
        }
    }

    public static void AddTo(this PortalUpgrade upgrade, PlayerStatus playerStatus)
    {
        var playerUpgrades = playerStatus.Upgrades;
        switch (upgrade)
        {
            case PortalUpgrade.AttackSpeed:
                playerUpgrades.AttackSpeedPr += .3f;
                break;
            case PortalUpgrade.AttackDamage:
                playerUpgrades.AttackDamagePr += .3f;
                break;
            case PortalUpgrade.AddBullets:
                playerUpgrades.BulletCountAb += 2;
                playerUpgrades.AttackDamagePr -= .3f;
                break;
            case PortalUpgrade.BulletSpeed:
                playerUpgrades.BulletSpeedPr += .3f;
                break;
            case PortalUpgrade.FlyingSpeed:
                playerUpgrades.FlyingSpeedPr += .3f;
                break;
            case PortalUpgrade.TurningSpeed:
                playerUpgrades.TurningSpeedPr += .3f;
                break;
            case PortalUpgrade.AimAccuracy:
                playerUpgrades.AimAccuracyPr += .3f;
                break;
            case PortalUpgrade.PiercingBullets:
                playerUpgrades.PierceCountAb += 1;
                break;
            case PortalUpgrade.MaxHealth:
                playerUpgrades.MaxHealthPr += .3f;
                playerStatus.CurrentHealth += (int)(playerStatus.BaseMaxHealth * .3f);
                break;
            // case PortalUpgrade.Shield:
            //     playerUpgrades.MaxShield += 1;
            //     break;
            // case PortalUpgrade.CriticalChance:
            //     playerUpgrades.CritChance += .2f;
            //     break;
            // case PortalUpgrade.CriticalDamage:
            //     playerUpgrades.CritDamage += .5f;
            //     break;
            case PortalUpgrade.ScrapValue:
                playerUpgrades.ScrapValuePr += .4f;
                break;
            case PortalUpgrade.PickupRange:
                playerUpgrades.PickupRangePr += .25f;
                break;
            // case PortalUpgrade.HealFromScrap:
            //     playerUpgrades.HealFromScrap += 1;
                // break;
            default:
                throw new System.Exception($"Bad upgrade received - {upgrade}");
        }
    }
}