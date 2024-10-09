using System.Collections;

public interface IEnemy
{
    public void ReceiveDamage(int damageDealed);
    public IEnumerator GetStuned(float timeStuned);
}
