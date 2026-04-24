using UnityEngine;

public class FxManager : MonoBehaviour
{
   public static FxManager Instance { get; private set; }

   public GameObject fxSlashPrefab;

   private void Awake()
   {
      if (Instance != null && Instance != this)
      {
         Destroy(gameObject);
         return;
      }
      Instance = this;
      DontDestroyOnLoad(gameObject);
   }

   public void PlaySlash(Vector3 position)
   {
      if (fxSlashPrefab == null) return;

      GameObject fx = Instantiate(fxSlashPrefab, position, fxSlashPrefab.transform.rotation);
      Destroy(fx, 2f);
   }
}
