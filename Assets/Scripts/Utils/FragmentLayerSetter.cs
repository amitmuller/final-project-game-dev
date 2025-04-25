namespace UnityEngine.InputSystem.Utilities
{
    public class FragmentLayerSetter: MonoBehaviour
    {
        private string fragmentLayer = "breakable";
        private string sortingLayerName = "breakable";
        private int orderInLayer = 0;

        private void OnEnable()
        {
            Invoke("setLayer",1f);
            
        }

        void setLayer()
        {
            gameObject.layer = LayerMask.NameToLayer(fragmentLayer);

            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sortingLayerName = sortingLayerName;
                renderer.sortingOrder = orderInLayer;
            }
        }
    }
}