using UnityEngine;

public class Pickup : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private Collider _collider;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void OnPickedup()
    {
        Debug.Log("Item Picked Up: " + gameObject.name);
        _rb.isKinematic = true;
        _rb.useGravity = false;
        _collider.enabled = false;
    }
}