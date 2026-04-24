using System.Collections;
using UnityEngine;

public class FxManager : MonoBehaviour
{
    public GameObject fxSamuraiSlashPrefab;
    public int sortingOrder = 10;

    public void SpawnSlashFx(Vector3 position)
    {
        GameObject fx = Instantiate(fxSamuraiSlashPrefab, position, fxSamuraiSlashPrefab.transform.rotation);

        foreach (Renderer r in fx.GetComponentsInChildren<Renderer>())
        {
            r.sortingOrder = sortingOrder;
        }

        foreach (ParticleSystem ps in fx.GetComponentsInChildren<ParticleSystem>())
        {
            ParticleSystem.MainModule main = ps.main;
            main.loop = false;
        }

        StartCoroutine(RemoveWhenFinished(fx));
    }

    IEnumerator RemoveWhenFinished(GameObject fx)
    {
        ParticleSystem[] systems = fx.GetComponentsInChildren<ParticleSystem>();

        yield return new WaitForSeconds(0.1f);

        yield return new WaitUntil(() =>
        {
            foreach (ParticleSystem ps in systems)
            {
                if (ps != null && ps.IsAlive()) return false;
            }
            return true;
        });

        Destroy(fx);
    }
}
