using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{
    public void KnockBack(GameObject knockbackSource, Vector2 knockbackVelocity, float knockbackLength);

    public void Die();
}
