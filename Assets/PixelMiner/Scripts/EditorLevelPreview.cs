using UnityEngine;
namespace PixelMiner {
 // The editable scene preview is replaced by the actual game when Play starts.
 public sealed class EditorLevelPreview : MonoBehaviour {
  void Awake(){if(Application.isPlaying)gameObject.SetActive(false);}
 }
}
