void Attack()
{
    attackTimer = attackCooldown;
    if (swordHitboxPrefab != null)
    {
        GameObject hitbox = Instantiate(swordHitboxPrefab, transform.position, Quaternion.identity);
        PlayerSwordDamage damageScript = hitbox.GetComponent<PlayerSwordDamage>();
        if (damageScript != null) damageScript.SetAttacker(this.gameObject, attackPower);
    }
}